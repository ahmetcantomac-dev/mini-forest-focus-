using System.Text.Json;
using MiniOrman.Models;

namespace MiniOrman.Services;

/// <summary>
/// Preferences API kullanarak ForestStats verisini kalıcı olarak saklar.
/// HTML'deki localStorage.getItem/setItem karşılığı.
/// </summary>
public class StorageService
{
    private const string StatsKey = "forestStatsV6";

    /// <summary>
    /// Kaydedilmiş istatistikleri yükler. Kayıt yoksa yeni boş nesne döner.
    /// </summary>
    public ForestStats LoadStats()
    {
        try
        {
            var json = Preferences.Get(StatsKey, string.Empty);
            if (string.IsNullOrEmpty(json))
                return new ForestStats();

            return JsonSerializer.Deserialize<ForestStats>(json) ?? new ForestStats();
        }
        catch
        {
            return new ForestStats();
        }
    }

    /// <summary>
    /// İstatistikleri Preferences'a JSON olarak kaydeder.
    /// </summary>
    public void SaveStats(ForestStats stats)
    {
        var json = JsonSerializer.Serialize(stats);
        Preferences.Set(StatsKey, json);
    }

    /// <summary>
    /// Tüm istatistikleri sıfırlar.
    /// </summary>
    public void ClearStats()
    {
        Preferences.Remove(StatsKey);
    }
}
