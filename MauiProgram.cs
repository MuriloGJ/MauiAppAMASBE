
using MauiAppAMASBE;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification; // adicionado para usar o plugin de notificações locais


namespace MauiAppAMASBE;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseLocalNotification() // adicionado para usar o plugin de notificações locais
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


#if ANDROID
        // Especifica explicitamente a interface do aplicativo para evitar ambiguidade com Plugin.LocalNotification.INotificationService
        builder.Services.AddSingleton<MauiAppAMASBE.INotificationService, NotificationServiceAndroid>();
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
