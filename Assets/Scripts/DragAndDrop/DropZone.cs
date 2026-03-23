using UnityEngine;
using UnityEngine.Events;

public class DropZone : MonoBehaviour
{
    public UnityEvent OnDrop;

    public void Drop(Draggable draggable)
    {
        Debug.Log("Dropped " + draggable.name);
        OnDrop?.Invoke();
    }
}