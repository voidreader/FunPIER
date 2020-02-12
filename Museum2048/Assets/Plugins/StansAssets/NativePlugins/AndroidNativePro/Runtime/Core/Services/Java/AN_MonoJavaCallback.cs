using System;
using UnityEngine;
using SA.Foundation.Threading;

namespace SA.Android.Utilities
{
    public static class AN_MonoJavaCallback
    {
        private static bool s_IsInited = false;

        private class AndroidCallbackHandler<T> : AndroidJavaProxy
        {
            private readonly Action<T> m_ResultHandler = delegate {};

            public AndroidCallbackHandler(Action<T> resultHandler) : base("com.stansassets.core.interfaces.AN_CallbackJsonHandler") {
                m_ResultHandler = resultHandler;
            }

            public void onHandleResult(string json, bool forceMainThread) {
                AN_Logger.LogCommunication("[Async] Sent to Unity ->: " + json);
                var result = JsonUtility.FromJson<T>(json);
                if (forceMainThread)
                {
                    SA_MainThreadDispatcher.Enqueue(() => {
                        m_ResultHandler.Invoke(result);
                    });
                }
                else
                {
                    m_ResultHandler.Invoke(result);
                }
            }
        }
        
        public static AndroidJavaProxy ActionToJavaObject<T>(Action<T> action) {

            if (Application.isEditor) { return null; }
            
            if(!s_IsInited) {
                SA_MainThreadDispatcher.Init();
                s_IsInited = true;
            }
            
            return new AndroidCallbackHandler<T>(action);
        }
    }
}