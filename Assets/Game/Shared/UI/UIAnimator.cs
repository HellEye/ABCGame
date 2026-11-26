using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;
using UnityEngine.UIElements;

public enum UIAnimationFieldType {
    Position,
    Rotation,
    Scale,
    Color
}

[Serializable]
public class UIAnimatorAnimationStyle {
    public UIAnimationFieldType animationField;
    public AnimationCurve curve;
    public float duration = 1f;
    public float startDelay;
    public LoopType loopType = LoopType.Restart;

    // Position (Vector2)
    public Vector2 fromPosition = Vector2.zero;
    public Vector2 toPosition = Vector2.zero;

    // Rotation (float, degrees)
    public float fromRotation;
    public float toRotation;

    // Scale (float, uniform)
    public float fromScale = 1f;
    public float toScale = 1f;

    // Color (via gradient)
    public Gradient colorGradient;
}

[Serializable]
public class UIAnimatorAnimation {
    [Tooltip("Name of the target VisualElement")]
    public string target;

    public List<UIAnimatorAnimationStyle> entryStyles = new();
    public List<UIAnimatorAnimationStyle> styles = new();
}

public class UIAnimator : MonoBehaviour {
    [SerializeField] List<UIAnimatorAnimation> animations;
    [SerializeField] UIDocument document;

    CompositeMotionHandle allLoopHandles = new();
    VisualElement root;

    void Start() => root = document.rootVisualElement;

    void OnDestroy() => Cancel();

    public void Play() => PlayAnimation().Forget();

    [ContextMenu("Play Animation")]
    async UniTask PlayAnimation() {
        Cancel();

        var entryTasks = new List<UniTask>();

        foreach (var anim in animations) {
            var target = root.Q<VisualElement>(anim.target);
            if (target == null) {
                Debug.LogWarning($"UIAnimator: Could not find target element '{anim.target}'");
                continue;
            }

            // Build entry sequences per field type
            var entryByField = anim.entryStyles
                .GroupBy(s => s.animationField)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var fieldType in entryByField.Keys)
                entryTasks.Add(PlayEntryFieldSequence(entryByField[fieldType], target));

            // Build loop sequences per field type
            var loopByField = anim.styles
                .GroupBy(s => s.animationField)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var fieldType in loopByField.Keys) {
                var loopHandle = BuildFieldSequence(loopByField[fieldType], target, true);
                if (loopHandle.IsValid()) allLoopHandles.Add(loopHandle);
            }
        }

        // Wait for all entry animations to complete
        if (entryTasks.Count > 0) await UniTask.WhenAll(entryTasks);
    }

    async UniTask PlayEntryFieldSequence(List<UIAnimatorAnimationStyle> styles, VisualElement target) {
        var handle = BuildFieldSequence(styles, target, false);
        if (handle.IsValid()) await handle.ToUniTask();
    }

    MotionHandle BuildFieldSequence(List<UIAnimatorAnimationStyle> styles, VisualElement target, bool isLoop) {
        if (styles.Count == 0) return default;

        var sequenceBuilder = LSequence.Create();

        foreach (var style in styles) {
            var tween = CreateTween(style, target);
            if (style.startDelay > 0)
                sequenceBuilder.Insert(style.startDelay, tween);
            else
                sequenceBuilder.Join(tween);
        }

        return isLoop
            ? sequenceBuilder.Run(builder => builder.WithLoops(-1, styles[0].loopType))
            : sequenceBuilder.Run();
    }

    MotionHandle CreateTween(UIAnimatorAnimationStyle animStyle, VisualElement target) {
        if (animStyle.curve == null) {
            Debug.LogWarning("UIAnimator: Animation curve is null");
            animStyle.curve = AnimationCurve.Linear(0, 0, 1, 1);
        }

        return animStyle.animationField switch {
            UIAnimationFieldType.Position =>
                LMotion.Create(animStyle.fromPosition, animStyle.toPosition, animStyle.duration)
                    .WithEase(animStyle.curve)
                    .Bind(target.style, (v, targetStyle) => targetStyle.translate = v),

            UIAnimationFieldType.Rotation =>
                LMotion.Create(animStyle.fromRotation, animStyle.toRotation, animStyle.duration)
                    .WithEase(animStyle.curve)
                    .Bind(
                        target.style,
                        (value, targetStyle) => targetStyle.rotate = new Rotate(new Angle(value, AngleUnit.Degree))),

            UIAnimationFieldType.Scale =>
                LMotion.Create(animStyle.fromScale, animStyle.toScale, animStyle.duration)
                    .WithEase(animStyle.curve)
                    .Bind(target.style, (value, targetStyle) => targetStyle.scale = new Vector2(value, value)),

            UIAnimationFieldType.Color =>
                LMotion.Create(0f, 1f, animStyle.duration)
                    .WithEase(animStyle.curve)
                    .Bind(
                        target,
                        animStyle,
                        (value, element, anim) => {
                            var color = anim.colorGradient.Evaluate(value);
                            if (element is Image img) img.tintColor = color;
                            else element.style.color = color;
                        }),
            var _ => LMotion.Create(0f, 1f, animStyle.duration).Bind(target.style, (_, _) => { })
        };
    }

    public void Cancel() {
        allLoopHandles.Cancel();
        allLoopHandles = new();
    }
}