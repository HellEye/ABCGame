using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour {
    [SerializeField] UIDocument uiDocument;
    Popup endGamePopup;
    [Inject] GameLoader gameLoader;
    [Inject] IGameManager gameManager;
    [Inject] ISpawnableGroup group;

    void Start() {
        gameManager.OnGameComplete += OnGameComplete;
        endGamePopup = uiDocument.rootVisualElement.Q<Popup>("EndGamePopup");
        var restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        var backToMenuButton = uiDocument.rootVisualElement.Q<Button>("ExitButton");
        restartButton.clicked += () => gameManager.RestartGame();
        backToMenuButton.clicked += () => gameLoader.LoadMainMenu();
        var objectiveLabel = uiDocument.rootVisualElement.Q<Label>("ObjectiveLabel");
        objectiveLabel.text = group.TargetText;
    }

    void OnGameComplete() => endGamePopup.IsOpen = true;
}