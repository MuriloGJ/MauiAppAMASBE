using Microsoft.Maui.Controls;
using MauiAppAMASBE.Models;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;
using Microsoft.Maui.Storage;

namespace MauiAppAMASBE.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        txtLogin.Text = Preferences.Get("login", "");
        txtSenha.Text = Preferences.Get("senha", "");
    }

    private async Task<bool> VerificarPerm()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.PostNotifications>();
        return status == PermissionStatus.Granted;
    }

    private async void OnEntrarClicked(object sender, EventArgs e)
    {
        try
        {
            string login = txtLogin.Text?.Trim() ?? "";
            string senha = txtSenha.Text ?? "";

            // CORREÇÃO: validar campos ANTES de consultar o banco
            if (string.IsNullOrWhiteSpace(login))
            {
                await DisplayAlert("Erro", "Digite o e-mail ou usuário", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(senha))
            {
                await DisplayAlert("Erro", "Digite a senha", "OK");
                return;
            }

            CadastroSaudeUsuario usuario = await App.Db.GetUsuario(login, login, senha);

            if (usuario == null)
            {
                await DisplayAlert("Erro", "E-mail/usuário ou senha inválidos", "OK");
                return;
            }

            Preferences.Set("login", login);
            Preferences.Set("senha", senha);
            App.UsuarioLogado = usuario;

            // Notificação de boas-vindas (não bloqueia o login se a permissão for negada)
            bool permitido = await VerificarPerm();
            if (permitido)
            {
                var notification = new NotificationRequest
                {
                    NotificationId = 100,
                    Title       = "Login realizado",
                    Description = "Você entrou no app com sucesso. Não se esqueça de atualizar sua agenda médica!",
                    Schedule    = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(3)
                    }
                };
                await LocalNotificationCenter.Current.Show(notification);
            }

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

    private void Button_RecuperarSenha(object sender, EventArgs e)
    {
        SenhaCard.IsVisible  = true;
        EmptyState.IsVisible = false;
    }

    private async void Button_DefinirNovaSenha(object sender, EventArgs e)
    {
        string nomeUsuario = txtNomeUsuario.Text?.Trim() ?? "";
        string novaSenha   = txtNovaSenha.Text ?? "";

        if (string.IsNullOrWhiteSpace(nomeUsuario) || string.IsNullOrWhiteSpace(novaSenha))
        {
            await DisplayAlert("Erro", "Preencha todos os campos", "OK");
            return;
        }

        if (novaSenha.Length < 4)
        {
            await DisplayAlert("Erro", "A nova senha deve ter pelo menos 4 caracteres", "OK");
            return;
        }

        // CORREÇÃO: buscar usuário DEPOIS de validar os campos; verificação de NomeUsuarioValido
        // era uma função local redundante — substituída por IsNullOrWhiteSpace já feito acima
        CadastroSaudeUsuario usuario = await App.Db.GetUsuarioPorNomeUsuario(nomeUsuario);

        if (usuario == null)
        {
            await DisplayAlert("Erro", "Nome de usuário não encontrado", "OK");
            return;
        }

        usuario.Senha = novaSenha;
        await App.Db.UpdateUsuario(usuario);
        await DisplayAlert("Sucesso", "Senha redefinida!", "OK");

        SenhaCard.IsVisible  = false;
        EmptyState.IsVisible = true;
    }

    private void Button_Voltar_Login(object sender, EventArgs e)
    {
        SenhaCard.IsVisible  = false;
        EmptyState.IsVisible = true;
    }
}
