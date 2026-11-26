using System;
using FMODUnity;
using UnityEngine;

public class FmodEvents : MonoBehaviour {
    public FmodEventsUI ui = new();
    public FmodEventsGame game = new();
}

[Serializable]
public class FmodEventsUI {
    public EventReference buttonClick;
}

[Serializable]
public class FmodEventsGame {
    public EventReference dropCorrect;
    public EventReference dropIncorrect;
}