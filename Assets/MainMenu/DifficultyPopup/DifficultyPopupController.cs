using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UIElements;

public class DifficultyPopupController : MonoBehaviour {
    MinigameCardView cardImageView;


    [Inject] DifficultyHolder difficultyHolder;
    Popup difficultyPopup;
    [Inject] UIDocument document;
    [Inject] GameLoader gameLoader;
    [Inject] MainMenuUI mainMenu;

    void Awake() {
        difficultyPopup = document.rootVisualElement.Q<Popup>("DifficultyPopup");

        var difficultyCancelButton = difficultyPopup.Q<Button>("DifficultyPopupCloseBtn");
        difficultyCancelButton.clicked += difficultyPopup.Close;
        BuildPopupCard();
    }

    void OnEnable() => mainMenu.OnMinigameCardClicked += HandleCardClicked;

    void OnDisable() => mainMenu.OnMinigameCardClicked -= HandleCardClicked;

    void BuildPopupCard() {
        cardImageView = new();
        var cardImageContainer = document.rootVisualElement.Q<VisualElement>("difficultyIcon");
        cardImageContainer.Add(cardImageView);
    }

    void HandleCardClicked(LevelMapping mapping) {
        SetupDifficultyButtons(mapping);
        cardImageView.Data = mapping;
        difficultyPopup.IsOpen = true;
    }

    void SetupDifficultyButtons(LevelMapping mapping) {
        var buttonList = document.rootVisualElement.Q<VisualElement>("DifficultyButtons");
        buttonList.Clear();
        foreach (var difficulty in mapping.difficultiesMappings) {
            var button = new Button {
                text = difficulty.difficultyData.Value.Name
            };
            button.AddToClassList("button");
            button.AddToClassList("difficulty-btn");
            button.AddToClassList(difficulty.difficultyData.Value.Difficulty.ToString().ToLower());
            buttonList.Add(button);
            button.clicked += () => OnDifficultyButtonClicked(mapping, difficulty);
        }
    }

    void OnDifficultyCancelButtonClicked() => difficultyPopup.IsOpen = false;

    void OnDifficultyButtonClicked(LevelMapping mapping, DifficultyMapping difficultyMap) {
        //difficultyPopup.IsOpen = false;
        var scene = mapping.sceneReference;
        var difficulty = difficultyMap.difficultyData.Value;
        if (scene == null || difficulty == null) return;
        difficultyHolder.selectedDifficulty = difficulty;
        difficultyHolder.selectedScene = scene;
        difficultyHolder.selectedMapping = mapping;
        gameLoader.LoadGameplaySceneFromHolder().Forget();
    }
}