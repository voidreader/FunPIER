using System;
using System.Collections.Generic;

namespace SA.Android.Utilities
{
    /// <summary>
    /// Extended version of <see cref="AN_LinkedObject"/> that provides protected API,
    /// to interact with the linked java object class.
    /// </summary>
    [Serializable]
    public abstract class AN_JavaObject : AN_LinkedObject
    {
        /// <summary>
        /// Java object class name.
        /// </summary>
        protected abstract string JavaClassName { get; }

        internal AN_JavaObject(int hasCode)
            : base(hasCode) { }

        /// <summary>
        /// Call static method from linked java object.
        /// </summary>
        /// <param name="methodName">Method name.</param>
        /// <param name="args">Arguments.</param>
        /// <typeparam name="T">Defined return type</typeparam>
        /// <returns>Method call result</returns>
        protected T CallStatic<T>(string methodName, params object[] args)
        {
            var arguments = new List<object>();
            arguments.Add(HashCode);
            arguments.AddRange(args);
            return AN_Java.Bridge.CallStatic<T>(JavaClassName, methodName, arguments.ToArray());
        }

        /// <summary>
        /// Call static void method from linked java object.
        /// </summary>
        /// <param name="methodName">Method name.</param>
        /// <param name="args">Arguments.</param>
        protected void CallStatic(string methodName, params object[] args)
        {
            var arguments = new List<object>();
            arguments.Add(HashCode);
            arguments.AddRange(args);

            AN_Java.Bridge.CallStatic(JavaClassName, methodName, arguments.ToArray());
        }
    }
}
