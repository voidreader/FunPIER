using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Android.App
{
    [Serializable]
    public class AN_AlarmNotificationTrigger 
    {

        [SerializeField] ulong m_seconds;
        [SerializeField] bool m_repeating = false;


        public void SetDate(TimeSpan time) {
            m_seconds = (ulong) time.TotalSeconds;
        }

        /// <summary>
        /// Define it trigger should be repeating
        /// </summary>
        public void SerRepeating(bool repeating) {
            m_repeating = repeating;
        }


        public ulong Seconds {
            get {
                return m_seconds;
            }
        }

        public bool Repeating {
            get {
                return m_repeating;
            }
        }
    }

}