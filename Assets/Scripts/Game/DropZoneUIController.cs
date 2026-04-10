using UnityEngine;
using UnityEngine.UI;

public class DropZoneUIController : MonoBehaviour
{
    [SerializeField] DropZoneGameManager gameManager;
    [SerializeField] Button restartButton;
    [SerializeField] Canvas canvas;

    void Start()
    {
        canvas.enabled = false;
        gameManager.OnGameComplete += OnGameComplete;
    }

    void OnGameComplete()
    {
        canvas.enabled = true;
        restartButton.onClick.AddListener(OnRestartClicked);
    }

    void OnRestartClicked()
    {
        canvas.enabled = false;
        restartButton.onClick.RemoveListener(OnRestartClicked);
        gameManager.RestartGame();
    }
}