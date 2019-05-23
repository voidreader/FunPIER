using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Foundation.Templates;

namespace SA.Android.GMS.Common
{
    [Serializable]
    public class AN_LinkedObjectResult<T> : SA_Result, ISerializationCallbackReceiver where T : AN_LinkedObject
    {
        public const int NATIVE_API_NULL_OBJECT_RESPONCE = 10;
        [SerializeField] protected T m_linkedObject;


        public AN_LinkedObjectResult() : base() { }
        public AN_LinkedObjectResult(T linkedObject) : base() {
            m_linkedObject = linkedObject;
        }
       
        public AN_LinkedObjectResult(SA_Error error):base(error) {}

        /// <summary>
        /// The operation result object
        /// </summary>
        public T Data {
            get {
                if(m_linkedObject.HashCode == AN_LinkedObject.NULL_OBJECT_HASH) {
                    return null;
                }
                return m_linkedObject;
            }
        }



        public void OnBeforeSerialize() {
          //Do nothing
        }


        public void OnAfterDeserialize() {
            if (Data == null) {
                m_error = new SA_Error(NATIVE_API_NULL_OBJECT_RESPONCE, "Native API responded with a null object");
            }
        }
    }
}