////////////////////////////////////////////////////////////////////////////////
//  
// @module Manifest Manager
// @author Alex Yaremenko (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;

namespace SA.Android.Manifest {

	public enum AMM_ManifestPermission {

		ACCESS_LOCATION_EXTRA_COMMANDS,
		ACCESS_NETWORK_STATE,
		ACCESS_NOTIFICATION_POLICY,
		ACCESS_WIFI_STATE,
		ACCESS_WIMAX_STATE,
		BLUETOOTH,
		BLUETOOTH_ADMIN,
		BROADCAST_STICKY,
		CHANGE_NETWORK_STATE,
		CHANGE_WIFI_MULTICAST_STATE,
		CHANGE_WIFI_STATE,
		CHANGE_WIMAX_STATE,
		DISABLE_KEYGUARD,
		EXPAND_STATUS_BAR,
		FLASHLIGHT,
		GET_PACKAGE_SIZE,
		INTERNET,
		KILL_BACKGROUND_PROCESSES,
		MODIFY_AUDIO_SETTINGS,
		NFC,
		READ_SYNC_SETTINGS,
		READ_SYNC_STATS,
		RECEIVE_BOOT_COMPLETED,
		REORDER_TASKS,
		REQUEST_INSTALL_PACKAGES,
		SET_TIME_ZONE,
		SET_WALLPAPER,
		SET_WALLPAPER_HINTS,
		SUBSCRIBED_FEEDS_READ,
		TRANSMIT_IR,
		USE_FINGERPRINT,
		VIBRATE,
		WAKE_LOCK,
		WRITE_SYNC_SETTINGS,
		SET_ALARM,
		INSTALL_SHORTCUT,
		UNINSTALL_SHORTCUT,
		
		
		
		READ_CALENDAR,
		WRITE_CALENDAR,
		
		CAMERA,
		
		READ_CONTACTS,
		WRITE_CONTACTS,
		GET_ACCOUNTS,
		
		ACCESS_FINE_LOCATION,
		ACCESS_COARSE_LOCATION,
		
		RECORD_AUDIO,
		
		
		READ_PHONE_STATE,
		CALL_PHONE,
		READ_CALL_LOG,
		WRITE_CALL_LOG,
		ADD_VOICEMAIL,
		USE_SIP,
		PROCESS_OUTGOING_CALLS,
		
		BODY_SENSORS,
		
		SEND_SMS,
		READ_SMS,
		RECEIVE_SMS,
		RECEIVE_WAP_PUSH,
		RECEIVE_MMS,
		
		
		
		
		READ_EXTERNAL_STORAGE,
		WRITE_EXTERNAL_STORAGE,

        BILLING,
        CHECK_LICENSE,


        UNDEFINED

	}


	public static class MenifestPermissionMethods {
		
		public static string GetFullName(this AMM_ManifestPermission permission) {

			string prefix = "android.permission.";

			switch(permission) {
    			case AMM_ManifestPermission.SET_ALARM:
    				prefix = "com.android.alarm.permission.";
    				break;

    			case AMM_ManifestPermission.INSTALL_SHORTCUT:
    			case AMM_ManifestPermission.UNINSTALL_SHORTCUT:
    				prefix = "com.android.launcher.permission.";
    				break;

    			case AMM_ManifestPermission.ADD_VOICEMAIL:
    				prefix = "com.android.voicemail.permission.";
    				break;

                case AMM_ManifestPermission.BILLING:
                case AMM_ManifestPermission.CHECK_LICENSE:
                    prefix = "com.android.vending.";
                    break;
			}

			return prefix + permission.ToString();
		}


        public static bool IsNormalPermission(this AMM_ManifestPermission permission) {
			switch(permission) {
			case AMM_ManifestPermission.ACCESS_LOCATION_EXTRA_COMMANDS:
			case AMM_ManifestPermission.ACCESS_NETWORK_STATE:
			case AMM_ManifestPermission.ACCESS_NOTIFICATION_POLICY:
			case AMM_ManifestPermission.ACCESS_WIFI_STATE:
			case AMM_ManifestPermission.ACCESS_WIMAX_STATE:
			case AMM_ManifestPermission.BLUETOOTH:
			case AMM_ManifestPermission.BLUETOOTH_ADMIN:
			case AMM_ManifestPermission.BROADCAST_STICKY:
			case AMM_ManifestPermission.CHANGE_NETWORK_STATE:
			case AMM_ManifestPermission.CHANGE_WIFI_MULTICAST_STATE:
			case AMM_ManifestPermission.CHANGE_WIFI_STATE:
			case AMM_ManifestPermission.CHANGE_WIMAX_STATE:
			case AMM_ManifestPermission.DISABLE_KEYGUARD:
			case AMM_ManifestPermission.EXPAND_STATUS_BAR:
			case AMM_ManifestPermission.FLASHLIGHT:
			case AMM_ManifestPermission.GET_PACKAGE_SIZE:
			case AMM_ManifestPermission.INTERNET:
			case AMM_ManifestPermission.KILL_BACKGROUND_PROCESSES:
			case AMM_ManifestPermission.MODIFY_AUDIO_SETTINGS:
			case AMM_ManifestPermission.NFC:
			case AMM_ManifestPermission.READ_SYNC_SETTINGS:
			case AMM_ManifestPermission.READ_SYNC_STATS:
			case AMM_ManifestPermission.RECEIVE_BOOT_COMPLETED:
			case AMM_ManifestPermission.REORDER_TASKS:
			case AMM_ManifestPermission.REQUEST_INSTALL_PACKAGES:
			case AMM_ManifestPermission.SET_TIME_ZONE:
			case AMM_ManifestPermission.SET_WALLPAPER:
			case AMM_ManifestPermission.SET_WALLPAPER_HINTS:
			case AMM_ManifestPermission.SUBSCRIBED_FEEDS_READ:
			case AMM_ManifestPermission.TRANSMIT_IR:
			case AMM_ManifestPermission.USE_FINGERPRINT:
			case AMM_ManifestPermission.VIBRATE:
			case AMM_ManifestPermission.WAKE_LOCK:
			case AMM_ManifestPermission.WRITE_SYNC_SETTINGS:
			case AMM_ManifestPermission.SET_ALARM:
			case AMM_ManifestPermission.INSTALL_SHORTCUT:
			case AMM_ManifestPermission.UNINSTALL_SHORTCUT:
				return true;
			default:
				return false;
			}
		}



	}

}
