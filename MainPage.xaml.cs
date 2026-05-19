using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;


namespace MauiAppAMASBE
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
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

        private async void OnCounterClicked(object? sender, EventArgs e)
        {
            {
                bool permitido = await VerificarPerm();

                if (!permitido)
                {
                    await DisplayAlert("Permissão", "A permissão de notificação não foi concedida.", "OK");
                    return;
                }
            }
                var notification = new NotificationRequest
            {
                NotificationId = 100,
                Title = "Notificação de teste",
                Description = $"Essa é a descrição da minha notificação",
                ReturningData = "Dados adicionais",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(5) // Notificar após 5 segundos
                },
            
            };
            LocalNotificationCenter.Current.Show(notification);
        }
    }
}
