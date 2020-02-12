using UnityEngine;

namespace SA.Android.Utilities
{

    public static class AN_Logger
    {

        private static AndroidJavaClass s_nativeLogger; 

        [RuntimeInitializeOnLoadMethod]
        static void Init() {
            if (Application.platform != RuntimePlatform.Android) { return; }

            if(s_nativeLogger == null) {
                InitNativeLogger();
            }
        }

        private static void InitNativeLogger() {
            s_nativeLogger = new AndroidJavaClass("com.stansassets.core.utility.AN_Logger");
            var logLevel = AN_Settings.Instance.LogLevel;
            s_nativeLogger.CallStatic("SetLogLevel", logLevel.Info, logLevel.Warning, logLevel.Error, AN_Settings.Instance.WTFLogging);
        }

        private static AndroidJavaClass NativeLogger {
            get {
                if (s_nativeLogger == null) {
                    InitNativeLogger();
                }
                return s_nativeLogger;
            }
        }


        public static void Log(object message) {
            if (!AN_Settings.Instance.LogLevel.Info) { return; }
            if(Application.platform == RuntimePlatform.Android) {
                message = "Unity: " + message;
                NativeLogger.CallStatic("Log", message);
            } else {
                Debug.Log(message);
            }
        }


        public static void LogCommunication(string methodName, params string[] methodParams) {
            string message = methodName;

            for (int i = 0; i < methodParams.Length; i++) {
                if(i == 0) {
                    message += ":: ";
                } else {
                    message += "| ";
                }

                message += methodParams[i];
            }

            Log(message);
        }

        public static void LogWarning(object message) {
            if (!AN_Settings.Instance.LogLevel.Warning) { return; }

            if (Application.platform == RuntimePlatform.Android) {
                message = "Unity: " + message;
                NativeLogger.CallStatic("LogWarning", message);
            } else {
                Debug.LogWarning(message);
            }

        }


        public static void LogError(object message) {
            if (!AN_Settings.Instance.LogLevel.Error) { return; }

            if (Application.platform == RuntimePlatform.Android) {
                message = "Unity: " + message;
                NativeLogger.CallStatic("LogError", message);
            } else {
                Debug.LogError(message);
            }
        }

    }
}