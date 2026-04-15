using System;
using UnityEngine;

[DefaultExecutionOrder(-999)]
public class Input : MonoBehaviour {
    public static Input instance { get; private set; }
    public InputSystem_Actions actions { get; private set; }

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        EnsureActions();

        actions.Player.Enable();
    }

    void OnEnable() {
        if (instance == null) {
            instance = this;
        }

        EnsureActions();
        actions.Player.Enable();
    }

    void OnDisable() { actions?.Player.Disable(); }

    void OnDestroy() {
        if (instance == this) {
            instance = null;
        }

        actions?.Dispose();
        actions = null;
    }

    void EnsureActions() { actions ??= new(); }
}