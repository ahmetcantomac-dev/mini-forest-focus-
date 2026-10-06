using MiniOrman.Models;

namespace MiniOrman.Services;

/// <summary>
/// 13 ağaç türünden oluşan veritabanı.
/// HTML'deki treeDatabase dizisinin C# karşılığı.
/// </summary>
public static class TreeDatabase
{
    public static readonly List<TreeInfo> Trees = new()
    {
        // Yaygın (Common) — Toplam ağırlık: 140
        new TreeInfo { Name = "Ulu Meşe",        IconSource = "oak_tree",        Rarity = "Yaygın",    ColorClass = "common",    Weight = 50 },
        new TreeInfo { Name = "Kara Çam",         IconSource = "pine_tree",       Rarity = "Yaygın",    ColorClass = "common",    Weight = 50 },
        new TreeInfo { Name = "Gür Kavak",        IconSource = "poplar_tree",     Rarity = "Yaygın",    ColorClass = "common",    Weight = 40 },

        // Nadir (Rare) — Toplam ağırlık: 90
        new TreeInfo { Name = "Sakura (Kiraz)",   IconSource = "cherry_blossom",  Rarity = "Nadir",     ColorClass = "rare",      Weight = 25 },
        new TreeInfo { Name = "Palmiye",          IconSource = "palm_tree",       Rarity = "Nadir",     ColorClass = "rare",      Weight = 25 },
        new TreeInfo { Name = "Çöl Kaktüsü",     IconSource = "cactus",          Rarity = "Nadir",     ColorClass = "rare",      Weight = 20 },
        new TreeInfo { Name = "Bambu Ormanı",     IconSource = "bamboo",          Rarity = "Nadir",     ColorClass = "rare",      Weight = 20 },

        // Destansı (Epic) — Toplam ağırlık: 30
        new TreeInfo { Name = "Kızıl Akçaağaç",  IconSource = "maple_tree",      Rarity = "Destansı",  ColorClass = "epic",      Weight = 12 },
        new TreeInfo { Name = "Muz Ağacı",        IconSource = "banana_tree",     Rarity = "Destansı",  ColorClass = "epic",      Weight = 10 },
        new TreeInfo { Name = "Büyülü Gül",       IconSource = "magic_rose",      Rarity = "Destansı",  ColorClass = "epic",      Weight = 8 },

        // Efsanevi (Legendary) — Toplam ağırlık: 6
        new TreeInfo { Name = "Kristal Hayat Ağacı", IconSource = "crystal_tree",  Rarity = "Efsanevi", ColorClass = "legendary", Weight = 3 },
        new TreeInfo { Name = "Antik Ejder Ağacı",   IconSource = "dragon_tree",   Rarity = "Efsanevi", ColorClass = "legendary", Weight = 2 },
        new TreeInfo { Name = "Kozmik Ağaç",         IconSource = "cosmic_tree",   Rarity = "Efsanevi", ColorClass = "legendary", Weight = 1 },
    };
}
