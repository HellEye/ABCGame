using System;
using System.Linq;
using UnityEngine.UIElements;

public static class VisualElementExtensions {
    public static void FlattenTemplateContainers(this VisualElement root) => FlattenTemplateContainersRecursive(root);

    static void FlattenTemplateContainersRecursive(VisualElement parent) {
        var children = parent.Children().ToList();

        foreach (var child in children)
            if (child is TemplateContainer templateContainer) {
                var grandchildren = templateContainer.Children().ToList();

                foreach (var grandchild in grandchildren) {
                    TransferStyles(templateContainer, grandchild);
                    parent.Add(grandchild);
                }

                parent.Remove(templateContainer);
                foreach (var grandchild in grandchildren) FlattenTemplateContainersRecursive(grandchild);
            }
            else {
                FlattenTemplateContainersRecursive(child);
            }
    }

    static void TransferStyles(VisualElement source, VisualElement destination) {
        foreach (var className in source.GetClasses())
            destination.AddToClassList(className);

        for (var i = source.styleSheets.count - 1; i >= 0; i--) {
            var styleSheet = source.styleSheets[i];
            destination.styleSheets.Add(styleSheet);
        }

        if (string.IsNullOrEmpty(destination.name))
            destination.name = source.name;

        destination.dataSource ??= source.dataSource;
    }

    public static LocalizedDropdown<T> ToLocalizedDropdown<T>(this DropdownField field, Func<T, string> getName)
        where T : Enum =>
        new(field, getName);
}