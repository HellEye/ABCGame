using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Reflex.Attributes;

[RequireComponent(typeof(UIDocument))]
public class MainMenuUI : MonoBehaviour
{
    [Header("Templates")]
    [SerializeField] private VisualTreeAsset minigameCardTemplate;

    [Header("Demo Sprites")]
    [SerializeField] private Sprite placeholderThumbnail;
    [SerializeField] private Sprite cornerSprite;
    [SerializeField] private Sprite heartSprite;
    
    [Inject] MinigameRegistry minigameRegistry;

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
        

        selectionView.SetCards(minigameRegistry.Mappings);
    }

    private void OnCardClicked(int index)
    {
        Debug.Log($"Clicked card {index}");
    }
}