using System;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Microsoft.Maui.ApplicationModel;

namespace MauiAppAMASBE;

public class NotificationServiceAndroid : INotificationService
{
    const string ChannelId = "habitos_channel";
    const string ChannelName = "Hábitos";
    const string ChannelDescription = "Lembretes de hábitos";

    public const string TitleKey = "title";
    public const string MessageKey = "message";

    bool channelInitialized;
    int messageId;
    int pendingIntentId;

    NotificationManagerCompat? compatManager;

    public event EventHandler<NotificationEventArgs>? NotificationReceived;

    public NotificationServiceAndroid()
    {
        CreateNotificationChannel();
        compatManager = NotificationManagerCompat.From(Platform.AppContext);
    }

    public void SendNotification(string title, string message, DateTime? notifyTime = null)
    {
        if (!channelInitialized)
            CreateNotificationChannel();

        if (notifyTime.HasValue)
        {
            Intent intent = new Intent(Platform.AppContext, typeof(AlarmHandler));
            intent.PutExtra(TitleKey, title);
            intent.PutExtra(MessageKey, message);
            intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

            var flags = Build.VERSION.SdkInt >= BuildVersionCodes.S
                ? PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable
                : PendingIntentFlags.CancelCurrent;

            var pendingIntent = PendingIntent.GetBroadcast(
                Platform.AppContext,
                pendingIntentId++,
                intent,
                flags);

            long triggerTime = GetNotifyTime(notifyTime.Value);

            var alarmManager = Platform.AppContext.GetSystemService(Context.AlarmService) as AlarmManager;
            alarmManager?.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerTime, pendingIntent);
        }
        else
        {
            Show(title, message);
        }
    }

    private long GetNotifyTime(DateTime notifyTime)
    {
        DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
        double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
        long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
        return utcAlarmTime;
    }

    public void ReceiveNotification(string title, string message)
    {
        NotificationReceived?.Invoke(this, new NotificationEventArgs
        {
            Title = title,
            Message = message
        });
    }

    public void Show(string title, string message)
    {
        Intent intent = new Intent(Platform.AppContext, typeof(MainActivity));
        intent.PutExtra(TitleKey, title);
        intent.PutExtra(MessageKey, message);
        intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

        var flags = Build.VERSION.SdkInt >= BuildVersionCodes.S
            ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
            : PendingIntentFlags.UpdateCurrent;

        var pendingIntent = PendingIntent.GetActivity(
            Platform.AppContext,
            pendingIntentId++,
            intent,
            flags);

        var builder = new NotificationCompat.Builder(Platform.AppContext, ChannelId)
            .SetContentTitle(title)
            .SetContentText(message)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetAutoCancel(true)
            .SetContentIntent(pendingIntent)
            .SetPriority((int)NotificationPriority.Default);

        compatManager?.Notify(messageId++, builder.Build());
    }

    void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(
                ChannelId,
                ChannelName,
                NotificationImportance.Default)
            {
                Description = ChannelDescription
            };

            var manager = (NotificationManager?)Platform.AppContext.GetSystemService(Context.NotificationService);
            manager?.CreateNotificationChannel(channel);
        }

        channelInitialized = true;
    }

    
}