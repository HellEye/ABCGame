using UnityEngine;

public interface ITextToSpeech {
    public AudioClip GenerateAudio(string text, float pitch = 1f, float rate = 1f, float volume = 1f);
}