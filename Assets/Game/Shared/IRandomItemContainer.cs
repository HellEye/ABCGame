using System.Collections.Generic;

public interface IRandomItemContainer {
    public IEnumerable<IElement> GetAllItems();
}