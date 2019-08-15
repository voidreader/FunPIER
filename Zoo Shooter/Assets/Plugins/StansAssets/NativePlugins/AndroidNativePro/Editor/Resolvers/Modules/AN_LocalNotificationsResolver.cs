using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Manifest;

namespace SA.Android
{
    public class AN_LocalNotificationsResolver : AN_APIResolver
    {
        public override bool IsSettingsEnabled {
            get { return AN_Settings.Instance.LocalNotifications; }
            set { AN_Settings.Instance.LocalNotifications = value; }
        }

        public override void AppendBuildRequirements(AN_AndroidBuildRequirements buildRequirements) {

            var AlarmNotificationService = new AMM_PropertyTemplate("service");
            AlarmNotificationService.SetValue("android:name", "com.stansassets.android.app.notifications.AN_AlarmNotificationService");
            AlarmNotificationService.SetValue("android:process", ":externalProcess");
            buildRequirements.AddApplicationProperty(AlarmNotificationService);
            
           
            var LocalNotificationReceiver = new AMM_PropertyTemplate("receiver");
            LocalNotificationReceiver.SetValue("android:name", "com.stansassets.android.app.notifications.AN_AlarmNotificationTriggerReceiver");
            LocalNotificationReceiver.SetValue("android:enabled", "true");
            LocalNotificationReceiver.SetValue("android:exported", "true");

            var ReceiverIntentFilter = LocalNotificationReceiver.GetOrCreateIntentFilterWithName("com.androidnative.local.intent.OPEN");
            ReceiverIntentFilter.GetOrCreatePropertyWithName("action", "android.intent.action.BOOT_COMPLETED");
            ReceiverIntentFilter.GetOrCreatePropertyWithName("category", "android.intent.category.DEFAULT");
            buildRequirements.AddApplicationProperty(LocalNotificationReceiver);


            var notificationLaunchActivity = new AMM_ActivityTemplate(false, "com.stansassets.android.app.notifications.AN_NotificationLaunchActivity");
            notificationLaunchActivity.SetValue("android:launchMode", "singleTask");
            //  notificationLaunchActivity.SetValue("android:label", "@string/app_name");
            notificationLaunchActivity.SetValue("android:configChanges", "fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen");
            notificationLaunchActivity.SetValue("android:theme", "@android:style/Theme.Translucent.NoTitleBar");
            buildRequirements.AddActivity(notificationLaunchActivity);




            buildRequirements.AddPermission(AMM_ManifestPermission.WAKE_LOCK);
            buildRequirements.AddBinaryDependency(AN_BinaryDependency.AndroidX);
            
        }


    }
}