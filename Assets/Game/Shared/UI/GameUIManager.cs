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
        gameManager.OnGameComplete += OpenEndScreen;
        var root = uiDocument.rootVisualElement;
        endGamePopup = root.Q<Popup>("EndGamePopup");

        root.Q<Button>("ContinueButton").clicked += endGamePopup.Close;
        root.Q<Button>("RestartButton").clicked += () => gameManager.RestartGame();
        root.Q<Button>("ExitButton").clicked += () => gameLoader.LoadMainMenu();
        var objectiveLabel = uiDocument.rootVisualElement.Q<Label>("ObjectiveLabel");
        objectiveLabel.text = TransformLabel(group.TargetText, itemContainer.GetTargets());
        uiDocument.rootVisualElement.Q<Button>("PauseButton").clicked += OpenPauseMenu;
    }


    void OpenPauseMenu() {
        endGamePopup.CloseOnBackdropClick = true;
        endGamePopup.EnableInClassList("pause-menu", true);
        endGamePopup.EnableInClassList("end-menu", false);
        endGamePopup.Open();
    }

    void OpenEndScreen() {
        endGamePopup.CloseOnBackdropClick = false;
        endGamePopup.EnableInClassList("pause-menu", false);
        endGamePopup.EnableInClassList("end-menu", true);
        endGamePopup.Open();
    }

    string TransformLabel(string label, IEnumerable<IElement> elements) =>
        label.Replace("{name}", string.Join(", ", elements.Select(e => e.TargetDisplayName)));
}