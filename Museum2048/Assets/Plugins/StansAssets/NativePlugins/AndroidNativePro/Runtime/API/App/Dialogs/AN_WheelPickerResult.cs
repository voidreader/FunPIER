using UnityEngine;
using SA.Foundation.Templates;
using System;

namespace SA.Android.App
{
    /// <summary>
    /// Object that contains a result of picking a element from wheel
    /// using the <see cref="AN_WheelPickerDialog"/>
    /// </summary>
    [Serializable]
    public class AN_WheelPickerResult: SA_Result
    {
        [SerializeField]string m_Value = null;

        /// <summary>
        /// The value that was picked by user.
        /// </summary>
        public string Value
        {
            get
            {
                return m_Value;
            }
        }
    }
}