#if AN_FIREBASE_MESSAGING && (UNITY_IOS || UNITY_ANDROID)
#define API_ENABLED
#endif

using System.Collections.Generic;

#if API_ENABLED
using Firebase.Messaging;
#endif

namespace SA.Android.Firebase.Messaging
{
    public class AN_FirebaseNotification
    {
#if API_ENABLED
		public AN_FirebaseNotification(FirebaseNotification notification) {
			Badge = notification.Badge;
			Body = notification.Body;
			BodyLocalizationKey = notification.BodyLocalizationKey;
			ClickAction = notification.ClickAction;
			Color = notification.Color;
			Icon = notification.Icon;
			Sound = notification.Sound;
			Tag = notification.Tag;
			Title = notification.Title;
			TitleLocalizationKey = notification.TitleLocalizationKey;
			BodyLocalizationArgs = notification.BodyLocalizationArgs;
			TitleLocalizationArgs = notification.TitleLocalizationArgs;
		}
#endif

        /// <summary>
        /// Indicates the badge on the client app home icon. iOS only.
        /// </summary>
        public string Badge { get; }

        /// <summary>
        /// Indicates notification body text.
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// Indicates the key to the body string for localization.
        /// On iOS, this corresponds to "loc-key" in APNS payload.
        /// On Android, use the key in the app string resources when populating this value.
        /// </summary>
        public string BodyLocalizationKey { get; }

        /// <summary>
        /// The action associated with a user click on the notification.
        /// On Android, if this is set, an activity with a matching intent filter is launched when user clicks the notification.
        /// If set on iOS, corresponds to category in APNS payload.
        /// </summary>
        public string ClickAction { get; }

        /// <summary>
        /// Indicates color of the icon, expressed in #rrggbb format. Android only.
        /// </summary>
        public string Color { get; }

        /// <summary>
        /// Indicates notification icon.
        /// Sets value to my icon for drawable resource my icon.
        /// </summary>
        public string Icon { get; }

        /// <summary>
        /// Indicates a sound to play when the device receives the notification.
        /// Supports default, or the filename of a sound resource bundled in the app.
        /// Android sound files must reside in /res/raw/, while iOS sound files can be in the main bundle of the client app or in the Library/Sounds folder of the app’s data container.
        /// </summary>
        public string Sound { get; }

        /// <summary>
        /// Indicates whether each notification results in a new entry in the notification drawer on Android.
        /// If not set, each request creates a new notification. If set, and a notification with the same tag is already being shown,
        /// the new notification replaces the existing one in the notification drawer.
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// Indicates notification title.
        /// This field is not visible on iOS phones and tablets.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Indicates the key to the title string for localization.
        /// On iOS, this corresponds to "title-loc-key" in APNS payload.
        /// On Android, use the key in the app's string resources when populating this value.
        /// </summary>
        public string TitleLocalizationKey { get; }

        /// <summary>
        /// Indicates the string value to replace format specifiers in body string for localization.
        /// On iOS, this corresponds to "loc-args" in APNS payload.
        /// On Android, these are the format arguments for the string resource.
        /// </summary>
        public IEnumerable<string> BodyLocalizationArgs { get; }

        /// <summary>
        /// Indicates the string value to replace format specifiers in title string for localization.
        /// On iOS, this corresponds to "title-loc-args" in APNS payload.
        /// On Android, these are the format arguments for the string resource.
        /// </summary>
        public IEnumerable<string> TitleLocalizationArgs { get; }
    }
}
