using Microsoft.Extensions.Logging;
using MiniOrman.Services;
using MiniOrman.ViewModels;
using Plugin.LocalNotification;

namespace MiniOrman;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Servis kayıtları (DI)
        builder.Services.AddSingleton<StorageService>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
