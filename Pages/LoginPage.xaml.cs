using Microsoft.Maui.Controls;
using MauiAppAMASBE.Models;


namespace MauiAppAMASBE.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnEntrarClicked(object sender, EventArgs e)
    {
        try
        {
            var usuario = await App.Db.GetUsuario(txtLogin.Text,txtLogin.Text, txtSenha.Text);

           if (usuario == null)
            {
                await DisplayAlert("Erro", "Email ou senha inválidos", "OK");
                return;
            }

            // 🔹 define usuário logado
            App.UsuarioLogado = usuario;

            await DisplayAlert("Sucesso", "Login realizado!", "OK");

            // 🔹 navega
            await Navigation.PushAsync(new HomePage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }
    private async void OnCadastrarClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CadastroPage());
    }
}