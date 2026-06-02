using MauiAppAMASBE;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace MauiAppAMASBE;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseLocalNotification()
            // REMOVIDO: .UseMauiMaps() — não é necessário para Map.Default.OpenAsync()
            // e causa exceção no Windows por exigir chave Bing Maps.
            // Map.Default.OpenAsync() abre o app de mapas externo sem precisar disso.
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if ANDROID
        // Especifica explicitamente a interface para evitar ambiguidade com Plugin.LocalNotification
        builder.Services.AddSingleton<MauiAppAMASBE.INotificationService, NotificationServiceAndroid>();
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
