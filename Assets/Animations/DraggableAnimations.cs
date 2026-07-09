using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

[RequireComponent(typeof(Draggable))]
public class DraggableAnimations : MonoBehaviour {
    [Header("Incorrect drop zone animation")]
    public float incorrectShakeDelay = 5f;

    public int incorrectShakeFrequency = 5;
    public float incorrectTransitionDuration = 1f;

    [Header("Destroy animation")]
    public float destroyDuration = 0.5f;

    CompositeMotionHandle currentHandle;
    Draggable draggable;

    void OnEnable() {
        draggable ??= GetComponent<Draggable>();
        currentHandle = new();
        draggable.OnPickedUp += CancelAnimation;
        draggable.OnDroppedIncorrect += AnimateIncorrect;
        draggable.OnDroppedCorrect += UniTask.Action(AnimateAndDestroy);
    }

    void OnDisable() {
        draggable.OnPickedUp -= CancelAnimation;
        draggable.OnDroppedIncorrect -= AnimateIncorrect;
        draggable.OnDroppedCorrect -= UniTask.Action(AnimateAndDestroy);
        currentHandle?.Cancel();
        currentHandle = null;
    }

    public void CancelAnimation() => currentHandle?.Cancel();

    public void AnimateIncorrect(Vector3 initialPosition) =>
        LMotion
            .Create(transform.position, initialPosition, incorrectTransitionDuration)
            .WithEase(Ease.InOutSine)
            .BindToPosition(transform)
            .AddTo(currentHandle);

    public async UniTaskVoid AnimateAndDestroy() {
        await LMotion.Create(transform.localScale, Vector3.zero, destroyDuration)
            .BindToLocalScale(transform)
            .ToUniTask();
        Destroy(gameObject);
    }
}