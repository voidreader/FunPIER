using System.Collections.Generic;

#if AN_FIREBASE_MESSAGING && (UNITY_IOS || UNITY_ANDROID)
using Firebase.Messaging;
#endif

namespace SA.Android.Firebase.Messaging {

	public class AN_FirebaseNotification {

		private string m_badge = string.Empty;
		private string m_body = string.Empty;
		private string m_bodyLocalizationKey = string.Empty;
		private string m_clickAction = string.Empty;
		private string m_color = string.Empty;
		private string m_icon = string.Empty;
		private string m_sound = string.Empty;
		private string m_tag = string.Empty;
		private string m_title = string.Empty;
		private string m_titleLocalizationKey = string.Empty;

#if AN_FIREBASE_MESSAGING && (UNITY_IOS || UNITY_ANDROID)
		public AN_FirebaseNotification(FirebaseNotification notification) {
			m_badge 				= notification.Badge;
			m_body 					= notification.Body;
			m_bodyLocalizationKey 	= notification.BodyLocalizationKey;
			m_clickAction 			= notification.ClickAction;
			m_color 				= notification.Color;
			m_icon 					= notification.Icon;
			m_sound 				= notification.Sound;
			m_tag 					= notification.Tag;
			m_title 				= notification.Title;
			m_titleLocalizationKey 	= notification.TitleLocalizationKey;
			BodyLocalizationArgs 	= notification.BodyLocalizationArgs;
			TitleLocalizationArgs 	= notification.TitleLocalizationArgs;
		}
#endif

        /// <summary>
        /// Indicates the badge on the client app home icon. iOS only.
        /// </summary>
        public string Badge {
			get { return m_badge; }
		}

		/// <summary>
		/// Indicates notification body text.
		/// </summary>
		public string Body {
			get { return m_body; }
		}

		/// <summary>
		/// Indicates the key to the body string for localization.
		/// On iOS, this corresponds to "loc-key" in APNS payload.
		/// On Android, use the key in the app's string resources when populating this value.
		/// </summary>
		public string BodyLocalizationKey {
			get { return m_bodyLocalizationKey; }
		}

		/// <summary>
		/// The action associated with a user click on the notification.
		/// On Android, if this is set, an activity with a matching intent filter is launched when user clicks the notification.
		/// If set on iOS, corresponds to category in APNS payload.
		/// </summary>
		public string ClickAction {
			get { return m_clickAction; }
		}

		/// <summary>
		/// Indicates color of the icon, expressed in #rrggbb format. Android only.
		/// </summary>
		public string Color {
			get { return m_color; }
		}

		/// <summary>
		/// Indicates notification icon.
		/// Sets value to myicon for drawable resource myicon.
		/// </summary>
		public string Icon {
			get { return m_icon; }
		}

		/// <summary>
		/// Indicates a sound to play when the device receives the notification.
		/// Supports default, or the filename of a sound resource bundled in the app.
		/// Android sound files must reside in /res/raw/, while iOS sound files can be in the main bundle of the client app or in the Library/Sounds folder of the app’s data container.
		/// </summary>
		public string Sound {
			get { return m_sound; }
		}

		/// <summary>
		/// Indicates whether each notification results in a new entry in the notification drawer on Android.
		/// If not set, each request creates a new notification. If set, and a notification with the same tag is already being shown,
		/// the new notification replaces the existing one in the notification drawer.
		/// </summary>
		public string Tag {
			get { return m_tag; }
		}

		/// <summary>
		/// Indicates notification title.
		/// This field is not visible on iOS phones and tablets.
		/// </summary>
		public string Title {
			get { return m_title; }
		}

		/// <summary>
		/// Indicates the key to the title string for localization.
		/// On iOS, this corresponds to "title-loc-key" in APNS payload.
		/// On Android, use the key in the app's string resources when populating this value.
		/// </summary>
		public string TitleLocalizationKey {
			get { return m_titleLocalizationKey; }
		}
		
		/// <summary>
		/// Indicates the string value to replace format specifiers in body string for localization.
		/// On iOS, this corresponds to "loc-args" in APNS payload.
		/// On Android, these are the format arguments for the string resource.
		/// </summary>
		public IEnumerable<string> BodyLocalizationArgs { get; private set; }
		
		/// <summary>
		/// Indicates the string value to replace format specifiers in title string for localization.
		/// On iOS, this corresponds to "title-loc-args" in APNS payload.
		/// On Android, these are the format arguments for the string resource.
		/// </summary>
		public IEnumerable<string> TitleLocalizationArgs { get; private set; }
	}
}