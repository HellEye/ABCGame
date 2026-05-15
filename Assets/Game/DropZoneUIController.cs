using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

public class DropZoneUIController : MonoBehaviour {
    [SerializeField] Button restartButton;
    [SerializeField] Button mainMenuButton;
    [SerializeField] Canvas canvas;
    [Inject] GameLoader gameLoader;
    [Inject] DropZoneGameManager gameManager;

    void Start() {
        canvas.enabled = false;
        gameManager.OnGameComplete += OnGameComplete;
        restartButton.onClick.AddListener(OnRestartClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    void OnMainMenuClicked() {
        canvas.enabled = false;
        gameLoader.LoadMainMenu();
    }

    void OnGameComplete() => canvas.enabled = true;

    void OnRestartClicked() {
        canvas.enabled = false;
        gameLoader.ReloadCurrentGameplayScene();
    }
}