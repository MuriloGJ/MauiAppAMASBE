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

            string login = txtLogin.Text?.Trim();
            string senha = txtSenha.Text;



            CadastroSaudeUsuario usuario = await App.Db.GetUsuario(txtLogin.Text,txtLogin.Text, txtSenha.Text);

           if (usuario == null)
            {
                await DisplayAlert("Erro", "Email ou senha inválidos", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(senha))
            {
                await DisplayAlert("Erro", "Digite a senha", "OK");
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

    private async void Button_RecuperarSenha(object sender, EventArgs e)
    {
        {
            SenhaCard.IsVisible = true;
            EmptyState.IsVisible = false;
        }

       
        
    }

    private async void Button_DefinirNovaSenha(object sender, EventArgs e)
    {
        string cpf = txtCpf.Text;
        string Email = txtEmail2.Text;
        string novaSenha = txtNovaSenha.Text;
       

        if (string.IsNullOrWhiteSpace(cpf) || string.IsNullOrWhiteSpace(novaSenha))
        {
            await DisplayAlert("Erro", "Preencha todos os campos", "OK");
            return;
        }

        CadastroSaudeUsuario usuario = await App.Db.GetUsuarioPorCpf(cpf);

        if (!CpfValido(cpf))
        {
            await DisplayAlert("Erro", "CPF inválido", "OK");
            return;
        }

        if (usuario == null)
        {
            await DisplayAlert("Erro", "CPF não encontrado", "OK");
            return;
        }
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

        usuario.Senha = novaSenha;

        await App.Db.UpdateUsuario(usuario);

        await DisplayAlert("Sucesso", "Senha redefinida!", "OK");

        await Navigation.PopAsync();

        SenhaCard.IsVisible = false;
        EmptyState.IsVisible = true;

    }
   

        // 🔥 importante pra atualizar tela
    }