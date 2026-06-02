using MauiAppAMASBE.Models;
using MauiAppAMASBE.ViewModel;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace MauiAppAMASBE.Pages
{
    public partial class HabitosPage : ContentPage
    {
        ObservableCollection<Habito> lista = new ObservableCollection<Habito>();
        HabitoViewModel viewModel         = new HabitoViewModel();
        Habito habitoSelecionado;

        public HabitosPage()
        {
            InitializeComponent();
            BindingContext        = viewModel;
            lst_habito.ItemsSource = lista;
        }

        // ── Ciclo de vida ───────────────────────────────────────────────────
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!App.VerificarLogin()) return;
            await CarregarListaAsync();
        }

        /// <summary>Carrega hábitos do usuário logado. Extraído para evitar chamar OnAppearing() diretamente.</summary>
        private async Task CarregarListaAsync()
        {
            try
            {
                CadastroSaudeUsuario usuario = App.UsuarioLogado;
                lista.Clear();
                List<Habito> tmp = await App.Db.GetHabitosPorUsuario(usuario.IdCadastro);
                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        // ── Novo hábito ─────────────────────────────────────────────────────
        private void OnNovoHabitoClicked(object sender, EventArgs e)
        {
            CadastroCard.IsVisible = true;
            EmptyState.IsVisible   = false;
        }

        private async void OnSalvarHabitoClicked(object sender, EventArgs e)
        {
            try
            {
                CadastroSaudeUsuario usuario = App.UsuarioLogado;
                if (usuario == null) { await DisplayAlert("Erro", "Usuário não está logado", "OK"); return; }

                // Validar nome primeiro (antes de buscar o banco)
                if (string.IsNullOrWhiteSpace(NomeEntry.Text))
                {
                    await DisplayAlert("Erro", "Nome do hábito é obrigatório", "OK"); return;
                }

                var habitosExistentes = await App.Db.GetHabitosPorUsuario(usuario.IdCadastro);
                if (habitosExistentes.Any(h => h.NomeHabito == NomeEntry.Text.Trim()))
                {
                    await DisplayAlert("Erro", "Você já tem um hábito com esse nome", "OK"); return;
                }

                if (!double.TryParse(MetaEntry.Text, out double meta) || meta <= 0)
                {
                    await DisplayAlert("Erro", "Meta inválida", "OK"); return;
                }
                if (string.IsNullOrEmpty(viewModel.FrequenciaSelecionada))
                {
                    await DisplayAlert("Erro", "Selecione a frequência", "OK"); return;
                }
                if (string.IsNullOrEmpty(viewModel.UnidadeSelecionada))
                {
                    await DisplayAlert("Erro", "Selecione a unidade", "OK"); return;
                }

                Habito habito = new Habito
                {
                    NomeHabito       = NomeEntry.Text.Trim(),
                    TipoHabito       = TipoEntry.Text,
                    DescricaoHabito  = DescricaoEntry.Text,
                    HorarioHabito    = timeHorario.Time ?? TimeSpan.Zero,
                    FrequenciaHabito = viewModel.FrequenciaSelecionada,
                    IdCadastro       = usuario.IdCadastro,
                    MetaValor        = meta,
                    MetaUnidade      = viewModel.UnidadeSelecionada,
                };

                await App.Db.InsertHabito(habito);
                await DisplayAlert("Sucesso!", "Hábito Inserido", "OK");

                NomeEntry.Text = ""; TipoEntry.Text = ""; DescricaoEntry.Text = ""; MetaEntry.Text = "";
                CadastroCard.IsVisible = false;
                EmptyState.IsVisible   = true;
                // CORREÇÃO: chama CarregarListaAsync() em vez de OnAppearing() diretamente
                await CarregarListaAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        private void OnCancelarClicked(object sender, EventArgs e)
        {
            CadastroCard.IsVisible = false;
            EmptyState.IsVisible   = true;
        }

        // ── Pull-to-Refresh ─────────────────────────────────────────────────
        private async void lst_habito_Refreshing(object sender, EventArgs e)
        {
            try
            {
                // CORREÇÃO: filtrar pelo usuário logado (antes chamava GetHabitos() — todos os usuários)
                await CarregarListaAsync();
            }
            finally
            {
                lst_habito.IsRefreshing = false;
            }
        }

        // ── Seleção de item ─────────────────────────────────────────────────
        private void lst_habito_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // CORREÇÃO: abrir formulário de edição, NÃO nova instância de HabitosPage (causava loop)
            if (e.SelectedItem is not Habito h) return;
            ((ListView)sender).SelectedItem = null;
            AbrirFormularioEdicao(h);
        }

        // ── Menu de contexto ────────────────────────────────────────────────
        private async void MenuItem_Remover_Habito(object sender, EventArgs e)
        {
            try
            {
                Habito h = (sender as MenuItem)?.BindingContext as Habito;
                bool confirma = await DisplayAlert("Tem Certeza?", $"Remover {h.NomeHabito}?", "Sim", "Não");
                if (confirma)
                {
                    await App.Db.DeleteHabito(h.IdHabito);
                    lista.Remove(h);
                    await DisplayAlert("Sucesso!", "Registro Apagado", "OK");
                }
            }
            catch (Exception ex) { await DisplayAlert("Ops", ex.Message, "OK"); }
        }

        private void MenuItem_Editar_Habito(object sender, EventArgs e)
        {
            Habito h = (sender as MenuItem)?.BindingContext as Habito;
            AbrirFormularioEdicao(h);
        }

        private void AbrirFormularioEdicao(Habito h)
        {
            BindingContext = null;
            BindingContext = viewModel;
            habitoSelecionado = h;

            Edit_NomeEntry.Text      = h.NomeHabito;
            Edit_DescricaoEntry.Text = h.DescricaoHabito;
            Edit_MetaEntry.Text      = h.MetaValor.ToString();
            Edit_timeHorario.Time    = h.HorarioHabito;
            viewModel.FrequenciaSelecionada = h.FrequenciaHabito;
            viewModel.UnidadeSelecionada    = h.MetaUnidade;

            EditCard.IsVisible   = true;
            EmptyState.IsVisible = false;
        }

        // ── Salvar edição ───────────────────────────────────────────────────
        private async void Button_Editar_Habito(object sender, EventArgs e)
        {
            CadastroSaudeUsuario usuario = App.UsuarioLogado;
            if (usuario == null) { await DisplayAlert("Erro", "Usuário não está logado", "OK"); return; }
            if (habitoSelecionado == null) { await DisplayAlert("Erro", "Nenhum hábito selecionado", "OK"); return; }

            try
            {
                // CORREÇÃO: lê campos Edit_* (não os do formulário de cadastro)
                if (string.IsNullOrWhiteSpace(Edit_NomeEntry.Text))
                {
                    await DisplayAlert("Erro", "Nome do hábito é obrigatório", "OK"); return;
                }

                var habitosExistentes = await App.Db.GetHabitosPorUsuario(usuario.IdCadastro);
                if (habitosExistentes.Any(h =>
                    h.NomeHabito == Edit_NomeEntry.Text.Trim() &&
                    h.IdHabito   != habitoSelecionado.IdHabito))
                {
                    await DisplayAlert("Erro", "Você já tem outro hábito com esse nome", "OK"); return;
                }

                if (!double.TryParse(Edit_MetaEntry.Text, out double meta) || meta <= 0)
                {
                    await DisplayAlert("Erro", "Meta inválida", "OK"); return;
                }
                if (string.IsNullOrEmpty(viewModel.FrequenciaSelecionada))
                {
                    await DisplayAlert("Erro", "Selecione a frequência", "OK"); return;
                }
                if (string.IsNullOrEmpty(viewModel.UnidadeSelecionada))
                {
                    await DisplayAlert("Erro", "Selecione a unidade", "OK"); return;
                }

                // CORREÇÃO: usa IdHabito original e chama UpdateHabito (antes chamava InsertHabito)
                Habito habitoAtualizado = new Habito
                {
                    IdHabito         = habitoSelecionado.IdHabito,
                    IdCadastro       = usuario.IdCadastro,
                    NomeHabito       = Edit_NomeEntry.Text.Trim(),
                    TipoHabito       = habitoSelecionado.TipoHabito,
                    DescricaoHabito  = Edit_DescricaoEntry.Text,
                    HorarioHabito    = Edit_timeHorario.Time ?? TimeSpan.Zero,
                    FrequenciaHabito = viewModel.FrequenciaSelecionada,
                    MetaValor        = meta,
                    MetaUnidade      = viewModel.UnidadeSelecionada,
                    ValorAtual       = habitoSelecionado.ValorAtual,
                };

                await App.Db.UpdateHabito(habitoAtualizado);
                await DisplayAlert("Sucesso!", "Hábito Atualizado", "OK");

                EditCard.IsVisible   = false;
                EmptyState.IsVisible = true;
                habitoSelecionado    = null;
                await CarregarListaAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        private void Button_Cancelar_edicao(object sender, EventArgs e)
        {
            EditCard.IsVisible   = false;
            EmptyState.IsVisible = true;
            habitoSelecionado    = null;
        }

        // ── Stepper ─────────────────────────────────────────────────────────
        private async void Stepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var habito = (sender as Stepper)?.BindingContext as Habito;
            if (habito == null) return;
            habito.ValorAtual = e.NewValue;
            await App.Db.UpdateHabito(habito);
        }
        private async void ButtonVoltar(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

       

        // CORREÇÃO: VerificarPerm implementado corretamente (estava com NotImplementedException)
        private async Task<bool> VerificarPerm()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            return status == PermissionStatus.Granted;
        }
    }
}
