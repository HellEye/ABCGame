using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class SlideToggle: BaseField<bool>
{
    public static readonly string className = "slide-toggle";
    public static readonly string inputClassName = "slide-toggle__input";
    public static readonly string inputKnobClassName = "slide-toggle__input-knob";
    public static readonly string inputCheckedClassName = "slide-toggle__input--checked";

    VisualElement input;
    VisualElement knob;
    
    string enabledText = "On";
    string disabledText = "Off";

    Color enabledBackgroundColor = new(0.2f, 0.5f, 0.85f);
    Color disabledBackgroundColor = new(0.3f, 0.3f, 0.3f);

    [UxmlAttribute]
    public string EnabledText
    {
        get => enabledText;
        set
        {
            enabledText = value;
            UpdateVisuals();
        }
        
    }

    [UxmlAttribute]
    public string DisabledText
    {
        get => disabledText;
        set {
            disabledText = value;
            UpdateVisuals();
        }
    }

    [UxmlAttribute]
    public Color EnabledBackgroundColor
    {
        get => enabledBackgroundColor;
        set {
            enabledBackgroundColor = value;
            UpdateVisuals();
        }
    }
    
    [UxmlAttribute]
    public Color DisabledBackgroundColor
    {
        get => disabledBackgroundColor;
        set{
            disabledBackgroundColor = value;
            UpdateVisuals();
        }
    }

    public SlideToggle(): base(null, new VisualElement())
    {
        AddToClassList(className);

        input = this.Q(className: inputUssClassName);
        input.AddToClassList(inputClassName);
        
        knob = new VisualElement();
        knob.AddToClassList(inputKnobClassName);
        input.Add(knob);
        
        RegisterCallback<ClickEvent>(OnClick);
        UpdateVisuals();
    }

    void OnClick(ClickEvent _)
    {
        value = !value;
    }

    public override void SetValueWithoutNotify(bool newValue)
    {
        base.SetValueWithoutNotify(newValue);
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        input.EnableInClassList(inputClassName, value);
        
        input.style.backgroundColor = value ? enabledBackgroundColor : disabledBackgroundColor;
        
        labelElement.text = value ? enabledText : disabledText;
    }
}
