using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ITextToSpeech {
    public AudioClip GenerateAudio(string text, float pitch = 1f, float rate = 1f, float volume = 1f);
}

public interface ITextToSpeechWarmup {
    public UniTask WarmupAsync(CancellationToken ct);
}