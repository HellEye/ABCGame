using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MinigameCardView
{
    #region Design Constants

    // Original design size: 300 x 200

    private const float AspectRatio = 3f / 2f;

    private const float CornerRatio = 52f / 300f;
    private const float HeartRatio = 22f / 300f;

    private const float ImagePaddingRatio = 15f / 300f;

    private const float TitleBottomRatio = 12f / 200f;

    private const float HeartOffsetX = 7f / 300f;
    private const float HeartOffsetY = 7f / 200f;

    #endregion

    public event Action<MinigameCardView> Clicked;

    public VisualElement Root => root;

    public MinigameCardData Data { get; private set; }

    private readonly VisualElement root;

    private readonly Image thumbnail;
    private readonly VisualElement corner;
    private readonly Image cornerBackground;
    private readonly Image heart;

    private readonly Label title;

    public MinigameCardView(VisualTreeAsset template)
    {
        root = template.Instantiate();

        thumbnail = root.Q<Image>("Thumbnail");

        corner = root.Q<VisualElement>("Corner");
        cornerBackground = root.Q<Image>("CornerBackground");
        heart = root.Q<Image>("Heart");

        title = root.Q<Label>("Title");

        RegisterEvents();
    }

    private void RegisterEvents()
    {
        root.RegisterCallback<ClickEvent>(_ =>
        {
            Clicked?.Invoke(this);
        });

        root.RegisterCallback<PointerEnterEvent>(_ =>
        {
            root.AddToClassList("hover");
        });

        root.RegisterCallback<PointerLeaveEvent>(_ =>
        {
            root.RemoveFromClassList("hover");
        });
    }

    public void SetData(MinigameCardData data)
    {
        Data = data;

        Title = data.title;
        Thumbnail = data.thumbnail;
        CornerSprite = data.cornerSprite;
        HeartSprite = data.heartSprite;
        HeartColor = data.heartColor;
    }

    #region Properties

    public string Title
    {
        get => title.text;
        set => title.text = value;
    }

    public Sprite Thumbnail
    {
        set => thumbnail.sprite = value;
    }

    public Sprite CornerSprite
    {
        set => cornerBackground.sprite = value;
    }

    public Sprite HeartSprite
    {
        set => heart.sprite = value;
    }

    public Color HeartColor
    {
        set => heart.tintColor = value;
    }

    public bool Visible
    {
        set => root.style.display =
            value ? DisplayStyle.Flex : DisplayStyle.None;
    }

    #endregion

    public void ApplyLayout(float width, ResponsiveLayout layout)
    {
        float height = width / layout.aspectRatio;
        
        root.style.width = width;
        root.style.height = height;

        corner.style.width = width * layout.cornerSize;
        corner.style.height = width * layout.cornerSize;

        heart.style.width = width * layout.heartSize;
        heart.style.height = width * layout.heartSize;

        heart.style.left = width * layout.heartOffset.x;
        heart.style.top = height * layout.heartOffset.y;

        float padding = width * layout.imagePadding;

        thumbnail.style.left = padding;
        thumbnail.style.right = padding;
        thumbnail.style.top = padding;
        thumbnail.style.bottom = height * layout.imageBottomPadding;

        title.style.bottom = height * layout.titleBottomPadding;

        title.style.fontSize = layout.cardTitleFont;
    }
}