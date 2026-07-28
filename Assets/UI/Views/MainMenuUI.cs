using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Reflex.Attributes;
using System.Linq;

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
    [Inject] GameLoader gameLoader;
    [Inject] DifficultyHolder difficultyHolder;
    Popup difficultyPopup;

    private UIDocument document;
    private MinigameSelectionView selectionView;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
        difficultyPopup = document.rootVisualElement.Q<Popup>("DifficultyPopup");
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
        selectionView.SetCards(minigameRegistry.Mappings, heartSprite, cornerSprite);
    }

    private void OnCardClicked(LevelMapping mapping)
    {
        SetupDifficultyButtons(mapping);
        difficultyPopup.IsOpen = true;
        //Debug.Log("Clicked");
    }
    void SetupDifficultyButtons(LevelMapping mapping) {
        var buttonList = document.rootVisualElement.Q<VisualElement>("DifficultyButtons");
        buttonList.Clear();
        for (var i = 0; i < mapping.difficultiesMappings.Length; i++)
        {
            var button = new Button();
            DifficultyMapping difficulty = mapping.difficultiesMappings[i];
            button.text = difficulty.difficultyData.Value.Name;
            button.AddToClassList("btn difficulty-btn");
            buttonList.Add(button);
            button.clicked += () => OnDifficultyButtonClicked(mapping, difficulty);
        }
    }
    
    void OnDifficultyButtonClicked(LevelMapping mapping, DifficultyMapping difficultyMap) {
        Debug.Log($"Scene {mapping.levelName} with difficulty {difficultyMap} selected");
        difficultyPopup.IsOpen = false;
        var scene = mapping.sceneReference;
        var difficulty = difficultyMap.difficultyData.Value;
        Debug.Log($"Scene {scene} with difficulty {difficulty} selected");
        if (scene == null || difficulty == null) return;
        difficultyHolder.selectedDifficulty = difficulty;
        difficultyHolder.selectedScene = scene;
        gameLoader.LoadGameplaySceneFromHolder().Forget();
    }
}