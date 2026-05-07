using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlObject]
public partial class ConditionalClassBinding : CustomBinding {
    public enum ConditionMode {
        Boolean,
        NumericRange
    }

    public ConditionalClassBinding() => updateTrigger = BindingUpdateTrigger.OnSourceChanged;

    [UxmlAttribute] public string TrueClass { get; set; } = string.Empty;
    [UxmlAttribute] public string FalseClass { get; set; } = string.Empty;
    [UxmlAttribute] public ConditionMode Mode { get; set; } = ConditionMode.Boolean;
    [UxmlAttribute] public float RangeMin { get; set; } = float.NegativeInfinity;
    [UxmlAttribute] public float RangeMax { get; set; } = float.PositiveInfinity;

    protected override void OnDataSourceChanged(in DataSourceContextChanged context) { }

    protected override BindingResult Update(in BindingContext context) {
        var source = context.dataSource;
        Debug.Log($"Custom binding update, {context.dataSource}.{context.dataSourcePath}");
        if (source == null)
            return Failure("No data source found.");

        if (!TryEvaluateCondition(context, out var condition))
            return Failure($"Could not read value at path '{context.dataSourcePath}'.");
        Debug.Log($"Condition evaluated to {condition}");
        var element = context.targetElement;
        if (!string.IsNullOrEmpty(TrueClass))
            element.EnableInClassList(TrueClass, condition);
        if (!string.IsNullOrEmpty(FalseClass))
            element.EnableInClassList(FalseClass, !condition);

        return new(BindingStatus.Success);
    }

    bool TryEvaluateCondition(in BindingContext context, out bool condition) {
        condition = false;
        return Mode switch {
            ConditionMode.Boolean =>
                TryGetValue<bool>(context, out var b) && (condition = b) == b,
            ConditionMode.NumericRange =>
                TryGetNumericValue(context, out var n)
                && (condition = n >= RangeMin && n <= RangeMax) == condition,
            var _ => false
        };
    }

    bool TryGetValue<T>(in BindingContext context, out T value) {
        var source = context.dataSource;
        return PropertyContainer.TryGetValue(
            ref source,
            context.dataSourcePath,
            out value,
            out var _);
    }

    bool TryGetNumericValue(in BindingContext context, out float value) {
        if (TryGetValue(context, out value)) return true;
        if (TryGetValue<int>(context, out var i)) {
            value = i;
            return true;
        }

        if (TryGetValue<double>(context, out var d)) {
            value = (float)d;
            return true;
        }

        value = 0f;
        return false;
    }

    static BindingResult Failure(string msg) => new(BindingStatus.Failure, msg);
}