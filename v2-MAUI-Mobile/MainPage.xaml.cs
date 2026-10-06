using MiniOrman.Controls;
using MiniOrman.Models;
using MiniOrman.ViewModels;

namespace MiniOrman;

/// <summary>
/// Ana sayfa code-behind. Animasyon mantığı ve CircularProgress ring yönetimi.
/// </summary>
public partial class MainPage : ContentPage
{
    private readonly MainViewModel _vm;
    private readonly CircularProgressDrawable _progressDrawable;
    private CancellationTokenSource? _legendaryPulseCts;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _vm = viewModel;

        // Circular progress ring ayarla
        _progressDrawable = new CircularProgressDrawable
        {
            TrackColor = Color.FromArgb("#E8E5DF"),
            ProgressColor = Color.FromArgb("#7CB68E"),
            StrokeWidth = 6f
        };
        ProgressRing.Drawable = _progressDrawable;

        // ViewModel event'lerine abone ol
        _vm.TreeStageChanged += OnTreeStageChanged;
        _vm.FocusCompleted += OnFocusCompleted;
        _vm.GaveUp += OnGaveUp;
        _vm.PropertyChanged += OnViewModelPropertyChanged;
    }

    /// <summary>
    /// TimerProgress değiştiğinde circular ring'i günceller.
    /// </summary>
    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.TimerProgress))
        {
            _progressDrawable.Progress = _vm.TimerProgress;
            ProgressRing.Invalidate();
        }
    }

    /// <summary>
    /// Ağaç aşaması değiştiğinde animasyon çalıştırır.
    /// </summary>
    private async void OnTreeStageChanged(object? sender, string stage)
    {
        switch (stage)
        {
            case "seed":
                await AnimateToSeed();
                break;
            case "sapling":
                await AnimateToSapling();
                break;
        }
    }

    private async void OnFocusCompleted(object? sender, TreeInfo tree)
    {
        await AnimateToFinal(tree);
    }

    private async void OnGaveUp(object? sender, EventArgs e)
    {
        await AnimateToDead();
    }

    // ===== ANİMASYON METODLARI =====

    private async Task AnimateToSeed()
    {
        StopLegendaryPulse();
        GlowEllipse.Opacity = 0;

        TreeImage.WidthRequest = 60;
        TreeImage.HeightRequest = 60;
        await TreeImage.ScaleTo(1.0, 300, Easing.CubicOut);
        await TreeImage.FadeTo(1.0, 300);
    }

    private async Task AnimateToSapling()
    {
        TreeImage.WidthRequest = 90;
        TreeImage.HeightRequest = 90;

        await Task.WhenAll(
            TreeImage.ScaleTo(1.3, 800, Easing.SpringOut),
            TreeImage.FadeTo(0.7, 400)
        );
    }

    private async Task AnimateToFinal(TreeInfo tree)
    {
        TreeImage.WidthRequest = 120;
        TreeImage.HeightRequest = 120;

        // Ağaç büyüme animasyonu
        await Task.WhenAll(
            TreeImage.ScaleTo(1.5, 800, Easing.SpringOut),
            TreeImage.FadeTo(1.0, 400)
        );

        // Bounce
        await TreeImage.ScaleTo(1.3, 200, Easing.CubicOut);

        // Glow efekti
        await Task.WhenAll(
            GlowEllipse.FadeTo(0.5, 600, Easing.CubicOut),
            GlowEllipse.ScaleTo(1.2, 600, Easing.CubicOut)
        );

        // Efsanevi ağaç pulse
        if (tree.ColorClass == "legendary")
        {
            StartLegendaryPulse();
        }
    }

    private async Task AnimateToDead()
    {
        StopLegendaryPulse();
        GlowEllipse.Opacity = 0;

        TreeImage.WidthRequest = 80;
        TreeImage.HeightRequest = 80;

        // Titreme
        await TreeImage.TranslateTo(-5, 0, 50);
        await TreeImage.TranslateTo(5, 0, 50);
        await TreeImage.TranslateTo(-3, 0, 50);
        await TreeImage.TranslateTo(0, 0, 50);

        // Solma
        await Task.WhenAll(
            TreeImage.ScaleTo(1.0, 500, Easing.CubicIn),
            TreeImage.FadeTo(0.5, 500)
        );
    }

    private async void StartLegendaryPulse()
    {
        StopLegendaryPulse();
        _legendaryPulseCts = new CancellationTokenSource();
        var token = _legendaryPulseCts.Token;

        try
        {
            while (!token.IsCancellationRequested)
            {
                await Task.WhenAll(
                    GlowEllipse.ScaleTo(1.4, 1000, Easing.SinInOut),
                    GlowEllipse.FadeTo(0.7, 1000, Easing.SinInOut)
                );

                if (token.IsCancellationRequested) break;

                await Task.WhenAll(
                    GlowEllipse.ScaleTo(1.0, 1000, Easing.SinInOut),
                    GlowEllipse.FadeTo(0.3, 1000, Easing.SinInOut)
                );
            }
        }
        catch (TaskCanceledException)
        {
            // Normal iptal
        }
    }

    private void StopLegendaryPulse()
    {
        _legendaryPulseCts?.Cancel();
        _legendaryPulseCts?.Dispose();
        _legendaryPulseCts = null;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopLegendaryPulse();
    }
}
