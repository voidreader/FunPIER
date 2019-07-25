using UnityEngine;

namespace LightVibration
{

    public static class AndroidVibration
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject vibrationObj = VibrationActivity.activityObj.Get<AndroidJavaObject>("vibration");
#else
        private static AndroidJavaObject vibrationObj;
#endif

        public static void Vibrate()
        {
            vibrationObj.Call("vibrate");
        }

        public static void Vibrate(long milliseconds)
        {
            vibrationObj.Call("vibrate", milliseconds);
        }

        public static void Vibrate(long[] pattern, int repeat)
        {
            vibrationObj.Call("vibrate", pattern, repeat);
        }

        public static bool HasVibrator()
        {
            return vibrationObj.Call<bool>("hasVibrator");
        }

        public static void Cancel()
        {
            vibrationObj.Call("cancel");
        }
    }
}