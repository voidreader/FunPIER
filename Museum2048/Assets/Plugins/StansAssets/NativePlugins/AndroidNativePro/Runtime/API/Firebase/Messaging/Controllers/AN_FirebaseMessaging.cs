#if AN_FIREBASE_MESSAGING && (UNITY_IOS || UNITY_ANDROID)
#define API_ENABLED
#endif

using System;
using SA.Foundation.Events;

#if API_ENABLED
using Firebase.Messaging;
#endif

namespace SA.Android.Firebase.Messaging
{
	/// <summary>
	/// Firebase Messaging proxy.
	/// </summary>
    public static class AN_FirebaseMessaging
    {
        static event Action<string> OnTokenReceived = delegate { };
        static readonly SA_Event<AN_FirebaseMessage> s_OnFbMessageReceived = new SA_Event<AN_FirebaseMessage>();

        static string s_SuccessfulTokenCache = string.Empty;
        static bool s_IsConnectionInProgress = false;
        static bool s_IsInitialized = false;

        /// <summary>
        /// Initialize FCM service. 
        /// Once the initialization is successfully established, OnFBTokenReceived will be invoked with registration token
        /// and available after method callback
        /// 
        /// The Firebase Cloud Message library will be initialized when adding handlers for either the TokenReceived or MessageReceived events.
        /// </summary>
        /// <param name="callback">The Initialize result callback</param>
        public static void Initialize(Action<string> callback)
        {
            if (!s_IsInitialized)
            {
                s_IsInitialized = true;
                HandleEvents();
            }

            if (!string.IsNullOrEmpty(s_SuccessfulTokenCache))
            {
                callback.Invoke(s_SuccessfulTokenCache);
                return;
            }

            OnTokenReceived += callback;
            if (s_IsConnectionInProgress) return;

            s_IsConnectionInProgress = true;
        }

        /// <summary>
        /// The Firebase Cloud Message library will be initialized when adding handlers for either the TokenReceived or MessageReceived events.
        /// </summary>
        static void HandleEvents()
        {
#if API_ENABLED
			FirebaseMessaging.TokenReceived += OnFbPushTokenReceived;
			FirebaseMessaging.MessageReceived += OnFbPushMessageReceived;
#endif
        }

#if API_ENABLED
		/// <summary>
		/// Upon initialization, a registration token is requested for the client app instance.
		/// The app will receive the token with the OnTokenReceived event, which should be cached for later use.
		/// You'll need this token if you want to target this specific device for messages.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="fbToken"></param>
		static void OnFbPushTokenReceived(object sender, TokenReceivedEventArgs fbToken) {
			s_IsConnectionInProgress = false;
			s_SuccessfulTokenCache = fbToken.Token;
			OnTokenReceived.Invoke(s_SuccessfulTokenCache);
			OnTokenReceived = delegate {};
		}

		/// <summary>
		/// Receive incoming messages.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="fbMessage"></param>
		static void OnFbPushMessageReceived(object sender, MessageReceivedEventArgs fbMessage) {
			s_OnFbMessageReceived.Invoke(new AN_FirebaseMessage(fbMessage.Message));
		}
#endif

        public static SA_iEvent<AN_FirebaseMessage> OnFbMessageReceived => s_OnFbMessageReceived;
    }
}
