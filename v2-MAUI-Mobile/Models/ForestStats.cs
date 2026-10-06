namespace MiniOrman.Models;

/// <summary>
/// Kullanıcının toplam istatistiklerini tutan model.
/// Preferences API ile JSON olarak saklanır.
/// </summary>
public class ForestStats
{
    /// <summary>Başarıyla yetiştirilen ağaç sayısı</summary>
    public int Success { get; set; }

    /// <summary>Pes edilen (kuruyan) ağaç sayısı</summary>
    public int Fail { get; set; }

    /// <summary>Son 24 ağacın geçmişi</summary>
    public List<HistoryItem> History { get; set; } = new();
}
