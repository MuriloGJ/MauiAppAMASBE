using Microsoft.Maui.Controls;
using MauiAppAMASBE.Models;
<<<<<<< HEAD
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;
=======
using Microsoft.Maui.Storage;
>>>>>>> eadc1d619ce31373820fa63bc6deec17dc40b30c


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
    // Verificar se a permissão de notificações está concedida
    {
        var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();

        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.PostNotifications>();
        }

        return status == PermissionStatus.Granted;
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
            // 🔥 salva dados
            Preferences.Set("login", txtLogin.Text);
            Preferences.Set("senha", txtSenha.Text);

            // 🔹 define usuário logado
            App.UsuarioLogado = usuario;

            // inserindo a notificação de login
            bool permitido = await VerificarPerm();

            if (!permitido)
            {
                await DisplayAlert("Permissão", "A permissão de notificação não foi concedida.", "OK");
                return;
            }
            {
                var notification = new NotificationRequest
                {
                    NotificationId = 100,
                    Title = "Login realizado",
                    Description = "Você entrou no app com sucesso. Não se esqueça de atualizar sua agenda médica!",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(3) // Notificar após 3 segundos
                    }
                };

                 await LocalNotificationCenter.Current.Show(notification);
            }
            // fim da configuração de notificação

            

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
        //string cpf = txtCpf.Text;
        string NomeUsuario = txtNomeUsuario.Text;
        string Email = txtEmail2.Text;
        string novaSenha = txtNovaSenha.Text;
       

        if (string.IsNullOrWhiteSpace(NomeUsuario) || string.IsNullOrWhiteSpace(novaSenha))
        {
            await DisplayAlert("Erro", "Preencha todos os campos", "OK");
            return;
        }

        CadastroSaudeUsuario usuario = await App.Db.GetUsuarioPorNomeUsuario(NomeUsuario);

        if (!NomeUsuarioValido(NomeUsuario))
        {
            await DisplayAlert("Erro", "Nome de Usuário inválido", "OK");
            return;
        }

        if (usuario == null)
        {
            await DisplayAlert("Erro", "Nome de Usuário não encontrado", "OK");
            return;
        }
        bool NomeUsuarioValido(string NomeUsuario)
        {
            return !string.IsNullOrWhiteSpace(NomeUsuario);
        }
       

        usuario.Senha = novaSenha;

        await App.Db.UpdateUsuario(usuario);

        await DisplayAlert("Sucesso", "Senha redefinida!", "OK");

        await Navigation.PopAsync();

        SenhaCard.IsVisible = false;
        EmptyState.IsVisible = true;

    }

    private void Button_Voltar_Login(object sender, EventArgs e)
    {
        SenhaCard.IsVisible = false;
        EmptyState.IsVisible = true;
    }


    
}