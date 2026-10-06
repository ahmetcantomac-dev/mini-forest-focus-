namespace MiniOrman.Models;

/// <summary>
/// Bir ağaç türünün tüm özelliklerini tanımlar.
/// HTML'deki treeDatabase dizisinin C# karşılığı.
/// </summary>
public class TreeInfo
{
    /// <summary>Ağacın Türkçe adı (ör: "Ulu Meşe")</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Ağacın ikon dosya adı (Resources/Images/ altında, uzantısız)</summary>
    public string IconSource { get; set; } = string.Empty;

    /// <summary>Nadirlik seviyesi metni (ör: "Yaygın", "Nadir", "Destansı", "Efsanevi")</summary>
    public string Rarity { get; set; } = string.Empty;

    /// <summary>Renk sınıfı tanımlayıcısı (ör: "common", "rare", "epic", "legendary")</summary>
    public string ColorClass { get; set; } = string.Empty;

    /// <summary>Ağırlıklı rastgele seçim için ağırlık değeri</summary>
    public int Weight { get; set; }
}
