using MauiAppAMASBE;

namespace SeuApp.Services;

public interface INotificationService
{
    event EventHandler<NotificationEventArgs> NotificationReceived;

    void SendNotification(string title, string message, DateTime? notifyTime = null);
}