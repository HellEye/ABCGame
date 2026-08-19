using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuUI : MonoBehaviour {
    [Header("Templates")]
    [SerializeField] VisualTreeAsset minigameCardTemplate;

    [Header("Demo Sprites")]
    [SerializeField] Sprite placeholderThumbnail;

    [SerializeField] Sprite cornerSprite;
    [SerializeField] Sprite heartSprite;
    [Inject] InputSystem_Actions actions;
    MinigameCardView cardImageView;

    Dictionary<Difficulty, string> difficultiesButtonsClasses;
    [Inject] DifficultyHolder difficultyHolder;
    Popup difficultyPopup;

    UIDocument document;
    [Inject] GameLoader gameLoader;
    [Inject] MainMenuData mainMenuData;
    VisualElement mainMenuOpening;
    VisualElement mainMenuSceneButtons;

    [Inject] MinigameRegistry minigameRegistry;
    MinigameSelectionView selectionView;
    Button settingsButton;
    Popup settingsPopup;

    void Awake() {
        document = GetComponent<UIDocument>();
        difficultyPopup = document.rootVisualElement.Q<Popup>("DifficultyPopup");
        settingsPopup = document.rootVisualElement.Q<Popup>("settings-popup");
        settingsButton = document.rootVisualElement.Q<Button>("SettingsButton");
        mainMenuOpening = document.rootVisualElement.Q<VisualElement>("MainMenuOpening");
        mainMenuSceneButtons = document.rootVisualElement.Q<VisualElement>("MainMenuMinigame");
        mainMenuSceneButtons.style.display = DisplayStyle.None;
        settingsButton.style.display = DisplayStyle.None;
        difficultiesButtonsClasses = new();
    }

    void Start() {
        settingsButton.clicked += () => OnOptionsClicked();
        actions.Player.Continue.performed += TransitionToSceneSelect;

        CreateSelectionView();

        LoadDemoCards();
        BuildPopupCard();
        difficultiesButtonsClasses.Add(Difficulty.Easy, "difficulty-btn-easy");
        difficultiesButtonsClasses.Add(Difficulty.Medium, "difficulty-btn-medium");
        difficultiesButtonsClasses.Add(Difficulty.Hard, "difficulty-btn-hard");
        SetupButtons();

        if (mainMenuData.initialized) TransitionToSceneSelect();
    }

    void OnEnable() => actions.Player.Continue.Enable();

    void CreateSelectionView() {
        selectionView = new(
            document.rootVisualElement,
            minigameCardTemplate);

        selectionView.CardClicked += OnCardClicked;
    }

    void LoadDemoCards() => selectionView.SetCards(minigameRegistry.Mappings, heartSprite, cornerSprite);

    void OnCardClicked(LevelMapping mapping) {
        SetupDifficultyButtons(mapping);
        cardImageView.Data = mapping;
        difficultyPopup.IsOpen = true;
    }

    void OnOptionsClicked() {
        settingsPopup.IsOpen = true;
        actions.Player.Continue.Disable();
    }

    void SetupDifficultyButtons(LevelMapping mapping) {
        var buttonList = document.rootVisualElement.Q<VisualElement>("DifficultyButtons");
        buttonList.Clear();
        for (var i = 0; i < mapping.difficultiesMappings.Length; i++) {
            var button = new Button();
            var difficulty = mapping.difficultiesMappings[i];
            button.text = difficulty.difficultyData.Value.Name;
            // SetupMinigameLabel(mapping);
            button.AddToClassList("difficulty-btn");
            button.AddToClassList(difficultiesButtonsClasses[difficulty.difficultyData.Value.Difficulty]);
            buttonList.Add(button);
            button.clicked += () => OnDifficultyButtonClicked(mapping, difficulty);
        }
    }

    void SetupButtons() {
        var difficultyCancelButton = document.rootVisualElement.Q<Button>("DifficultyPopupCloseBtn");
        var settingsCancelButton = document.rootVisualElement.Q<Button>("settings-close");
        difficultyCancelButton.clicked += () => OnDifficultyCancelButtonClicked();
        settingsCancelButton.clicked += () => OnSettingsCancelButtonClicked();
    }


    void SetupMinigameLabel(LevelMapping mapping) {
        var label = document.rootVisualElement.Q<Label>("DifficultyText");
        label.text = mapping.levelName;
    }

    void OnDifficultyButtonClicked(LevelMapping mapping, DifficultyMapping difficultyMap) {
        Debug.Log($"Scene {mapping.levelName} with difficulty {difficultyMap} selected");
        //difficultyPopup.IsOpen = false;
        var scene = mapping.sceneReference;
        var difficulty = difficultyMap.difficultyData.Value;
        Debug.Log($"Scene {scene} with difficulty {difficulty} selected");
        if (scene == null || difficulty == null) return;
        difficultyHolder.selectedDifficulty = difficulty;
        difficultyHolder.selectedScene = scene;
        gameLoader.LoadGameplaySceneFromHolder().Forget();
    }

    void OnDifficultyCancelButtonClicked() => difficultyPopup.IsOpen = false;

    void OnSettingsCancelButtonClicked() => settingsPopup.IsOpen = false;

    void BuildPopupCard() {
        cardImageView = new();
        var cardImageContainer = document.rootVisualElement.Q<VisualElement>("difficultyIcon");
        cardImageView.InitMinigameCardView(minigameCardTemplate);

        cardImageView.SetFrame(cornerSprite, heartSprite);

        cardImageContainer.Add(cardImageView);
    }

    void TransitionToSceneSelect(InputAction.CallbackContext ctx) {
        TransitionToSceneSelect();
        mainMenuData.initialized = true;
    }
    //transition may fix one click double transition problem

    void TransitionToSceneSelect() {
        mainMenuOpening.style.display = DisplayStyle.None;
        mainMenuSceneButtons.style.display = DisplayStyle.Flex;
        settingsButton.style.display = DisplayStyle.Flex;
        actions.Player.Continue.Disable();
    }
}