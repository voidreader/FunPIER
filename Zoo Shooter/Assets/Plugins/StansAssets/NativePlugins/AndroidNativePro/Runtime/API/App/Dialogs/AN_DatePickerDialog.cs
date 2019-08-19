using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;

namespace SA.Android.App
{
    /// <summary>
    /// A simple dialog containing an DatePicker.
    /// </summary>
    [Serializable]
    public class AN_DatePickerDialog 
    {

#pragma warning disable 414
        [SerializeField] int m_year;
        [SerializeField] int m_month;
        [SerializeField] int m_day;
#pragma warning restore 414

        const string AN_DATEPICKERDIALOG_CLASS = "com.stansassets.android.app.dialogs.AN_DatePickerDialog";

        /// <summary>
        /// Creates a new date picker dialog for the specified date.
        /// </summary>
        /// <param name="year">the initially selected year.</param>
        /// <param name="month">the initially selected month of the year 0-11.</param>
        /// <param name="day">the initially selected day of month (1-31, depending on month)</param>
        public AN_DatePickerDialog(int year, int month, int day)
        {
            m_year = year;
            m_month = month;
            m_day = day;
        }

        /// <summary>
        /// Start the dialog and display it on screen.
        /// </summary>
        public void Show(Action<AN_DatePickerResult> callback) 
        {
            var json = JsonUtility.ToJson(this);
            AN_Java.Bridge.CallStaticWithCallback<AN_DatePickerResult>(
                AN_DATEPICKERDIALOG_CLASS,
                "Show", callback, json);

        }

    }
}