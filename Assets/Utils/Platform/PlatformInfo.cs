using UnityEngine;

public static class PlatformInfo {
    public static bool IsMobile =>
        Application.platform is RuntimePlatform.Android
            or RuntimePlatform.IPhonePlayer;
}