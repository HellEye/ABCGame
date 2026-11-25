using Reflex.Attributes;
using UnityEngine;

public class TtsAudio : MonoBehaviour {
    [SerializeField] AudioSource audioSource;
    [Inject] ITextToSpeech tts;

    public void Play(string text) {
        var clip = tts.GenerateAudio(text);
        audioSource.clip = clip;
        audioSource.Play();
    }
}