using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine.UIElements;

public class TtsState : INotifyBindablePropertyChanged, IDataSourceViewHashProvider {
    public bool available;
    public string error = "";

    long hash;
    public Action<bool> OnAvailableChanged;

    [CreateProperty]
    public bool Available {
        get => available;
        set {
            available = value;
            Notify();
            OnAvailableChanged?.Invoke(value);
        }
    }

    [CreateProperty]
    public string Error {
        get => error;
        set {
            error = value;
            Notify();
        }
    }

    public long GetViewHashCode() => hash;
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

    void Notify([CallerMemberName] string propertyName = "") {
        hash++;
        propertyChanged?.Invoke(this, new(propertyName));
    }
}