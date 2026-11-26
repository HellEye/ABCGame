using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CustomProgress : VisualElement {
    int discreteSteps = 5;

    Texture leftIcon;
    float max = 1f;
    float min;
    List<Image> ticks;

    float value;

    public CustomProgress() => AddToClassList("progress-wrapper");

    [UxmlAttribute("discrete")] public bool Discrete { get; set; } = true;

    [UxmlAttribute("discrete-steps")]
    [CreateProperty]
    public int DiscreteSteps {
        get => discreteSteps;
        set {
            if (value == discreteSteps) return;
            discreteSteps = value;
            NotifyPropertyChanged(nameof(DiscreteSteps));
            Initialize();
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
            if (Mathf.Approximately(min, value)) return;
            min = value;
            NotifyPropertyChanged(nameof(Min));
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

    [UxmlAttribute("left-icon")]
    [CreateProperty]
    public Texture LeftIcon {
        get => leftIcon;
        set {
            leftIcon = value;
            Initialize();
        }
    }

    void Initialize() {
        Clear();
        if (LeftIcon != null) {
            var icon = new Image();
            icon.image = LeftIcon;
            icon.AddToClassList("progress-icon");
            Add(icon);
        }

        if (!Discrete) throw new NotSupportedException("Non discrete not supported yet");
        InitializeDiscrete();
    }


    void InitializeDiscrete() {
        if (DiscreteSteps <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(DiscreteSteps),
                DiscreteSteps,
                "Discrete steps must be greater than 0.");
        ticks = new();
        var container = new VisualElement();
        container.AddToClassList("ticks-container");
        for (var i = 0; i < DiscreteSteps; i++) {
            var tick = new Image();
            tick.AddToClassList("tick");
            tick.AddToClassList("tick-false");
            container.Add(tick);
            ticks.Add(tick);
        }

        Add(container);
        UpdateDiscrete();
    }

    void UpdateProgressBar() {
        if (Discrete) UpdateDiscrete();
        else UpdateContinuous();
    }

    void UpdateDiscrete() {
        if (ticks == null || ticks.Count != DiscreteSteps) return;

        int activeTickCount;
        if (Mathf.Approximately(min, max)) {
            activeTickCount = value >= max ? DiscreteSteps : 0;
        }
        else {
            var normalized = Mathf.InverseLerp(min, max, value);
            activeTickCount = Mathf.Clamp(Mathf.FloorToInt(normalized * DiscreteSteps), 0, DiscreteSteps);
        }

        for (var i = 0; i < DiscreteSteps; i++) {
            var tick = ticks[i];
            var isActive = i < activeTickCount;
            tick.EnableInClassList("tick-true", isActive);
            tick.EnableInClassList("tick-false", !isActive);
        }
    }

    void UpdateContinuous() { }
}