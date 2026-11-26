using UnityEngine.UIElements;

public class UiAudio {
    readonly AudioManager audioManager;
    readonly FmodEvents fmodEvents;

    public UiAudio(FmodEvents fmodEvents, AudioManager audioManager) {
        this.fmodEvents = fmodEvents;
        this.audioManager = audioManager;
    }

    public void PlayClickSound() => audioManager.PlayOneShot(fmodEvents.ui.buttonClick);

    public void Initialize(VisualElement root) {
        var buttons = root.Query<Button>(null, "click-sound").ToList();
        foreach (var button in buttons) button.clicked += PlayClickSound;
    }
}