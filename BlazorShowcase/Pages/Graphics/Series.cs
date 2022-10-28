namespace BlazorShowcase.Pages.Graphics;

public sealed class Series
{
    public string Label { get; set; }
    public System.Drawing.Color Color { get; set; }
    public string JsColor => $"rgb({Color.R}, {Color.G}, {Color.B}, {Color.A})";
    public int PointRadius { get; set; } = 2;
    public bool ShowLine { get; set; }
    public int LineWidth { get; set; } = 1;
    public bool UseY2 { get; set; }
    public bool Hidden { get; set; }
    public PointType PointType { get; set; }
    public string PointStyle => PointType switch
    {
        PointType.Circle => "circle",
        PointType.Cross => "cross",
        PointType.X => "crossRot",
        PointType.Rect => "rect",
        PointType.RectRounded => "rectRounded",
        PointType.RectRotated => "rectRot",
        PointType.Star => "star",
        PointType.Triangle => "triangle",
        _ => string.Empty,
    };
}