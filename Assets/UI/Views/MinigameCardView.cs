using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MinigameCardView : VisualElement {
    VisualElement corner;
    Image cornerBackground;

    LevelMapping data;
    Image heart;

    Image thumbnail;

    Label title;

    public VisualElement Root { get; private set; }

    public LevelMapping Data {
        get => data;
        set {
            data = value;
            SetData(value);
        }
    }

    public event Action<MinigameCardView> Clicked;

    public void InitMinigameCardView(VisualTreeAsset template) {
        Root = template.Instantiate();
        Add(Root);
        thumbnail = Root.Q<Image>("Thumbnail");

        corner = Root.Q<VisualElement>("Corner");
        cornerBackground = Root.Q<Image>("CornerBackground");
        heart = Root.Q<Image>("Heart");

        title = Root.Q<Label>("Title");

        RegisterEvents();
    }

    void RegisterEvents() =>
        Root.RegisterCallback<ClickEvent>(_ => {
            Clicked?.Invoke(this);
        });

    void SetData(LevelMapping data) {
        Title = data.levelName;
        Thumbnail = data.levelIcon;
    }

    public void SetFrame(Sprite cornerSprite, Sprite heartSprite) {
        HeartSprite = heartSprite;
        HeartColor = Color.white;
        CornerSprite = cornerSprite;
    }

    public void ApplyLayout(float width, ResponsiveLayout layout) {
        var height = width / layout.aspectRatio;

        Root.style.width = width;
        Root.style.height = height;

        corner.style.width = width * layout.cornerSize;
        corner.style.height = width * layout.cornerSize;

        heart.style.width = width * layout.heartSize;
        heart.style.height = width * layout.heartSize;

        heart.style.left = width * layout.heartOffset.x;
        heart.style.top = height * layout.heartOffset.y;

        var padding = width * layout.imagePadding;

        thumbnail.style.left = padding;
        thumbnail.style.right = padding;
        thumbnail.style.top = padding;
        thumbnail.style.bottom = height * layout.imageBottomPadding;

        title.style.bottom = height * layout.titleBottomPadding;

        title.style.fontSize = layout.cardTitleFont;
    }

    #region Design Constants

    // Original design size: 300 x 200

    const float AspectRatio = 3f / 2f;

    const float CornerRatio = 52f / 300f;
    const float HeartRatio = 22f / 300f;

    const float ImagePaddingRatio = 15f / 300f;

    const float TitleBottomRatio = 12f / 200f;

    const float HeartOffsetX = 7f / 300f;
    const float HeartOffsetY = 7f / 200f;

    #endregion

    #region Properties

    public string Title { get => title.text; set => title.text = value; }

    public Sprite Thumbnail { set => thumbnail.sprite = value; }

    public Sprite CornerSprite { set => cornerBackground.sprite = value; }

    public Sprite HeartSprite { set => heart.sprite = value; }

    public Color HeartColor { set => heart.tintColor = value; }

    public bool Visible {
        set =>
            Root.style.display =
                value ? DisplayStyle.Flex : DisplayStyle.None;
    }

    #endregion
}