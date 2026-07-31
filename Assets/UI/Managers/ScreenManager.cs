using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UIScreenManager : MonoBehaviour
{
    private UIDocument document;

    private VisualElement mainMenu;
    private VisualElement minigameSelection;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        var root = document.rootVisualElement;

        mainMenu = root.Q<VisualElement>("MainMenu");
        minigameSelection = root.Q<VisualElement>("MinigameSelection");
    }

    private void Start()
    {
        ShowMainMenu();

        RegisterButtons();
    }

    void RegisterButtons()
    {
        // Press anywhere on the main menu
        mainMenu.RegisterCallback<ClickEvent>(_ =>
        {
            ShowMinigameSelection();
        });

        // Back button
        var backButton =
            minigameSelection.Q<Button>("BackButton");

        backButton.clicked += ShowMainMenu;
    }

    public void ShowMainMenu()
    {
        mainMenu.style.display = DisplayStyle.Flex;
        minigameSelection.style.display = DisplayStyle.None;
    }

    public void ShowMinigameSelection()
    {
        mainMenu.style.display = DisplayStyle.None;
        minigameSelection.style.display = DisplayStyle.Flex;
    }
}