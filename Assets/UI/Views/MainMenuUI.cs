using System;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuUI : MonoBehaviour {
    [Header("Templates")]
    [SerializeField] public VisualTreeAsset minigameCardTemplate;

    [Header("Demo Sprites")]
    [SerializeField] Sprite placeholderThumbnail;

    [Inject] InputSystem_Actions actions;

    UIDocument document;
    [Inject] MainMenuData mainMenuData;
    VisualElement mainMenuOpening;
    VisualElement mainMenuSceneButtons;

    [Inject] MinigameRegistry minigameRegistry;
    MinigameSelectionView selectionView;
    Button settingsButton;
    Popup settingsPopup;

    void Awake() {
        document = GetComponent<UIDocument>();
        settingsPopup = document.rootVisualElement.Q<Popup>("settings-popup");
        settingsButton = document.rootVisualElement.Q<Button>("SettingsButton");
        mainMenuOpening = document.rootVisualElement.Q<VisualElement>("MainMenuOpening");
        mainMenuSceneButtons = document.rootVisualElement.Q<VisualElement>("MainMenuMinigame");
        mainMenuSceneButtons.style.display = DisplayStyle.None;
        settingsButton.style.display = DisplayStyle.None;
    }

    void Start() {
        settingsButton.clicked += OnOptionsClicked;
        actions.Player.Continue.performed += TransitionToSceneSelect;

        CreateSelectionView();

        LoadDemoCards();

        SetupButtons();

        if (mainMenuData.initialized) TransitionToSceneSelect();
    }

    void OnEnable() => actions.Player.Continue.Enable();
    public event Action<LevelMapping> OnMinigameCardClicked;

    void CreateSelectionView() {
        selectionView = new(
            document.rootVisualElement,
            minigameCardTemplate);

        selectionView.CardClicked += OnCardClicked;
    }

    void LoadDemoCards() => selectionView.SetCards(minigameRegistry.Mappings);

    void OnCardClicked(LevelMapping mapping) => OnMinigameCardClicked?.Invoke(mapping);

    void OnOptionsClicked() {
        settingsPopup.IsOpen = true;
        actions.Player.Continue.Disable();
    }


    void SetupButtons() {
        var settingsCancelButton = document.rootVisualElement.Q<Button>("settings-close");
        settingsCancelButton.clicked += settingsPopup.Close;
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