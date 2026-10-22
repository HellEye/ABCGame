using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public class WireDragAnimations : MonoBehaviour {
    public float snapTime = 0.1f;
    public float incorrectTransitionDuration = 0.2f;

    [SerializeField] AnimationCurve incorrectAnimation;

    CompositeMotionHandle currentHandle;
    Draggable draggable;
    ScreenPositionPlacer placer;
    public bool IsEnabled => enabled;

    void OnEnable() {
        draggable = GetComponent<Draggable>();
        currentHandle = new();
        draggable.OnDropped += HandleDrag;
    }

    void OnDisable() {
        draggable.OnDropped -= HandleDrag;
        currentHandle?.Cancel();
    }

    void HandleDrag(DropInfo dropInfo) {
        switch (dropInfo.dropType) {
            case DropType.Incorrect:
            case DropType.Empty:
                AnimateIncorrect(dropInfo);
                break;
            case DropType.Correct:
                AnimateIntoPosition(dropInfo.targetPos).Forget();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    void AnimateIncorrect(DropInfo dropInfo) =>
        LMotion.Create(transform.position, dropInfo.startPos, incorrectTransitionDuration)
            .WithEase(incorrectAnimation)
            .BindToPosition(transform)
            .AddTo(currentHandle);

    async UniTask AnimateIntoPosition(Vector3 position) {
        await LMotion
            .Create(transform.position, position, snapTime)
            .WithEase(Ease.InOutCubic)
            .BindToPosition(transform)
            .ToUniTask();

        draggable.enabled = false;
    }
}