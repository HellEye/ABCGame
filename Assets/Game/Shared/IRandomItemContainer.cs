using System.Collections.Generic;

public interface IRandomItemContainer {
    public IEnumerable<ItemSO> GetAllItems();
}