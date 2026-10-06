using System.Globalization;

namespace MiniOrman.Converters;

/// <summary>
/// Nadirlik renk sınıfını (common, rare, epic, legendary) MAUI Color nesnesine dönüştürür.
/// XAML'de binding ile kullanılır.
/// </summary>
public class RarityToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var colorClass = value as string;
        return colorClass switch
        {
            "common"    => Color.FromArgb("#66bb6a"),
            "rare"      => Color.FromArgb("#42a5f5"),
            "epic"      => Color.FromArgb("#ab47bc"),
            "legendary" => Color.FromArgb("#ffb300"),
            _           => Color.FromArgb("#90a4ae"),
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>
/// Nadirlik renk sınıfını kenarlık (border) rengine dönüştürür.
/// </summary>
public class RarityToBorderColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var colorClass = value as string;
        return colorClass switch
        {
            "common"    => Color.FromArgb("#a5d6a7"),
            "rare"      => Color.FromArgb("#90caf9"),
            "epic"      => Color.FromArgb("#ce93d8"),
            "legendary" => Color.FromArgb("#ffd54f"),
            _           => Color.FromArgb("#e0e0e0"),
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>
/// Nadirlik renk sınıfını gradyan başlangıç rengine dönüştürür.
/// </summary>
public class RarityToGradientStartConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var colorClass = value as string;
        return colorClass switch
        {
            "common"    => Color.FromArgb("#a5d6a7"),
            "rare"      => Color.FromArgb("#90caf9"),
            "epic"      => Color.FromArgb("#ce93d8"),
            "legendary" => Color.FromArgb("#ffd54f"),
            _           => Colors.Transparent,
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>
/// Bool değerini ters çevirir (IsRunning → !IsRunning gibi)
/// </summary>
public class InverseBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b ? !b : value;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b ? !b : value;
}
