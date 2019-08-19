using System;
using System.Collections.Generic;

namespace SA.Android.Utilities
{
    [Serializable]
    public abstract class AN_JavaObject : AN_LinkedObject
    {
        protected abstract string JavaClassName { get; }
        
        internal AN_JavaObject(int hasCode):base(hasCode) {}

        protected T CallStatic<T>(string methodName, params object[] args)
        {
            var arguments = new List<object>();
            arguments.Add(HashCode);
            arguments.AddRange(args);
            return AN_Java.Bridge.CallStatic<T>(JavaClassName, methodName, arguments.ToArray());
        }

        protected void CallStatic(string methodName, params object[] args)
        {
            var arguments = new List<object>();
            arguments.Add(HashCode);
            arguments.AddRange(args);

            AN_Java.Bridge.CallStatic(JavaClassName, methodName, arguments.ToArray());
        }
    }
}