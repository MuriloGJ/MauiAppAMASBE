using MauiAppAMASBE.Models;
using MauiAppAMASBE.ViewModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Globalization;


namespace MauiAppAMASBE.Pages;



public partial class DadosUsuario : ContentPage
{

    CadastroViewModel viewModel = new CadastroViewModel();

    public DadosUsuario()
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        

        CadastroSaudeUsuario usuario = App.UsuarioLogado;

        viewModel.SexoSelecionado = usuario.Sexo;
        viewModel.EstadoSelecionado = usuario.EstadoUsuario;
        viewModel.TipoSanguineoSelecionado = usuario.TipoSanguineo;

        dtp_nascimento.Date = usuario.DataNascimento;

        viewModel.Usuario = usuario;
    }
    private void Button_Atualizar(object sender, EventArgs e)
    {
        CadastroCard.IsVisible = true;
        EmptyState.IsVisible = false;
    }
    private async void Button_Salvar(object sender, EventArgs e)
    {
        try
        {
            #region validações
            CadastroSaudeUsuario usuario = App.UsuarioLogado;

            if (usuario == null)
            {
                await DisplayAlert("Erro", "Usuário não está logado", "OK");
                return;

            }
            if (usuario == null)
            {
                await DisplayAlert("Erro", "Usuário não logado", "OK");
                return;
            }
            if (dtp_nascimento.Date == null) { 
                await DisplayAlert("Erro", "Data inválida", "OK");
            return;
            } // 🔹 VALIDAR TELEFONE
            if (string.IsNullOrWhiteSpace(usuario.TelefoneUsuario))
            {
                await DisplayAlert("Erro", "Telefone é obrigatório", "OK");
                return;
            }
            // 🔹 TELEFONE
            if (string.IsNullOrWhiteSpace(usuario.TelefoneUsuario) || usuario.TelefoneUsuario.Length < 10||usuario.TelefoneUsuario.Length > 11)
            {
                await DisplayAlert("Erro", "Telefone inválido", "OK");
                return;
            }

            // 🔹 PESO
            if ( usuario.Peso <= 0 || usuario.Peso > 500)
            {
                await DisplayAlert("Erro", "Peso inválido", "OK");
                return;
            }

            // 🔹 ALTURA
            if ( usuario.Altura <= 20 || usuario.Altura > 300)
            {
                await DisplayAlert("Erro", "Altura inválida", "OK");
                return;
            }
            // 🔹 SEXO
            if (string.IsNullOrEmpty(viewModel.SexoSelecionado))
            {
                await DisplayAlert("Erro", "Selecione o sexo", "OK");
                return;
            }
            #endregion


            // 🔹 ATRIBUIR DADOS
            usuario.DataNascimento = dtp_nascimento.Date.Value;
            usuario.EstadoUsuario = viewModel.EstadoSelecionado;
            usuario.Sexo = viewModel.SexoSelecionado;
            usuario.TipoSanguineo = viewModel.TipoSanguineoSelecionado;
            await App.Db.UpdateUsuario(usuario);

            await DisplayAlert("Sucesso!", "Dados atualizados", "OK");

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        CadastroCard.IsVisible = false;
        EmptyState.IsVisible = true;

        // 🔥 importante pra atualizar lista
        OnAppearing();
    }
    

    private async void Button_Voltar(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HomePage());
    }

    
}
