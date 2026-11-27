using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

public class GameProgressData : IDataSourceViewHashProvider, INotifyBindablePropertyChanged {
    float progress;

    float totalProgress;
    long viewVersion;

    [CreateProperty]
    public float Progress {
        get => progress;
        set {
            if (Mathf.Approximately(progress, value)) return;
            progress = value;
            NotifyPropertyChanged();
        }
    }

    [CreateProperty]
    public float TotalProgress {
        get => totalProgress;
        set {
            if (Mathf.Approximately(totalProgress, value)) return;
            totalProgress = value;
            NotifyPropertyChanged();
        }
    }

    public long GetViewHashCode() => viewVersion;
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

    void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
        viewVersion++;
        propertyChanged?.Invoke(this, new(propertyName));
    }
}