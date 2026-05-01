using MauiAppAMASBE.Models;

namespace MauiAppAMASBE.Pages;

public partial class CadastroPage : ContentPage
{
    public CadastroPage()
    {
        InitializeComponent();
    }
    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    private async void OnSalvarClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNome.Text) ||
            string.IsNullOrWhiteSpace(txtEmail.Text) ||
            string.IsNullOrWhiteSpace(txtCpf.Text) ||
            string.IsNullOrWhiteSpace(txtSenha.Text))
        {
            lblMensagem.Text = "Preencha todos os campos";
            return;
        }
        

        CadastroSaudeUsuario usuario = new CadastroSaudeUsuario
        {
            Nome = txtNome.Text,
            Email = txtEmail.Text,
            Cpf = txtCpf.Text,
            Senha = txtSenha.Text

         
        };
        var existente = await App.Db.GetUsuario(txtEmail.Text, txtCpf.Text, txtSenha.Text);
        if (existente != null)
        {
            await DisplayAlert("Erro", "Usuário já cadastrado", "OK");
            return;
        }

        await App.Db.InsertUsuario(usuario);
        

       

        await DisplayAlert("Sucesso", "Cadastro realizado!", "OK");
        await Navigation.PopAsync();
    }
}   