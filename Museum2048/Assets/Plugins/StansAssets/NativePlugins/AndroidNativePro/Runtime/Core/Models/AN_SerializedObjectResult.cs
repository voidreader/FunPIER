using System;
using SA.Foundation.Templates;
using UnityEngine;

namespace SA.Android.Utilities
{
    /// <summary>
    /// Serialized object result.
    /// </summary>
    /// <typeparam name="T">Result data type.</typeparam>
    [Serializable]
    public class AN_SerializedObjectResult<T> : SA_Result
    {
        [SerializeField]
        string m_ObjectJSON = string.Empty;
        T m_Data;

        internal AN_SerializedObjectResult(T data)
        {
            m_Data = data;
        }

        /// <summary>
        /// Result data.
        /// </summary>
        public T Data
        {
            get
            {
                if (m_Data == null) m_Data = JsonUtility.FromJson<T>(m_ObjectJSON);

                return m_Data;
            }
        }
    }
}
