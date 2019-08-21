using UnityEngine;

using SmartBeat;

using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace SmartBeat{
	
	public static class SmartBeat{

#if UNITY_IPHONE
		[DllImport("libc")]
		private static extern int sigaction (int sig, IntPtr act, IntPtr oact);
#endif

#if !UNITY_EDITOR
		class SingleInstance{
			private static SingleInstance _self;
			private static object _lock = new object();
			private bool initialized = false;
			private int mImageCount = 0;
			private bool mEnableScreenshot = false;
			private bool mEnableSmartBeat = true;
			private bool mEnableLogRedirect = false;
			private string mRedirectLogTag = "";

#if UNITY_ANDROID
			private AndroidJavaClass mSmartBeatAndroid = null;
#endif

			public static SingleInstance getInstance(){
				if(_self == null){
					lock(_lock){
						if(_self == null){
							_self = new SingleInstance();
						}
					}
				}
				return _self;
			}

			public bool getInitialized(){
				return initialized;
			}

			public void setInitialized(){
				initialized = true;
			}

#if UNITY_ANDROID
			public void setSmartBeatAndroid(AndroidJavaClass clz){
				mSmartBeatAndroid = clz;
			}

			public AndroidJavaClass getSmartBeatAndroid(){
				if(_self == null) return null;
				return mSmartBeatAndroid;
			}
#endif
			public int getImageCount(){
				if(mImageCount >= 100) mImageCount = 0;
				return mImageCount++;
			}

			public void enableScreenshot(bool enable){
				mEnableScreenshot = enable;
			}

			public bool isEnabledScreenshot(){
				return mEnableScreenshot;
			}

			public void enableSmartBeat(bool enable){
				mEnableSmartBeat = enable;
			}

			public bool isEnabled(){
				return mEnableSmartBeat;
			}

			public void enableLogRedirect(bool enable, string tag){
				mEnableLogRedirect = enable;
				mRedirectLogTag = tag;
			}

			public bool isEnabledLogRedirect(){
				return mEnableLogRedirect;
			}

			public string getTagRedirectLog(){
				return mRedirectLogTag;
			}
		}
#endif

#if !UNITY_EDITOR
		private const string SCREENSHOT_FILE_NAME = "/smartbeat_screenshot";
		private const string SCREENSHOT_FILE_EXT = ".png";
		private const string UNITY_PLAYER = "com.unity3d.player.UnityPlayer";
		private const double SKIP_LOGGING_MEANTIME = 2;//2sec.
		private static System.DateTime mLatestTime;
		private static object mTimeLock = new object();

#endif
		//keep this api for BC
		public static void init(string appKey){
#if !UNITY_EDITOR
			init (appKey, true);
#endif
		}

		//keep this api for BC
		public static void init(string appKey, bool enable){
#if !UNITY_EDITOR
			init (appKey, true, false);
#endif
		}

		public static void init(string appKey, bool enable, bool enableSIGSEGVDetection){
#if !UNITY_EDITOR
			SingleInstance s = SingleInstance.getInstance();
			if(s.getInitialized()) return;
			lock(s){
				if(s.getInitialized() == false){
					s.setInitialized();
#if UNITY_2_6 || UNITY_2_6_1 || UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1
					//not supported
#elif UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
					Application.RegisterLogCallback(HandleLogPrivate);
#else
					Application.logMessageReceivedThreaded += HandleLogPrivate;
#endif

#if UNITY_ANDROID
					AndroidJavaClass up = new AndroidJavaClass(UNITY_PLAYER);
					AndroidJavaObject activity = up.GetStatic<AndroidJavaObject>("currentActivity");
					AndroidJavaObject application = activity.Call<AndroidJavaObject>("getApplication");

					AndroidJavaObject objConfig = new AndroidJavaObject("com.smrtbeat.SmartBeatConfig");
					objConfig.Call<AndroidJavaObject>("setApiKey", appKey);
					objConfig.Call<AndroidJavaObject>("setEnabled", enable);
					if(!enableSIGSEGVDetection) {
						objConfig.Call<AndroidJavaObject>("addIgnoredSignal", 11); //disable handling SIGSEGV
					}
					AndroidJavaClass clz = new AndroidJavaClass("com.smrtbeat.SmartBeat");
					SingleInstance.getInstance().setSmartBeatAndroid(clz);
					clz.CallStatic("initAndStartSession", application, objConfig);
					clz.CallStatic("notifyOnResume", activity);
					clz.CallStatic("notifyActivityStarted", activity);
#endif
#if UNITY_IPHONE
					int ptrSize = 16;//sizeOf(sigaction) for 64bit
					if(IntPtr.Size == 4){
						ptrSize = 12;//sizeOf(sigaction) for 32bit
					}
					IntPtr sigsegv = Marshal.AllocHGlobal (ptrSize);
					if(!enableSIGSEGVDetection) {
						sigaction (11 ,IntPtr.Zero, sigsegv);
					}
					SmartBeatIOSBinding.init(appKey, enable);
					if(!enableSIGSEGVDetection) {
						sigaction (11, sigsegv, IntPtr.Zero);
					}
					Marshal.FreeHGlobal (sigsegv);
#endif
				}
			}
			SingleInstance.getInstance().enableSmartBeat(enable);
#endif
		}

		/// <summary>
		/// Leaves the breadcrumb.
		/// </summary>
		/// <param name="breadcrumb">Breadcrumb.</param>
		public static void leaveBreadcrumb(string breadcrumb){
#if !UNITY_EDITOR
#if UNITY_ANDROID
			AndroidJavaClass clz = SingleInstance.getInstance().getSmartBeatAndroid();
			if(clz != null){
				clz.CallStatic("leaveBreadcrumbs", breadcrumb);
			}
#endif
#if UNITY_IPHONE
			SmartBeatIOSBinding.leaveBreadcrumb(breadcrumb);
#endif
#endif
		}

		public static void setUserId(string userId){
#if !UNITY_EDITOR
#if UNITY_ANDROID
			AndroidJavaClass clz = SingleInstance.getInstance().getSmartBeatAndroid();
			if(clz != null){
				clz.CallStatic("setUserId", userId);
			}
#endif
#if UNITY_IPHONE
			SmartBeatIOSBinding.setUserId(userId);
#endif
#endif
		}

		public static void enableLog(){
#if !UNITY_EDITOR
#if UNITY_ANDROID
			AndroidJavaClass clz = SingleInstance.getInstance().getSmartBeatAndroid();
			if(clz != null){
				clz.CallStatic("enableLogCat");
			}
#endif
#if UNITY_IPHONE
			SmartBeatIOSBinding.enableNSLog();
#endif
#endif
		}

		public static void enable(){
#if !UNITY_EDITOR
			SingleInstance.getInstance().enableSmartBeat(true);
#if UNITY_ANDROID
			AndroidJavaClass clz = SingleInstance.getInstance().getSmartBeatAndroid();
			if(clz != null){
				clz.CallStatic("enable");
			}
#elif UNITY_IPHONE
			SmartBeatIOSBinding.enable();
#endif
#endif
		}

		public static void disable(){
#if !UNITY_EDITOR
			SingleInstance.getInstance().enableSmartBeat(false);
#if UNITY_ANDROID
			AndroidJavaClass clz = SingleInstance.getInstance().getSmartBeatAndroid();
			if(clz != null){
				clz.CallStatic("disable");
			}
#elif UNITY_IPHONE
			SmartBeatIOSBinding.disable();
#endif
#endif
		}

		public static void enableLogRedirect(string tag){
#if !UNITY_EDITOR
			SingleInstance.getInstance().enableLogRedirect(true, tag);
#endif
		}

		public static void disableLogRedirect(){
#if !UNITY_EDITOR
			SingleInstance.getInstance().enableLogRedirect(false, "");
#endif
		}

		public static void addExtraData(string key, string value){
#if !UNITY_EDITOR
#if UNITY_ANDROID
			AndroidJavaClass clz = SingleInstance.getInstance().getSmartBeatAndroid();
			if(clz != null){
				clz.CallStatic("addExtraData", key, value);
			}
#endif
#if UNITY_IPHONE
			SmartBeatIOSBinding.setExtra(key, value);
#endif
#endif
		}

		public static void enableScreenshot(){
#if !UNITY_EDITOR
			SingleInstance.getInstance().enableScreenshot(true);
#endif
		}

		public static void disableScreenshot(){
#if !UNITY_EDITOR
			SingleInstance.getInstance().enableScreenshot(false);
#endif
		}

		public static bool isReadyForDuplicateUserCountPrevention(){
			bool result = false;
#if !UNITY_EDITOR
#if UNITY_ANDROID
			AndroidJavaClass clz = SingleInstance.getInstance().getSmartBeatAndroid();
			if(clz != null){
				result = clz.CallStatic<bool>("isReadyForDuplicateUserCountPrevention");
			}
#endif
#if UNITY_IPHONE
			result = SmartBeatIOSBinding.isReadyForDuplicateUserCountPrevention();
#endif
#endif
			return result;
		}

		public static void onPause(bool pauseStatus){
#if !UNITY_EDITOR
#if UNITY_ANDROID
			AndroidJavaClass clz = SingleInstance.getInstance().getSmartBeatAndroid();
			if(clz != null){
				AndroidJavaClass up = new AndroidJavaClass(UNITY_PLAYER);
				AndroidJavaObject activity = up.GetStatic<AndroidJavaObject>("currentActivity");
				if(pauseStatus){
					clz.CallStatic("notifyOnPause", activity);
				}else{
					clz.CallStatic("notifyOnResume", activity);
				}
			}
#endif
#if UNITY_IPHONE
			//nothing to do.
#endif
#endif
		}

		public static void enableDebugLog(string tag){
#if !UNITY_EDITOR
#if UNITY_ANDROID
			AndroidJavaClass clz = SingleInstance.getInstance().getSmartBeatAndroid();
			if(clz != null){
				clz.CallStatic("enableDebugLog", tag);
			}
#endif
#if UNITY_IPHONE
			SmartBeatIOSBinding.enableDebugLog();
#endif
#endif
		}
		static void HandleLogPrivate(string logString, string stackTrace, LogType type) {
			HandleLog(logString, stackTrace, type);
		}

		static IEnumerator doLog(string logString, string stackTrace, LogType type)
		{
#if !UNITY_EDITOR
			string file = "";
			//taking screen shot
			bool enableScreenshot = SingleInstance.getInstance().isEnabledScreenshot();
			if(enableScreenshot){
				file = SCREENSHOT_FILE_NAME + string.Format("_{0:00}", SingleInstance.getInstance().getImageCount()) + SCREENSHOT_FILE_EXT;
#if UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5
				Application.CaptureScreenshot(file);
#elif UNITY_2017_1_OR_NEWER
				ScreenCapture.CaptureScreenshot(file);
#endif
			}
#if UNITY_ANDROID
			AndroidJavaClass clz = SingleInstance.getInstance().getSmartBeatAndroid();
			if(clz != null){
				AndroidJavaClass up = new AndroidJavaClass(UNITY_PLAYER);
				AndroidJavaObject activity = up.GetStatic<AndroidJavaObject>("currentActivity");
				if(enableScreenshot){
					clz.CallStatic("logHandleExceptionForUnity", activity, logString, stackTrace, Application.persistentDataPath + file);
				}else{
					clz.CallStatic("logHandleExceptionForUnity", activity, logString, stackTrace);
				}
			}
#endif
#if UNITY_IPHONE
			if(enableScreenshot){
				SmartBeatIOSBinding.logException(stackTrace, logString, Application.persistentDataPath + file);
			}else{
				SmartBeatIOSBinding.logException(stackTrace, logString, "");
			}
#endif
#endif
			yield return null;
		}

		static IEnumerator doLogRedirect(string logString, LogType type)
		{
#if !UNITY_EDITOR
			string tag = SingleInstance.getInstance().getTagRedirectLog();

			string logmsg = tag + " (";
			switch(type){
			case LogType.Error:
				logmsg = logmsg + "Error";
				break;
			case LogType.Assert:
				logmsg = logmsg + "Assert";
				break;
			case LogType.Warning:
				logmsg = logmsg + "Warning";
				break;
			case LogType.Log:
				logmsg = logmsg + "Log";
				break;
			case LogType.Exception:
				logmsg = logmsg + "Exception";
				break;
			}

			logmsg = logmsg + ") : " + logString;

#if UNITY_ANDROID
			AndroidJavaClass clz = SingleInstance.getInstance().getSmartBeatAndroid();
			if(clz != null){
				clz.CallStatic("log", logmsg);
			}
#endif
#if UNITY_IPHONE
			SmartBeatIOSBinding.printLog(logmsg);
#endif
#endif
			yield return null;
		}

		//return true if it is okay to send log.
		static bool checkAndUpdateLatestLoggingTime() {
#if !UNITY_EDITOR
			lock(mTimeLock){
				System.DateTime now = System.DateTime.Now;
				if(now.Subtract(mLatestTime).TotalSeconds < SKIP_LOGGING_MEANTIME) return false;
				mLatestTime = now;
			}
#endif
			return true;
		}

		public static bool HandleLog(string logString, string stackTrace, LogType type) {
			bool result = false;
#if !UNITY_EDITOR
			//check if logging by SDK is enabled.
			bool enabled = SingleInstance.getInstance().isEnabled();

			//handle only Exception or Error
			if(enabled && (type == LogType.Exception || type == LogType.Error)){
				string _stackTrace = stackTrace;
#if !ENABLE_IL2CPP
				if(_stackTrace.Length <= 0){
					System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(2, true);
					_stackTrace =  trace.ToString();
				}
#endif
				if(!checkAndUpdateLatestLoggingTime()) return false;
				
				SmartBeatBehaviour.Enqueue(doLog(logString, _stackTrace, type));
				result = true;
			}

			if(SingleInstance.getInstance().isEnabledLogRedirect()){
				SmartBeatBehaviour.Enqueue(doLogRedirect(logString, type));
			}
#endif
			return result;
		}
	}
}
