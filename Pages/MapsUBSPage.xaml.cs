using MauiAppAMASBE.Models;
using MauiAppAMASBE.Services;
using Microsoft.Maui.Controls;
using System;

namespace MauiAppAMASBE.Pages
{
    public partial class MapsUBSPage : ContentPage
    {
        private readonly MapaService _mapaService = new();

        private List<LocalizacaoItem> _todosLocais = new();
        private LocalizacaoItem? _localSelecionado;

        private bool _filtroUbs     = true;
        private bool _filtroParques = true;
        private double _raioKm      = 5.0;

        // Fallback: São Paulo (centro)
        private double _latUsuario = -23.5505;
        private double _lonUsuario = -46.6333;

        // ── Cores dos filtros (futurista) ─────────────────────────────
        private static readonly Color CorUbsAtivo      = Color.FromArgb("#00C2E0");
        private static readonly Color CorParqueAtivo   = Color.FromArgb("#00E5A0");
        private static readonly Color CorFiltroInativo = Color.FromArgb("#141C30");
        private static readonly Color CorTextoAtivo    = Color.FromArgb("#0A0E1A");
        private static readonly Color CorTextoInativo  = Color.FromArgb("#1E4060");

        public MapsUBSPage()
        {
            InitializeComponent();
        }

        // ── Ciclo de vida ─────────────────────────────────────────────

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ObterLocalizacaoAsync();
            await CarregarLocaisAsync();
        }

        // ── Localização ───────────────────────────────────────────────

        private async Task ObterLocalizacaoAsync()
        {
            try
            {
                var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted) return;

                var loc = await Geolocation.GetLastKnownLocationAsync()
                       ?? await Geolocation.GetLocationAsync(
                              new GeolocationRequest(GeolocationAccuracy.Medium,
                                                     TimeSpan.FromSeconds(8)));
                if (loc != null)
                {
                    _latUsuario = loc.Latitude;
                    _lonUsuario = loc.Longitude;
                }
            }
            catch { /* GPS indisponível — usa São Paulo como fallback */ }
        }

        // ── Carregar locais ───────────────────────────────────────────

        private async Task CarregarLocaisAsync()
        {
            Loading.IsRunning = true;
            Loading.IsVisible = true;
            LblStatus.Text    = "Escaneando...";

            _todosLocais = await _mapaService.BuscarTodosAsync(_latUsuario, _lonUsuario, _raioKm);

            Loading.IsRunning = false;
            Loading.IsVisible = false;

            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            var busca = EntryBusca.Text ?? "";

            var filtrado = _todosLocais.Where(l =>
            {
                bool tipoOk = (l.Tipo == "UBS"    && _filtroUbs) ||
                              (l.Tipo == "Parque"  && _filtroParques);

                bool textoOk = string.IsNullOrWhiteSpace(busca)
                            || l.Nome.Contains(busca, StringComparison.OrdinalIgnoreCase)
                            || l.Endereco.Contains(busca, StringComparison.OrdinalIgnoreCase);

                return tipoOk && textoOk;
            }).ToList();

            ListaLocais.ItemsSource = filtrado;

            int totalUbs     = _todosLocais.Count(l => l.Tipo == "UBS");
            int totalParques = _todosLocais.Count(l => l.Tipo == "Parque");

            LblFiltroUbs.Text     = $"UBS ({totalUbs})";
            LblFiltroParques.Text = $"Parques ({totalParques})";
            LblStatus.Text        = $"{filtrado.Count} detectado(s)";
        }

        // ── Eventos — filtros ─────────────────────────────────────────

        private void OnFiltroUbsTapped(object sender, TappedEventArgs e)
        {
            _filtroUbs = !_filtroUbs;
            FrameFiltroUbs.BackgroundColor = _filtroUbs ? CorUbsAtivo : CorFiltroInativo;
            LblFiltroUbs.TextColor         = _filtroUbs ? CorTextoAtivo : CorTextoInativo;
            AplicarFiltros();
        }

        private void OnFiltroParquesTapped(object sender, TappedEventArgs e)
        {
            _filtroParques = !_filtroParques;
            FrameFiltroParques.BackgroundColor = _filtroParques ? CorParqueAtivo : CorFiltroInativo;
            LblFiltroParques.TextColor         = _filtroParques ? CorTextoAtivo : CorTextoInativo;
            AplicarFiltros();
        }

        private void OnBuscaTextChanged(object sender, TextChangedEventArgs e) =>
            AplicarFiltros();

        private async void OnRaioTapped(object sender, TappedEventArgs e)
        {
            var escolha = await DisplayActionSheet(
                "Raio de varredura", "Cancelar", null,
                "2 km", "5 km", "10 km", "20 km");

            if (escolha == null || escolha == "Cancelar") return;

            _raioKm      = double.Parse(escolha.Replace(" km", ""));
            LblRaio.Text = escolha;

            // Invalida cache e recarrega
            Preferences.Remove("mapa_cache_ubs_ts");
            Preferences.Remove("mapa_cache_parques_ts");
            await CarregarLocaisAsync();
        }

        // ── Eventos — seleção de local ────────────────────────────────

        private void OnLocalTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is not LocalizacaoItem local) return;

            _localSelecionado = local;

            LblDetalheNome.Text     = local.Nome;
            LblDetalheTipo.Text     = $"{local.Icone}  {local.Tipo.ToUpper()}";
            LblDetalheEndereco.Text = local.Endereco;

            RowTelefone.IsVisible   = !string.IsNullOrWhiteSpace(local.Telefone);
            LblDetalheTelefone.Text = local.Telefone;

            RowHorario.IsVisible    = !string.IsNullOrWhiteSpace(local.Horario);
            LblDetalheHorario.Text  = local.Horario;

            PainelDetalhe.IsVisible = true;
        }

        private void OnFecharDetalheTapped(object sender, EventArgs e)
        {
            PainelDetalhe.IsVisible = false;
            _localSelecionado = null;
        }

        // ── Navegação externa (Maps nativo) ───────────────────────────

        private async void OnNavegacaoTapped(object sender, EventArgs e)
        {
            if (_localSelecionado == null) return;

            try
            {
                var destino = new Location(_localSelecionado.Latitude, _localSelecionado.Longitude);
                var opcoes  = new MapLaunchOptions { Name = _localSelecionado.Nome };
                await Map.Default.OpenAsync(destino, opcoes);
            }
            catch
            {
                await DisplayAlert("Erro", "Não foi possível abrir o aplicativo de mapas.", "OK");
            }
        }
    }
}
