using UnityEngine;

[System.Serializable]
public class MinigameCardData
{
    [Header("Content")]
    public string title;

    [TextArea]
    public string description;

    [Header("Images")]
    public Sprite thumbnail;
    public Sprite cornerSprite;
    public Sprite heartSprite;

    [Header("Colours")]
    public Color heartColor = Color.white;
}