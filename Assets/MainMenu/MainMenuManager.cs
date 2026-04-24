using Cysharp.Threading.Tasks.Triggers;
using LitMotion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private UIDocument mainMenuDoc;
    private VisualElement rootElement;
    private VisualElement popupOverlay;
    private int startIndex = 0;
    private Button[] slotButtons = new Button[4]; //need for new if you initialize onEnable?

    public int buttonsPerPage = 4;
    public int maxButtons = 12;
    void OnEnable()
    {
        rootElement = mainMenuDoc.rootVisualElement;

        for (int i = 0; i < buttonsPerPage; i++)
        {
            slotButtons[i] = rootElement.Q<Button>($"slot-{i}"); //will throw a bug if it is different than uxml slot buttons number
        }

        rootElement.Q<Button>("btn-left").clicked += () => ChangePage(-buttonsPerPage);
        rootElement.Q<Button>("btn-right").clicked += () => ChangePage(buttonsPerPage);


        popupOverlay = rootElement.Q<VisualElement>("options-popup");

        rootElement.Q<Button>("options").clicked += () => TogglePopup(true);
        rootElement.Q<Button>("btn-close-options").clicked += () => TogglePopup(false);


        UpdateUI();
    }

    private void ChangePage(int step)
    {
        startIndex += step;

        if (startIndex >= maxButtons)
        {
            startIndex = 0;
        }
        else if (startIndex < 0)
        {
            startIndex = maxButtons - buttonsPerPage;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < buttonsPerPage; i++)
        {
            slotButtons[i].text = (startIndex + i + 1).ToString();
        }
    }

    private void TogglePopup(bool show)
    {
        popupOverlay.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
