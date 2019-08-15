using System;
using SA.Foundation.Events;
#if AN_FIREBASE_MESSAGING && (UNITY_IOS || UNITY_ANDROID)
using Firebase.Messaging;
#endif

namespace SA.Android.Firebase.Messaging {

	public class AN_FirebaseMessaging {
		
		private static event Action<string> OnFBTokenReceived = delegate {};
		private static SA_Event<AN_FirebaseMessage> m_onFbMessageReceived = new SA_Event<AN_FirebaseMessage>();
		
		private static string m_successfulTokenCache = string.Empty;
		private static bool m_isConnectionInProgress = false;
		private static bool m_isInited = false;
		
		/// <summary>
		/// Initialize FCM service. 
		/// Once the initialization is successfully established, OnFBTokenReceived will be invoked with registration token
		/// and available after method callback
		/// 
		/// The Firebase Cloud Message library will be initialized when adding handlers for either the TokenReceived or MessageReceived events.
		/// </summary>
		/// <param name="callback">The Initialize result callback</param>
		public static void Initialize(Action<string> callback) {
			if (!m_isInited) {
				m_isInited = true;
				HandleEvents();
			}

			if (!string.IsNullOrEmpty(m_successfulTokenCache)) {
				callback.Invoke(m_successfulTokenCache);
				return;
			}

			OnFBTokenReceived += callback;
			if (m_isConnectionInProgress) { return; }

			m_isConnectionInProgress = true;
		}

		/// <summary>
		/// The Firebase Cloud Message library will be initialized when adding handlers for either the TokenReceived or MessageReceived events.
		/// </summary>
		private static void HandleEvents() {
#if AN_FIREBASE_MESSAGING && (UNITY_IOS || UNITY_ANDROID)
			FirebaseMessaging.TokenReceived += OnFbPushTokenReceived;
			FirebaseMessaging.MessageReceived += OnFbPushMessageReceived;
#endif
        }

#if AN_FIREBASE_MESSAGING && (UNITY_IOS || UNITY_ANDROID)
		/// <summary>
		/// Upon initialization, a registration token is requested for the client app instance.
		/// The app will receive the token with the OnTokenReceived event, which should be cached for later use.
		/// You'll need this token if you want to target this specific device for messages.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="fbToken"></param>
		private static void OnFbPushTokenReceived(object sender, TokenReceivedEventArgs fbToken) {
			m_isConnectionInProgress = false;
			m_successfulTokenCache = fbToken.Token;
			OnFBTokenReceived.Invoke(m_successfulTokenCache);
			OnFBTokenReceived = delegate {};
		}

		/// <summary>
		/// Receive incoming messages.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="fbMessage"></param>
		private static void OnFbPushMessageReceived(object sender, MessageReceivedEventArgs fbMessage) {
			m_onFbMessageReceived.Invoke(new AN_FirebaseMessage(fbMessage.Message));
		}
#endif

        public static SA_iEvent<AN_FirebaseMessage> OnFbMessageReceived {
			get {
				return m_onFbMessageReceived;
			}
		}
	}
}