using System;
using UnityEngine;

namespace SA.Android.App
{
    /// <summary>
    /// A representation of settings that apply to a collection of similarly themed notifications.
    /// </summary>
    [Serializable]
    public class AN_NotificationChannel
    {
        [SerializeField]
        string m_Id;
        [SerializeField]
        string m_Name;
        [SerializeField]
        string m_Description;
        [SerializeField]
        int m_Importance;
        [SerializeField]
        string m_Sound;
        [SerializeField]
        bool m_ShowBadge;

        /// <summary>
        /// The id of the default channel for an app. This id is reserved by the system. 
        /// 
        /// All notifications posted from apps targeting <see cref="OS.AN_Build.VERSION_CODES.N_MR1"/> or earlier 
        /// without a notification channel specified are posted to this channel.
        /// </summary>
        public static string DEFAULT_CHANNEL_ID = "miscellaneous";

        /// <summary>
        /// This channel id will be used by plugin in case you havne't spesifayed any channell id with your request.
        /// </summary>
        public static string ANDROID_NATIVE_DEFAULT_CHANNEL_ID = DEFAULT_CHANNEL_ID + "_android_native_pro";

        /// <summary>
        /// Creates a notification channel.
        /// </summary>
        /// <param name="id">
        /// The id of the channel. Must be unique per package. 
        /// The value may be truncated if it is too long.
        /// </param>
        /// <param name="name">
        /// The user visible name of the channel. 
        /// You can rename this channel when the system locale changes 
        /// by listening for the Intent.ACTION_LOCALE_CHANGED broadcast. 
        /// The recommended maximum length is 40 characters; the value may be truncated if it is too long.
        /// </param>
        /// <param name="importance">
        /// The importance of the channel. 
        /// This controls how interruptive notifications posted to this channel are.
        /// </param>
        public AN_NotificationChannel(string id, string name, AN_NotificationManager.Importance importance)
        {
            m_Id = id;
            m_Name = name;
            m_ShowBadge = true;
            m_Importance = (int)importance;
        }

        /// <summary>
        /// Description of this channel.
        /// </summary>
        public string Description
        {
            get => m_Description;
            set => m_Description = value;
        }

        /// <summary>
        /// The id of this channel.
        /// </summary>
        public string Id => m_Id;

        /// <summary>
        /// The user visible name of this channel.
        /// </summary>
        public string Name
        {
            get => m_Name;
            set => m_Name = value;
        }

        /// <summary>
        /// Returns whether notifications posted to this channel can appear as badges in a Launcher application.
        /// </summary>
        public bool CanShowBadge => m_ShowBadge;

        public string Sound
        {
            //native part will have a sound URL so we need to only return it's name
            get => m_Sound.Substring(m_Sound.LastIndexOf('/') + 1);

            set => m_Sound = value;
        }

        /// <summary>
        /// The user specified importance
        /// </summary>
        public AN_NotificationManager.Importance Importance => (AN_NotificationManager.Importance)m_Importance;

        /// <summary>
        /// Sets whether notifications posted to this channel can appear as application icon badges in a Launcher.
        /// Only modifiable before the channel is submitted to <see cref="AN_NotificationManager.CreateNotificationChannel"/>
        /// </summary>
        /// <param name="showBadge">true if badges should be allowed to be shown.</param>
        public void SetShowBadge(bool showBadge)
        {
            m_ShowBadge = showBadge;
        }
    }
}
