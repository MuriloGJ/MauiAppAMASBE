using MauiAppAMASBE.Models;
using System.Text.RegularExpressions;

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
        // CORREÇÃO 1: validar campos antes de qualquer consulta ao banco
        if (string.IsNullOrWhiteSpace(txtNome.Text) ||
            string.IsNullOrWhiteSpace(txtEmail.Text) ||
            string.IsNullOrWhiteSpace(txtNomeUsuario.Text) ||
            string.IsNullOrWhiteSpace(txtSenha.Text))
        {
            lblMensagem.Text = "Preencha todos os campos";
            return;
        }

        // CORREÇÃO 2: validar formato do e-mail
        if (!EmailValido(txtEmail.Text))
        {
            await DisplayAlert("Erro", "E-mail inválido", "OK");
            return;
        }

        // CORREÇÃO 3: verificar e-mail INDEPENDENTE da senha (antes incluía senha na query)
        bool emailExiste = await App.Db.EmailExiste(txtEmail.Text.Trim().ToLower());
        if (emailExiste)
        {
            await DisplayAlert("Erro", "Este e-mail já está cadastrado", "OK");
            return;
        }

        // CORREÇÃO 4: verificar nome de usuário único separadamente
        var usuarioExistente = await App.Db.GetUsuarioPorNomeUsuario(txtNomeUsuario.Text.Trim());
        if (usuarioExistente != null)
        {
            await DisplayAlert("Erro", "Nome de usuário já em uso. Escolha outro.", "OK");
            return;
        }

        try
        {
            CadastroSaudeUsuario usuario = new CadastroSaudeUsuario
            {
                Nome        = txtNome.Text.Trim(),
                Email       = txtEmail.Text.Trim().ToLower(),
                NomeUsuario = txtNomeUsuario.Text.Trim(),
                Senha       = txtSenha.Text
            };

            await App.Db.InsertUsuario(usuario);
            await DisplayAlert("Sucesso", "Cadastro realizado!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private static bool EmailValido(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        return Regex.IsMatch(email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}
