using System;
using System.Collections.Generic;
using UnityEngine;
using SA.Android.Utilities;

namespace SA.Android.App
{
    /// <summary>
    /// A simple dialog containing an NumberPicker.
    /// </summary>
    [Serializable]
    public class AN_WheelPickerDialog
    {
#pragma warning disable 414

        //Serialized values used within native part.
        [SerializeField]
        List<string> m_Values;

        [SerializeField]
        int m_Default;
#pragma warning restore 414

        const string k_WheelPickerDialogClass = "com.stansassets.android.app.dialogs.AN_WheelPickerDialog";

        /// <summary>
        /// Creates a new number picker dialog.
        /// </summary>
        /// <param name="values">list of the elements to choose from.</param>
        /// <param name="defaultValueIndex">Default value index.</param>
        public AN_WheelPickerDialog(List<string> values, int defaultValueIndex = 0)
        {
            m_Values = values;
            m_Default = defaultValueIndex;
        }

        /// <summary>
        /// Picker values.
        /// </summary>
        public List<string> Values => m_Values;

        /// <summary>
        /// Default value index. `0` by default.
        /// </summary>
        public int DefaultValueIndex => m_Default;

        /// <summary>
        /// Start the dialog and display it on screen.
        /// </summary>
        public void Show(Action<AN_WheelPickerResult> callback)
        {
            var json = JsonUtility.ToJson(this);
            AN_Java.Bridge.CallStaticWithCallback(
                k_WheelPickerDialogClass,
                "Show", callback, json);
        }
    }
}
