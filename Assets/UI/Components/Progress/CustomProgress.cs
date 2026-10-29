using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CustomProgress : VisualElement {
    int discreteSteps = 1;
    float max = 1f;
    float min;
    List<Image> ticks;

    float value;
    float valueStep;

    public CustomProgress() {
        AddToClassList("progress-wrapper");
        if (!Discrete) throw new NotSupportedException("Non discrete not supported yet");
        InitializeDiscrete();
    }

    [UxmlAttribute("discrete")] public bool Discrete { get; set; } = true;

    [UxmlAttribute("discrete-steps")]
    [CreateProperty]
    public int DiscreteSteps {
        get => discreteSteps;
        set {
            if (value == discreteSteps) return;
            discreteSteps = value;
            NotifyPropertyChanged(nameof(DiscreteSteps));
            InitializeDiscrete();
        }
    }

    [UxmlAttribute("value")]
    [CreateProperty]
    public float Value {
        get => value;
        set {
            if (Mathf.Approximately(this.value, value)) return;
            this.value = value;
            NotifyPropertyChanged(nameof(Value));
            UpdateProgressBar();
        }
    }

    [UxmlAttribute("min")]
    [CreateProperty]
    public float Min {
        get => min;
        set {
            min = value;
            UpdateProgressBar();
        }
    }

    [UxmlAttribute("max")]
    [CreateProperty]
    public float Max {
        get => max;
        set {
            if (Mathf.Approximately(max, value)) return;
            max = value;
            NotifyPropertyChanged(nameof(Max));
            UpdateProgressBar();
        }
    }

    [UxmlAttribute("image-false")] public Texture ImageFalse { get; set; }

    [UxmlAttribute("image-true")] public Texture ImageTrue { get; set; }

    void InitializeDiscrete() {
        if (DiscreteSteps <= 0) throw new ArgumentOutOfRangeException("discrete-steps must be greater than 0");
        valueStep = (max - min) / DiscreteSteps;
        Clear();
        ticks = new();
        for (var i = 0; i < DiscreteSteps; i++) {
            var tick = new Image();
            tick.AddToClassList("tick");
            Add(tick);
            ticks.Add(tick);
        }

        UpdateDiscrete();
    }

    void UpdateProgressBar() {
        if (Discrete) UpdateDiscrete();
        else UpdateContinuous();
    }

    void UpdateDiscrete() {
        for (var i = 0; i < DiscreteSteps; i++) {
            var tick = ticks[i];
            var isActive = value >= min + valueStep * (i + 1);
            tick.image = isActive ? ImageTrue : ImageFalse;
        }

        Debug.Log(ticks[0]?.image);
        Debug.Log("Discrete steps: " + DiscreteSteps + ", value: " + value + ", min: " + min + ", max: " + max);
    }

    void UpdateContinuous() { }
}