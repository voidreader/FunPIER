using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using SmartBeat;


namespace SmartBeat
{

	public class SmartBeatBehaviour : MonoBehaviour {
		private static SmartBeatBehaviour mInstance = null;
		private static readonly Queue<Action> mExecutionQueue = new Queue<Action>();
		private Thread mMainThread = null;

		void Awake(){
			//Load from existing asset
			SmartBeatPreferences pref = Resources.Load("SmartBeat") as SmartBeatPreferences;
			if (pref != null) {
#if UNITY_IPHONE
				InitIOS(pref);
#endif
#if UNITY_ANDROID
				InitAndroid(pref);
#endif
				mInstance = this;
				mMainThread = Thread.CurrentThread;
				// Never destroy for handling ApplicationPause event.
				DontDestroyOnLoad(gameObject);
				
			}
		}

		private void InitIOS(SmartBeatPreferences pref) {
			SmartBeat.init (pref.iosApiKey, true, pref.iosSIGSEGVDetection);
			if (pref.iosScreenshot)
				SmartBeat.enableScreenshot ();
			if (pref.iosLog)
				SmartBeat.enableLog ();
			if (pref.iosDebugLog)
				SmartBeat.enableDebugLog ("Tag");
			if (pref.iosLogRedirect != null && pref.iosLogRedirect.Length > 0)
				SmartBeat.enableLogRedirect (pref.iosLogRedirect);
		}

		private void InitAndroid(SmartBeatPreferences pref) {
			SmartBeat.init (pref.androidApiKey, true, pref.androidSIGSEGVDetection);
			if (pref.androidScreenshot)
				SmartBeat.enableScreenshot ();
			if (pref.androidLog)
				SmartBeat.enableLog ();
			if (pref.androidDebugLog != null && pref.androidDebugLog.Length > 0)
				SmartBeat.enableDebugLog (pref.androidDebugLog);
			if (pref.androidLogRedirect != null && pref.androidLogRedirect.Length > 0)
				SmartBeat.enableLogRedirect (pref.androidLogRedirect);
		}

		void OnApplicationPause(bool pauseStatus){
			SmartBeat.onPause(pauseStatus);
		}

		public void Update() {
			lock(mExecutionQueue) {
				while(mExecutionQueue.Count > 0) {
					mExecutionQueue.Dequeue().Invoke();
				}
			}
		}

		public static void Enqueue(IEnumerator action) {
			if (mInstance != null) {
				mInstance.DoEnqueue (action);
			}
		}

		private void DoEnqueue(IEnumerator action) {
			if (Thread.CurrentThread == mMainThread) {
				StartCoroutine (action);
			} else {
				lock(mExecutionQueue) {
					mExecutionQueue.Enqueue( () => {
						StartCoroutine(action);
					});
				}
			}
		}
	}

}