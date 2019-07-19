using System;
using System.Collections.Generic;

#if AN_FIREBASE_MESSAGING && (UNITY_IOS || UNITY_ANDROID)
using Firebase.Messaging;
#endif

namespace SA.Android.Firebase.Messaging {
	
	public class AN_FirebaseMessage {

		private string m_collapseKey = string.Empty;
		private string m_error = string.Empty;
		private string m_errorDescription = string.Empty;
		private string m_from = string.Empty;
		private string m_messageId = string.Empty;
		private string m_messageType = string.Empty;
		private bool m_notificationOpened = false;
		private string m_priority = string.Empty;
		private string m_rawData = string.Empty;
		private string m_to = string.Empty;

#if AN_FIREBASE_MESSAGING && (UNITY_IOS || UNITY_ANDROID)
		public AN_FirebaseMessage(FirebaseMessage message) {
			m_collapseKey 			= message.CollapseKey;
			m_error 				= message.Error;
			m_errorDescription 		= message.ErrorDescription;
			m_from 					= message.From;
			m_messageId 			= message.MessageId;
			m_messageType 			= message.MessageType;
			m_notificationOpened 	= message.NotificationOpened;
			m_priority 				= message.Priority;
			m_rawData 				= message.RawData;
			m_to 					= message.To;
			Data 					= message.Data;
			Link 					= message.Link;
			TimeToLive 				= message.TimeToLive;
			
			if(message.Notification != null)
				Notification = new AN_FirebaseNotification(message.Notification);
		}
#endif

        /// <summary>
        /// Gets the collapse key used for collapsible messages.
        /// This field is only used for downstream messages received through the FirebaseMessaging.MessageReceived event.
        /// </summary>
        public string CollapseKey {
			get { return m_collapseKey; }
		}

		/// <summary>
		/// Gets the error code.
		/// Used in "nack" messages for CCS, and in responses from the server. See the CCS specification for the externally-supported list.
		/// This field is only used for downstream messages received through the FirebaseMessaging.MessageReceived event.
		/// </summary>
		public string Error {
			get { return m_error; }
		}

		/// <summary>
		/// Gets the human readable details about the error.
		/// This field is only used for downstream messages received through the FirebaseMessaging.MessageReceived event.
		/// </summary>
		public string ErrorDescription {
			get { return m_errorDescription; }
		}

		/// <summary>
		/// Gets the authenticated ID of the sender.
		/// This is a project number in most cases.
		/// This field is only used for downstream messages received through the FirebaseMessaging.MessageReceived event.
		/// </summary>
		public string From {
			get { return m_from; }
		}

		/// <summary>
		/// Gets or sets the message ID.
		/// This can be specified by sender. Internally a hash of the message ID and other elements will be used for storage.
		/// The ID must be unique for each topic subscription - using the same ID may result in overriding the original message or duplicate delivery.
		/// This field is used for both upstream messages sent with firebase::messaging::Send() and downstream messages received through the FirebaseMessaging.MessageReceived event.
		/// </summary>
		public string MessageId {
			get { return m_messageId; }
		}

		/// <summary>
		/// Gets the message type, equivalent with a content-type.
		/// CCS uses "ack", "nack" for flow control and error handling. "control" is used by CCS for connection control.
		/// This field is only used for downstream messages received through the FirebaseMessaging.MessageReceived event.
		/// </summary>
		public string MessageType {
			get { return m_messageType; }
		}
				
		/// <summary>
		/// Gets a flag indicating whether this message was opened by tapping a notification in the OS system tray.
		/// If the message was received this way this flag is set to true.
		/// </summary>
		public bool NotificationOpened {
			get { return m_notificationOpened; }
		}
		
		/// <summary>
		/// Gets the priority level.
		/// Defined values are "normal" and "high". By default messages are sent with normal priority.
		/// This field is only used for downstream messages received through the FirebaseMessaging.MessageReceived event.
		/// </summary>
		public string Priority {
			get { return m_priority; }
		}

		/// <summary>
		/// Gets the binary payload.
		/// For webpush and non-json messages, this is the body of the request entity.
		/// This field is only used for downstream messages received through the FirebaseMessaging.MessageReceived event.
		/// </summary>
		public string RawData {
			get { return m_rawData; }
		}
		
		/// <summary>
		/// Gets or sets recipient of a message.
		/// For example it can be a registration token, a topic name, a IID or project ID.
		/// This field is used for both upstream messages sent with firebase::messaging:Send() and downstream messages received through the FirebaseMessaging.MessageReceived event. For upstream messages, PROJECT_ID@gcm.googleapis.com or the more general IID format are accepted.
		/// </summary>
		public string To {
			get { return m_to; }
		}
		
		/// <summary>
		/// Gets or sets the metadata, including all original key/value pairs.
		/// Includes some of the HTTP headers used when sending the message. gcm, google and goog prefixes are reserved for internal use.
		/// This field is used for both upstream messages sent with firebase::messaging::Send() and downstream messages received through the FirebaseMessaging.MessageReceived event.
		/// </summary>
		public IDictionary<string, string> Data { get; private set; }
		
		/// <summary>
		/// The link into the app from the message.
		/// This field is only used for downstream messages.
		/// </summary>
		public Uri Link { get; private set; }
		
		/// <summary>
		/// Optional notification to show.
		/// This only set if a notification was received with this message, otherwise it is null.
		/// This field is only used for downstream messages received through FirebaseMessaging.MessageReceived.
		/// </summary>
		public AN_FirebaseNotification Notification { get; private set; }
		
		/// <summary>
		/// The Time To Live (TTL) for the message.
		/// This field is only used for downstream messages received through FirebaseMessaging.MessageReceived().
		/// </summary>
		public TimeSpan TimeToLive { get; private set; }
	}
}
//#endif
