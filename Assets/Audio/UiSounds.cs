using System.Collections.Generic;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UIElements;

[DefaultExecutionOrder(500)]
public class UiSounds : MonoBehaviour {
    [SerializeField] UIDocument document;
    [Inject] AudioManager audioManager;
    List<Button> buttons = new();
    [Inject] FmodEvents fmodEvents;
    VisualElement root;

    void Start() {
        root = document.rootVisualElement;
        Reinitialize();
    }

    public void PlayClickSound() => audioManager.PlayOneShot(fmodEvents.ui.buttonClick);

    public void Reinitialize() {
        foreach (var button in buttons) button.clicked -= PlayClickSound;
        buttons = root.Query<Button>(null, "click-sound").ToList();
        foreach (var button in buttons) button.clicked += PlayClickSound;
    }
}