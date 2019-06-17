using System;
using System.Collections.Generic;
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
    public class AN_LinkedObject 
    {
        private const string ANDROID_HASH_STORAGE = "com.stansassets.core.utility.AN_HashStorage";
        public const int NULL_OBJECT_HASH = -1;

        [SerializeField] protected int m_hashCode;

        public AN_LinkedObject() {}

        public AN_LinkedObject(int hasCode) {
            m_hashCode = hasCode;
        }


        /// <summary>
        /// The object hash code, matched with java object hash on native side.
        /// </summary>
        public int HashCode {
            get {
                return m_hashCode;
            }
        }


        /// <summary>
        /// Removes object from hash map in native java side
        /// </summary>
        public void Dispose() {
            AN_Java.Bridge.CallStatic(ANDROID_HASH_STORAGE, "Dispose", m_hashCode);
        }

    }
}