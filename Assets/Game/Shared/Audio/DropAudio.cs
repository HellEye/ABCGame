public class DropAudio {
    readonly AudioManager audioManager;
    readonly FmodEvents fmodEvents;

    public DropAudio(AudioManager audioManager, FmodEvents fmodEvents) {
        this.audioManager = audioManager;
        this.fmodEvents = fmodEvents;
    }

    public void AddDropZone(DropZone dropZone) {
        dropZone.OnDropCorrect += PlayCorrect;
        dropZone.OnDropIncorrect += PlayIncorrect;
    }

    void PlayCorrect() => audioManager.PlayOneShot(fmodEvents.game.dropCorrect);

    void PlayIncorrect() => audioManager.PlayOneShot(fmodEvents.game.dropIncorrect);
}