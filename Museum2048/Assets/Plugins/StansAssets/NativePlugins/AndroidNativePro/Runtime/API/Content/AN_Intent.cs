using System;
using System.Collections.Generic;
using UnityEngine;
using SA.Android.Utilities;

namespace SA.Android.Content
{
    /// <summary>
    /// An intent is an abstract description of an operation to be performed. 
    /// It can be used with startActivity to launch an Activity, broadcastIntent to send it to any interested BroadcastReceiver components, and Context.startService(Intent) or Context.bindService(Intent, ServiceConnection, int) to communicate with a background Service.
    /// </summary>
    [Serializable]
    public class AN_Intent : AN_LinkedObject
    {
#pragma warning disable 414

        [SerializeField] string m_Action;
        [SerializeField] string m_Type;
        [SerializeField] string m_PackageName;

        [SerializeField] string m_Uri;

        [SerializeField] bool m_IsChooser = false;
        [SerializeField] AN_IntentChooserInfo m_ChooserInfo;

        [SerializeField] List<int> m_Flags = new List<int>();
        [SerializeField] List<string> m_Categories = new List<string>();

        [SerializeField] List<string> m_ExtraKeys = new List<string>();
        [SerializeField] List<AN_IntentExtraValue> m_ExtraValues = new List<AN_IntentExtraValue>();

#pragma warning restore 414


        //--------------------------------------
        // Initialization
        //--------------------------------------

        /// <summary>
        /// Create an empty intent.
        /// </summary>
        public AN_Intent() { }

        /// <summary>
        /// For plugin internal use only.
        /// This constructor will link C# Intent with Intent class that was created with Java native side
        /// </summary>
        public AN_Intent(int hashCode) 
        {
            m_HashCode = hashCode;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="o">AN_Intent</param>
        public AN_Intent(AN_Intent o) 
        {
            var json = JsonUtility.ToJson(o);
            JsonUtility.FromJsonOverwrite(json, this);
        }

        /// <summary>
        /// Create an intent with a given action. 
        /// All other fields (data, type, class) are null. 
        /// Note that the action must be in a namespace because Intents are used globally in the system. 
        /// For example the system VIEW action is <see cref="ACTION_VIEW"/>; 
        /// an application's custom action would be something like com.google.app.myapp.CUSTOM_ACTION.
        /// </summary>
        /// <param name="action">
        /// An action name, such as <see cref="ACTION_VIEW"/>. Application-specific actions should be prefixed with the vendor's package name.
        /// </param>
        public AN_Intent(string action) 
        {
            SetAction(action);
        }

        /// <summary>
        /// Create an intent with a given action and for a given data url.
        /// Note that the action must be in a namespace because Intents are used globally in the system 
        /// for example the system VIEW action is <see cref="ACTION_VIEW"/> an application's custom action 
        /// would be something like com.google.app.myapp.CUSTOM_ACTION.
        ///
        /// Note: scheme and host name matching in the Android framework is case-sensitive, 
        /// unlike the formal RFC.As a result, you should always ensure that you write your Uri with these elements using lower case letters, 
        /// and normalize any Uris you receive from outside of Android to ensure the scheme and host is lower case.
        /// </summary>
        /// <param name="action">
        /// An action name, such as <see cref="ACTION_VIEW"/>. Application-specific actions should be prefixed with the vendor's package name.
        /// </param>
        public AN_Intent(string action, Uri uri) 
        {
            SetAction(action);
            m_Uri = uri.AbsoluteUri;
        }

        //--------------------------------------
        // Public Methods
        //--------------------------------------

        /// <summary>
        /// Set the general action to be performed.
        /// </summary>
        /// <param name="action">
        ///  An action name, such as <see cref="ACTION_VIEW"/>. Application-specific actions should be prefixed with the vendor's package name.
        /// </param>
        public void SetAction(string action) 
        {
            m_Action = action;
        }

        /// <summary>
        /// Add a new category to the intent. Categories provide additional detail about the action the intent performs.
        /// When resolving an intent, only activities that provide all of the requested categories will be used.
        /// </summary>
        /// <param name="category">
        /// The desired category. 
        /// This can be either one of the predefined Intent categories, or a custom category in your own namespace.
        /// </param>
        public void AddCategory(string category) 
        {
            m_Categories.Add(category);
        }

        /// <summary>
        /// (Usually optional) Set an explicit application package name that limits the components this Intent will resolve to. 
        /// If left to the default value of null, all components in all applications will considered. 
        /// If non-null, the Intent can only match the components in the given application package.
        /// </summary>
        /// <param name="packageName">The name of the application package to handle the intent, or null to allow any application package.</param>
        public void SetPackage(string packageName) 
        {
            m_PackageName = packageName;
        }

        /// <summary>
        /// Set an explicit MIME data type.
        /// This is used to create intents that only specify a type and not data, 
        /// for example to indicate the type of data to return.
        /// 
        /// This method automatically clears any data that was previously set 
        /// 
        /// Note: MIME type matching in the Android framework is case-sensitive, unlike formal RFC MIME types. 
        /// As a result, you should always write your MIME types with lower case letters, 
        /// </summary>
        /// <param name="type"></param>
        public void SetType(string type) 
        {
            m_Type = type;
        }

        /// <summary>
        /// Add additional flags to the intent (or with existing flags value).
        /// </summary>
        /// <param name="flags">
        /// Value is either 0 or combination of 
        /// FLAG_GRANT_READ_URI_PERMISSION, FLAG_GRANT_WRITE_URI_PERMISSION, FLAG_FROM_BACKGROUND, 
        /// FLAG_DEBUG_LOG_RESOLUTION, FLAG_EXCLUDE_STOPPED_PACKAGES, FLAG_INCLUDE_STOPPED_PACKAGES, 
        /// FLAG_GRANT_PERSISTABLE_URI_PERMISSION, FLAG_GRANT_PREFIX_URI_PERMISSION, FLAG_ACTIVITY_MATCH_EXTERNAL, 
        /// FLAG_RECEIVER_REGISTERED_ONLY, FLAG_RECEIVER_REPLACE_PENDING, FLAG_RECEIVER_FOREGROUND, FLAG_RECEIVER_NO_ABORT, 
        /// FLAG_ACTIVITY_CLEAR_TOP, FLAG_ACTIVITY_FORWARD_RESULT, FLAG_ACTIVITY_PREVIOUS_IS_TOP, FLAG_ACTIVITY_EXCLUDE_FROM_RECENTS, 
        /// FLAG_ACTIVITY_BROUGHT_TO_FRONT, FLAG_RECEIVER_VISIBLE_TO_INSTANT_APPS, FLAG_ACTIVITY_LAUNCHED_FROM_HISTORY, 
        /// FLAG_ACTIVITY_NEW_DOCUMENT, FLAG_ACTIVITY_NO_USER_ACTION, FLAG_ACTIVITY_REORDER_TO_FRONT, FLAG_ACTIVITY_NO_ANIMATION, 
        /// FLAG_ACTIVITY_CLEAR_TASK, FLAG_ACTIVITY_TASK_ON_HOME, FLAG_ACTIVITY_RETAIN_IN_RECENTS or FLAG_ACTIVITY_LAUNCH_ADJACENT.
        /// </param>
        public void AddFlags(int flags) 
        {
            m_Flags.Add(flags);
        }

        /// <summary>
        /// Add extended data to the intent. The name must include a package prefix, 
        /// for example the app com.android.contacts would use names like "com.android.contacts.ShowAll".
        /// </summary>
        /// <param name="name"> The name of the extra data, with package prefix.</param>
        /// <param name="value">The string data value.</param>
        public void PutExtra(string name, string value) 
        {
            var extra = new AN_IntentExtraValue(AN_IntentExtraValueType.String);
            extra.SetValue(value);

            PutExtra(name, extra);
        }
        
        /// <summary>
        /// Add extended data to the intent. The name must include a package prefix, 
        /// for example the app com.android.contacts would use names like "com.android.contacts.ShowAll".
        /// </summary>
        /// <param name="name"> The name of the extra data, with package prefix.</param>
        /// <param name="value">The string data value.</param>
        public void PutExtra(string name, params string[] values) 
        {
            var extra = new AN_IntentExtraValue(AN_IntentExtraValueType.StringsArray);
            foreach(var str in values) {
                extra.AddValue(str);
            }

            PutExtra(name, extra);
        }
        
        /// <summary>
        /// Add extended data to the intent. The name must include a package prefix, 
        /// for example the app com.android.contacts would use names like "com.android.contacts.ShowAll".
        /// </summary>
        /// <param name="name"> The name of the extra data, with package prefix.</param>
        /// <param name="value">The Texture2D data value.</param>
        public void PutExtra(string name, Texture2D value) 
        {
            var extra = new AN_IntentExtraValue(AN_IntentExtraValueType.Texture);
            extra.SetValue( new AN_Image(value).ToJson() );

            PutExtra(name, extra);
        }

        /// <summary>
        /// Add extended data to the intent. The name must include a package prefix, 
        /// for example the app com.android.contacts would use names like "com.android.contacts.ShowAll".
        /// </summary>
        /// <param name="name"> The name of the extra data, with package prefix.</param>
        /// <param name="values">The  List<Texture2D> data value.</param>
        public void PutExtra(string name, params Texture2D[] values) 
        {
            var extra = new AN_IntentExtraValue(AN_IntentExtraValueType.TexturesArray);
            foreach (var tex in values) {
                extra.AddValue(new AN_Image(tex).ToJson());
            }

            PutExtra(name, extra);
        }
        
        /// <summary>
        /// Add extended data to the intent. The name must include a package prefix, 
        /// for example the app com.android.contacts would use names like "com.android.contacts.ShowAll".
        /// </summary>
        /// <param name="name"> The name of the extra data, with package prefix.</param>
        /// <param name="uri">The Uri data value.</param>
        public void PutExtra(string name, Uri uri) 
        {
            var extra = new AN_IntentExtraValue(AN_IntentExtraValueType.Uri);
            extra.SetValue(uri.AbsoluteUri);

            PutExtra(name, extra);
        }

        /// <summary>
        /// Add extended data to the intent. The name must include a package prefix, 
        /// for example the app com.android.contacts would use names like "com.android.contacts.ShowAll".
        /// </summary>
        /// <param name="name"> The name of the extra data, with package prefix.</param>
        /// <param name="values">The List<Uri> data value.</param>
        public void PutExtra(string name, params Uri[] values) 
        {

            var extra = new AN_IntentExtraValue(AN_IntentExtraValueType.Uri);
            foreach (var uri in values) {
                extra.AddValue(uri.AbsoluteUri);
            }

            PutExtra(name, extra);
        }

        //--------------------------------------
        // Private Methods
        //--------------------------------------

        private void PutExtra(string name, AN_IntentExtraValue extra) 
        {
            m_ExtraKeys.Add(name);
            m_ExtraValues.Add(extra);
        }

        private void SetChooserParams(AN_IntentChooserInfo chooserInfo) 
        {
            m_IsChooser = true;
            m_ChooserInfo = chooserInfo;
        }

        //--------------------------------------
        // Public Static Methods
        //--------------------------------------


        /// <summary>
        /// Convenience function for creating a ACTION_CHOOSER Intent.
        /// 
        /// Builds a new ACTION_CHOOSER Intent that wraps the given target intent, 
        /// also optionally supplying a title. 
        /// If the target intent has specified FLAG_GRANT_READ_URI_PERMISSION or FLAG_GRANT_WRITE_URI_PERMISSION, 
        /// then these flags will also be set in the returned chooser intent
        /// </summary>
        /// <param name="intent">The Intent that the user will be selecting an activity to perform.</param>
        /// <param name="title">Title that will be displayed in the chooser.</param>
        /// <param name="extraInitialIntents">Optional, additional activities to place a the front of the list of choices, when shown to the user with a ACTION_CHOOSER.</param>
        /// <returns></returns>
        public static AN_Intent CreateChooser(AN_Intent intent, string title, params AN_Intent[] extraInitialIntents) 
        {
            var chooser = new AN_IntentChooserInfo(title);
            foreach(var extraIntent in extraInitialIntents) {
                chooser.AddExtraIntent(extraIntent);
            }

            intent.SetChooserParams(chooser);
            return intent;
        }
        
        //--------------------------------------
        // Private Classes
        //--------------------------------------

        [Serializable]
        private class AN_IntentChooserInfo
        {
			#pragma warning disable 414
            [SerializeField] string m_title;
            [SerializeField] List<string> m_extra_initial_intents = new List<string>();
			#pragma warning restore 414

            public AN_IntentChooserInfo(string title) {
                m_title = title;
            }

            public void AddExtraIntent(AN_Intent intent) {
                m_extra_initial_intents.Add(JsonUtility.ToJson(intent));
            }
        }

        [Serializable]
        private class AN_IntentExtraValue
        {
			#pragma warning disable 414
            [SerializeField] string m_value = string.Empty;
            [SerializeField] List<string> m_values = new List<string>();
            [SerializeField] AN_IntentExtraValueType m_type;
			#pragma warning restore 414


            public AN_IntentExtraValue(AN_IntentExtraValueType type) {
                m_type = type;
            }

            public void SetValue(string value) {
                m_value = value;
            }

            public void AddValue(string value) {
                m_values.Add(value);
            }
        }

        private enum AN_IntentExtraValueType
        {
          
            String = 0,
            StringsArray = 1,
            Texture = 2,
            TexturesArray = 3,
            Uri = 4
        }



        //--------------------------------------
        // Constants
        //--------------------------------------


        public const string ACTION_AIRPLANE_MODE_CHANGED = "android.intent.action.AIRPLANE_MODE";
        public const string ACTION_ALL_APPS = "android.intent.action.ALL_APPS";
        public const string ACTION_ANSWER = "android.intent.action.ANSWER";
        public const string ACTION_APPLICATION_PREFERENCES = "android.intent.action.APPLICATION_PREFERENCES";
        public const string ACTION_APPLICATION_RESTRICTIONS_CHANGED = "android.intent.action.APPLICATION_RESTRICTIONS_CHANGED";
        public const string ACTION_APP_ERROR = "android.intent.action.APP_ERROR";
        public const string ACTION_ASSIST = "android.intent.action.ASSIST";
        public const string ACTION_ATTACH_DATA = "android.intent.action.ATTACH_DATA";
        public const string ACTION_BATTERY_CHANGED = "android.intent.action.BATTERY_CHANGED";
        public const string ACTION_BATTERY_LOW = "android.intent.action.BATTERY_LOW";
        public const string ACTION_BATTERY_OKAY = "android.intent.action.BATTERY_OKAY";
        public const string ACTION_BOOT_COMPLETED = "android.intent.action.BOOT_COMPLETED";
        public const string ACTION_BUG_REPORT = "android.intent.action.BUG_REPORT";
        public const string ACTION_CALL = "android.intent.action.CALL";
        public const string ACTION_CALL_BUTTON = "android.intent.action.CALL_BUTTON";
        public const string ACTION_CAMERA_BUTTON = "android.intent.action.CAMERA_BUTTON";
        public const string ACTION_CARRIER_SETUP = "android.intent.action.CARRIER_SETUP";
        public const string ACTION_CHOOSER = "android.intent.action.CHOOSER";
        public const string ACTION_CLOSE_SYSTEM_DIALOGS = "android.intent.action.CLOSE_SYSTEM_DIALOGS";
        public const string ACTION_CONFIGURATION_CHANGED = "android.intent.action.CONFIGURATION_CHANGED";
        public const string ACTION_CREATE_DOCUMENT = "android.intent.action.CREATE_DOCUMENT";
        public const string ACTION_CREATE_SHORTCUT = "android.intent.action.CREATE_SHORTCUT";
        public const string ACTION_DATE_CHANGED = "android.intent.action.DATE_CHANGED";
        public const string ACTION_DEFAULT = "android.intent.action.VIEW";
        public const string ACTION_DELETE = "android.intent.action.DELETE";
  
        public const string ACTION_DIAL = "android.intent.action.DIAL";
        public const string ACTION_DOCK_EVENT = "android.intent.action.DOCK_EVENT";
        public const string ACTION_DREAMING_STARTED = "android.intent.action.DREAMING_STARTED";
        public const string ACTION_DREAMING_STOPPED = "android.intent.action.DREAMING_STOPPED";
        public const string ACTION_EDIT = "android.intent.action.EDIT";
        public const string ACTION_EXTERNAL_APPLICATIONS_AVAILABLE = "android.intent.action.EXTERNAL_APPLICATIONS_AVAILABLE";
        public const string ACTION_EXTERNAL_APPLICATIONS_UNAVAILABLE = "android.intent.action.EXTERNAL_APPLICATIONS_UNAVAILABLE";
        public const string ACTION_FACTORY_TEST = "android.intent.action.FACTORY_TEST";
        public const string ACTION_GET_CONTENT = "android.intent.action.GET_CONTENT";
        public const string ACTION_GET_RESTRICTION_ENTRIES = "android.intent.action.GET_RESTRICTION_ENTRIES";
        public const string ACTION_GTALK_SERVICE_CONNECTED = "android.intent.action.GTALK_CONNECTED";
        public const string ACTION_GTALK_SERVICE_DISCONNECTED = "android.intent.action.GTALK_DISCONNECTED";
        public const string ACTION_HEADSET_PLUG = "android.intent.action.HEADSET_PLUG";
        public const string ACTION_INPUT_METHOD_CHANGED = "android.intent.action.INPUT_METHOD_CHANGED";
        public const string ACTION_INSERT = "android.intent.action.INSERT";
        public const string ACTION_INSERT_OR_EDIT = "android.intent.action.INSERT_OR_EDIT";
        public const string ACTION_INSTALL_PACKAGE = "android.intent.action.INSTALL_PACKAGE";
        public const string ACTION_LOCALE_CHANGED = "android.intent.action.LOCALE_CHANGED";
        public const string ACTION_LOCKED_BOOT_COMPLETED = "android.intent.action.LOCKED_BOOT_COMPLETED";
        public const string ACTION_MAIN = "android.intent.action.MAIN";
        public const string ACTION_MANAGED_PROFILE_ADDED = "android.intent.action.MANAGED_PROFILE_ADDED";
        public const string ACTION_MANAGED_PROFILE_AVAILABLE = "android.intent.action.MANAGED_PROFILE_AVAILABLE";
        public const string ACTION_MANAGED_PROFILE_REMOVED = "android.intent.action.MANAGED_PROFILE_REMOVED";
        public const string ACTION_MANAGED_PROFILE_UNAVAILABLE = "android.intent.action.MANAGED_PROFILE_UNAVAILABLE";
        public const string ACTION_MANAGED_PROFILE_UNLOCKED = "android.intent.action.MANAGED_PROFILE_UNLOCKED";
        public const string ACTION_MANAGE_NETWORK_USAGE = "android.intent.action.MANAGE_NETWORK_USAGE";
        public const string ACTION_MANAGE_PACKAGE_STORAGE = "android.intent.action.MANAGE_PACKAGE_STORAGE";
        public const string ACTION_MEDIA_BAD_REMOVAL = "android.intent.action.MEDIA_BAD_REMOVAL";
        public const string ACTION_MEDIA_BUTTON = "android.intent.action.MEDIA_BUTTON";
        public const string ACTION_MEDIA_CHECKING = "android.intent.action.MEDIA_CHECKING";
        public const string ACTION_MEDIA_EJECT = "android.intent.action.MEDIA_EJECT";
        public const string ACTION_MEDIA_MOUNTED = "android.intent.action.MEDIA_MOUNTED";
        public const string ACTION_MEDIA_NOFS = "android.intent.action.MEDIA_NOFS";
        public const string ACTION_MEDIA_REMOVED = "android.intent.action.MEDIA_REMOVED";
        public const string ACTION_MEDIA_SCANNER_FINISHED = "android.intent.action.MEDIA_SCANNER_FINISHED";
        public const string ACTION_MEDIA_SCANNER_SCAN_FILE = "android.intent.action.MEDIA_SCANNER_SCAN_FILE";
        public const string ACTION_MEDIA_SCANNER_STARTED = "android.intent.action.MEDIA_SCANNER_STARTED";
        public const string ACTION_MEDIA_SHARED = "android.intent.action.MEDIA_SHARED";
        public const string ACTION_MEDIA_UNMOUNTABLE = "android.intent.action.MEDIA_UNMOUNTABLE";
        public const string ACTION_MEDIA_UNMOUNTED = "android.intent.action.MEDIA_UNMOUNTED";
        public const string ACTION_MY_PACKAGE_REPLACED = "android.intent.action.MY_PACKAGE_REPLACED";
        public const string ACTION_NEW_OUTGOING_CALL = "android.intent.action.NEW_OUTGOING_CALL";
        public const string ACTION_OPEN_DOCUMENT = "android.intent.action.OPEN_DOCUMENT";
        public const string ACTION_OPEN_DOCUMENT_TREE = "android.intent.action.OPEN_DOCUMENT_TREE";
        public const string ACTION_PACKAGES_SUSPENDED = "android.intent.action.PACKAGES_SUSPENDED";
        public const string ACTION_PACKAGES_UNSUSPENDED = "android.intent.action.PACKAGES_UNSUSPENDED";
        public const string ACTION_PACKAGE_ADDED = "android.intent.action.PACKAGE_ADDED";
        public const string ACTION_PACKAGE_CHANGED = "android.intent.action.PACKAGE_CHANGED";
        public const string ACTION_PACKAGE_DATA_CLEARED = "android.intent.action.PACKAGE_DATA_CLEARED";
        public const string ACTION_PACKAGE_FIRST_LAUNCH = "android.intent.action.PACKAGE_FIRST_LAUNCH";
        public const string ACTION_PACKAGE_FULLY_REMOVED = "android.intent.action.PACKAGE_FULLY_REMOVED";
    
        public const string ACTION_PACKAGE_INSTALL = "android.intent.action.PACKAGE_INSTALL";
        public const string ACTION_PACKAGE_NEEDS_VERIFICATION = "android.intent.action.PACKAGE_NEEDS_VERIFICATION";
        public const string ACTION_PACKAGE_REMOVED = "android.intent.action.PACKAGE_REMOVED";
        public const string ACTION_PACKAGE_REPLACED = "android.intent.action.PACKAGE_REPLACED";
        public const string ACTION_PACKAGE_RESTARTED = "android.intent.action.PACKAGE_RESTARTED";
        public const string ACTION_PACKAGE_VERIFIED = "android.intent.action.PACKAGE_VERIFIED";
        public const string ACTION_PASTE = "android.intent.action.PASTE";
        public const string ACTION_PICK = "android.intent.action.PICK";
        public const string ACTION_PICK_ACTIVITY = "android.intent.action.PICK_ACTIVITY";
        public const string ACTION_POWER_CONNECTED = "android.intent.action.ACTION_POWER_CONNECTED";
        public const string ACTION_POWER_DISCONNECTED = "android.intent.action.ACTION_POWER_DISCONNECTED";
        public const string ACTION_POWER_USAGE_SUMMARY = "android.intent.action.POWER_USAGE_SUMMARY";
        public const string ACTION_PROCESS_TEXT = "android.intent.action.PROCESS_TEXT";
        public const string ACTION_PROVIDER_CHANGED = "android.intent.action.PROVIDER_CHANGED";
        public const string ACTION_QUICK_CLOCK = "android.intent.action.QUICK_CLOCK";
        public const string ACTION_QUICK_VIEW = "android.intent.action.QUICK_VIEW";
        public const string ACTION_REBOOT = "android.intent.action.REBOOT";
        public const string ACTION_RUN = "android.intent.action.RUN";
        public const string ACTION_SCREEN_OFF = "android.intent.action.SCREEN_OFF";
        public const string ACTION_SCREEN_ON = "android.intent.action.SCREEN_ON";
        public const string ACTION_SEARCH = "android.intent.action.SEARCH";
        public const string ACTION_SEARCH_LONG_PRESS = "android.intent.action.SEARCH_LONG_PRESS";
        public const string ACTION_SEND = "android.intent.action.SEND";
        public const string ACTION_SENDTO = "android.intent.action.SENDTO";
        public const string ACTION_SEND_MULTIPLE = "android.intent.action.SEND_MULTIPLE";
        public const string ACTION_SET_WALLPAPER = "android.intent.action.SET_WALLPAPER";
        public const string ACTION_SHOW_APP_INFO = "android.intent.action.SHOW_APP_INFO";
        public const string ACTION_SHUTDOWN = "android.intent.action.ACTION_SHUTDOWN";
        public const string ACTION_SYNC = "android.intent.action.SYNC";
        public const string ACTION_SYSTEM_TUTORIAL = "android.intent.action.SYSTEM_TUTORIAL";
        public const string ACTION_TIMEZONE_CHANGED = "android.intent.action.TIMEZONE_CHANGED";
        public const string ACTION_TIME_CHANGED = "android.intent.action.TIME_SET";
        public const string ACTION_TIME_TICK = "android.intent.action.TIME_TICK";
        public const string ACTION_UID_REMOVED = "android.intent.action.UID_REMOVED";
  
  
        public const string ACTION_UNINSTALL_PACKAGE = "android.intent.action.UNINSTALL_PACKAGE";
        public const string ACTION_USER_BACKGROUND = "android.intent.action.USER_BACKGROUND";
        public const string ACTION_USER_FOREGROUND = "android.intent.action.USER_FOREGROUND";
        public const string ACTION_USER_INITIALIZE = "android.intent.action.USER_INITIALIZE";
        public const string ACTION_USER_PRESENT = "android.intent.action.USER_PRESENT";
        public const string ACTION_USER_UNLOCKED = "android.intent.action.USER_UNLOCKED";
        public const string ACTION_VIEW = "android.intent.action.VIEW";
        public const string ACTION_VOICE_COMMAND = "android.intent.action.VOICE_COMMAND";
   
        public const string ACTION_WEB_SEARCH = "android.intent.action.WEB_SEARCH";
        public const string CATEGORY_ALTERNATIVE = "android.intent.category.ALTERNATIVE";
        public const string CATEGORY_APP_BROWSER = "android.intent.category.APP_BROWSER";
        public const string CATEGORY_APP_CALCULATOR = "android.intent.category.APP_CALCULATOR";
        public const string CATEGORY_APP_CALENDAR = "android.intent.category.APP_CALENDAR";
        public const string CATEGORY_APP_CONTACTS = "android.intent.category.APP_CONTACTS";
        public const string CATEGORY_APP_EMAIL = "android.intent.category.APP_EMAIL";
        public const string CATEGORY_APP_GALLERY = "android.intent.category.APP_GALLERY";
        public const string CATEGORY_APP_MAPS = "android.intent.category.APP_MAPS";
        public const string CATEGORY_APP_MARKET = "android.intent.category.APP_MARKET";
        public const string CATEGORY_APP_MESSAGING = "android.intent.category.APP_MESSAGING";
        public const string CATEGORY_APP_MUSIC = "android.intent.category.APP_MUSIC";
        public const string CATEGORY_BROWSABLE = "android.intent.category.BROWSABLE";
        public const string CATEGORY_CAR_DOCK = "android.intent.category.CAR_DOCK";
        public const string CATEGORY_CAR_MODE = "android.intent.category.CAR_MODE";
        public const string CATEGORY_DEFAULT = "android.intent.category.DEFAULT";
        public const string CATEGORY_DESK_DOCK = "android.intent.category.DESK_DOCK";
        public const string CATEGORY_DEVELOPMENT_PREFERENCE = "android.intent.category.DEVELOPMENT_PREFERENCE";
        public const string CATEGORY_EMBED = "android.intent.category.EMBED";
        public const string CATEGORY_FRAMEWORK_INSTRUMENTATION_TEST = "android.intent.category.FRAMEWORK_INSTRUMENTATION_TEST";
        public const string CATEGORY_HE_DESK_DOCK = "android.intent.category.HE_DESK_DOCK";
        public const string CATEGORY_HOME = "android.intent.category.HOME";
        public const string CATEGORY_INFO = "android.intent.category.INFO";
        public const string CATEGORY_LAUNCHER = "android.intent.category.LAUNCHER";
        public const string CATEGORY_LEANBACK_LAUNCHER = "android.intent.category.LEANBACK_LAUNCHER";
        public const string CATEGORY_LE_DESK_DOCK = "android.intent.category.LE_DESK_DOCK";
        public const string CATEGORY_MONKEY = "android.intent.category.MONKEY";
        public const string CATEGORY_OPENABLE = "android.intent.category.OPENABLE";
        public const string CATEGORY_PREFERENCE = "android.intent.category.PREFERENCE";
        public const string CATEGORY_SAMPLE_CODE = "android.intent.category.SAMPLE_CODE";
        public const string CATEGORY_SELECTED_ALTERNATIVE = "android.intent.category.SELECTED_ALTERNATIVE";
        public const string CATEGORY_TAB = "android.intent.category.TAB";
        public const string CATEGORY_TEST = "android.intent.category.TEST";
        public const string CATEGORY_TYPED_OPENABLE = "android.intent.category.TYPED_OPENABLE";
        public const string CATEGORY_UNIT_TEST = "android.intent.category.UNIT_TEST";
        public const string CATEGORY_VOICE = "android.intent.category.VOICE";
        public const string CATEGORY_VR_HOME = "android.intent.category.VR_HOME";
        public const string EXTRA_ALARM_COUNT = "android.intent.extra.ALARM_COUNT";
        public const string EXTRA_ALLOW_MULTIPLE = "android.intent.extra.ALLOW_MULTIPLE";
   
        public const string EXTRA_ALTERNATE_INTENTS = "android.intent.extra.ALTERNATE_INTENTS";
        public const string EXTRA_ASSIST_CONTEXT = "android.intent.extra.ASSIST_CONTEXT";
        public const string EXTRA_ASSIST_INPUT_DEVICE_ID = "android.intent.extra.ASSIST_INPUT_DEVICE_ID";
        public const string EXTRA_ASSIST_INPUT_HINT_KEYBOARD = "android.intent.extra.ASSIST_INPUT_HINT_KEYBOARD";
        public const string EXTRA_ASSIST_PACKAGE = "android.intent.extra.ASSIST_PACKAGE";
        public const string EXTRA_ASSIST_UID = "android.intent.extra.ASSIST_UID";
        public const string EXTRA_BCC = "android.intent.extra.BCC";
        public const string EXTRA_BUG_REPORT = "android.intent.extra.BUG_REPORT";
        public const string EXTRA_CC = "android.intent.extra.CC";
   
        public const string EXTRA_CHANGED_COMPONENT_NAME_LIST = "android.intent.extra.changed_component_name_list";
        public const string EXTRA_CHANGED_PACKAGE_LIST = "android.intent.extra.changed_package_list";
        public const string EXTRA_CHANGED_UID_LIST = "android.intent.extra.changed_uid_list";
        public const string EXTRA_CHOOSER_REFINEMENT_INTENT_SENDER = "android.intent.extra.CHOOSER_REFINEMENT_INTENT_SENDER";
        public const string EXTRA_CHOOSER_TARGETS = "android.intent.extra.CHOOSER_TARGETS";
        public const string EXTRA_CHOSEN_COMPONENT = "android.intent.extra.CHOSEN_COMPONENT";
        public const string EXTRA_CHOSEN_COMPONENT_INTENT_SENDER = "android.intent.extra.CHOSEN_COMPONENT_INTENT_SENDER";
        public const string EXTRA_COMPONENT_NAME = "android.intent.extra.COMPONENT_NAME";
        public const string EXTRA_CONTENT_ANNOTATIONS = "android.intent.extra.CONTENT_ANNOTATIONS";
        public const string EXTRA_DATA_REMOVED = "android.intent.extra.DATA_REMOVED";
        public const string EXTRA_DOCK_STATE = "android.intent.extra.DOCK_STATE";
        public const int EXTRA_DOCK_STATE_CAR = 2;
        public const int EXTRA_DOCK_STATE_DESK = 1;
        public const int EXTRA_DOCK_STATE_HE_DESK = 4;
        public const int EXTRA_DOCK_STATE_LE_DESK = 3;
        public const int EXTRA_DOCK_STATE_UNDOCKED = 0;
        public const string EXTRA_DONT_KILL_APP = "android.intent.extra.DONT_KILL_APP";
        public const string EXTRA_EMAIL = "android.intent.extra.EMAIL";
        public const string EXTRA_EXCLUDE_COMPONENTS = "android.intent.extra.EXCLUDE_COMPONENTS";
        public const string EXTRA_FROM_STORAGE = "android.intent.extra.FROM_STORAGE";
        public const string EXTRA_HTML_TEXT = "android.intent.extra.HTML_TEXT";
        public const string EXTRA_INDEX = "android.intent.extra.INDEX";
        public const string EXTRA_INITIAL_INTENTS = "android.intent.extra.INITIAL_INTENTS";
        public const string EXTRA_INSTALLER_PACKAGE_NAME = "android.intent.extra.INSTALLER_PACKAGE_NAME";
        public const string EXTRA_INTENT = "android.intent.extra.INTENT";
        public const string EXTRA_KEY_EVENT = "android.intent.extra.KEY_EVENT";
        public const string EXTRA_LOCAL_ONLY = "android.intent.extra.LOCAL_ONLY";
        public const string EXTRA_MIME_TYPES = "android.intent.extra.MIME_TYPES";
        public const string EXTRA_NOT_UNKNOWN_SOURCE = "android.intent.extra.NOT_UNKNOWN_SOURCE";
        public const string EXTRA_ORIGINATING_URI = "android.intent.extra.ORIGINATING_URI";
        public const string EXTRA_PACKAGE_NAME = "android.intent.extra.PACKAGE_NAME";
        public const string EXTRA_PHONE_NUMBER = "android.intent.extra.PHONE_NUMBER";
        public const string EXTRA_PROCESS_TEXT = "android.intent.extra.PROCESS_TEXT";
        public const string EXTRA_PROCESS_TEXT_READONLY = "android.intent.extra.PROCESS_TEXT_READONLY";
        public const string EXTRA_QUICK_VIEW_FEATURES = "android.intent.extra.QUICK_VIEW_FEATURES";
        public const string EXTRA_QUIET_MODE = "android.intent.extra.QUIET_MODE";
        public const string EXTRA_REFERRER = "android.intent.extra.REFERRER";
        public const string EXTRA_REFERRER_NAME = "android.intent.extra.REFERRER_NAME";
        public const string EXTRA_REMOTE_INTENT_TOKEN = "android.intent.extra.remote_intent_token";
        public const string EXTRA_REPLACEMENT_EXTRAS = "android.intent.extra.REPLACEMENT_EXTRAS";
        public const string EXTRA_REPLACING = "android.intent.extra.REPLACING";
        public const string EXTRA_RESTRICTIONS_BUNDLE = "android.intent.extra.restrictions_bundle";
        public const string EXTRA_RESTRICTIONS_INTENT = "android.intent.extra.restrictions_intent";
        public const string EXTRA_RESTRICTIONS_LIST = "android.intent.extra.restrictions_list";
        public const string EXTRA_RESULT_RECEIVER = "android.intent.extra.RESULT_RECEIVER";
        public const string EXTRA_RETURN_RESULT = "android.intent.extra.RETURN_RESULT";
  

 
   
        public const string EXTRA_SHUTDOWN_USERSPACE_ONLY = "android.intent.extra.SHUTDOWN_USERSPACE_ONLY";
        public const string EXTRA_STREAM = "android.intent.extra.STREAM";
        public const string EXTRA_SUBJECT = "android.intent.extra.SUBJECT";
        public const string EXTRA_TEMPLATE = "android.intent.extra.TEMPLATE";
        public const string EXTRA_TEXT = "android.intent.extra.TEXT";
        public const string EXTRA_TITLE = "android.intent.extra.TITLE";
        public const string EXTRA_UID = "android.intent.extra.UID";
        public const string EXTRA_USER = "android.intent.extra.USER";
        public const int FILL_IN_ACTION = 1;
        public const int FILL_IN_CATEGORIES = 4;
        public const int FILL_IN_CLIP_DATA = 128;
        public const int FILL_IN_COMPONENT = 8;
        public const int FILL_IN_DATA = 2;
        public const int FILL_IN_PACKAGE = 16;
        public const int FILL_IN_SELECTOR = 64;
        public const int FILL_IN_SOURCE_BOUNDS = 32;
        public const int FLAG_ACTIVITY_BROUGHT_TO_FRONT = 4194304;
        public const int FLAG_ACTIVITY_CLEAR_TASK = 32768;
        public const int FLAG_ACTIVITY_CLEAR_TOP = 67108864;
      
        public const int FLAG_ACTIVITY_EXCLUDE_FROM_RECENTS = 8388608;
        public const int FLAG_ACTIVITY_FORWARD_RESULT = 33554432;
        public const int FLAG_ACTIVITY_LAUNCHED_FROM_HISTORY = 1048576;
        public const int FLAG_ACTIVITY_LAUNCH_ADJACENT = 4096;
        public const int FLAG_ACTIVITY_MULTIPLE_TASK = 134217728;
        public const int FLAG_ACTIVITY_NEW_DOCUMENT = 524288;
        public const int FLAG_ACTIVITY_NEW_TASK = 268435456;
        public const int FLAG_ACTIVITY_NO_ANIMATION = 65536;
        public const int FLAG_ACTIVITY_NO_HISTORY = 1073741824;
        public const int FLAG_ACTIVITY_NO_USER_ACTION = 262144;
        public const int FLAG_ACTIVITY_PREVIOUS_IS_TOP = 16777216;
        public const int FLAG_ACTIVITY_REORDER_TO_FRONT = 131072;
        public const int FLAG_ACTIVITY_RESET_TASK_IF_NEEDED = 2097152;
        public const int FLAG_ACTIVITY_RETAIN_IN_RECENTS = 8192;
        public const int FLAG_ACTIVITY_SINGLE_TOP = 536870912;
        public const int FLAG_ACTIVITY_TASK_ON_HOME = 16384;
        public const int FLAG_DEBUG_LOG_RESOLUTION = 8;
        public const int FLAG_EXCLUDE_STOPPED_PACKAGES = 16;
        public const int FLAG_FROM_BACKGROUND = 4;
        public const int FLAG_GRANT_PERSISTABLE_URI_PERMISSION = 64;
        public const int FLAG_GRANT_PREFIX_URI_PERMISSION = 128;
        public const int FLAG_GRANT_READ_URI_PERMISSION = 1;
        public const int FLAG_GRANT_WRITE_URI_PERMISSION = 2;
        public const int FLAG_INCLUDE_STOPPED_PACKAGES = 32;
        public const int FLAG_RECEIVER_FOREGROUND = 268435456;
        public const int FLAG_RECEIVER_NO_ABORT = 134217728;
        public const int FLAG_RECEIVER_REGISTERED_ONLY = 1073741824;
        public const int FLAG_RECEIVER_REPLACE_PENDING = 536870912;
        public const int FLAG_RECEIVER_VISIBLE_TO_INSTANT_APPS = 2097152;
        public const string METADATA_DOCK_HOME = "android.dock_home";
        public const int URI_ALLOW_UNSAFE = 4;
        public const int URI_ANDROID_APP_SCHEME = 2;
        public const int URI_INTENT_SCHEME = 1;

    }
}