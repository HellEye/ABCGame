using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuUI : MonoBehaviour
{
    [Header("Templates")]
    [SerializeField] private VisualTreeAsset minigameCardTemplate;

    [Header("Demo Sprites")]
    [SerializeField] private Sprite placeholderThumbnail;
    [SerializeField] private Sprite cornerSprite;
    [SerializeField] private Sprite heartSprite;

    private UIDocument document;
    private MinigameSelectionView selectionView;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    private void Start()
    {
        CreateSelectionView();

        LoadDemoCards();
    }

    private void CreateSelectionView()
    {
        selectionView = new MinigameSelectionView(
            document.rootVisualElement,
            minigameCardTemplate);

        selectionView.CardClicked += OnCardClicked;
    }

    private void LoadDemoCards()
    {
        List<MinigameCardData> cards = new();

        for (int i = 0; i < 8; i++)
        {
            cards.Add(new MinigameCardData
            {
                title = $"Minigame {i + 1}",
                thumbnail = placeholderThumbnail,
                cornerSprite = cornerSprite,
                heartSprite = heartSprite,
                heartColor = Color.white
            });
        }

        selectionView.SetCards(cards);
    }

    private void OnCardClicked(int index)
    {
        Debug.Log($"Clicked card {index}");
    }
}