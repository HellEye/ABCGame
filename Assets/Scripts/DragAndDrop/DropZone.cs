using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class DropZone : MonoBehaviour
{
    public VisualEffect correctEffect;
    public void Drop(Draggable draggable)
    {
        Debug.Log("Dropped " + draggable.name);
        
        // Logic to check if the draggable is correct
        // Correct(draggable);
        Incorrect(draggable);
    }

    void Correct(Draggable draggable)
    {
        correctEffect.Play();
        draggable.DropCorrect();
    }

    void Incorrect(Draggable draggable)
    {
        draggable.DropIncorrect();
    }
}