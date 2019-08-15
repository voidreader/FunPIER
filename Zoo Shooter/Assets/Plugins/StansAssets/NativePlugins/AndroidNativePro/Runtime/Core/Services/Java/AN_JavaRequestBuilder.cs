using System;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Android.Utilities
{
    internal class AN_JavaRequestBuilder
    {
        private string m_ClassName;
        private string m_MethodName;
        private List<object> m_Arguments = new List<object>();
        
        public AN_JavaRequestBuilder(string className, string methodName)
        {
            m_ClassName = className;
            m_MethodName = methodName;
        }

        public void AddArgument(object arg)
        {
            var convertedArg =  AN_Java.Bridge.ConvertObjectData(arg);
            m_Arguments.Add(convertedArg);
        }
        
        public void AddCallback<T>(Action<T> callback)
        {
            m_Arguments.Add(AN_MonoJavaCallback.ActionToJavaObject(callback));
        }

        public void Invoke()
        {
            AN_Java.Bridge.LogCommunication(m_ClassName, m_MethodName, m_Arguments);
            if (Application.isEditor)
            {
                return;
            }
            var javaClass =  AN_Java.Bridge.GetJavaClass(m_ClassName);
            javaClass.CallStatic(m_MethodName, m_Arguments.ToArray());
        }
        
        
        public R Invoke<R>()
        {
            AN_Java.Bridge.LogCommunication(m_ClassName, m_MethodName, m_Arguments);
            if (Application.isEditor)
            {
                return default(R);
            }

            var javaClass =  AN_Java.Bridge.GetJavaClass(m_ClassName);
            if ( AN_Java.Bridge.IsPrimitive(typeof(R)))
            {
                var result =  javaClass.CallStatic<R>(m_MethodName, m_Arguments.ToArray());
                AN_Logger.LogCommunication("[Sync] Sent to Unity ->: " + result);
                return result;
            }

            var json = javaClass.CallStatic<string>(m_MethodName, m_Arguments.ToArray());
            AN_Logger.LogCommunication("[Sync] Sent to Unity ->: " + json);
            return JsonUtility.FromJson<R>(json);
        }
        
    }
}