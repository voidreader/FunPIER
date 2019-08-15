using UnityEngine;

using SA.Foundation.Templates;

namespace SA.Android.App
{
    /// <summary>
    /// Object that contains a result of picking a date
    /// using the <see cref="AN_DatePickerDialog"/>
    /// </summary>
    public class AN_DatePickerResult : SA_Result
    {
        [SerializeField] int m_year = 0;
        [SerializeField] int m_month = 0;
        [SerializeField] int m_day = 0;


        /// <summary>
        /// The year that was set.
        /// </summary>
        public int Year {
            get {
                return m_year;
            }
        }

        /// <summary>
        /// The month that was set (0-11)
        /// </summary>
        public int Month {
            get {
                return m_month;
            }
        }


        /// <summary>
        /// The day of the month that was set.
        /// </summary>
        public int Day {
            get {
                return m_day;
            }
        }
    }
}