using System;
using UnityEngine;

namespace SA.Android.Utilities
{ 
    /// <summary>
    /// The class holds link to the native object create on java side.
    /// 
    /// When you done using the object, it's important to call <see cref="Dispose"/> method.
    /// Otherwise Object on java native side will remain with strong link, and will not be collected by GC.
    /// </summary>
    [Serializable]
    public class AN_LinkedObject : IDisposable
    {
        private const string k_AndroidHashStorage = "com.stansassets.core.utility.AN_HashStorage";
        public const int k_NullObjectHash = -1;

        [SerializeField] protected int m_HashCode;

        public AN_LinkedObject() {}

        public AN_LinkedObject(int hasCode) 
        {
            m_HashCode = hasCode;
        }

        /// <summary>
        /// The object hash code, matched with java object hash on native side.
        /// </summary>
        public int HashCode 
        {
            get { return m_HashCode; }
        }

        /// <summary>
        /// Returns true in case native object is null.
        /// </summary>
        public bool IsNull
        {
            get { return m_HashCode.Equals(k_NullObjectHash); }
        }

        /// <summary>
        /// Will free native object hard link.
        /// </summary>
        public void Dispose() 
        {
            AN_Java.Bridge.CallStatic(k_AndroidHashStorage, "Dispose", m_HashCode);
        }
    }
}