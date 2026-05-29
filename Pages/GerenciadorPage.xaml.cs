using MauiAppAMASBE.Models;
using MauiAppAMASBE.ViewModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
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
            BindingContext = viewModel;

            lst_conteudo.ItemsSource = lista;

        }
        protected async override void OnAppearing()
        {
            try
            {
                lista.Clear();

                List<ConteudoSaude> tmp = await App.Db.GetConteudo();

                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }

        private void OnNovoConteudoClicked(object sender, EventArgs e)
        {
            CadastroSaudeUsuario usuario = App.UsuarioLogado;

            CadastroCard.IsVisible = true;
            EmptyState.IsVisible = false;
        }

        private async void OnSalvarConteudoClicked(object sender, EventArgs e)
        {


            try
            {
                CadastroSaudeUsuario usuario = App.UsuarioLogado;

                if (usuario == null)
                {
                    await DisplayAlert("Erro", "Usuário não está logado", "OK");
                    return;
                }

       
                ConteudoSaude conteudo = new ConteudoSaude
                {
                    TituloConteudo = TituloEntry.Text,
                    TextoConteudo = TextoEntry.Text,
                    CategoriaConteudo = viewModel.CategoriaSelecionada,

                };

                await App.Db.InsertConteudo(conteudo);

                await DisplayAlert("Sucesso!", "Conteúdo Inserido", "OK");

            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }

            // limpar campos
            TituloEntry.Text = "";
            TextoEntry.Text = "";

            CadastroCard.IsVisible = false;
            EmptyState.IsVisible = true;

            // 🔥 importante pra atualizar lista 
            OnAppearing();
        }

        private async Task<bool> VerificarPerm()
        {
            throw new NotImplementedException();
        }

        private void OnCancelarClicked(object sender, EventArgs e)
        {
            CadastroCard.IsVisible = false;
            EmptyState.IsVisible = true;
        }
        private async void lst_conteudo_Refreshing(object sender, EventArgs e)
        {
            CadastroSaudeUsuario usuario = App.UsuarioLogado;
            try
            {
                lista.Clear();

                List<ConteudoSaude> tmp = await App.Db.GetConteudo();
                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
            finally
            {
                lst_conteudo.IsRefreshing = false;
            }
        }
        private void lst_conteudo_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            CadastroSaudeUsuario usuario = App.UsuarioLogado;
            try
            {
                ConteudoSaude c = e.SelectedItem as ConteudoSaude;

                Navigation.PushAsync(new Pages.ConteudosPage
                { BindingContext = c, });


            }
            catch (Exception ex)
            {
                DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }
        private async void MenuItem_Remover_Conteudo(object sender, EventArgs e)
        {
            CadastroSaudeUsuario usuario = App.UsuarioLogado;
            try
            {
                MenuItem item = sender as MenuItem;

                ConteudoSaude c = item.BindingContext as ConteudoSaude;

                bool confirma = await DisplayAlertAsync("Tem Certeza?", $"Remover {c.TituloConteudo}", "Sim", "Não");

                if (confirma)
                {
                    await App.Db.DeleteConteudo(c.IdConteudo);
                    lista.Remove(c);
                    await DisplayAlertAsync("Sucesso!", "Registro Apagado", "OK");

                }
                else
                {
                    await DisplayAlertAsync("Falha!", "Registro Mantido", "OK");
                }



            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }
        private void MenuItem_Editar_Conteudo(object sender, EventArgs e)
        {


            BindingContext = null;
            BindingContext = viewModel;
            var menuItem = sender as MenuItem;
            conteudoSelecionado = menuItem.BindingContext as ConteudoSaude;

            EditCard.IsVisible = true;

            // Preenche campos
            Edit_TituloEntry.Text = conteudoSelecionado.TituloConteudo;
            Edit_TextoEntry.Text = conteudoSelecionado.TextoConteudo;

            viewModel.CategoriaSelecionada = conteudoSelecionado.CategoriaConteudo;
        }
        private async void Button_Editar_Conteudo(object sender, EventArgs e)
        {
            base.OnAppearing();
            CadastroSaudeUsuario usuario = App.UsuarioLogado;

            if (usuario == null)
            {
                await DisplayAlert("Erro", "Usuário não está logado", "OK");
                return;
            }

            try
            {
                if (conteudoSelecionado == null)
                {
                    await DisplayAlert("Erro", "Nenhum conteúdo selecionado.", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Edit_TituloEntry.Text))
                {
                    await DisplayAlert("Erro", "Título do conteúdo é obrigatório.", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Edit_TextoEntry.Text))
                {
                    await DisplayAlert("Erro", "Texto do conteúdo é obrigatório.", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(viewModel.CategoriaSelecionada))
                {
                    await DisplayAlert("Erro", "Selecione uma categoria.", "OK");
                    return;
                }

                conteudoSelecionado.TituloConteudo = Edit_TituloEntry.Text;
                conteudoSelecionado.TextoConteudo = Edit_TextoEntry.Text;
                conteudoSelecionado.CategoriaConteudo = viewModel.CategoriaSelecionada;

                await App.Db.UpdateConteudo(conteudoSelecionado);

                await DisplayAlert("Sucesso", "Conteúdo atualizado!", "OK");

                EditCard.IsVisible = false;
                EmptyState.IsVisible = true;

                OnAppearing();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }



        private void Button_Cancelar_edicao(object sender, EventArgs e)
        {
            EditCard.IsVisible = false;
            EmptyState.IsVisible = true;
        }



        private async void ButtonVoltar(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
