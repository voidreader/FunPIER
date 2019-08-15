using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Android.App
{

    /// <summary>
    /// The channels list result
    /// </summary>
    [Serializable]
    public class AN_NotificationChannelsList 
    {

        [SerializeField] List<AN_NotificationChannel> m_channels = new List<AN_NotificationChannel>();


        /// <summary>
        /// List of the <see cref="AN_NotificationChannel"/>
        /// </summary>
        public List<AN_NotificationChannel> Channels {
            get {
                return m_channels;
            }
        }
    }
}