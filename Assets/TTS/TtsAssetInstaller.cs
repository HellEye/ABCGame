#define TTS_FORCE_WEBREQUEST
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
#if TTS_FORCE_WEBREQUEST || (UNITY_ANDROID && !UNITY_EDITOR)
using UnityEngine.Networking;
#endif

public sealed class TtsInsufficientStorageException : IOException {
    public TtsInsufficientStorageException(
        string message,
        long? requiredBytes,
        long? availableBytes,
        Exception innerException = null) : base(message, innerException) {
        RequiredBytes = requiredBytes;
        AvailableBytes = availableBytes;
    }

    public long? RequiredBytes { get; }
    public long? AvailableBytes { get; }

    public string RequiredDisplay => FormatBytes(RequiredBytes);
    public string AvailableDisplay => FormatBytes(AvailableBytes);

    public string ToUserMessage() {
        if (RequiredBytes.HasValue && AvailableBytes.HasValue)
            return $"Not enough storage space. Need about {RequiredDisplay}, but only {AvailableDisplay} is available.";
        if (RequiredBytes.HasValue)
            return $"Not enough storage space. Need about {RequiredDisplay}.";
        return "Not enough storage space to install TTS assets.";
    }

    public static string FormatBytes(long? bytes) {
        if (!bytes.HasValue) return "unknown";
        if (bytes.Value < 0) return "unknown";

        var value = bytes.Value;
        string[] units = { "B", "KB", "MB", "GB", "TB" };
        var unitIndex = 0;
        double displayValue = value;
        while (displayValue >= 1024d && unitIndex < units.Length - 1) {
            displayValue /= 1024d;
            unitIndex++;
        }

        return $"{displayValue:0.##} {units[unitIndex]}";
    }
}

public static class TtsAssetInstaller {
    // Bump this when the archive changes.
    public const string Version = "tts-0.1.4";
    const int ExtractionSpaceMultiplier = 3;

    public static async UniTask<string> EnsureInstalledAsync(
        string zipRelPath, CancellationToken ct) {
        var root = Path.Combine(Application.persistentDataPath, "tts");
        var installDir = Path.Combine(root, Version);
        if (Directory.Exists(installDir)) return installDir;
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Directory.CreateDirectory(root);
        var zipTmp = Path.Combine(root, Version + ".zip.tmp");
        var staging = installDir + ".tmp";

        try {
            var src = Path.Combine(Application.streamingAssetsPath, zipRelPath);
            Debug.Log("Request src: " + src);
            await CopyStreamingAssetAsync(zipRelPath, zipTmp, ct);
            EnsureEnoughSpaceForExtraction(root, zipTmp);


            await UniTask.SwitchToThreadPool();
            ct.ThrowIfCancellationRequested();
            if (Directory.Exists(staging)) Directory.Delete(staging, true);
            ZipFile.ExtractToDirectory(zipTmp, staging);

            Directory.Move(staging, installDir); // commit point
            CleanupOldVersions(root, Version);


            await UniTask.SwitchToMainThread();
            return installDir;
        }
        catch (Exception e) {
            TryDelete(staging);
            TryDelete(installDir);
            throw NormalizeInstallException(root, zipTmp, e);
        }
        finally {
            // Runs on success, failure, and cancellation. Only touches temp paths.
            stopwatch.Stop();
            await UniTask.SwitchToMainThread();
            Debug.Log($"TTS zip extract took: {stopwatch.ElapsedMilliseconds} ms");
            TryDelete(zipTmp);
            if (Directory.Exists(staging)) TryDelete(staging);
        }
    }

    static async UniTask CopyStreamingAssetAsync(
        string relPath, string dest, CancellationToken ct) {
#if TTS_FORCE_WEBREQUEST
        var src = new Uri(Path.Combine(Application.streamingAssetsPath, relPath)).AbsoluteUri;
#else
        var src = Path.Combine(Application.streamingAssetsPath, relPath);
#endif

#if (UNITY_ANDROID && !UNITY_EDITOR) || TTS_FORCE_WEBREQUEST
        // APK-backed path: must go through UnityWebRequest on the main thread.
        using var req = UnityWebRequest.Get(src);
        req.downloadHandler = new DownloadHandlerFile(dest);
        await req.SendWebRequest().ToUniTask(cancellationToken: ct);
        if (req.result != UnityWebRequest.Result.Success)
            throw new IOException($"Failed to fetch {src}: {req.error}");
#else
        // Plain filesystem path: copy directly off the main thread.
        await UniTask.SwitchToThreadPool();
        ct.ThrowIfCancellationRequested();
        File.Copy(src, dest, true);
        await UniTask.SwitchToMainThread();
#endif
    }

    static void TryDelete(string path) {
        try {
            if (File.Exists(path)) File.Delete(path);
            else if (Directory.Exists(path)) Directory.Delete(path, true);
        }
        catch (IOException e) {
            Debug.LogWarning($"Could not delete {path}: {e.Message}");
        }
    }

    public static void CleanupOldVersions(string root, string keep) {
        // TODO run cleanup after failed install in a catch block in a loader of some kind
        foreach (var dir in Directory.GetDirectories(root))
            if (Path.GetFileName(dir) != keep)
                Directory.Delete(dir, true);
        foreach (var file in Directory.GetFiles(root, "*.tmp"))
            File.Delete(file);
    }

    static void EnsureEnoughSpaceForExtraction(string root, string zipPath) {
        var requiredBytes = new FileInfo(zipPath).Length * ExtractionSpaceMultiplier;
        var availableBytes = TryGetAvailableBytes(root);
        if (!availableBytes.HasValue || availableBytes.Value >= requiredBytes) return;

        throw new TtsInsufficientStorageException(
            $"Not enough storage to extract TTS assets. Required about {TtsInsufficientStorageException.FormatBytes(requiredBytes)}, available {TtsInsufficientStorageException.FormatBytes(availableBytes.Value)}.",
            requiredBytes,
            availableBytes.Value);
    }

    static long? TryGetAvailableBytes(string path) {
        try {
            var fullPath = Path.GetFullPath(path);
            var root = Path.GetPathRoot(fullPath);
            if (string.IsNullOrEmpty(root)) return null;
            return new DriveInfo(root).AvailableFreeSpace;
        }
        catch (Exception e) when (
            e is IOException ||
            e is UnauthorizedAccessException ||
            e is ArgumentException ||
            e is NotSupportedException) {
            Debug.LogWarning($"Unable to read free space for {path}: {e.Message}");
            return null;
        }
    }

    static Exception NormalizeInstallException(string root, string zipPath, Exception e) {
        if (e is TtsInsufficientStorageException) return e;
        if (e is not IOException io || !IsNoSpaceException(io)) return e;

        var requiredBytes = File.Exists(zipPath)
            ? (long?)(new FileInfo(zipPath).Length * ExtractionSpaceMultiplier)
            : null;
        var availableBytes = TryGetAvailableBytes(root);

        return new TtsInsufficientStorageException(
            "TTS asset installation failed because the device appears to be out of storage.",
            requiredBytes,
            availableBytes,
            io);
    }

    static bool IsNoSpaceException(IOException e) {
        const int hResultDiskFull = unchecked((int)0x80070070);
        const int hResultHandleDiskFull = unchecked((int)0x80070027);

        if (e.HResult == hResultDiskFull || e.HResult == hResultHandleDiskFull) return true;

        var message = e.Message;
        return message.IndexOf("No space left on device", StringComparison.OrdinalIgnoreCase) >= 0 ||
               message.IndexOf("disk full", StringComparison.OrdinalIgnoreCase) >= 0;
    }
}