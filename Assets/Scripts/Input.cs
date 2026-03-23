using System;
using UnityEngine;

[DefaultExecutionOrder(-999)]
public class Input : MonoBehaviour
{
    public static Input instance;
    public InputSystem_Actions actions;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        actions = new();
    }

    void OnEnable() { actions.Player.Enable(); }
    void OnDisable() { actions.Player.Disable(); }
}