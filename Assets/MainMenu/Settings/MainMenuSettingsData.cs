using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

public enum SettingsIntensity {
    Off,
    Reduced,
    Full
}

public enum ColorblindLevel {
    None,
    Reduced,
    Full
}

public class MainMenuSettingsData : IDataSourceViewHashProvider, INotifyBindablePropertyChanged {
    long viewVersion;

    public static Dictionary<SettingsIntensity, string> IntensityText { get; } = new() {
        { SettingsIntensity.Off, "Off" },
        { SettingsIntensity.Reduced, "Reduced" },
        { SettingsIntensity.Full, "Full" }
    };

    // TODO better names for this?
    public static Dictionary<ColorblindLevel, string> ColorblindText { get; } = new() {
        { ColorblindLevel.None, "None" },
        { ColorblindLevel.Reduced, "Reduced" },
        { ColorblindLevel.Full, "Full" }
    };

    public static Func<SettingsIntensity, string> IntensityTextGetter { get; } = v => IntensityText[v];
    public static Func<ColorblindLevel, string> ColorblindTextGetter { get; } = v => ColorblindText[v];

    public long GetViewHashCode() => viewVersion;
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

    void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
        viewVersion++;
        propertyChanged?.Invoke(this, new(propertyName));
    }

    public void Save() {
        PlayerPrefs.SetInt("SoundVolume", _soundVolume);
        PlayerPrefs.SetInt("MusicVolume", _musicVolume);
        PlayerPrefs.SetInt("VoEnabled", _voEnabled ? 1 : 0);
        PlayerPrefs.SetInt("VoVolume", _voVolume);
        PlayerPrefs.SetInt("ParticleIntensity", (int)_particleIntensity);
        PlayerPrefs.SetInt("MotionIntensity", (int)_motionIntensity);
        PlayerPrefs.SetInt("HapticIntensity", (int)_hapticIntensity);
        PlayerPrefs.SetInt("SpriteScale", _spriteScale);
        PlayerPrefs.SetInt("ColorblindLevel", (int)_colorblindLevel);
        PlayerPrefs.Save();
    }

    public void Load() {
        SoundVolume = PlayerPrefs.GetInt("SoundVolume", 0);
        MusicVolume = PlayerPrefs.GetInt("MusicVolume", 0);
        VoEnabled = PlayerPrefs.GetInt("VoEnabled", 0) == 1;
        VoVolume = PlayerPrefs.GetInt("VoVolume", 0);
        ParticleIntensity = (SettingsIntensity)PlayerPrefs.GetInt("ParticleIntensity", 0);
        MotionIntensity = (SettingsIntensity)PlayerPrefs.GetInt("MotionIntensity", 0);
        HapticIntensity = (SettingsIntensity)PlayerPrefs.GetInt("HapticIntensity", 0);
        ColorblindLevel = (ColorblindLevel)PlayerPrefs.GetInt("ColorblindLevel", 0);
        SpriteScale = PlayerPrefs.GetInt("SpriteScale", 0);
    }

    public void Reset() {
        SoundVolume = 10;
        MusicVolume = 10;
        VoEnabled = false;
        VoVolume = 10;
        ParticleIntensity = SettingsIntensity.Full;
        MotionIntensity = SettingsIntensity.Full;
        HapticIntensity = SettingsIntensity.Full;
        ColorblindLevel = ColorblindLevel.None;
        SpriteScale = 10;
    }

    #region Properties

    ColorblindLevel _colorblindLevel;
    SettingsIntensity _hapticIntensity;
    SettingsIntensity _motionIntensity;
    int _musicVolume;
    SettingsIntensity _particleIntensity;
    int _soundVolume;
    bool _voEnabled;
    int _voVolume;
    int _spriteScale;

    [CreateProperty]
    public int SoundVolume {
        get => _soundVolume;
        set {
            _soundVolume = value;
            NotifyPropertyChanged();
        }
    }

    [CreateProperty]
    public int MusicVolume {
        get => _musicVolume;
        set {
            _musicVolume = value;
            NotifyPropertyChanged();
        }
    }

    [CreateProperty]
    public bool VoEnabled {
        get => _voEnabled;
        set {
            _voEnabled = value;
            NotifyPropertyChanged();
        }
    }

    [CreateProperty]
    public int VoVolume {
        get => _voVolume;
        set {
            _voVolume = value;
            NotifyPropertyChanged();
        }
    }

    [CreateProperty]
    public SettingsIntensity ParticleIntensity {
        get => _particleIntensity;
        set {
            _particleIntensity = value;
            NotifyPropertyChanged();
        }
    }

    [CreateProperty]
    public SettingsIntensity MotionIntensity {
        get => _motionIntensity;
        set {
            _motionIntensity = value;
            NotifyPropertyChanged();
        }
    }

    [CreateProperty]
    public SettingsIntensity HapticIntensity {
        get => _hapticIntensity;
        set {
            _hapticIntensity = value;
            NotifyPropertyChanged();
        }
    }

    [CreateProperty]
    public int SpriteScale {
        get => _spriteScale;
        set {
            _spriteScale = value;
            NotifyPropertyChanged();
        }
    }

    [CreateProperty]
    public ColorblindLevel ColorblindLevel {
        get => _colorblindLevel;
        set {
            _colorblindLevel = value;
            NotifyPropertyChanged();
        }
    }

    #endregion
}