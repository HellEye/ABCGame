using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PiperPhonemize;
using SherpaOnnx;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SherpaTextToSpeech : ITextToSpeech, ITextToSpeechWarmup {
    // Paths are relative to the project root (the folder containing Assets/).
    // TODO change paths to be relative to the extracted archive (dir parameter of CreateTts), essentially "root" is Assets/TTS
    const string ModelPath = "GosiaPL/gosia-medium.onnx";
    const string TokensPath = "GosiaPL/tokens.txt";
    const string EspeakDataPath = "espeak-ng-data";
    const string Language = "pl";
    const int SpeakerId = 0;
    static string _initializedPath;
    readonly object _lock = new();

    UniTask<OfflineTts>? _ready;
    readonly Notifications notifications;
    OfflineTts tts;
    Task<OfflineTts> ttsTask;

    public SherpaTextToSpeech(Notifications notifications) => this.notifications = notifications;

    public AudioClip GenerateAudio(
        string text,
        float pitch = 1f, // Not supported by sherpa-onnx VITS, ignored.
        float rate = 1f,
        float volume = 1f) {
        var time = Stopwatch.StartNew();
        Debug.Log("TTS for: " + text);
        float[] samples;
        int sampleRate;
        if (tts == null)
            // we'll see how long it takes to load
            // If this ever shows up, we need to make GenerateAudio async and await the _ready task.
            throw new InvalidOperationException(
                "TTS not initialized. If this shows up we need some changes, or something broke horribly.");
        using (var phonemes = PiperPhonemizeApi.Text(text, Language)) {
            if (phonemes == null)
                throw new InvalidOperationException("Phonemization failed.");

            var codepoints = new int[phonemes.NumSentences][];
            Debug.Log($"Sentences: {phonemes.NumSentences}");
            for (var i = 0; i < phonemes.NumSentences; i++) {
                Debug.Log($"Phonemes[{i}]: '{phonemes.GetPhonemesAsString(i)}'");
                var ipa = phonemes.GetPhonemesAsString(i) ?? "";
                var cps = new int[ipa.Length];
                for (var j = 0; j < ipa.Length; j++)
                    cps[j] = ipa[j];
                codepoints[i] = cps;
            }


            var genConfig = new OfflineTtsGenerationConfig {
                Sid = SpeakerId,
                Speed = rate,
                PhonemeCodepoints = codepoints,
                ReferenceText = text
            };

            var audio = tts.GenerateWithConfig("", genConfig, null);
            samples = audio.Samples;
            sampleRate = audio.SampleRate;
        }


        if (!Mathf.Approximately(volume, 1f))
            for (var i = 0; i < samples.Length; i++)
                samples[i] = Mathf.Clamp(samples[i] * volume, -1f, 1f);

        time.Stop();
        Debug.Log("TTS generation took: " + time.ElapsedMilliseconds + " ms");
        var clip = AudioClip.Create(
            "SherpaTTS",
            samples.Length,
            1,
            sampleRate,
            false);
        clip.SetData(samples, 0);
        return clip;
    }

    // Call once from the Loader. Safe to call repeatedly.
    public UniTask WarmupAsync(CancellationToken ct = default) {
        lock (_lock) {
            _ready ??= InitAsync(ct).Preserve();
        }

        return _ready?.AsUniTask() ?? UniTask.FromException(new InvalidOperationException("Failed to initialize TTS."));
    }

    async UniTask<OfflineTts> InitAsync(CancellationToken ct) {
        var dir = "";
        try {
            dir = await TtsAssetInstaller.EnsureInstalledAsync(
                "TTS/TTS.zip",
                ct);
        }
        catch (TtsInsufficientStorageException e) {
            notifications.Show(
                "Instalacja Głosu nie powiodła się",
                "Nie udało się zainstalować głosu. Brak wystarczającej ilości miejsca na dysku.\n Spróbuj zwolnić miejsce i uruchomić grę ponownie.\n");
            return null;
        }
        catch (Exception e) {
            notifications.Show(
                "Instalacja Głosu nie powiodła się",
                "Nie udało się zainstalować głosu. Spróbuj uruchomić grę ponownie.\n");
            Debug.LogException(e);
            return null;
        }

        await UniTask.SwitchToThreadPool();
        var tts = CreateTts(dir); // no UnityEngine calls
        await UniTask.SwitchToMainThread();
        return tts;
    }


    OfflineTts CreateTts(string dataPath) {
        var espeakDir = ToAbsolutePath(EspeakDataPath, dataPath);

        if (!Directory.Exists(espeakDir))
            throw new DirectoryNotFoundException(espeakDir);

        var sampleRate = PiperPhonemizeApi.Initialize(espeakDir);
        if (sampleRate < 0)
            throw new InvalidOperationException("Failed to initialize piper-phonemize.");

        var config = new OfflineTtsConfig();
        config.Model.Vits.Model = ToAbsolutePath(ModelPath, dataPath);
        config.Model.Vits.Tokens = ToAbsolutePath(TokensPath, dataPath);
        config.Model.NumThreads = 2;
        config.Model.Provider = "cpu";
        tts = new(config);
        return tts;
    }

    static string ToAbsolutePath(string relativePath, string dataPath) =>
        Path.GetFullPath(
            Path.Combine(dataPath, relativePath));
}