using System;
using SA.Foundation.Templates;
using UnityEngine;

namespace SA.Android.Utilities
{
    [Serializable]
    public class AN_SerializedObjectResult<T> : SA_Result
    {
        [SerializeField] private string m_ObjectJSON = string.Empty;
        private T m_Data;

        public AN_SerializedObjectResult(T data)
        {
            m_Data = data;
        }
        
        public T Data
        {
            get
            {
                if (m_Data == null)
                {
                    m_Data = JsonUtility.FromJson<T>(m_ObjectJSON);
                } 
                
                return m_Data;
            }
        }

    }
}