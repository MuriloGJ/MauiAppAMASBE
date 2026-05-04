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

        //Validação do Email
        bool EmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
        if (!EmailValido(txtEmail.Text))
        {
            await DisplayAlert("Erro", "Email inválido", "OK");
            return;
        }

        //Validação do CPF
        bool CpfValido(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = cpf.Replace(".", "").Replace("-", "").Trim();

            if (cpf.Length != 11)
                return false;

            // evita CPF tipo 11111111111
            if (new string(cpf[0], 11) == cpf)
                return false;

            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();

            tempCpf += digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }
        if (!CpfValido(txtCpf.Text))
        {
            await DisplayAlert("Erro", "CPF inválido", "OK");
            return;
        }

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