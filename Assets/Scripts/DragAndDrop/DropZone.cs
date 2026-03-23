using UnityEngine;

public class DropZone : MonoBehaviour
{
    public void Drop(Draggable draggable) { Debug.Log("Dropped " + draggable.name); }
}