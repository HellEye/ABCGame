using UnityEngine;

[CreateAssetMenu(
    fileName = "ResponsiveProfile",
    menuName = "Lumisie/UI/Responsive Profile")]
public class ResponsiveProfile : ScriptableObject
{
    [Header("Reference Resolution")]
    public Vector2 referenceResolution = new(1280, 800);

    [Header("Breakpoints")]
    public float phoneWidth = 700;
    public float tabletWidth = 1100;

    [Header("Scale")]
    public float minimumScale = 0.8f;
    public float maximumScale = 1.5f;

    [Space]

    public ResponsiveLayout desktop = new();
    public ResponsiveLayout tablet = new();
    public ResponsiveLayout phone = new();

    public ResponsiveLayout GetLayout(float width)
    {
        if (width < phoneWidth)
            return phone;

        if (width < tabletWidth)
            return tablet;

        return desktop;
    }

    public float GetGlobalScale(int width, int height)
    {
        float scale = Mathf.Min(
            width / referenceResolution.x,
            height / referenceResolution.y);

        return Mathf.Clamp(scale, minimumScale, maximumScale);
    }
}

[System.Serializable]
public class ResponsiveLayout
{
    [Header("Grid")]

    public int cardsPerRow = 3;

    public float spacing = 40;

    public float horizontalMargin = 40;

    public float verticalMargin = 40;

    [Header("Card")]

    public float minimumCardWidth = 260;

    public float maximumCardWidth = 320;

    public float aspectRatio = 1.5f;

    public float borderRadius = 30;

    public float imagePadding = 15;

    public float imageBottomPadding = 30;

    public float cornerSize = 52;

    public float heartSize = 22;

    public Vector2 heartOffset = new(7, 7);

    public float titleBottomPadding = 12;

    [Header("Fonts")]

    public int titleFont = 48;

    public int subtitleFont = 30;

    public int cardTitleFont = 24;

    public int buttonFont = 24;

    [Header("Buttons")]

    public float iconButtonSize = 96;

    public float arrowButtonSize = 90;

    [Header("Logo")]

    public Vector2 logoSize = new(520, 240);
}