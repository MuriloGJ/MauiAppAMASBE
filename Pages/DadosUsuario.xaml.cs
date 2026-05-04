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

        var usuario = App.UsuarioLogado;

       /* if (usuario != null)
        {
            DisplayAlert("OK", usuario.Nome, "OK");
        }*/

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
            CadastroSaudeUsuario usuario = App.UsuarioLogado;

            if(usuario == null)
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


            // 🔹 ATRIBUIR DADOS
            usuario.DataNascimento = dtp_nascimento.Date.Value;
            //usuario.RuaUsuario = txtRua.Text;
            //usuario.NumeroUsuario = txtNumero.Text;
            //usuario.BairroUsuario = txtBairro.Text;
            //usuario.CidadeUsuario = txtCidade.Text;
            usuario.EstadoUsuario = viewModel.EstadoSelecionado;
            //usuario.CepUsuario = txtCep.Text;
           // usuario.ComplementoUsuario = txtComplemento.Text;
            //usuario.TelefoneUsuario = txtFone.Text;
            //usuario.ContatoEmergencia = txtFoneEmer.Text;
            //usuario.Peso = peso;
            //usuario.Altura = altura;

            usuario.Sexo = viewModel.SexoSelecionado;

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
