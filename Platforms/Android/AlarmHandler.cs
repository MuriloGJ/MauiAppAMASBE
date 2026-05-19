using Android.Content;

namespace MauiAppAMASBE;

[BroadcastReceiver(Enabled = true, Exported = false)]
public class AlarmHandler : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (intent == null)
            return;

        var title = intent.GetStringExtra(NotificationServiceAndroid.TitleKey) ?? "Lembrete";
        var message = intent.GetStringExtra(NotificationServiceAndroid.MessageKey) ?? "Você tem uma tarefa pendente.";

        var service = IPlatformApplication.Current?.Services?.GetService<SeuApp.Services.INotificationService>();

        if (service is NotificationServiceAndroid androidService)
        {
            androidService.Show(title, message);
        }
    }
}


