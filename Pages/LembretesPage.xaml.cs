using MauiAppAMASBE.Helpers.HelperNotificacao;
using MauiAppAMASBE.Models;
using MauiAppAMASBE.ViewModel;
using Microsoft.Maui.Controls;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;
using System.Collections.ObjectModel;

namespace MauiAppAMASBE.Pages
{
    public partial class LembretesPage : ContentPage
    {
        ObservableCollection<Lembrete> lista = new ObservableCollection<Lembrete>();
        LembreteViewModel viewModel          = new LembreteViewModel();
        Lembrete lembreteSelecionado;

        public LembretesPage()
        {
            InitializeComponent();
            BindingContext          = viewModel;
            lst_lembrete.ItemsSource = lista;
        }

        // ── Ciclo de vida ───────────────────────────────────────────────────
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!App.VerificarLogin()) return;
            await CarregarListaAsync();
        }

        private async Task CarregarListaAsync()
        {
            try
            {
                CadastroSaudeUsuario usuario = App.UsuarioLogado;
                lista.Clear();

                List<Lembrete> tmp =
                    await App.Db.GetLembretePorUsuario(usuario.IdCadastro);

                tmp.ForEach(i => lista.Add(i));

                var pendentes = tmp.Where(l => !l.Concluido);

                if (pendentes.Any())
                {
                    await LocalNotificationCenter.Current.Show(
                        new NotificationRequest
                        {
                            NotificationId = 999,
                            Title = "Lembretes pendentes",
                            Description = $"Você possui {pendentes.Count()} lembrete(s) pendente(s)."
                        });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        // ── Novo lembrete ───────────────────────────────────────────────────
        private void OnNovoLembreteClicked(object sender, EventArgs e)
        {
            CadastroCard.IsVisible = true;
            EmptyState.IsVisible   = false;
        }

        private async void OnSalvarLembreteClicked(object sender, EventArgs e)
        {
            try
            {
                CadastroSaudeUsuario usuario = App.UsuarioLogado;
                if (usuario == null) { await DisplayAlert("Erro", "Usuário não está logado", "OK"); return; }

                if (string.IsNullOrWhiteSpace(TituloEntry.Text))
                {
                    await DisplayAlert("Erro", "Título é obrigatório", "OK"); return;
                }
                if (dtp_lembrete.Date == null)
                {
                    await DisplayAlert("Erro", "Selecione uma data", "OK"); return;
                }
                if (string.IsNullOrEmpty(viewModel.TipoLSelecionada))
                {
                    await DisplayAlert("Erro", "Selecione o tipo do lembrete", "OK"); return;
                }
                if (string.IsNullOrEmpty(viewModel.FrequenciaLSelecionada))
                {
                    await DisplayAlert("Erro", "Selecione a frequência", "OK"); return;
                }

                Lembrete lembrete = new Lembrete
                {
                    TituloLembrete     = TituloEntry.Text.Trim(),
                    DataLembrete       = dtp_lembrete.Date.Value,
                    HorarioLembrete    = HoraLembrete.Time ?? TimeSpan.Zero,
                    TipoLembrete       = viewModel.TipoLSelecionada,
                    FrequenciaLembrete = viewModel.FrequenciaLSelecionada,
                    IdCadastro         = usuario.IdCadastro,
                };

                await App.Db.InsertLembrete(lembrete);
                await NotificacaoHelper.CriarNotificacao(lembrete);
                await DisplayAlert("Sucesso!", "Lembrete Inserido", "OK");

                TituloEntry.Text       = "";
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
        private async void lst_lembrete_Refreshing(object sender, EventArgs e)
        {
            try
            {
                // CORREÇÃO: filtrar pelo usuário logado (antes chamava GetLembrete() — todos os usuários)
                await CarregarListaAsync();
            }
            finally
            {
                lst_lembrete.IsRefreshing = false;
            }
        }

        // ── Seleção de item ─────────────────────────────────────────────────
        private void lst_lembrete_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // CORREÇÃO: abrir formulário de edição, NÃO nova LembretesPage (causava loop infinito)
            if (e.SelectedItem is not Lembrete l) return;
            ((ListView)sender).SelectedItem = null;
            AbrirFormularioEdicao(l);
        }

        // ── Menu de contexto ────────────────────────────────────────────────
        private async void MenuItem_RemoverLembrete(object sender, EventArgs e)
        {
            try
            {
                Lembrete l = (sender as MenuItem)?.BindingContext as Lembrete;
                bool confirma = await DisplayAlert("Tem Certeza?", $"Remover {l.TituloLembrete}?", "Sim", "Não");
                if (confirma)
                {
                    await App.Db.DeleteLembrete(l.IdLembrete);
                    lista.Remove(l);
                    await DisplayAlert("Sucesso!", "Registro Apagado", "OK");
                }
            }
            catch (Exception ex) { await DisplayAlert("Ops", ex.Message, "OK"); }
        }

        private void MenuItem_EditarLembrete(object sender, EventArgs e)
        {
            Lembrete l = (sender as MenuItem)?.BindingContext as Lembrete;
            AbrirFormularioEdicao(l);
        }
        private async void CheckBox_ConcluidoChanged(object sender, CheckedChangedEventArgs e)
        {
            try
            {
                CheckBox checkBox = sender as CheckBox;

                Lembrete lembrete = checkBox.BindingContext as Lembrete;

                if (lembrete == null)
                    return;

                lembrete.Concluido = e.Value;

                await App.Db.UpdateLembrete(lembrete);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }
        private async void MenuItem_ConcluirLembrete(object sender, EventArgs e)
        {
            try
            {
                MenuItem item = sender as MenuItem;

                Lembrete lembrete = item.BindingContext as Lembrete;

                lembrete.Concluido = true;

                await App.Db.UpdateLembrete(lembrete);

                await DisplayAlert("Sucesso", "Lembrete marcado como concluído.", "OK");

                lista.Remove(lembrete);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }

        private void AbrirFormularioEdicao(Lembrete l)
        {
            BindingContext = null;
            BindingContext = viewModel;
            lembreteSelecionado = l;

            TituloEntryEdit.Text = l.TituloLembrete;
            Edit_timeHorario.Time = l.HorarioLembrete;
            viewModel.FrequenciaLSelecionada = l.FrequenciaLembrete;
            viewModel.TipoLSelecionada       = l.TipoLembrete;

            EditCard.IsVisible   = true;
            EmptyState.IsVisible = false;
        }

        // ── Salvar edição ───────────────────────────────────────────────────
        private async void Button_EditarLembrete(object sender, EventArgs e)
        {
            CadastroSaudeUsuario usuario = App.UsuarioLogado;
            if (usuario == null) { await DisplayAlert("Erro", "Usuário não está logado", "OK"); return; }
            if (lembreteSelecionado == null) { await DisplayAlert("Erro", "Nenhum lembrete selecionado", "OK"); return; }

            if (string.IsNullOrWhiteSpace(TituloEntryEdit.Text))
            {
                await DisplayAlert("Erro", "Título é obrigatório", "OK"); return;
            }

            try
            {
                lembreteSelecionado.TituloLembrete     = TituloEntryEdit.Text.Trim();
                lembreteSelecionado.TipoLembrete       = viewModel.TipoLSelecionada;
                // CORREÇÃO: linha duplicada removida (HorarioLembrete era atribuído duas vezes)
                lembreteSelecionado.HorarioLembrete    = Edit_timeHorario.Time ?? TimeSpan.Zero;
                lembreteSelecionado.FrequenciaLembrete = viewModel.FrequenciaLSelecionada;

                await App.Db.UpdateLembrete(lembreteSelecionado);
                await DisplayAlert("Sucesso", "Lembrete atualizado!", "OK");

                EditCard.IsVisible   = false;
                EmptyState.IsVisible = true;
                lembreteSelecionado  = null;
                await CarregarListaAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }
        
        private void Button_Cancelar(object sender, EventArgs e)
        {
            EditCard.IsVisible   = false;
            EmptyState.IsVisible = true;
            lembreteSelecionado  = null;
        }

        private async void ButtonVoltar(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
