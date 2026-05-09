using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuAspect : MonoBehaviour
{
    [SerializeField] private UIDocument mainMenuDoc;
    public new Camera camera;
    private void Update()
    {
        float ratio = camera.aspect;
        if (ratio < 0.7)
        {
            mainMenuDoc.rootVisualElement.AddToClassList("tight");
        }
        else
        {
            mainMenuDoc.rootVisualElement.RemoveFromClassList("tight");
        }
    }
}
