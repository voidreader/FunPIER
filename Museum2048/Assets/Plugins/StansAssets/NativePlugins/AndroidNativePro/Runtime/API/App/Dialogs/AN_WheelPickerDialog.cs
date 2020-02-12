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
        [SerializeField] private List<string> m_Values;
#pragma warning restore 414

        const string k_WheelPickerDialogClass = "com.stansassets.android.app.dialogs.AN_WheelPickerDialog";

        /// <summary>
        /// Creates a new number picker dialog.
        /// </summary>
        /// <param name="values">list of the elements to choose from.</param>
        public AN_WheelPickerDialog(List<string> values)
        {
            m_Values = values;
        }

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