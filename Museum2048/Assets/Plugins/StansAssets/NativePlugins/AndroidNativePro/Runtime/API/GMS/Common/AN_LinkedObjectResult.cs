using System;
using UnityEngine;
using SA.Android.Utilities;
using SA.Foundation.Templates;
using UnityEngine.Assertions;

namespace SA.Android.GMS.Common
{
    [Serializable]
    public class AN_LinkedObjectResult<T> : SA_Result, ISerializationCallbackReceiver where T : AN_LinkedObject
    {
        public const int NATIVE_API_NULL_OBJECT_RESPONSE = 10;
        [SerializeField] protected T m_linkedObject;

        public AN_LinkedObjectResult() { }
        
        public AN_LinkedObjectResult(T linkedObject)
        {
            m_linkedObject = linkedObject;
        }
       
        public AN_LinkedObjectResult(SA_Error error):base(error) {}

        /// <summary>
        /// The operation result object
        /// </summary>
        public T Data 
        {
            get 
            {
                if(m_linkedObject.HashCode == AN_LinkedObject.k_NullObjectHash) 
                {
                    return null;
                }
                return m_linkedObject;
            }
        }

        public void OnBeforeSerialize() 
        {
          //Do nothing
        }

        public void OnAfterDeserialize()
        {
            var serializableAttribute =  Attribute.GetCustomAttribute(typeof(T), typeof(SerializableAttribute));
            Assert.IsNotNull(serializableAttribute, "AN_LinkedObjectResult Generic type: " +  typeof(T).Name + " is not Serializable!");
                
            if (Data == null) {
                m_error = new SA_Error(NATIVE_API_NULL_OBJECT_RESPONSE, "Native API responded with a null object");
            }
        }
    }
}