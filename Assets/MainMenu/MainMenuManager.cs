using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class MainMenuManager : MonoBehaviour {
    public int buttonsPerPage = 4;
    public int maxButtons = 12;
    public int maxDifficulties = 3;

    readonly string[] buttonTexts = new string[2] { "Drag and Drop", "Drag Letters" };
    [Inject] readonly MainMenuSettingsData settingsData;
    readonly Button[] slotButtons = new Button[4]; //need for new if you initialize onEnable?

    [Inject] DifficultyHolder difficultyHolder;
    Popup difficultyPopup;
    int gameIndex;
    [Inject] GameLoader gameLoader;
    [Inject] UIDocument mainMenuDoc;
    [Inject] MinigameRegistry minigameRegistry;
    VisualElement popupOverlay;
    VisualElement rootElement;
    int startIndex;

    void Awake() {
        rootElement = mainMenuDoc.rootVisualElement;
        for (var i = 0; i < buttonsPerPage; i++)
            slotButtons[i] =
                rootElement.Q<Button>($"slot-{i}"); //will throw a bug if it is different than uxml slot buttons number

        rootElement.Q<Button>("btn-left").clicked += () => ChangePage(-buttonsPerPage);
        rootElement.Q<Button>("btn-right").clicked += () => ChangePage(buttonsPerPage);
        difficultyPopup = rootElement.Q<Popup>("DifficultyPopup");
        InitMaxDifficulties();
        InitMaxLevels();
        UpdateUI();
        SetupGameButtons();
        SetupDifficultyButtons();
    }


    void Start() => rootElement.FlattenTemplateContainers();

    void SetupGameButtons() {
        for (var i = 0; i < slotButtons.Length; i++) {
            var index = i;
            slotButtons[i].clicked += () => OnSlotButtonClicked(index);
        }
    }

    void SetupDifficultyButtons() {
        var buttonList = rootElement.Q<VisualElement>("DifficultyButtons");
        for (var i = 0; i < maxDifficulties; i++) {
            // I know there's some issues with indexes in lambdas, so this is a workaround
            var index = i;
            buttonList.Q<Button>($"difficulty-{index}").clicked += () => OnDifficultyButtonClicked(index);
        }
    }

    void OnSlotButtonClicked(int slotIndex) {
        gameIndex = slotIndex + startIndex;
        if (gameIndex < minigameRegistry.Count)
            difficultyPopup.IsOpen = true;
    }

    void OnDifficultyButtonClicked(int difficultyIndex) {
        Debug.Log($"Scene {gameIndex} with difficulty {difficultyIndex} selected");
        difficultyPopup.IsOpen = false;
        var (scene, difficulty) = minigameRegistry.GetLevelData(gameIndex, difficultyIndex);
        Debug.Log($"Scene {scene} with difficulty {difficulty} selected");
        if (scene == null || difficulty == null) return;
        difficultyHolder.selectedDifficulty = difficulty;
        difficultyHolder.selectedScene = scene;
        gameLoader.LoadGameplaySceneFromHolder().Forget();
    }

    void ChangePage(int step)
    {
        if (step >= maxButtons) return;
        startIndex += step;
        
        if (startIndex >= maxButtons)
            startIndex = 0;
        else if (startIndex < 0) startIndex = maxButtons - buttonsPerPage;

        UpdateUI();
    }

    void UpdateUI() {
        for (var i = 0; i < buttonsPerPage; i++) {
            var index = i + startIndex;
            slotButtons[i].text =
                index < buttonTexts.Length
                    ? buttonTexts[index]
                    : "Coming soon";
        }
    }

    void InitMaxDifficulties()
    {
        int initMaxDifficulties = minigameRegistry.Mappings.Select(level => level.difficultiesMappings.Count()).Max();
        maxDifficulties = initMaxDifficulties;
    }
    
    void InitMaxLevels()
    {
        int initMaxLevels = 0;
        foreach (LevelMapping minigame in minigameRegistry.Mappings)
        {
            initMaxLevels++;
        }
        maxButtons = initMaxLevels;
    }
}