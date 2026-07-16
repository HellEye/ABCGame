using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("PillSlider")]
public partial class PillSlider : BaseField<float>, INotifyBindablePropertyChanged {
    readonly Button minusButton;
    readonly VisualElement pillsContainer;
    readonly Button plusButton;

    public PillSlider() : base(null, null) {
        AddToClassList("PillSlider__container");
        pillsContainer = new();
        pillsContainer.AddToClassList("PillSlider__pillsContainer");

        minusButton = new() {
            text = "-"
        };
        minusButton.AddToClassList("PillSlider__button");
        minusButton.AddToClassList("PillSlider__minusButton");
        minusButton.clicked += () => ChangeCurrentStep(-1);
        plusButton = new() {
            text = "+"
        };
        plusButton.AddToClassList("PillSlider__button");
        plusButton.clicked += () => ChangeCurrentStep(1);

        Add(minusButton);
        Add(pillsContainer);
        Add(plusButton);

        RecalculatePerStep();
        RedrawPillAmount();
        RefreshPills();
    }

    int CurrentStep {
        get => Mathf.FloorToInt((value - Min) / perStep);
        set => this.value = Mathf.Clamp(Min + value * perStep, Min, Max);
    }

    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;


    public void SetValueWithoutNotify(float newValue) {
        currentValue = newValue;
        RefreshPills();
        // no events
    }


    void ChangeCurrentStep(int change) => CurrentStep += change;

    void RecalculatePerStep() => perStep = (Max - Min) / Steps;

    void RefreshPills() {
        var currentStep = CurrentStep;
        foreach (var (pill, i) in pillsContainer.Children().OfType<Button>().Indexed())
            pill.EnableInClassList("PillSlider__pill--active", i < currentStep);

        Debug.Log("Pills Refreshed");
        Debug.Log($"Current Step: {currentStep}, value: {currentValue}");
    }

    void RedrawPillAmount() {
        Debug.Log("Drawing Pills");
        pillsContainer.Clear();
        for (var i = 0; i < Steps; i++) {
            var pill = new Button();
            pill.AddToClassList("PillSlider__pill");
            pillsContainer.Add(pill);
            var index = i;
            pill.clicked += () => CurrentStep = index + 1;
        }
    }

    void Notify([CallerMemberName] string prop = "") {
        propertyChanged?.Invoke(this, new(prop));
        NotifyPropertyChanged(prop);
    }

    #region Attributes

    float currentValue;
    float max;
    float min;
    int steps;

    float perStep;

    [UxmlAttribute("max")]
    [CreateProperty]
    public float Max {
        get => max;
        set {
            if (Mathf.Approximately(max, value)) return;
            max = value;
            RecalculatePerStep();
            RefreshPills();
        }
    }

    [UxmlAttribute("min")]
    [CreateProperty]
    public float Min {
        get => min;
        set {
            if (Mathf.Approximately(min, value)) return;
            min = value;
            RecalculatePerStep();
            RefreshPills();
        }
    }

    [UxmlAttribute("steps")]
    [CreateProperty]
    public int Steps {
        get => steps;
        set {
            if (value == steps) return;
            steps = value;
            RecalculatePerStep();
            RedrawPillAmount();
            RefreshPills();
        }
    }


    [CreateProperty]
    public override float value {
        get => currentValue;
        set {
            if (Mathf.Approximately(currentValue, value)) return;
            var prev = currentValue;
            SetValueWithoutNotify(value);
            showMixedValue = false;
            Notify();
            using var evt = ChangeEvent<float>.GetPooled(prev, currentValue);
            evt.target = this;
            SendEvent(evt);
        }
    }

    #endregion
}