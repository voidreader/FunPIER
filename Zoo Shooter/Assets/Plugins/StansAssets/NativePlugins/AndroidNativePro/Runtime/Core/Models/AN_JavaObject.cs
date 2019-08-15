using System;
using System.Collections.Generic;

namespace SA.Android.Utilities
{
    [Serializable]
    public abstract class AN_JavaObject : AN_LinkedObject
    {
        internal abstract string JavaClassName { get; }
        
        internal AN_JavaObject(int hasCode):base(hasCode) {}

        protected T CallStatic<T>(string methodName, params object[] args)
        {
            var arguments = new List<object>();
            arguments.Add(HashCode);
            foreach (var a in args) 
            {
                arguments.Add(a);
            }
            return AN_Java.Bridge.CallStatic<T>(JavaClassName, methodName, arguments.ToArray());
        }

        protected void CallStatic(string methodName, params object[] args)
        {
            var arguments = new List<object>();
            arguments.Add(HashCode);
            foreach (var a in args) 
            {
                arguments.Add(a);
            }
            
            AN_Java.Bridge.CallStatic(JavaClassName, methodName, HashCode, arguments.ToArray());
        }
    }
}