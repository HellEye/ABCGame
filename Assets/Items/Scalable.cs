using UnityEngine;
using Reflex.Attributes;
using Unity.VisualScripting;
using UnityEditor.SettingsManagement;
using UnityEngine.UIElements;
using System;
using Unity.Properties;

public class Scalable : MonoBehaviour
{
    [Inject] MainMenuSettingsData settingsData;


    [SerializeField] float _minScaleValue;
    [SerializeField] float _maxScaleValue;

    private const float clampScalingValue = 0.01f;

    private void Start()
    {
        SetScale();
    }
    void OnEnable()
    {
        settingsData.propertyChanged += OnScaleChange;
    }

    void OnDisable()
    {
        settingsData.propertyChanged -= OnScaleChange;
    }

    private void OnScaleChange(object sender, BindablePropertyChangedEventArgs args)
    {
        if (args.propertyName == nameof(settingsData.SpriteScale))
        {
            SetScale();
        }
    }

    private void SetScale()
    {
        float targetSpriteScale = Mathf.Lerp(MinScaleValue, MaxScaleValue, settingsData.SpriteScale * clampScalingValue);
        this.transform.localScale = new Vector3(targetSpriteScale, targetSpriteScale, 1);
    }

    [CreateProperty]
    public float MinScaleValue
    {
        get => _minScaleValue;
        set
        {
            _minScaleValue = value;
        }
    }

    [CreateProperty]
    public float MaxScaleValue
    {
        get => _maxScaleValue;
        set
        {
            _maxScaleValue = value;
        }
    }
}
