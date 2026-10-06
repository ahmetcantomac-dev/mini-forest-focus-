namespace MiniOrman.Models;

/// <summary>
/// Koleksiyon geçmişindeki tek bir ağaç kaydı.
/// </summary>
public class HistoryItem
{
    /// <summary>Ağacın ikon dosya adı</summary>
    public string IconSource { get; set; } = string.Empty;

    /// <summary>Nadirlik renk sınıfı ("common", "rare", "epic", "legendary")</summary>
    public string Rarity { get; set; } = string.Empty;

    /// <summary>Ağacın adı</summary>
    public string Name { get; set; } = string.Empty;
}
