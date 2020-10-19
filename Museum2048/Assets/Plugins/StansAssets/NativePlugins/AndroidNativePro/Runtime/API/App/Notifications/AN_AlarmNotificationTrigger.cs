using System;
using UnityEngine;

namespace SA.Android.App
{
    /// <summary>
    /// Notification alarm trigger.
    /// </summary>
    [Serializable]
    public class AN_AlarmNotificationTrigger
    {
        [SerializeField]
        ulong m_seconds;
        [SerializeField]
        bool m_repeating = false;

        public void SetDate(TimeSpan time)
        {
            m_seconds = (ulong)time.TotalSeconds;
        }

        /// <summary>
        /// Define it trigger should be repeating.
        /// </summary>
        public void SerRepeating(bool repeating)
        {
            m_repeating = repeating;
        }

        /// <summary>
        /// Delay in seconds before notification will be fired.
        /// </summary>
        public ulong Seconds => m_seconds;

        /// <summary>
        /// Repeating trigger value/
        /// </summary>
        public bool Repeating => m_repeating;
    }
}
