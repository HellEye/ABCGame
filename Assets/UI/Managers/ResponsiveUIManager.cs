using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ResponsiveUIManager : MonoBehaviour
{
    public static ResponsiveUIManager Instance { get; private set; }

    [Header("Responsive Profile")]
    [SerializeField]
    private ResponsiveProfile profile;
    private float currentScale;
    private ResponsiveLayout currentLayout;
    public ResponsiveLayout CurrentLayout => currentLayout;
    public float CurrentScale => currentScale;
    public ResponsiveProfile Profile => profile;

    public event Action LayoutChanged;

    private int lastWidth;
    private int lastHeight;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Refresh();
    }

    private void Update()
    {
        if (Screen.width != lastWidth ||
            Screen.height != lastHeight)
        {
            Refresh();
        }
    }

    public void Refresh()
    {
        if (profile == null)
        {
            Debug.LogWarning("Responsive Profile is not assigned.");
            return;
        }

        lastWidth = Screen.width;
        lastHeight = Screen.height;

        currentLayout = profile.GetLayout(Screen.width);

        currentScale = profile.GetGlobalScale(
            Screen.width,
            Screen.height);

        LayoutChanged?.Invoke();
    }

    /// <summary>
    /// Calculates the best card width based on available space.
    /// </summary>
    public float CalculateCardWidth(float availableWidth)
    {
        float width =
            (availableWidth -
            currentLayout.spacing * (currentLayout.cardsPerRow - 1))
            / currentLayout.cardsPerRow;

        return Mathf.Clamp(
            width,
            currentLayout.minimumCardWidth,
            currentLayout.maximumCardWidth);
    }

    public float CalculateCardHeight(float cardWidth)
    {
        return cardWidth / currentLayout.aspectRatio;
    }
}