using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public static class EnumDropdownBinding {
    public static void RegisterConverter<TEnum>(Func<TEnum, string> getLabel)
        where TEnum : Enum {
        var groupName = $"{typeof(TEnum).Name}Binding";
        if (ConverterGroups.TryGetConverterGroup(groupName, out var _))
            return;

        var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
        var toDisplay = values.ToDictionary(v => v, getLabel);
        var toEnum = toDisplay.ToDictionary(kv => kv.Value, kv => kv.Key);

        var group = new ConverterGroup(groupName);
        group.AddConverter((ref TEnum v) => toDisplay.GetValueOrDefault(v, string.Empty));
        group.AddConverter((ref string v) => toEnum.GetValueOrDefault(v, default));
        ConverterGroups.RegisterConverterGroup(group);
    }

    public static void SetChoices<TEnum>(DropdownField field, Func<TEnum, string> getLabel)
        where TEnum : Enum =>
        field.choices = Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Select(getLabel)
            .ToList();
}