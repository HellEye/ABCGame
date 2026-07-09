using Reflex.Attributes;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

public class Scalable : MonoBehaviour {
    const float clampScalingValue = 0.01f;


    [SerializeField] float _minScaleValue;
    [SerializeField] float _maxScaleValue;
    [Inject] MainMenuSettingsData settingsData;

    [CreateProperty] public float MinScaleValue { get => _minScaleValue; set => _minScaleValue = value; }

    [CreateProperty] public float MaxScaleValue { get => _maxScaleValue; set => _maxScaleValue = value; }

    void Start() => SetScale();

    void OnEnable() => settingsData.propertyChanged += OnScaleChange;

    void OnDisable() => settingsData.propertyChanged -= OnScaleChange;

    void OnScaleChange(object sender, BindablePropertyChangedEventArgs args) {
        if (args.propertyName == nameof(settingsData.SpriteScale)) SetScale();
    }

    void SetScale() {
        var targetSpriteScale = Mathf.Lerp(MinScaleValue, MaxScaleValue, settingsData.SpriteScale * clampScalingValue);
        transform.localScale = new(targetSpriteScale, targetSpriteScale, 1);
    }
}