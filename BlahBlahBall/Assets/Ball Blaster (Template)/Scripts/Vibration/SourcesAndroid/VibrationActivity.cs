using UnityEngine;

namespace LightVibration
{

    public static class VibrationActivity
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass activityClass = new AndroidJavaClass("com.izaron.androideffects.vibration.MyMainActivity");
    public static AndroidJavaClass unityActivityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject activityObj = unityActivityClass.GetStatic<AndroidJavaObject>("currentActivity");
#else
        public static AndroidJavaClass activityClass;
        public static AndroidJavaClass unityActivityClass;
        public static AndroidJavaObject activityObj;
#endif
    }
}