using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class PillSliderData : IDataSourceViewHashProvider, INotifyBindablePropertyChanged {
    [SerializeField] [Range(0, 10)] float currentValue;
    long viewVersion;

    [CreateProperty]
    public float Value {
        get => currentValue;
        set {
            if (Mathf.Approximately(currentValue, value)) return;
            currentValue = value;
            NotifyPropertyChanged();
        }
    }

    public long GetViewHashCode() => viewVersion;
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

    void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
        viewVersion++;
        propertyChanged?.Invoke(this, new(propertyName));
    }

    public void Notify() => NotifyPropertyChanged(nameof(Value));
}

public class PillSliderTest : MonoBehaviour {
    [SerializeField] PillSliderData data;


    UIDocument document;

    void Start() {
        document = GetComponent<UIDocument>();
        var root = document.rootVisualElement;
        root.dataSource = data;

        var slider = root.Q<PillSlider>("pill-slider");
        slider.propertyChanged += (target, data) => {
            Debug.Log($"Property changed on {target}");
            Debug.Log($"name: {data.propertyName}");
        };
        slider.RegisterCallback<ChangeEvent<float>>(e => {
            Debug.Log($"Value changed: {e.newValue}");
        });
    }


    [ContextMenu("Refresh")]
    public void Notify() => data.Notify();
}