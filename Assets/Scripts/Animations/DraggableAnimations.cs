using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using DelayType = LitMotion.DelayType;

[RequireComponent(typeof(Draggable))]
public class DraggableAnimations : MonoBehaviour {
    [Header("Incorrect drop zone animation")]
    public float incorrectShakeDelay = 5f;

    public int incorrectShakeFrequency = 5;
    public float incorrectShakeDuration = 1f;

    [Header("Destroy animation")]
    public float destroyDuration = 0.5f;

    CancellationTokenSource cts;
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

    public void CancelAnimation() =>
        // cts?.Cancel();
        // cts?.Dispose();
        // cts = new();
        currentHandle?.Cancel();

    public void AnimateIncorrect() =>
        LMotion.Punch
            .Create(transform.position, new(0.1f, 0, 0), incorrectShakeDuration)
            .WithEase(Ease.OutCubic)
            .WithDampingRatio(2f)
            .WithDelay(incorrectShakeDelay, DelayType.EveryLoop)
            .WithFrequency(incorrectShakeFrequency)
            .WithLoops(-1)
            .BindToPosition(transform)
            .AddTo(currentHandle);

    public async UniTaskVoid AnimateAndDestroy() {
        await LMotion.Create(Vector3.one, Vector3.zero, destroyDuration)
            .BindToLocalScale(transform)
            .ToUniTask();
        Destroy(gameObject);
    }
}