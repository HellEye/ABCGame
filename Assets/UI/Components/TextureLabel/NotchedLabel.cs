using System;
using UnityEngine;
using UnityEngine.UIElements;

public enum NotchedLabelMode {
    Text,
    Icon
}

[UxmlElement]
public partial class NotchedLabel : VisualElement {
    static readonly CustomStyleProperty<Color> HighlightColorProperty = new("--highlight-color");
    static readonly CustomStyleProperty<float> NotchRadiusProperty = new("--notch-radius");
    static readonly CustomStyleProperty<float> CornerRadiusProperty = new("--corner-radius");
    static readonly CustomStyleProperty<float> BorderWidthProperty = new("--border-width");
    static readonly CustomStyleProperty<float> HighlightWidthProperty = new("--highlight-width");
    static readonly CustomStyleProperty<float> NotchRightProperty = new("--notch-right");
    static readonly CustomStyleProperty<Color> FillColorProperty = new("--fill-color");
    static readonly CustomStyleProperty<Color> BorderColorProperty = new("--border-color");
    static readonly CustomStyleProperty<Color> NotchColorProperty = new("--notch-color");
    Texture actionIcon;

    // Geometry computed in Draw, used by the shape helpers.
    Rect body;
    Color borderColor = new(0.16f, 0.16f, 0.16f);
    float borderRadius = 2;
    Vector2 center;
    float cornerRadius = 10;
    Color fillColor = new(1f, 1f, 1f);
    Color highlightColor = new(1f, 0.82f, 0.3f);
    float highlightWidth = 2;
    Texture icon;

    float mCornerRadius;
    float mNotchRadius;
    NotchedLabelMode mode;
    Color notchColor = new(0.19f, 0.30f, 0.47f);

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

        ActionButton.AddToClassList("notched-label__action-button");
        ActionButton.Add(ActionIconImage);
        ActionIconImage.AddToClassList("notched-label__action-icon");
        content.Add(ActionButton);
        Add(content);

        generateVisualContent += Draw;
        RegisterCallback<CustomStyleResolvedEvent>(OnStyleResolved);
        Refresh();
    }

    [UxmlAttribute("text")] public string Text { get => TextLabel.text; set => TextLabel.text = value; }

    [UxmlAttribute("mode")]
    public NotchedLabelMode Mode {
        get => mode;
        set {
            if (mode == value) return;
            mode = value;
            Refresh();
        }
    }

    [UxmlAttribute("icon")]
    public Texture IconTexture {
        get => icon;
        set {
            if (icon == value) return;
            icon = value;
            Icon.image = icon;
            Refresh();
        }
    }

    [UxmlAttribute("action-icon")]
    public Texture ActionIcon {
        get => actionIcon;
        set {
            if (actionIcon == value) return;
            actionIcon = value;
            ActionIconImage.image = actionIcon;
            Refresh();
        }
    }

    public Label TextLabel { get; } = new() { name = "text" };

    public Image Icon { get; } = new() { name = "icon" };

    public Button ActionButton { get; } = new() { name = "action-button" };

    public Image ActionIconImage { get; } = new() { name = "action-icon" };

    float Pad => borderRadius + highlightWidth;

    public event Action Clicked { add => ActionButton.clicked += value; remove => ActionButton.clicked -= value; }

    void Refresh() {
        TextLabel.style.display = mode == NotchedLabelMode.Text ? DisplayStyle.Flex : DisplayStyle.None;
        Icon.style.display = mode == NotchedLabelMode.Icon ? DisplayStyle.Flex : DisplayStyle.None;
        ActionButton.style.display = actionIcon != null ? DisplayStyle.Flex : DisplayStyle.None;
        MarkDirtyRepaint();
    }

    void OnStyleResolved(CustomStyleResolvedEvent e) {
        var cs = e.customStyle;
        if (cs.TryGetValue(HighlightColorProperty, out var h)) highlightColor = h;
        if (cs.TryGetValue(NotchRadiusProperty, out var n)) notchRadius = n;
        if (cs.TryGetValue(CornerRadiusProperty, out var c)) cornerRadius = c;
        if (cs.TryGetValue(BorderWidthProperty, out var b)) borderRadius = b;
        if (cs.TryGetValue(HighlightWidthProperty, out var hw)) highlightWidth = hw;
        if (cs.TryGetValue(NotchRightProperty, out var nr)) right = nr > 0.5f;
        if (cs.TryGetValue(FillColorProperty, out var fc)) fillColor = fc;
        if (cs.TryGetValue(BorderColorProperty, out var bc)) borderColor = bc;
        if (cs.TryGetValue(NotchColorProperty, out var nc)) notchColor = nc;

        // Constant insets: selecting never shifts the layout.
        style.paddingTop = style.paddingBottom = Pad;
        style.paddingLeft = Pad + (right ? 0 : notchRadius);
        style.paddingRight = Pad + (right ? notchRadius : 0);

        MarkDirtyRepaint();
    }

    #region Painter

    void Draw(MeshGenerationContext mgc) {
        float w = layout.width, h = layout.height, pad = Pad, r = notchRadius;
        body = right
            ? Rect.MinMaxRect(pad, pad, w - pad - r, h - pad)
            : Rect.MinMaxRect(pad + r, pad, w - pad, h - pad);
        center = new(right ? body.xMax : body.xMin, body.center.y);
        mCornerRadius = Mathf.Min(cornerRadius, body.height * 0.5f);
        mNotchRadius = r;

        var p = mgc.painter2D;
        if (ClassListContains("selected")) {
            Outline(p, highlightColor, 2f * Pad);
            p.fillColor = highlightColor;
            p.BeginPath();
            Circle(p, mNotchRadius + borderRadius + highlightWidth);
            p.Fill();
        }

        Outline(p, borderColor, 2f * borderRadius);
        Fill(p, fillColor);
    }

    // Stroke is centered on the path, so the inner half is covered by Fill.
    void Outline(Painter2D p, Color color, float width) {
        p.strokeColor = color;
        p.lineWidth = width;
        p.BeginPath();
        RoundRect(p);
        p.Stroke();
    }

    // The highlight circle is drawn first, before the border and label body.
    void Fill(Painter2D p, Color color) {
        p.fillColor = borderColor;
        p.BeginPath();
        Circle(p, mNotchRadius + borderRadius);
        p.Fill();

        p.fillColor = color;
        p.BeginPath();
        RoundRect(p);
        p.Fill();

        p.fillColor = notchColor;
        p.BeginPath();
        Circle(p, mNotchRadius);
        p.Fill();

        p.fillColor = borderColor;
        p.BeginPath();
        Circle(p, mNotchRadius * 0.5f);
        p.Fill();
    }

    void RoundRect(Painter2D p) {
        var rect = body;
        var corner = mCornerRadius;
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

    void Circle(Painter2D p, float radius) {
        // Two half-arcs, which is more reliable than one full arc.
        var c = center;
        p.MoveTo(c + new Vector2(radius, 0));
        p.Arc(c, radius, new(0, AngleUnit.Degree), new(180, AngleUnit.Degree));
        p.Arc(c, radius, new(180, AngleUnit.Degree), new(360, AngleUnit.Degree));
        p.ClosePath();
    }

    #endregion
}