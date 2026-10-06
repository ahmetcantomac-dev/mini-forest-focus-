using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiniOrman.Models;
using MiniOrman.Services;
using Plugin.LocalNotification;

namespace MiniOrman.ViewModels;

/// <summary>
/// Ana sayfa ViewModel'i. Timer mantığı, ağaç seçimi, istatistik yönetimi.
/// HTML'deki tüm JavaScript mantığının C# karşılığı.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly StorageService _storage;
    private readonly Random _random = new();
    private IDispatcherTimer? _timer;
    private int _timeLeft;
    private int _totalTime;
    private TreeInfo? _currentTree;
    private ForestStats _stats;

    // ===== OBSERVABLE PROPERTIES =====

    [ObservableProperty]
    private string _timerText = "00:00";

    [ObservableProperty]
    private string _treeIconSource = "seed.png";

    [ObservableProperty]
    private string _treeName = string.Empty;

    [ObservableProperty]
    private string _rarityText = string.Empty;

    [ObservableProperty]
    private string _rarityColorClass = string.Empty;

    [ObservableProperty]
    private bool _isRarityVisible;

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private bool _isIdle = true;

    [ObservableProperty]
    private string _statusText = "Süreyi ayarla, bakalım toprakta ne gizli?";

    [ObservableProperty]
    private Color _statusColor = Color.FromArgb("#78909c");

    [ObservableProperty]
    private int _successCount;

    [ObservableProperty]
    private int _failCount;

    [ObservableProperty]
    private int _minuteInput = 25;

    [ObservableProperty]
    private int _secondInput = 0;

    [ObservableProperty]
    private string _startButtonText = "Tohumu Ek ve Başla";

    [ObservableProperty]
    private bool _hasHistory;

    [ObservableProperty]
    private string _glowColorClass = string.Empty;

    [ObservableProperty]
    private bool _isGlowVisible;

    /// <summary>Ağaç büyüme aşaması: "seed", "sapling", "final", "dead"</summary>
    [ObservableProperty]
    private string _treeStage = "seed";

    /// <summary>Ağaç görsel boyutu (animasyon için)</summary>
    [ObservableProperty]
    private double _treeScale = 1.0;

    /// <summary>Ağaç opaklığı</summary>
    [ObservableProperty]
    private double _treeOpacity = 1.0;

    /// <summary>Timer ilerleme değeri (0.0 - 1.0, dairesel progress ring için)</summary>
    [ObservableProperty]
    private double _timerProgress;

    /// <summary>Koleksiyon geçmişi</summary>
    public ObservableCollection<HistoryItem> HistoryItems { get; } = new();

    // ===== EVENT'LER (View animasyonları için) =====
    
    /// <summary>Ağaç aşaması değiştiğinde View'a bildirir</summary>
    public event EventHandler<string>? TreeStageChanged;

    /// <summary>Timer tamamlandığında View'a bildirir</summary>
    public event EventHandler<TreeInfo>? FocusCompleted;

    /// <summary>Pes edildiğinde View'a bildirir</summary>
    public event EventHandler? GaveUp;

    // ===== CONSTRUCTOR =====

    public MainViewModel(StorageService storage)
    {
        _storage = storage;
        _stats = _storage.LoadStats();
        UpdateStatsUI();
    }

    // ===== KOMUTLAR =====

    [RelayCommand]
    private void StartFocus()
    {
        // Android 13+ bildirim izni
#if ANDROID
        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)
        {
            var activity = Platform.CurrentActivity;
            if (activity != null && activity.CheckSelfPermission(Android.Manifest.Permission.PostNotifications) != Android.Content.PM.Permission.Granted)
            {
                activity.RequestPermissions(new[] { Android.Manifest.Permission.PostNotifications }, 0);
            }
        }
#endif

        int totalSeconds = (MinuteInput * 60) + SecondInput;

        if (totalSeconds <= 0)
        {
            StatusText = "⚠️ Lütfen geçerli bir süre girin!";
            StatusColor = Color.FromArgb("#E88B8B");
            return;
        }

        // Rastgele ağaç seç
        _currentTree = PickRandomTree();
        _totalTime = totalSeconds;
        _timeLeft = totalSeconds;

        // UI durumunu güncelle
        IsRunning = true;
        IsIdle = false;
        StatusText = "Odaklan... Toprakta bir hareketlilik var.";
        StatusColor = Color.FromArgb("#808B8C");
        TreeName = "Filizleniyor...";
        IsRarityVisible = false;
        TreeIconSource = "seed.png";
        TreeStage = "seed";
        TreeScale = 1.0;
        TreeOpacity = 1.0;
        IsGlowVisible = false;
        GlowColorClass = string.Empty;
        TimerProgress = 0;

        TimerText = FormatTime(_timeLeft);

        // View'a aşama değişikliğini bildir
        TreeStageChanged?.Invoke(this, "seed");

        // Timer başlat
        StartTimer();
    }

    [RelayCommand]
    private async Task GiveUp()
    {
        // Onay dialog'u — HTML'deki confirm() karşılığı
        bool confirmed = await Application.Current!.MainPage!.DisplayAlert(
            "Emin Misin?",
            "Bu nadide parça kuruyup gidecek!",
            "Evet, Pes Et",
            "Vazgeç"
        );

        if (!confirmed) return;

        StopTimer();

        // Pes Et görselini ayarla
        TreeIconSource = "dead_flower.png";
        TreeStage = "dead";
        TreeName = "Kurumuş Umutlar";
        StatusText = "Odaklanma bozuldu, bitki kurudu.";
        StatusColor = Color.FromArgb("#c62828");
        IsGlowVisible = false;

        // View'a bildir (animasyon için)
        GaveUp?.Invoke(this, EventArgs.Empty);

        // İstatistik güncelle
        _stats.Fail++;
        _storage.SaveStats(_stats);
        UpdateStatsUI();

        // Kısa gecikme sonrası UI'ı sıfırla
        await Task.Delay(1500);
        ResetUI();
    }

    // ===== TIMER MANTIĞI =====

    private void StartTimer()
    {
        StopTimer();
        
        _timer = Application.Current!.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    private void StopTimer()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Tick -= OnTimerTick;
            _timer = null;
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _timeLeft--;
        TimerText = FormatTime(_timeLeft);

        // Progress güncelle (dairesel ring için)
        if (_totalTime > 0)
        {
            TimerProgress = (double)(_totalTime - _timeLeft) / _totalTime;

            double percent = TimerProgress * 100;
            UpdateTreeGrowth(percent);
        }

        // Süre bitti
        if (_timeLeft <= 0)
        {
            FinishFocus();
        }
    }

    /// <summary>
    /// Büyüme ilerlemesine göre ağaç aşamasını günceller.
    /// HTML'deki updateTreeGrowth() fonksiyonunun karşılığı.
    /// </summary>
    private void UpdateTreeGrowth(double percent)
    {
        if (percent >= 20 && _currentTree != null && TreeStage == "seed")
        {
            // Fidan aşamasına geç
            TreeIconSource = $"{_currentTree.IconSource}.png";
            TreeStage = "sapling";
            TreeOpacity = 0.7;
            TreeStageChanged?.Invoke(this, "sapling");
        }
    }

    /// <summary>
    /// Timer başarıyla tamamlandığında çağrılır.
    /// HTML'deki finishFocus() fonksiyonunun karşılığı.
    /// </summary>
    private void FinishFocus()
    {
        StopTimer();

        if (_currentTree == null) return;

        // Son aşama
        TreeIconSource = $"{_currentTree.IconSource}.png";
        TreeStage = "final";
        TreeOpacity = 1.0;

        TreeName = _currentTree.Name;
        RarityText = _currentTree.Rarity;
        RarityColorClass = _currentTree.ColorClass;
        IsRarityVisible = true;

        GlowColorClass = _currentTree.ColorClass;
        IsGlowVisible = true;

        StatusText = $"Harika! {_currentTree.Rarity} bir tür keşfettin!";
        StatusColor = Color.FromArgb("#2e7d32");

        // View'a bildir (animasyon için)
        FocusCompleted?.Invoke(this, _currentTree);

        // İstatistik güncelle
        _stats.Success++;
        _stats.History.Add(new HistoryItem
        {
            IconSource = _currentTree.IconSource,
            Rarity = _currentTree.ColorClass,
            Name = _currentTree.Name
        });
        _storage.SaveStats(_stats);
        UpdateStatsUI();

        // Local notification gönder
        SendCompletionNotification(_currentTree);

        // UI'ı sıfırla (butonlar)
        ResetUI();
    }

    // ===== YARDIMCI METODLAR =====

    /// <summary>
    /// Ağırlıklı rastgele ağaç seçimi.
    /// HTML'deki pickRandomTree() fonksiyonunun birebir karşılığı.
    /// </summary>
    private TreeInfo PickRandomTree()
    {
        int totalWeight = TreeDatabase.Trees.Sum(t => t.Weight);
        double randomValue = _random.NextDouble() * totalWeight;

        foreach (var tree in TreeDatabase.Trees)
        {
            if (randomValue < tree.Weight)
                return tree;
            randomValue -= tree.Weight;
        }

        return TreeDatabase.Trees[0];
    }

    /// <summary>
    /// Saniyeyi "MM:SS" formatına dönüştürür.
    /// HTML'deki formatTime() fonksiyonunun karşılığı.
    /// </summary>
    private static string FormatTime(int seconds)
    {
        int m = seconds / 60;
        int s = seconds % 60;
        return $"{m:D2}:{s:D2}";
    }

    /// <summary>
    /// UI'ı başlangıç durumuna döndürür.
    /// HTML'deki resetUI() fonksiyonunun karşılığı.
    /// </summary>
    private void ResetUI()
    {
        IsRunning = false;
        IsIdle = true;
        TimerProgress = 0;
        TimerText = "00:00";
        StartButtonText = "Yeni Bir Tohum Ek";
    }

    /// <summary>
    /// İstatistik UI'ını günceller.
    /// HTML'deki updateStatsUI() fonksiyonunun karşılığı.
    /// </summary>
    private void UpdateStatsUI()
    {
        SuccessCount = _stats.Success;
        FailCount = _stats.Fail;

        HistoryItems.Clear();
        
        // Son 24 ağacı ters sırayla göster (en yeni başta)
        var recent = _stats.History.TakeLast(24).Reverse();
        foreach (var item in recent)
        {
            HistoryItems.Add(item);
        }

        HasHistory = HistoryItems.Count > 0;
    }

    /// <summary>
    /// Timer tamamlandığında local notification gönderir.
    /// </summary>
    private void SendCompletionNotification(TreeInfo tree)
    {
        try
        {
            var notification = new NotificationRequest
            {
                NotificationId = 1000,
                Title = "🌳 Ağacın Yetişti!",
                Description = $"{tree.Rarity} bir {tree.Name} büyüttün!",
                CategoryType = NotificationCategoryType.Status,
                Sound = DeviceInfo.Platform == DevicePlatform.Android ? "success_chime" : "success_chime.wav",
                Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions
                {
                    ChannelId = "mini_orman_timer_v2", // New channel ID to ensure sound update is picked up
                    Priority = Plugin.LocalNotification.AndroidOption.AndroidPriority.High,
                }
            };

            LocalNotificationCenter.Current.Show(notification);
        }
        catch
        {
            // Bildirim gönderilemezse sessizce devam et
        }
    }

    /// <summary>
    /// Arka plandan döndüğünde timer durumunu geri yükler.
    /// </summary>
    public void OnAppResuming(DateTime pausedAt)
    {
        if (!IsRunning || _timer == null) return;

        // Geçen süreyi hesapla
        var elapsed = (int)(DateTime.Now - pausedAt).TotalSeconds;
        _timeLeft = Math.Max(0, _timeLeft - elapsed);
        TimerText = FormatTime(_timeLeft);

        if (_timeLeft <= 0)
        {
            FinishFocus();
        }
        else
        {
            double progress = ((double)(_totalTime - _timeLeft) / _totalTime) * 100;
            UpdateTreeGrowth(progress);
        }
    }

    /// <summary>
    /// Uygulama arka plana gittiğinde çağrılır.
    /// Timer süresini kaydeder.
    /// </summary>
    public DateTime OnAppPausing()
    {
        return DateTime.Now;
    }
}
