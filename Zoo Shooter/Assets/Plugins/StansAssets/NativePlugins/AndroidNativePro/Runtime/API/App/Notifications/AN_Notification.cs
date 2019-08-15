using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;


namespace SA.Android.App
{

    /// <summary>
    /// A class that represents how a persistent notification is to be presented to the user using the <see cref="AN_NotificationManager"/>
    /// </summary>
    public class AN_Notification : AN_LinkedObject
    {


        private const string NATIVE_CLASS_NAME = "com.stansassets.android.app.notifications.AN_Notification";


        /// <summary>
        /// Use all default values (where applicable).
        /// </summary>
        public const int DEFAULT_ALL = -1;

        /// <summary>
        /// Use the default notification lights.
        /// </summary>
        public const int DEFAULT_LIGHTS = 4;

        /// <summary>
        /// Use the default notification sound.
        /// </summary>
        public const int DEFAULT_SOUND = 1;

        /// <summary>
        /// Use the default notification vibrate
        /// </summary>
        public const int DEFAULT_VIBRATE = 2;


        public AN_Notification(int hashCode):base(hashCode) {
          
        }


    }
}