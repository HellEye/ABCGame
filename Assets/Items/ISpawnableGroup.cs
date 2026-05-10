using System.Collections.Generic;

public interface ISpawnableGroup {
    (List<ItemSO> targets, List<ItemSO> allItems) PickItems(DropZoneGameDifficulty difficulty);
}