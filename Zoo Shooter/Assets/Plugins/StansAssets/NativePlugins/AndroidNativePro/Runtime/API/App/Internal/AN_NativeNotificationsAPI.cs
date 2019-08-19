using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;

namespace SA.Android.App.Internal
{
    
    public class AN_NativeNotificationsAPI  : AN_iNotificationsAPI
    {

       
        const string AN_NOTIFICATIONS_MANAGER = "com.stansassets.android.app.notifications.AN_NotificationManager";


       
        public void CancelAll() {
            AN_Java.Bridge.CallStatic(AN_NOTIFICATIONS_MANAGER, "CancelAll");
        } 


    }
}