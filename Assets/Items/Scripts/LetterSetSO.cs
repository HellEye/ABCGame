using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "LetterSet", menuName = "Items/LetterSet")]
public class LetterSetSO : ScriptableObject, ISpawnableGroup {
    [SerializeField] string groupName;
    public string letters;

    [SerializeField] [Tooltip("Text to display as a hint")]
    string targetText;

    public Difficulty difficulty;
    public Difficulty Difficulty => difficulty;
    public string Title => groupName;

    public string TargetText => targetText;

    public (IEnumerable<IElement> targets, IEnumerable<IElement> allItems) PickItems(
        DropZoneGameDifficulty gameDifficulty,
        MainMenuSettingsData data,
        ExcludeItemsSO excludeItems = null) {
        var uniqueLetters = (letters ?? string.Empty).Distinct().Select(letter => new Letter(letter.ToString()))
            .ToList();
        var pickedItems = uniqueLetters.PickRandom(gameDifficulty.itemTypes);
        var targets = pickedItems.PickRandom(gameDifficulty.targetTypes);
        return (targets, pickedItems);
    }
}