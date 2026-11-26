using System;

public interface ISelectEvents {
    public event Action OnSelected;
    public event Action OnDeselected;
}