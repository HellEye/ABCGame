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
    Popup instructionPopup;

    [Inject] IRandomItemContainer itemContainer;
    [Inject] GameProgressData progressData;
    [Inject] ITextToSpeech textToSpeech;
    string Instructions => TransformLabel(group.TargetText, itemContainer.GetTargets());

    void Start() {
        gameManager.OnGameComplete += OpenEndScreen;
        var root = uiDocument.rootVisualElement;
        endGamePopup = root.Q<Popup>("EndGamePopup");

        root.Q<Button>("ContinueButton").clicked += endGamePopup.Close;
        root.Q<Button>("RestartButton").clicked += gameManager.RestartGame;
        root.Q<Button>("ExitButton").clicked += gameLoader.LoadMainMenu;
        root.Q<CustomProgress>("ProgressBar").dataSource = progressData;
        SetupInstructionPopup(root);
        root.Q<Button>("OpenInstructionsButton").clicked += instructionPopup.Open;

        uiDocument.rootVisualElement.Q<Button>("PauseButton").clicked += OpenPauseMenu;
    }

    void PlayTTSInstructions() {
        var audioClip = textToSpeech.GenerateAudio(Instructions);
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    void SetupInstructionPopup(VisualElement root) {
        instructionPopup = root.Q<Popup>("InstructionPopup");
        instructionPopup.Open();
        instructionPopup.Q<Button>("InstructionContinueButton").clicked += () => {
            instructionPopup.Close();
            instructionPopup.Q<Button>("InstructionContinueButton").text = "Gram Dalej!";
        };
        instructionPopup.Q<Label>("instruction-text").text =
            Instructions;
        // TODO setup image


        // TODO setup audio here
        var audioButton = instructionPopup.Q<Button>("InstructionAudioButton");
        if (textToSpeech == null)
            audioButton.SetEnabled(false);
        else
            audioButton.clicked += PlayTTSInstructions;
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
        label.Replace("{name}", string.Join(", ", elements.Select(e => e.DisplayNamePlural)));
}