using Microsoft.Maui.Graphics;

namespace MiniOrman.Controls;

/// <summary>
/// Dairesel progress ring çizici.
/// Timer ilerlemesini zarif bir halka olarak gösterir.
/// </summary>
public class CircularProgressDrawable : IDrawable
{
    /// <summary>İlerleme değeri (0.0 - 1.0)</summary>
    public double Progress { get; set; }

    /// <summary>Halka arka plan rengi</summary>
    public Color TrackColor { get; set; } = Color.FromArgb("#E8E5DF");

    /// <summary>İlerleme rengi</summary>
    public Color ProgressColor { get; set; } = Color.FromArgb("#7CB68E");

    /// <summary>Halka kalınlığı</summary>
    public float StrokeWidth { get; set; } = 6f;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float size = Math.Min(dirtyRect.Width, dirtyRect.Height);
        float radius = (size - StrokeWidth * 2) / 2;
        float cx = dirtyRect.Width / 2;
        float cy = dirtyRect.Height / 2;

        // Anti-aliasing
        canvas.Antialias = true;

        // Arka plan halkası (track)
        canvas.StrokeColor = TrackColor;
        canvas.StrokeSize = StrokeWidth;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.DrawCircle(cx, cy, radius);

        // İlerleme arkı
        if (Progress > 0.001)
        {
            canvas.StrokeColor = ProgressColor;
            canvas.StrokeSize = StrokeWidth;
            canvas.StrokeLineCap = LineCap.Round;

            float startAngle = 90f;  // Üstten başla (MAUI koordinat sistemi)
            float endAngle = 90f - (float)(360.0 * Progress);

            canvas.DrawArc(
                cx - radius, cy - radius,
                radius * 2, radius * 2,
                startAngle, endAngle,
                clockwise: true,
                closed: false
            );
        }
    }
}
