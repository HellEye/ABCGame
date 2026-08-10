using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Reflex.Attributes;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UIDocument))]
public class MainMenuUI : MonoBehaviour
{
    [Header("Templates")]
    [SerializeField] private VisualTreeAsset minigameCardTemplate;

    [Header("Demo Sprites")]
    [SerializeField] private Sprite placeholderThumbnail;
    [SerializeField] private Sprite cornerSprite;
    [SerializeField] private Sprite heartSprite;
    
    private Dictionary <Difficulty, string> difficultiesButtonsClasses;
    
    [Inject] MinigameRegistry minigameRegistry;
    [Inject] GameLoader gameLoader;
    [Inject] DifficultyHolder difficultyHolder;
    Popup difficultyPopup;
    VisualElement mainMenuOpening;
    VisualElement mainMenuSceneButtons;

    private UIDocument document;
    private MinigameSelectionView selectionView;
    private MinigameCardView cardImageView;
    [Inject] InputSystem_Actions actions;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
        difficultyPopup = document.rootVisualElement.Q<Popup>("DifficultyPopup");
        mainMenuOpening = document.rootVisualElement.Q<VisualElement>("MainMenuOpening");
        mainMenuSceneButtons = document.rootVisualElement.Q<VisualElement>("MainMenuMinigame");
        mainMenuSceneButtons.style.display = DisplayStyle.None;
        difficultiesButtonsClasses = new();
    }

    private void Start()
    {
        actions.Player.Continue.performed += TransitionToSceneSelect;
        
        CreateSelectionView();

        LoadDemoCards();
        BuildPopupCard();
        difficultiesButtonsClasses.Add(Difficulty.Easy, "difficulty-btn-easy");
        difficultiesButtonsClasses.Add(Difficulty.Medium, "difficulty-btn-medium");
        difficultiesButtonsClasses.Add(Difficulty.Hard, "difficulty-btn-hard");
        SetupCancelButton();
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
        cardImageView.Data = mapping;
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
            SetupMinigameLabel(mapping);
            button.AddToClassList("difficulty-btn");
            button.AddToClassList(difficultiesButtonsClasses[difficulty.difficultyData.Value.Difficulty]);
            buttonList.Add(button);
            button.clicked += () => OnDifficultyButtonClicked(mapping, difficulty);
        }
    }

    void SetupCancelButton()
    {
        var cancelButton = document.rootVisualElement.Q<Button>("DifficultyPopupCloseBtn");
        cancelButton.clicked += () => OnCancelButtonClicked();
    }

    void SetupMinigameLabel(LevelMapping mapping)
    {
        var label = document.rootVisualElement.Q<Label>("DifficultyText");
        label.text = mapping.levelName;
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

    void OnCancelButtonClicked()
    {
        difficultyPopup.IsOpen = false;
    }

    private void BuildPopupCard()
    {
        cardImageView = new MinigameCardView();
        var cardImageContainer = document.rootVisualElement.Q<VisualElement>("difficultyIcon");
        cardImageView.InitMinigameCardView(minigameCardTemplate);
        
        cardImageView.SetFrame(cornerSprite, heartSprite);

        cardImageContainer.Add(cardImageView);
    }

    void OnEnable()
    {
        actions.Player.Continue.Enable();
    }

    void TransitionToSceneSelect(InputAction.CallbackContext ctx)
    {
        mainMenuOpening.style.display = DisplayStyle.None;
        mainMenuSceneButtons.style.display = DisplayStyle.Flex;
        actions.Player.Continue.Disable();
    }
}