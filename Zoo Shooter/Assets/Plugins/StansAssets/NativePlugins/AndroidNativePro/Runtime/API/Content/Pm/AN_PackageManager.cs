using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.App;
using SA.Android.Utilities;

namespace SA.Android.Content.Pm
{

    /// <summary>
    /// Class for retrieving various kinds of information related to the application packages 
    /// that are currently installed on the device. 
    /// You can find this class through <see cref="AN_Context.GetPackageManager"/>.
    /// </summary>
    public class AN_PackageManager 
    {

        public  enum PermissionState
        {

            /// <summary>
            /// Permission check result: this is returned by <see cref="AN_ContextCompat.CheckSelfPermission"/>
            /// if the permission has been granted to the given package.
            /// </summary>
            Granted = 0,

            /// <summary>
            /// Permission check result: this is returned by <see cref="AN_ContextCompat.CheckSelfPermission"/>
            /// if the permission has not been granted to the given package.
            /// </summary>
            Denied = -1

        }

        private AN_Context m_context;
        private static string ANDROID_CLASS = "com.stansassets.android.content.pm.AN_PackageManager";


        public AN_PackageManager(AN_Context context) {
            m_context = context;
        }





        /// <summary>
        /// Find a single content provider by its base path name.
        /// </summary>
        public AN_ProviderInfo ResolveContentProvider() {
            return null;
        }


        /// <summary>
        /// Retrieve all activities that can be performed for the given intent.
        /// </summary>
        /// <param name="intent">The desired intent as per resolveActivity().</param>
        /// <param name="flags">
        /// Additional option flags to modify the data returned. 
        /// The most important is MATCH_DEFAULT_ONLY, to limit the resolution to only those activities that support the AN_Intent.CATEGORY_DEFAULT. 
        /// Or, set MATCH_ALL to prevent any filtering of the results.
        /// Value is either 0 or combination of GET_META_DATA, GET_SIGNATURES, GET_SHARED_LIBRARY_FILES, MATCH_ALL, 
        /// MATCH_DISABLED_COMPONENTS, MATCH_DISABLED_UNTIL_USED_COMPONENTS, 
        /// MATCH_DEFAULT_ONLY, MATCH_DIRECT_BOOT_AWARE, MATCH_DIRECT_BOOT_UNAWARE, 
        /// MATCH_SYSTEM_ONLY or MATCH_UNINSTALLED_PACKAGES.
        /// </param>
        /// <returns></returns>
        public List<AN_ResolveInfo> QueryIntentActivities(AN_Intent intent, int flags = 0) {

           if(Application.isEditor) {
                return new List<AN_ResolveInfo>();
           }
          
           var json =  AN_Java.Bridge.CallStatic<string>(ANDROID_CLASS, "QueryIntentActivities", m_context, intent, flags);
           var result = JsonUtility.FromJson<AN_PackageManagerResolveInfoResult>(json);

           return result.m_list;
        }


        /// <summary>
        /// Retrieve overall information about an application package that is installed on the system.
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="flags"></param>
        /// <returns>
        /// Returns a List of ResolveInfo objects containing one entry for each matching activity, ordered from best to worst. 
        /// In other words, the first item is what would be returned by resolveActivity(Intent, int). 
        /// If there are no matching activities, an empty list is returned.
        /// </returns>
        public AN_PackageInfo GetPackageInfo(string packageName, int flags) {

            if(Application.isEditor) {
                string version = "undefined";
                if (packageName.Equals(Application.identifier)) {
                    version = Application.version;
                }

                return new AN_PackageInfo(packageName, version);
            }

            var json = AN_Java.Bridge.CallStatic<string>(ANDROID_CLASS, "GetPackageInfo", m_context, packageName, flags);
            if(json == null) {
                return null;
            }

            var result = JsonUtility.FromJson<AN_PackageInfo>(json);
            return result;
        }


        /// <summary>
        /// Returns a "good" intent to launch a front-door activity in a package. 
        /// This is used, for example, to implement an "open" button when browsing through packages. 
        /// The current implementation looks first for a main activity in the category <see cref="AN_Intent.CATEGORY_INFO"/>, 
        /// and next for a main activity in the category <see cref="AN_Intent.CATEGORY_LAUNCHER"/>  
        /// Returns null if neither are found.
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public AN_Intent GetLaunchIntentForPackage(String packageName) {
            int instanceId = AN_Java.Bridge.CallStatic<int>(ANDROID_CLASS, "GetLaunchIntentForPackage", m_context, packageName);
            if(instanceId == 0) {
                return null;
            }

            return new AN_Intent(instanceId);
        }



        //--------------------------------------
        // Private Classes
        //--------------------------------------

        [Serializable]
        private class AN_PackageManagerResolveInfoResult
        {
            public List<AN_ResolveInfo> m_list = new List<AN_ResolveInfo>();
        }



        //--------------------------------------
        // Constants
        //--------------------------------------


        public static int CERT_INPUT_RAW_X509 = 0;
        public static int CERT_INPUT_SHA256 = 1;
        public static int COMPONENT_ENABLED_STATE_DEFAULT = 0;
        public static int COMPONENT_ENABLED_STATE_DISABLED = 2;
        public static int COMPONENT_ENABLED_STATE_DISABLED_UNTIL_USED = 4;
        public static int COMPONENT_ENABLED_STATE_DISABLED_USER = 3;
        public static int COMPONENT_ENABLED_STATE_ENABLED = 1;
        public static int DONT_KILL_APP = 1;
        public static string EXTRA_VERIFICATION_ID = "android.content.pm.extra.VERIFICATION_ID";
        public static string EXTRA_VERIFICATION_RESULT = "android.content.pm.extra.VERIFICATION_RESULT";
        public static string FEATURE_ACTIVITIES_ON_SECONDARY_DISPLAYS = "android.software.activities_on_secondary_displays";
        public static string FEATURE_APP_WIDGETS = "android.software.app_widgets";
        public static string FEATURE_AUDIO_LOW_LATENCY = "android.hardware.audio.low_latency";
        public static string FEATURE_AUDIO_OUTPUT = "android.hardware.audio.output";
        public static string FEATURE_AUDIO_PRO = "android.hardware.audio.pro";
        public static string FEATURE_AUTOFILL = "android.software.autofill";
        public static string FEATURE_AUTOMOTIVE = "android.hardware.type.automotive";
        public static string FEATURE_BACKUP = "android.software.backup";
        public static string FEATURE_BLUETOOTH = "android.hardware.bluetooth";
        public static string FEATURE_BLUETOOTH_LE = "android.hardware.bluetooth_le";
        public static string FEATURE_CAMERA = "android.hardware.camera";
        public static string FEATURE_CAMERA_ANY = "android.hardware.camera.any";
        public static string FEATURE_CAMERA_AR = "android.hardware.camera.ar";
        public static string FEATURE_CAMERA_AUTOFOCUS = "android.hardware.camera.autofocus";
        public static string FEATURE_CAMERA_CAPABILITY_MANUAL_POST_PROCESSING = "android.hardware.camera.capability.manual_post_processing";
        public static string FEATURE_CAMERA_CAPABILITY_MANUAL_SENSOR = "android.hardware.camera.capability.manual_sensor";
        public static string FEATURE_CAMERA_CAPABILITY_RAW = "android.hardware.camera.capability.raw";
        public static string FEATURE_CAMERA_EXTERNAL = "android.hardware.camera.external";
        public static string FEATURE_CAMERA_FLASH = "android.hardware.camera.flash";
        public static string FEATURE_CAMERA_FRONT = "android.hardware.camera.front";
        public static string FEATURE_CAMERA_LEVEL_FULL = "android.hardware.camera.level.full";
        public static string FEATURE_CANT_SAVE_STATE = "android.software.cant_save_state";
        public static string FEATURE_COMPANION_DEVICE_SETUP = "android.software.companion_device_setup";
        public static string FEATURE_CONNECTION_SERVICE = "android.software.connectionservice";
        public static string FEATURE_CONSUMER_IR = "android.hardware.consumerir";
        public static string FEATURE_DEVICE_ADMIN = "android.software.device_admin";
        public static string FEATURE_EMBEDDED = "android.hardware.type.embedded";
        public static string FEATURE_ETHERNET = "android.hardware.ethernet";
        public static string FEATURE_FAKETOUCH = "android.hardware.faketouch";
        public static string FEATURE_FAKETOUCH_MULTITOUCH_DISTINCT = "android.hardware.faketouch.multitouch.distinct";
        public static string FEATURE_FAKETOUCH_MULTITOUCH_JAZZHAND = "android.hardware.faketouch.multitouch.jazzhand";
        public static string FEATURE_FINGERPRINT = "android.hardware.fingerprint";
        public static string FEATURE_FREEFORM_WINDOW_MANAGEMENT = "android.software.freeform_window_management";
        public static string FEATURE_GAMEPAD = "android.hardware.gamepad";
        public static string FEATURE_HIFI_SENSORS = "android.hardware.sensor.hifi_sensors";
        public static string FEATURE_HOME_SCREEN = "android.software.home_screen";
        public static string FEATURE_INPUT_METHODS = "android.software.input_methods";
        public static string FEATURE_LEANBACK = "android.software.leanback";
        public static string FEATURE_LEANBACK_ONLY = "android.software.leanback_only";
        public static string FEATURE_LIVE_TV = "android.software.live_tv";
        public static string FEATURE_LIVE_WALLPAPER = "android.software.live_wallpaper";
        public static string FEATURE_LOCATION = "android.hardware.location";
        public static string FEATURE_LOCATION_GPS = "android.hardware.location.gps";
        public static string FEATURE_LOCATION_NETWORK = "android.hardware.location.network";
        public static string FEATURE_MANAGED_USERS = "android.software.managed_users";
        public static string FEATURE_MICROPHONE = "android.hardware.microphone";
        public static string FEATURE_MIDI = "android.software.midi";
        public static string FEATURE_NFC = "android.hardware.nfc";
        public static string FEATURE_NFC_HOST_CARD_EMULATION = "android.hardware.nfc.hce";
        public static string FEATURE_NFC_HOST_CARD_EMULATION_NFCF = "android.hardware.nfc.hcef";
        public static string FEATURE_OPENGLES_EXTENSION_PACK = "android.hardware.opengles.aep";
        public static string FEATURE_PC = "android.hardware.type.pc";
        public static string FEATURE_PICTURE_IN_PICTURE = "android.software.picture_in_picture";
        public static string FEATURE_PRINTING = "android.software.print";
        public static string FEATURE_RAM_LOW = "android.hardware.ram.low";
        public static string FEATURE_RAM_NORMAL = "android.hardware.ram.normal";
        public static string FEATURE_SCREEN_LANDSCAPE = "android.hardware.screen.landscape";
        public static string FEATURE_SCREEN_PORTRAIT = "android.hardware.screen.portrait";
        public static string FEATURE_SECURELY_REMOVES_USERS = "android.software.securely_removes_users";
        public static string FEATURE_SENSOR_ACCELEROMETER = "android.hardware.sensor.accelerometer";
        public static string FEATURE_SENSOR_AMBIENT_TEMPERATURE = "android.hardware.sensor.ambient_temperature";
        public static string FEATURE_SENSOR_BAROMETER = "android.hardware.sensor.barometer";
        public static string FEATURE_SENSOR_COMPASS = "android.hardware.sensor.compass";
        public static string FEATURE_SENSOR_GYROSCOPE = "android.hardware.sensor.gyroscope";
        public static string FEATURE_SENSOR_HEART_RATE = "android.hardware.sensor.heartrate";
        public static string FEATURE_SENSOR_HEART_RATE_ECG = "android.hardware.sensor.heartrate.ecg";
        public static string FEATURE_SENSOR_LIGHT = "android.hardware.sensor.light";
        public static string FEATURE_SENSOR_PROXIMITY = "android.hardware.sensor.proximity";
        public static string FEATURE_SENSOR_RELATIVE_HUMIDITY = "android.hardware.sensor.relative_humidity";
        public static string FEATURE_SENSOR_STEP_COUNTER = "android.hardware.sensor.stepcounter";
        public static string FEATURE_SENSOR_STEP_DETECTOR = "android.hardware.sensor.stepdetector";
        public static string FEATURE_SIP = "android.software.sip";
        public static string FEATURE_SIP_VOIP = "android.software.sip.voip";
        public static string FEATURE_STRONGBOX_KEYSTORE = "android.hardware.strongbox_keystore";
        public static string FEATURE_TELEPHONY = "android.hardware.telephony";
        public static string FEATURE_TELEPHONY_CDMA = "android.hardware.telephony.cdma";
        public static string FEATURE_TELEPHONY_EUICC = "android.hardware.telephony.euicc";
        public static string FEATURE_TELEPHONY_GSM = "android.hardware.telephony.gsm";
        public static string FEATURE_TELEPHONY_MBMS = "android.hardware.telephony.mbms";
  
        public static string FEATURE_TOUCHSCREEN = "android.hardware.touchscreen";
        public static string FEATURE_TOUCHSCREEN_MULTITOUCH = "android.hardware.touchscreen.multitouch";
        public static string FEATURE_TOUCHSCREEN_MULTITOUCH_DISTINCT = "android.hardware.touchscreen.multitouch.distinct";
        public static string FEATURE_TOUCHSCREEN_MULTITOUCH_JAZZHAND = "android.hardware.touchscreen.multitouch.jazzhand";
        public static string FEATURE_USB_ACCESSORY = "android.hardware.usb.accessory";
        public static string FEATURE_USB_HOST = "android.hardware.usb.host";
        public static string FEATURE_VERIFIED_BOOT = "android.software.verified_boot";
        public static string FEATURE_VR_HEADTRACKING = "android.hardware.vr.headtracking";
  
        public static string FEATURE_VR_MODE_HIGH_PERFORMANCE = "android.hardware.vr.high_performance";
        public static string FEATURE_VULKAN_HARDWARE_COMPUTE = "android.hardware.vulkan.compute";
        public static string FEATURE_VULKAN_HARDWARE_LEVEL = "android.hardware.vulkan.level";
        public static string FEATURE_VULKAN_HARDWARE_VERSION = "android.hardware.vulkan.version";
        public static string FEATURE_WATCH = "android.hardware.type.watch";
        public static string FEATURE_WEBVIEW = "android.software.webview";
        public static string FEATURE_WIFI = "android.hardware.wifi";
        public static string FEATURE_WIFI_AWARE = "android.hardware.wifi.aware";
        public static string FEATURE_WIFI_DIRECT = "android.hardware.wifi.direct";
        public static string FEATURE_WIFI_PASSPOINT = "android.hardware.wifi.passpoint";
        public static string FEATURE_WIFI_RTT = "android.hardware.wifi.rtt";
        public static int GET_ACTIVITIES = 1;
        public static int GET_CONFIGURATIONS = 16384;
      
        public static int GET_GIDS = 256;
        public static int GET_INSTRUMENTATION = 16;
        public static int GET_INTENT_FILTERS = 32;
        public static int GET_META_DATA = 128;
        public static int GET_PERMISSIONS = 4096;
        public static int GET_PROVIDERS = 8;
        public static int GET_RECEIVERS = 2;
        public static int GET_RESOLVED_FILTER = 64;
        public static int GET_SERVICES = 4;
        public static int GET_SHARED_LIBRARY_FILES = 1024;
      
        public static int GET_SIGNING_CERTIFICATES = 134217728;
       
        public static int GET_URI_PERMISSION_PATTERNS = 2048;
        public static int INSTALL_REASON_DEVICE_RESTORE = 2;
        public static int INSTALL_REASON_DEVICE_SETUP = 3;
        public static int INSTALL_REASON_POLICY = 1;
        public static int INSTALL_REASON_UNKNOWN = 0;
        public static int INSTALL_REASON_USER = 4;
        public static int MATCH_ALL = 131072;
        public static int MATCH_DEFAULT_ONLY = 65536;
        public static int MATCH_DIRECT_BOOT_AWARE = 524288;
        public static int MATCH_DIRECT_BOOT_UNAWARE = 262144;
        public static int MATCH_DISABLED_COMPONENTS = 512;
        public static int MATCH_DISABLED_UNTIL_USED_COMPONENTS = 32768;
        public static int MATCH_SYSTEM_ONLY = 1048576;
        public static int MATCH_UNINSTALLED_PACKAGES = 8192;
        public static long MAXIMUM_VERIFICATION_TIMEOUT = 3600000L;
        public static int PERMISSION_DENIED = -1;
        public static int PERMISSION_GRANTED = 0;
        public static int SIGNATURE_FIRST_NOT_SIGNED = -1;
        public static int SIGNATURE_MATCH = 0;
        public static int SIGNATURE_NEITHER_SIGNED = 1;
        public static int SIGNATURE_NO_MATCH = -3;
        public static int SIGNATURE_SECOND_NOT_SIGNED = -2;
        public static int SIGNATURE_UNKNOWN_PACKAGE = -4;
        public static int VERIFICATION_ALLOW = 1;
        public static int VERIFICATION_REJECT = -1;
        public static int VERSION_CODE_HIGHEST = -1;

    }
}

