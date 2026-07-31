using System.Collections.Generic;
using System.Linq;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour {
    [SerializeField] UIDocument uiDocument;
    Popup endGamePopup;
    [Inject] GameLoader gameLoader;
    [Inject] IGameManager gameManager;
    [Inject] ISpawnableGroup group;
    [Inject] IRandomItemContainer itemContainer;

    void Start() {
        gameManager.OnGameComplete += OnGameComplete;
        endGamePopup = uiDocument.rootVisualElement.Q<Popup>("EndGamePopup");
        var restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        var backToMenuButton = uiDocument.rootVisualElement.Q<Button>("ExitButton");
        restartButton.clicked += () => gameManager.RestartGame();
        backToMenuButton.clicked += () => gameLoader.LoadMainMenu();
        var objectiveLabel = uiDocument.rootVisualElement.Q<Label>("ObjectiveLabel");
        objectiveLabel.text = TransformLabel(group.TargetText, itemContainer.GetTargets());
    }

    string TransformLabel(string label, IEnumerable<IElement> elements) =>
        label.Replace("{name}", string.Join(", ", elements.Select(e => e.TargetDisplayName)));

    void OnGameComplete() => endGamePopup.IsOpen = true;
}