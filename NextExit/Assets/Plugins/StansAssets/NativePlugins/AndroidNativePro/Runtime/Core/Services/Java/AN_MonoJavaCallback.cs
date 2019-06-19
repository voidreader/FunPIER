
namespace SA.Android.Utilities
{

    using System;
    using UnityEngine;

    using SA.Foundation.Threading;

    public static class AN_MonoJavaCallback
    {
        private static bool s_isInited = false;


        private class AndroidCallbackHandler<T> : AndroidJavaProxy
        {
            private readonly Action<T> m_resultHandler = delegate {};

            public AndroidCallbackHandler(Action<T> resultHandler) : base("com.stansassets.core.interfaces.AN_CallbackJsonHandler") {
                m_resultHandler = resultHandler;
            }

            public void onHandleResult(string json, bool forceMainThread) {
                AN_Logger.LogCommunication("[Async] Sent to Unity ->: " + json);
                var result = JsonUtility.FromJson<T>(json);
                if (forceMainThread)
                {
                    SA_MainThreadDispatcher.Enqueue(() => {
                        m_resultHandler.Invoke(result);
                    });
                }
                else
                {
                    m_resultHandler.Invoke(result);
                }
            }
        }

        // В дальнейшем будем использовать эту функцию для оборачивания C# делегата
        public static AndroidJavaProxy ActionToJavaObject<T>(Action<T> action) {

            if(!s_isInited) {
                SA_MainThreadDispatcher.Init();
                s_isInited = true;
            }
            
            return new AndroidCallbackHandler<T>(action);
        }
    }
}