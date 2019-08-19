using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Utility;

namespace SA.Android.Utilities
{
    public class AN_JavaBridge
    {

        private readonly Dictionary<string, AndroidJavaClass> m_classes = new Dictionary<string, AndroidJavaClass>();


        //--------------------------------------
        //  Initialization
        //--------------------------------------

        public AN_JavaBridge() {
            //Registering the message handler
            CallStatic("com.stansassets.core.utility.AN_UnityBridge", "RegisterMessageHandler");
        }


        //--------------------------------------
        //  Public Methods
        //--------------------------------------


        public void CallStatic(string javaClassName, string methodName, params object[] args) {
            var javaClass = GetJavaClass(javaClassName);

            List<object> arguments = new List<object>();
            foreach (object p in args) {
                arguments.Add(ConvertObjectData(p));
            }

            LogCommunication(javaClassName, methodName, arguments);

            if (Application.isEditor) { return; }
            javaClass.CallStatic(methodName, arguments.ToArray());
        }

        public T CallStatic<T>(string javaClassName, string methodName, params object[] args) {
            var javaClass = GetJavaClass(javaClassName);

            var arguments = new List<object>();
            foreach (object p in args) 
            {
                arguments.Add(ConvertObjectData(p));
            }


            LogCommunication(javaClassName, methodName, arguments);

            if (Application.isEditor) { return default(T);}

            if (IsPrimitive(typeof(T)))
            {
                var result = javaClass.CallStatic<T>(methodName, arguments.ToArray());
                AN_Logger.LogCommunication("[Sync] Sent to Unity ->: " + result);
                return result;
            }

            var json = javaClass.CallStatic<string>(methodName, arguments.ToArray());
            AN_Logger.LogCommunication("[Sync] Sent to Unity ->: " + json);
            return JsonUtility.FromJson<T>(json);
        }

        public R CallStaticWithCallback<R,T>(string javaClassName, string methodName, Action<T> callback, params object[] args) {
            var javaClass = GetJavaClass(javaClassName);
            var arguments = new List<object>();

            foreach (var p in args) {
                arguments.Add(ConvertObjectData(p));
            }

            LogCommunication(javaClassName, methodName, arguments);
            arguments.Add(AN_MonoJavaCallback.ActionToJavaObject(callback));

            if (Application.isEditor) { return default(R); }
            return javaClass.CallStatic<R>(methodName, arguments.ToArray());
        }

        public void CallStaticWithCallback<T>(string javaClassName, string methodName, Action<T> callback, params object[] args) {
            var javaClass = GetJavaClass(javaClassName);
            var arguments = new List<object>();
            foreach(var p in args) {
                arguments.Add(ConvertObjectData(p));
            }

            LogCommunication(javaClassName, methodName, arguments);
            arguments.Add(AN_MonoJavaCallback.ActionToJavaObject(callback));

            if (Application.isEditor) { return; }
            javaClass.CallStatic(methodName, arguments.ToArray());
        }


        //--------------------------------------
        //  Private Methods
        //--------------------------------------

        private static string LogArguments(List<object> arguments) 
        {
            var log = string.Empty;
            foreach(var p in arguments) 
            {
                if (p == null) continue;
                
                if(log != string.Empty) 
                {
                    log += " | ";
                }
                log += p.ToString();
            }

            return log;
        }

        public void LogCommunication(string className, string methodName, List<object> arguments) {

            var strippedClassName = SA_PathUtil.GetExtension(className);
            strippedClassName = strippedClassName.Substring(1);
            var argumentsLog = LogArguments(arguments);
            if(!string.IsNullOrEmpty(argumentsLog)) {
                argumentsLog = " :: " + argumentsLog;
            }
            AN_Logger.LogCommunication("Sent to Java -> " + strippedClassName + "." + methodName + argumentsLog);
        }


        public object ConvertObjectData(object param) {
            if (param is string) {
                return param.ToString();
            } else if (param is bool) {
                return param;
            } else if (param is int) {
                return param;
            } else if (param is long) {
                return param;
            } else if (param is float) {
                return param;
            } else if (param is Texture2D) {
                return (param as Texture2D).ToBase64String();
            } else {
                return JsonUtility.ToJson(param);
            }
        }

        public bool IsPrimitive(Type type)
        {
            if (type == typeof(byte) ||
                type == typeof(ushort) ||
                type == typeof(short) ||
                type == typeof(uint) ||
                type == typeof(int) ||
                type == typeof(ulong) ||
                type == typeof(long) ||
                type == typeof(float) ||
                type == typeof(double) ||
                type == typeof(bool) ||
                type == typeof(string) ||
                type == typeof(char) 

                )
            {
                return true;
            }

            return false;
        }
        
        public AndroidJavaClass GetJavaClass(string javaClassName) {

            if (Application.isEditor) {
                return null;
            }

            if (m_classes.ContainsKey(javaClassName)) {
                return m_classes[javaClassName];
            }

            var javaClass = new AndroidJavaClass(javaClassName);
            m_classes.Add(javaClassName, javaClass);
            return javaClass;
        }
    }
}