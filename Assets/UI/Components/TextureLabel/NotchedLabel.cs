using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class NotchedLabel : VisualElement {
    static readonly CustomStyleProperty<Color> HighlightColorProperty = new("--highlight-color");
    static readonly CustomStyleProperty<float> NotchRadiusProperty = new("--notch-radius");
    static readonly CustomStyleProperty<float> CornerRadiusProperty = new("--corner-radius");
    static readonly CustomStyleProperty<float> BorderWidthProperty = new("--border-width");
    static readonly CustomStyleProperty<float> HighlightWidthProperty = new("--highlight-width");
    static readonly CustomStyleProperty<float> NotchRightProperty = new("--notch-right");

    // Geometry computed in Draw, used by the shape helpers.
    Rect body;
    float borderRadius = 2;
    Vector2 center;
    float cornerRadius = 10;
    Color highlightColor = new(1f, 0.82f, 0.3f);
    float highlightWidth = 2;
    float m_CornerRadius;
    float m_NotchRadius;

    float notchRadius = 16;
    bool right;

    public NotchedLabel() {
        AddToClassList("notched-label");
        TextLabel.AddToClassList("notched-label__text");
        Icon.AddToClassList("notched-label__icon");

        var content = new VisualElement();
        content.AddToClassList("notched-label__content");
        content.Add(TextLabel);
        content.Add(Icon);
        Add(content);

        generateVisualContent += Draw;
        RegisterCallback<CustomStyleResolvedEvent>(OnStyleResolved);
    }

    [UxmlAttribute] public string Text { get => TextLabel.text; set => TextLabel.text = value; }

    public Label TextLabel { get; } = new() { name = "text" };

    public VisualElement Icon { get; } = new() { name = "icon" };

    float Pad => borderRadius + highlightWidth;

    void OnStyleResolved(CustomStyleResolvedEvent e) {
        var cs = e.customStyle;
        if (cs.TryGetValue(HighlightColorProperty, out var h)) highlightColor = h;
        if (cs.TryGetValue(NotchRadiusProperty, out var n)) notchRadius = n;
        if (cs.TryGetValue(CornerRadiusProperty, out var c)) cornerRadius = c;
        if (cs.TryGetValue(BorderWidthProperty, out var b)) borderRadius = b;
        if (cs.TryGetValue(HighlightWidthProperty, out var hw)) highlightWidth = hw;
        if (cs.TryGetValue(NotchRightProperty, out var nr)) right = nr > 0.5f;

        // Constant insets: selecting never shifts the layout.
        style.paddingTop = style.paddingBottom = Pad;
        style.paddingLeft = Pad + (right ? 0 : notchRadius);
        style.paddingRight = Pad + (right ? notchRadius : 0);

        MarkDirtyRepaint();
    }

    void Draw(MeshGenerationContext mgc) {
        float w = layout.width, h = layout.height, pad = Pad, r = notchRadius;
        body = right
            ? Rect.MinMaxRect(pad, pad, w - pad - r, h - pad)
            : Rect.MinMaxRect(pad + r, pad, w - pad, h - pad);
        center = new(right ? body.xMax : body.xMin, body.center.y);
        m_CornerRadius = Mathf.Min(cornerRadius, body.height * 0.5f);
        m_NotchRadius = r;

        var p = mgc.painter2D;
        if (ClassListContains("selected"))
            Outline(p, highlightColor, 2f * Pad);
        Outline(p, resolvedStyle.borderTopColor, 2f * borderRadius);
        Fill(p, resolvedStyle.backgroundColor);
    }

    // Stroke is centered on the path, so the inner half is covered by Fill.
    void Outline(Painter2D p, Color color, float width) {
        p.strokeColor = color;
        p.lineWidth = width;
        p.BeginPath();
        RoundRect(p);
        p.Stroke();
        p.BeginPath();
        Circle(p);
        p.Stroke();
    }

    // Separate fills avoid winding-rule holes where the circle overlaps.
    void Fill(Painter2D p, Color color) {
        p.fillColor = color;
        p.BeginPath();
        RoundRect(p);
        p.Fill();
        p.BeginPath();
        Circle(p);
        p.Fill();
    }

    void RoundRect(Painter2D p) {
        var rect = body;
        var corner = m_CornerRadius;
        p.MoveTo(new(rect.xMin + corner, rect.yMin));
        p.LineTo(new(rect.xMax - corner, rect.yMin));
        p.ArcTo(new(rect.xMax, rect.yMin), new(rect.xMax, rect.yMin + corner), corner);
        p.LineTo(new(rect.xMax, rect.yMax - corner));
        p.ArcTo(new(rect.xMax, rect.yMax), new(rect.xMax - corner, rect.yMax), corner);
        p.LineTo(new(rect.xMin + corner, rect.yMax));
        p.ArcTo(new(rect.xMin, rect.yMax), new(rect.xMin, rect.yMax - corner), corner);
        p.LineTo(new(rect.xMin, rect.yMin + corner));
        p.ArcTo(new(rect.xMin, rect.yMin), new(rect.xMin + corner, rect.yMin), corner);
        p.ClosePath();
    }

    void Circle(Painter2D p) {
        // Two half-arcs, which is more reliable than one full arc.
        var c = center;
        var r = m_NotchRadius;
        p.MoveTo(c + new Vector2(r, 0));
        p.Arc(c, r, new(0, AngleUnit.Degree), new(180, AngleUnit.Degree));
        p.Arc(c, r, new(180, AngleUnit.Degree), new(360, AngleUnit.Degree));
        p.ClosePath();
    }
}