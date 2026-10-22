using System;
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
        draggable.OnDropped += HandleDrop;
    }

    void OnDisable() {
        draggable.OnPickedUp -= CancelAnimation;
        draggable.OnDropped -= HandleDrop;
        currentHandle?.Cancel();
        currentHandle = null;
    }

    void HandleDrop(DropInfo dropInfo) {
        switch (dropInfo.dropType) {
            case DropType.Empty:
                break;
            case DropType.Correct:
                AnimateAndDestroy(dropInfo.targetPos).Forget();
                break;
            case DropType.Incorrect:
                AnimateIncorrect(dropInfo.startPos);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void CancelAnimation() => currentHandle?.Cancel();

    public void AnimateIncorrect(Vector3 initialPosition) =>
        LMotion
            .Create(transform.position, initialPosition, incorrectTransitionDuration)
            .WithEase(Ease.InOutSine)
            .BindToPosition(transform)
            .AddTo(currentHandle);

    public async UniTask AnimateAndDestroy(Vector3 targetPosition) {
        await LMotion.Create(transform.localScale, Vector3.zero, destroyDuration)
            .BindToLocalScale(transform)
            .ToUniTask();
        Destroy(gameObject);
    }
}