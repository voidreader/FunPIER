using UnityEngine;

namespace LightVibration
{

    public static class VibrationManager
    {
        public static void VibrateLight()
        {
#if UNITY_IOS
            if (TapticManager.IsSupport())
                TapticManager.Impact(ImpactFeedback.Light);
#endif
#if UNITY_EDITOR
            Debug.Log("Vibrate Light");
#endif
        }


        public static void VibrateMedium()
        {
#if UNITY_IOS
            if (TapticManager.IsSupport())
                TapticManager.Impact(ImpactFeedback.Midium);
#endif
#if UNITY_EDITOR
            Debug.Log("Vibrate Medium");
#endif
        }

        public static void VibrateHeavy()
        {
#if UNITY_IOS
            if (TapticManager.IsSupport())
                TapticManager.Impact(ImpactFeedback.Heavy);
#endif
#if UNITY_EDITOR
            Debug.Log("Vibrate Heavy");
#endif
        }


        public static void VibrateGameOver()
        {
#if UNITY_IOS
            if (TapticManager.IsSupport())
                TapticManager.Notification(NotificationFeedback.Warning);
#endif
#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
#if UNITY_EDITOR
            Debug.Log("Vibrate GameOver");
#endif
        }

    }
}