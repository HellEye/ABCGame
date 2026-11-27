using UnityEngine;

public class EndGameAnimator : MonoBehaviour {
    [SerializeField] GameUIManager gameUIManager;
    [SerializeField] UIAnimator animator;

    void Start() => gameUIManager.OnEndGamePopupOpen += PlayEndGameAnimation;

    void OnDestroy() => gameUIManager.OnEndGamePopupOpen -= PlayEndGameAnimation;

    void PlayEndGameAnimation() => animator.Play();
}