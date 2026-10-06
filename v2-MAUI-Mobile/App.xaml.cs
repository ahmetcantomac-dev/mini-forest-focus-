using MiniOrman.ViewModels;

namespace MiniOrman;

/// <summary>
/// Uygulama sınıfı. Yaşam döngüsü olaylarını yönetir.
/// Arka plana geçiş/dönüş durumlarında timer süresini korur.
/// </summary>
public partial class App : Application
{
    private DateTime _pausedAt;

    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }

    /// <summary>
    /// Uygulama arka plana gittiğinde timer süresini kaydeder.
    /// </summary>
    protected override void OnSleep()
    {
        base.OnSleep();
        _pausedAt = DateTime.Now;
    }

    /// <summary>
    /// Uygulama ön plana döndüğünde timer süresini günceller.
    /// </summary>
    protected override void OnResume()
    {
        base.OnResume();

        if (_pausedAt != default)
        {
            // MainViewModel'e arka planda geçen süreyi bildir
            var handler = IPlatformApplication.Current?.Services.GetService<MainViewModel>();
            handler?.OnAppResuming(_pausedAt);
        }
    }
}
