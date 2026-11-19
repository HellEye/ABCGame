using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

public enum LabelMode {
    Text,
    Image
}

[UxmlElement]
public partial class TextureLabelOld : VisualElement {
    readonly VisualElement innerWrap;

    public bool hasSound;
    bool highlight;
    Texture image;
    LabelMode mode;
    VisualElement notchWrap;
    VisualElement soundWrap;

    string text;

    public TextureLabelOld() {
        AddToClassList("texture-label-outer");
        innerWrap = new();
        innerWrap.AddToClassList("texture-label-inner");
        Add(innerWrap);
        Refresh();
    }

    [UxmlAttribute("mode")]
    [CreateProperty]
    public LabelMode Mode {
        get => mode;
        set {
            if (value == mode) return;
            mode = value;
            NotifyPropertyChanged(nameof(Mode));
            Refresh();
        }
    }

    [UxmlAttribute("text")]
    [CreateProperty]
    public string Text {
        get => text;
        set {
            if (value == text) return;
            text = value;
            NotifyPropertyChanged(nameof(Text));
            Refresh();
        }
    }

    [UxmlAttribute("image")]
    [CreateProperty]
    public Texture Image {
        get => image;
        set {
            if (value == image) return;
            image = value;
            NotifyPropertyChanged(nameof(Image));
            Refresh();
        }
    }

    [UxmlAttribute("highlight")]
    [CreateProperty]
    public bool Highlight {
        get => highlight;
        set {
            if (value == highlight) return;
            highlight = value;
            NotifyPropertyChanged(nameof(Highlight));
            SetHighlight(highlight);
        }
    }

    [UxmlAttribute("has-sound")]
    [CreateProperty]
    public bool HasSound {
        get => hasSound;
        set {
            if (value == hasSound) return;
            hasSound = value;
            NotifyPropertyChanged(nameof(HasSound));
            DrawSound();
        }
    }

    void Refresh() {
        innerWrap.Clear();
        if (mode == LabelMode.Text) {
            var label = new Label(text);
            label.AddToClassList("texture-label-text");
            innerWrap.Add(label);
        }
        else if (mode == LabelMode.Image) {
            var imageElement = new Image {
                image = image
            };
            imageElement.AddToClassList("texture-label-image");
            innerWrap.Add(imageElement);
        }

        SetHighlight(highlight);
    }

    void DrawSound() {
        if (!hasSound) return;
    }

    public void SetHighlight(bool highlight) => EnableInClassList("texture-label-highlight", highlight);
}