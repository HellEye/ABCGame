using System.Linq;
using UnityEngine;

public class AndroidTextToSpeech : ITextToSpeech {
    public AudioClip GenerateAudio(string text, float pitch = 1, float rate = 1, float volume = 1) {
        using var tts = new AndroidJavaObject(
            "android.speech.tts.TextToSpeech",
            new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"),
            null);
        tts.Call<int>("setPitch", pitch);
        tts.Call<int>("setSpeechRate", rate);
        tts.Call<int>("setVolume", volume);
        var audioData = tts.Call<byte[]>("synthesizeToFile", text, null, "/sdcard/tts_output.wav");
        var audioClip = AudioClip.Create("TTS_Audio", audioData.Length, 1, 44100, false);
        audioClip.SetData(audioData.Select(b => b / 255f).ToArray(), 0);
        return audioClip;
    }
}