using UnityEngine;
using System.Runtime.InteropServices;

namespace SmartBeat{

	public class SmartBeatIOSBinding {

#if UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern void smartbeat_init_ (string apikey, bool enable);
		[DllImport("__Internal")]
		private static extern void smartbeat_setExtraData_ (string key, string value);
		[DllImport("__Internal")]
		private static extern void smartbeat_setUserId_ (string userid);
		[DllImport("__Internal")]
		private static extern void smartbeat_leaveBreadcrumb_ (string breadcrumb);
		[DllImport("__Internal")]
		private static extern void smartbeat_enableNSLog_ ();
		[DllImport("__Internal")]
		private static extern void smartbeat_enableDebugLog_ ();
		[DllImport("__Internal")]
		private static extern void smartbeat_logException_ (string stackTrace, string message, string imagePath);
		[DllImport("__Internal")]
		private static extern void smartbeat_enable_ ();
		[DllImport("__Internal")]
		private static extern void smartbeat_disable_ ();
		[DllImport("__Internal")]
		private static extern bool smartbeat_isReadyForDuplicateUserCountPrevention_ ();
		[DllImport("__Internal")]
		private static extern void smartbeat_printlog_ (string log);

		public static void init(string apiKey, bool enable){
			if (Application.platform != RuntimePlatform.OSXEditor) {
				smartbeat_init_(apiKey, enable);
			}
		}

		public static void setExtra(string key, string value){
			if (Application.platform != RuntimePlatform.OSXEditor) {
				smartbeat_setExtraData_(key, value);
			}
		}

		public static void setUserId(string userid){
			if (Application.platform != RuntimePlatform.OSXEditor) {
				smartbeat_setUserId_(userid);
			}
		}

		public static void leaveBreadcrumb(string breadcrumb){
			if (Application.platform != RuntimePlatform.OSXEditor) {
				smartbeat_leaveBreadcrumb_(breadcrumb);
			}
		}

		public static void enableNSLog(){
			if (Application.platform != RuntimePlatform.OSXEditor) {
				smartbeat_enableNSLog_();
			}
		}

		public static void enableDebugLog(){
			if (Application.platform != RuntimePlatform.OSXEditor) {
				smartbeat_enableDebugLog_();
			}
		}
		
		public static void logException(string stackTrace, string message, string imagePath){
			if (Application.platform != RuntimePlatform.OSXEditor) {
				smartbeat_logException_(stackTrace, message, imagePath);
			}
		}

		public static void enable(){
			if (Application.platform != RuntimePlatform.OSXEditor) {
				smartbeat_enable_();
			}
		}

		public static void disable(){
			if (Application.platform != RuntimePlatform.OSXEditor) {
				smartbeat_disable_();
			}
		}

		public static bool isReadyForDuplicateUserCountPrevention(){
			if (Application.platform != RuntimePlatform.OSXEditor) {
				return smartbeat_isReadyForDuplicateUserCountPrevention_();
			} else {
				return false;
			}
		}

		public static void printLog(string log)
		{
			if (Application.platform != RuntimePlatform.OSXEditor) {
				smartbeat_printlog_(log);
			}
		}

#endif
	}
}