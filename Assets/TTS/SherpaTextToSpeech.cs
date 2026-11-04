using System;
using System.Diagnostics;
using System.IO;
using PiperPhonemize;
using SherpaOnnx;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SherpaTextToSpeech : ITextToSpeech {
    // Paths are relative to the project root (the folder containing Assets/).
    const string ModelPath = "Assets/TTS/Voices/GosiaPL/pl_PL-gosia-medium.onnx";
    const string TokensPath = "Assets/TTS/Voices/GosiaPL/tokens.txt";
    const string EspeakDataPath = "Assets/TTS/Voices/espeak-ng-data";
    const string Language = "pl";
    const int SpeakerId = 0;

    static readonly object InitLock = new();
    static OfflineTts _tts;

    public AudioClip GenerateAudio(
        string text,
        float pitch = 1f, // Not supported by sherpa-onnx VITS, ignored.
        float rate = 1f,
        float volume = 1f) {
        var time = Stopwatch.StartNew();
        var tts = GetTts();
        float[] samples;
        int sampleRate;

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

    static OfflineTts GetTts() {
        lock (InitLock) {
            if (_tts != null)
                return _tts;

            var espeakDir = ToAbsolutePath(EspeakDataPath);
            if (!Directory.Exists(espeakDir))
                throw new DirectoryNotFoundException(espeakDir);

            var sampleRate = PiperPhonemizeApi.Initialize(espeakDir);
            if (sampleRate < 0)
                throw new InvalidOperationException("Failed to initialize piper-phonemize.");

            var config = new OfflineTtsConfig();
            config.Model.Vits.Model = ToAbsolutePath(ModelPath);
            config.Model.Vits.Tokens = ToAbsolutePath(TokensPath);
            config.Model.NumThreads = 2;
            config.Model.Provider = "cpu";
            _tts = new(config);
            return _tts;
        }
    }

    static string[][] GetTokensContents(string tokensPath) {
        var lines = File.ReadAllLines(tokensPath);
        var tokens = new string[lines.Length][];
        for (var i = 0; i < lines.Length; i++)
            tokens[i] = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return tokens;
    }

    static string ToAbsolutePath(string projectRelativePath) =>
        Path.GetFullPath(
            Path.Combine(Application.dataPath, "..", projectRelativePath));
}