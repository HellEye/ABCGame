using System;
using FMODUnity;
using UnityEngine;

public class FmodEvents : MonoBehaviour {
    public FmodEventsUI ui = new();
}

[Serializable]
public class FmodEventsUI {
    public EventReference buttonClick;
}