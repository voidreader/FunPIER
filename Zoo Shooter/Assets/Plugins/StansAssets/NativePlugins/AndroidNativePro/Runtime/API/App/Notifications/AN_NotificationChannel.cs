using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Android.App
{

    /// <summary>
    /// A representation of settings that apply to a collection of similarly themed notifications.
    /// </summary>
    [Serializable]
    public class AN_NotificationChannel 
    {

        [SerializeField] string m_id;
        [SerializeField] string m_name;
        [SerializeField] int m_importance;

        [SerializeField] string m_sound;
        [SerializeField] string m_description;


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
        public AN_NotificationChannel(string id, string name, AN_NotificationManager.Importance importance) {
            m_id = id;
            m_name = name;
            m_importance = (int)importance;
        }


        /// <summary>
        /// Description of this channel.
        /// </summary>
        public string Description {
            get {
                return m_description;
            }

            set {
                m_description = value;
            }
        }


        /// <summary>
        /// The id of this channel.
        /// </summary>
        public string Id {
            get {
                return m_id;
            }
        }


        /// <summary>
        /// The user visible name of this channel.
        /// </summary>
        public string Name {
            get {
                return m_name;
            }

            set {
                m_name = value;
            }
        }


        public string Sound {
            get {
                //native part will have a sound URL so we need to only retur it's name
                return m_sound.Substring(m_sound.LastIndexOf('/') + 1);
            }

            set {
                m_sound = value;
            }
        }

        /// <summary>
        /// The user specified importance
        /// </summary>
        public AN_NotificationManager.Importance Importance {
            get {
                return (AN_NotificationManager.Importance) m_importance;
            }
        }
    }
}