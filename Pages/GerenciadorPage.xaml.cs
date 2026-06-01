using MauiAppAMASBE.Models;
using MauiAppAMASBE.ViewModel;
using System.Collections.ObjectModel;

namespace MauiAppAMASBE.Pages
{
    public partial class GerenciadorPage : ContentPage
    {
        ObservableCollection<ConteudoSaude> lista = new ObservableCollection<ConteudoSaude>();
        ConteudosViewModel viewModel = new ConteudosViewModel();
        ConteudoSaude conteudoSelecionado;

        public GerenciadorPage()
        {
            InitializeComponent();
            BindingContext        = viewModel;
            lst_conteudo.ItemsSource = lista;
        }

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
                lista.Clear();
                List<ConteudoSaude> tmp = await App.Db.GetConteudo();
                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex) { await DisplayAlert("Ops", ex.Message, "OK"); }
        }

        private void OnNovoConteudoClicked(object sender, EventArgs e)
        {
            CadastroCard.IsVisible = true;
            EmptyState.IsVisible   = false;
        }

        private async void OnSalvarConteudoClicked(object sender, EventArgs e)
        {
            try
            {
                CadastroSaudeUsuario usuario = App.UsuarioLogado;
                if (usuario == null) { await DisplayAlert("Erro", "Usuário não está logado", "OK"); return; }

                if (string.IsNullOrWhiteSpace(TituloEntry.Text))
                {
                    await DisplayAlert("Erro", "Título é obrigatório", "OK"); return;
                }
                if (string.IsNullOrWhiteSpace(TextoEntry.Text))
                {
                    await DisplayAlert("Erro", "Texto é obrigatório", "OK"); return;
                }

                ConteudoSaude conteudo = new ConteudoSaude
                {
                    TituloConteudo    = TituloEntry.Text.Trim(),
                    TextoConteudo     = TextoEntry.Text.Trim(),
                    CategoriaConteudo = viewModel.CategoriaSelecionada,
                };

                await App.Db.InsertConteudo(conteudo);
                await DisplayAlert("Sucesso!", "Conteúdo Inserido", "OK");

                TituloEntry.Text = ""; TextoEntry.Text = "";
                CadastroCard.IsVisible = false;
                EmptyState.IsVisible   = true;
                await CarregarListaAsync();
            }
            catch (Exception ex) { await DisplayAlert("Ops", ex.Message, "OK"); }
        }

        private void OnCancelarClicked(object sender, EventArgs e)
        {
            CadastroCard.IsVisible = false;
            EmptyState.IsVisible   = true;
        }

        private async void lst_conteudo_Refreshing(object sender, EventArgs e)
        {
            try { await CarregarListaAsync(); }
            finally { lst_conteudo.IsRefreshing = false; }
        }

        private void lst_conteudo_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is not ConteudoSaude c) return;
            ((ListView)sender).SelectedItem = null;
            AbrirFormularioEdicao(c);
        }

        private async void MenuItem_Remover_Conteudo(object sender, EventArgs e)
        {
            try
            {
                ConteudoSaude c = (sender as MenuItem)?.BindingContext as ConteudoSaude;
                bool confirma = await DisplayAlert("Tem Certeza?", $"Remover {c.TituloConteudo}?", "Sim", "Não");
                if (confirma)
                {
                    await App.Db.DeleteConteudo(c.IdConteudo);
                    lista.Remove(c);
                    await DisplayAlert("Sucesso!", "Registro Apagado", "OK");
                }
            }
            catch (Exception ex) { await DisplayAlert("Ops", ex.Message, "OK"); }
        }

        private void MenuItem_Editar_Conteudo(object sender, EventArgs e)
        {
            ConteudoSaude c = (sender as MenuItem)?.BindingContext as ConteudoSaude;
            AbrirFormularioEdicao(c);
        }

        private void AbrirFormularioEdicao(ConteudoSaude c)
        {
            BindingContext = null;
            BindingContext = viewModel;
            conteudoSelecionado = c;
            Edit_TituloEntry.Text = c.TituloConteudo;
            Edit_TextoEntry.Text  = c.TextoConteudo;
            viewModel.CategoriaSelecionada = c.CategoriaConteudo;
            EditCard.IsVisible   = true;
            EmptyState.IsVisible = false;
        }

        private async void Button_Editar_Conteudo(object sender, EventArgs e)
        {
            CadastroSaudeUsuario usuario = App.UsuarioLogado;
            if (usuario == null) { await DisplayAlert("Erro", "Usuário não está logado", "OK"); return; }

            try
            {
                if (conteudoSelecionado == null) { await DisplayAlert("Erro", "Nenhum conteúdo selecionado.", "OK"); return; }
                if (string.IsNullOrWhiteSpace(Edit_TituloEntry.Text)) { await DisplayAlert("Erro", "Título é obrigatório.", "OK"); return; }
                if (string.IsNullOrWhiteSpace(Edit_TextoEntry.Text))  { await DisplayAlert("Erro", "Texto é obrigatório.", "OK"); return; }
                if (string.IsNullOrWhiteSpace(viewModel.CategoriaSelecionada)) { await DisplayAlert("Erro", "Selecione uma categoria.", "OK"); return; }

                conteudoSelecionado.TituloConteudo    = Edit_TituloEntry.Text.Trim();
                conteudoSelecionado.TextoConteudo     = Edit_TextoEntry.Text.Trim();
                conteudoSelecionado.CategoriaConteudo = viewModel.CategoriaSelecionada;

                await App.Db.UpdateConteudo(conteudoSelecionado);
                await DisplayAlert("Sucesso", "Conteúdo atualizado!", "OK");

                EditCard.IsVisible   = false;
                EmptyState.IsVisible = true;
                conteudoSelecionado  = null;
                await CarregarListaAsync();
            }
            catch (Exception ex) { await DisplayAlert("Ops", ex.Message, "OK"); }
        }

        private void Button_Cancelar_edicao(object sender, EventArgs e)
        {
            EditCard.IsVisible   = false;
            EmptyState.IsVisible = true;
        }

        private async void ButtonVoltar(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
