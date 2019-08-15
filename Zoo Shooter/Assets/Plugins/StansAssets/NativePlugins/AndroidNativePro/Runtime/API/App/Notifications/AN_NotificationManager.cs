using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.OS;
using SA.Android.Utilities;
using SA.Foundation.Events;

namespace SA.Android.App {

    /// <summary>
    /// Class to notify the user of events that happen. 
    /// This is how you tell the user that something has happened in the background.
    /// </summary>
    public static class AN_NotificationManager
    {
        public enum Importance
        {

            /// <summary>
            /// A notification with no importance: does not show in the shade.
            /// </summary>
            NONE = 0,

            /// <summary>
            /// Min notification importance: 
            /// only shows in the shade, below the fold. 
            /// This should not be used with Service.startForeground 
            /// since a foreground service is supposed to be something the user cares about 
            /// so it does not make semantic sense to mark its notification as minimum importance. 
            /// If you do this as of Android version <see cref="OS.AN_Build.VERSION_CODES.O"/> 
            /// the system will show a higher-priority notification about your app running in the background.
            /// </summary>
            MIN = 1,

            /// <summary>
            /// Low notification importance: shows everywhere, but is not intrusive.
            /// </summary>
            LOW = 2,

            /// <summary>
            /// Default notification importance: shows everywhere, makes noise, but does not visually intrude.
            /// </summary>
            DEFAULT = 3,

            /// <summary>
            /// Higher notification importance: shows everywhere, makes noise and peeks. May use full screen intents.
            /// </summary>
            HIGH = 4,

            /// <summary>
            /// Unused
            /// </summary>
            MAX = 5,

            /// <summary>
            /// Value signifying that the user has not expressed an importance. 
            /// This value is for persisting preferences, and should never be associated with an actual notification.
            /// </summary>
            UNSPECIFIED = -1000
        }

        private static SA_Event<AN_NotificationRequest> s_onNotificationClick;
        private static SA_Event<AN_NotificationRequest> s_onNotificationReceived;

        private const string k_ScheduledNotificationsListPrefsKey = "SCHEDULED_NOTIFICATIONS_LIST_PREFS_KEY";
        private const string k_NotificationsManager = "com.stansassets.android.app.notifications.AN_NotificationManager";
        private const string k_AlarmNotificationService = "com.stansassets.android.app.notifications.AN_AlarmNotificationService";

        static AN_NotificationManager()
        {
            #if UNITY_2018_1_OR_NEWER
             Application.quitting += () =>
            {
                AN_Java.Bridge.CallStatic(k_AlarmNotificationService, "Restart");
            };
            #endif
        }
        
        /// <summary>
        /// Cancel a previously shown notification. 
        /// If it's transient, the view will be hidden. If it's persistent, it will be removed from the status bar.
        /// </summary>
        /// <param name="Identifier">Notification request id</param>
        public static void Cancel(int Identifier) {
            AN_Java.Bridge.CallStatic(k_NotificationsManager, "Cancel", Identifier);
        }

        /// <summary>
        /// Cancel all previously shown notifications.
        /// </summary>
        public static void CancelAll() {
            AN_Java.Bridge.CallStatic(k_NotificationsManager, "CancelAll");
        }

        /// <summary>
        /// Schedules a local notification for delivery.
        /// See <see cref="Cancel(int)"/> for the detailed behavior.
        /// </summary>
        /// <param name="request">The notification request to schedule.This parameter must not be <c>null</c>.</param>
        public static void Schedule(AN_NotificationRequest request) {
            ValidateRequest(request);
            AN_Java.Bridge.CallStatic(k_NotificationsManager, "Schedule", request);
        }

        /// <summary>
        /// Unschedule the specified notification request.
        /// </summary>
        /// <param name="request">request to Unschedule.</param>
        public static void Unschedule(AN_NotificationRequest request) {
            Unschedule(request.Identifier);
        }

        /// <summary>
        /// Unschedule the specified notification request by id
        /// </summary>
        /// <param name="Identifier">notification request id</param>
        public static void Unschedule(int Identifier) {
            AN_Java.Bridge.CallStatic(k_NotificationsManager, "Unschedule", Identifier);
        }

        public static void UnscheduleAll() {
            var list = GetScheduledList();
            foreach(var id in list.Ids) 
            {
                Unschedule(id);
            }
            PlayerPrefs.DeleteKey(k_ScheduledNotificationsListPrefsKey);
        }

        /// <summary>
        /// Creates a notification channel that notifications can be posted to. 
        /// This can also be used to restore a deleted channel and to update an existing channel's name, description, group, and/or importance.
        /// 
        /// The name and description should only be changed if the locale changes or in response to the user renaming this channel. 
        /// For example, if a user has a channel named 'John Doe' that represents messages from a 'John Doe', 
        /// and 'John Doe' changes his name to 'John Smith,' the channel can be renamed to match.
        /// 
        /// The importance of an existing channel will only be changed 
        /// if the new importance is lower than the current value and the user has not altered any settings on this channel.
        /// 
        /// The group an existing channel will only be changed if the channel does not already belong to a group. 
        /// All other fields are ignored for channels that already exist.
        /// </summary>
        /// <param name="channel">
        /// The channel to create. 
        /// Note that the created channel may differ from this value. 
        /// If the provided channel is malformed, a RemoteException will be thrown.
        /// This value must never be <c>null</c>.
        /// </param>
        public static void CreateNotificationChannel(AN_NotificationChannel channel) {
            AN_Java.Bridge.CallStatic(k_NotificationsManager, "CreateNotificationChannel", JsonUtility.ToJson(channel));
        }

        /// <summary>
        /// Returns all notification channels belonging to the calling package.
        /// </summary>
        public static List<AN_NotificationChannel> GetNotificationChannels() {
            if(Application.isEditor) 
            {
                return new List<AN_NotificationChannel>();
            }

            var json = AN_Java.Bridge.CallStatic<string>(k_NotificationsManager, "GetNotificationChannels");
          
            if(string.IsNullOrEmpty(json)) 
            {
                return null;
            }
            return JsonUtility.FromJson<AN_NotificationChannelsList>(json).Channels;
        }

        /// <summary>
        /// Returns the notification channel settings for a given channel id. T
        /// he channel must belong to your package, or it will not be returned.
        /// </summary>
        /// <param name="channelId">Channel id</param>
        public static AN_NotificationChannel GetNotificationChannel(string channelId) {
            if (Application.isEditor) {
                return null;
            }

            var json = AN_Java.Bridge.CallStatic<string>(k_NotificationsManager, "GetNotificationChannel", channelId);
            if (string.IsNullOrEmpty(json)) 
            {
                return null;
            }
            return JsonUtility.FromJson<AN_NotificationChannel>(json);
        }


        /// <summary>
        /// Deletes the given notification channel.
        /// If you <see cref="CreateNotificationChannel"/> with this same id, 
        /// the deleted channel will be un-deleted with all of the same settings it had before it was deleted.
        /// </summary>
        /// <param name="channelId">Channel id</param>
        public static void DeleteNotificationChannel(string channelId) {
            AN_Java.Bridge.CallStatic(k_NotificationsManager, "DeleteNotificationChannel", channelId);
        }

        /// <summary>
        /// Contains the last Notification Request opened by a user, via the notification banner click
        /// Make sense to check this filed value on your app start. Before you subscribe to <see cref="OnNotificationClick"/>
        /// 
        /// If value is not <c>null</c>, it mean that user has launched your app by clicking on notification banner.
        /// You can analyse the <see cref="AN_NotificationRequest"/> that caused clicked notification to show up, 
        /// for better understanding the context user launched your app in.
        /// </summary>
        public static AN_NotificationRequest LastOpenedNotificationRequest {
            get {
                var json = AN_Java.Bridge.CallStatic<string>(k_NotificationsManager, "GetLastOpenNotification");
                
                if(string.IsNullOrEmpty(json)) 
                {
                    return null;
                }
                return JsonUtility.FromJson<AN_NotificationRequest>(json);    
            }
        }

        /// <summary>
        /// The event is fired when user clicked on local notification banner
        /// </summary>
        public static SA_iEvent<AN_NotificationRequest> OnNotificationClick {
            get {

                if(s_onNotificationClick == null) 
                {
                    s_onNotificationClick = new SA_Event<AN_NotificationRequest>();
                    AN_Java.Bridge.CallStaticWithCallback<AN_NotificationRequest>(k_NotificationsManager, "Subscribe", (result) => {
                        s_onNotificationClick.Invoke(result);
                    });
                }
                return s_onNotificationClick;
            }
        }

        /// <summary>
        /// Event fired when local notification received 
        /// </summary>
        public static SA_iEvent<AN_NotificationRequest> OnNotificationReceived {
            get {
                if (s_onNotificationReceived == null) 
                {
                    s_onNotificationReceived = new SA_Event<AN_NotificationRequest>();

                    AN_Java.Bridge.CallStaticWithCallback<AN_NotificationRequest>(k_NotificationsManager, "SubscribeToNotificationReceived", (result) => 
                    {
                        s_onNotificationReceived.Invoke(result);
                    });
                }

                return s_onNotificationReceived;
            }
        }

        private static void ValidateRequest(AN_NotificationRequest request) {
            //Skipping this in the editor
            if (Application.isEditor) {
                return;
            }

            var builder = request.Content;

            //Saving the request id
            var list = GetScheduledList();
            if(!list.Ids.Contains(request.Identifier)) {
                list.Ids.Add(request.Identifier);
                SaveScheduledNotificationsList(list);
            }

            //Skipping for the Android versions that does not support notification channels
            if (AN_Build.VERSION.SDK_INT < AN_Build.VERSION_CODES.O) {
                return;
            }

            //Let' check if user has provided a custom channel 
            if (string.IsNullOrEmpty(builder.ChanelId)) {
                builder.SetChanelId(AN_NotificationChannel.ANDROID_NATIVE_DEFAULT_CHANNEL_ID);
            }

            var chanelId = builder.ChanelId;
            var channel = GetNotificationChannel(chanelId);

            //We will create channel in 2 cases
            //1 if you specified channel that doesn't yet exists
            //2 fall back to default channel, that should always be updated according to a builder settings
            if(channel == null || chanelId.Equals(AN_NotificationChannel.ANDROID_NATIVE_DEFAULT_CHANNEL_ID)) 
            {
                channel = new AN_NotificationChannel(chanelId, "Default", Importance.DEFAULT);
                channel.Sound = builder.SoundName;
                CreateNotificationChannel(channel);
            }
        }

        private static ScheduledNotificationsList GetScheduledList() 
        {
            ScheduledNotificationsList list;
            if (PlayerPrefs.HasKey(k_ScheduledNotificationsListPrefsKey)) 
            {
                var json = PlayerPrefs.GetString(k_ScheduledNotificationsListPrefsKey);
                list = JsonUtility.FromJson<ScheduledNotificationsList>(json);
            } else {
                list = new ScheduledNotificationsList();
            }

            return list;
        }

        private static void SaveScheduledNotificationsList(ScheduledNotificationsList list) 
        {
            var json = JsonUtility.ToJson(list);
            PlayerPrefs.SetString(k_ScheduledNotificationsListPrefsKey, json);
        }


        [Serializable]
        private class ScheduledNotificationsList 
        {
            public List<int> Ids = new List<int>();
        }
    }
}