using System;
using UnityEngine;
using StansAssets.Foundation.Async;

namespace SA.Android.Utilities
{
    static class AN_MonoJavaCallback
    {
        static bool s_IsInited = false;

        class AndroidCallbackHandler<T> : AndroidJavaProxy
        {
            readonly Action<T> m_ResultHandler = delegate { };

            public AndroidCallbackHandler(Action<T> resultHandler)
                : base("com.stansassets.core.interfaces.AN_CallbackJsonHandler")
            {
                m_ResultHandler = resultHandler;
            }

            public void onHandleResult(string json, bool forceMainThread)
            {
                AN_Logger.LogCommunication("[Async] Sent to Unity ->: " + json);
                var result = JsonUtility.FromJson<T>(json);
                if (forceMainThread)
                    MainThreadDispatcher.Enqueue(() =>
                    {
                        m_ResultHandler.Invoke(result);
                    });
                else
                    m_ResultHandler.Invoke(result);
            }
        }

        public static AndroidJavaProxy ActionToJavaObject<T>(Action<T> action)
        {
            if (Application.isEditor) return null;

            if (!s_IsInited)
            {
                MainThreadDispatcher.Init();
                s_IsInited = true;
            }

            return new AndroidCallbackHandler<T>(action);
        }
    }
}
