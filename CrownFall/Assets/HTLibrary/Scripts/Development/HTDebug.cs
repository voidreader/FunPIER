using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class HTDebugInfo
	{
		public const string Error_TemplateCastingFailed = "[HTObjectPool] Can't instantiate object casting to GameObject!";
		public const string Error_PureFunctionCall = "[HTFrameworks] Pure function call!";
	}

	public class HTInvokedLog
	{
		public eMessageType _type;
		public string _desc;
	}


	/////////////////////////////////////////
	//---------------------------------------
	public class HTDebug
	{
		//---------------------------------------
#if ENABLE_DEBUG || UNITY_EDITOR
		public static Action<eMessageType, string> onLog = null;

		private static object _lockObject = new object();
		private static List<HTInvokedLog> _invokedLogs = new List<HTInvokedLog>();
#endif //ENABLE_DEBUG || UNITY_EDITOR

		//---------------------------------------
		[System.Diagnostics.Conditional("ENABLE_DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
		static public void UpdateDebug()
		{
#if ENABLE_DEBUG || UNITY_EDITOR
			lock(_lockObject)
			{
				for(int nInd = 0; nInd < _invokedLogs.Count; ++nInd)
					PrintLog(_invokedLogs[nInd]._type, _invokedLogs[nInd]._desc);

				_invokedLogs.Clear();
			}
#endif //ENABLE_DEBUG || UNITY_EDITOR
		}

		//---------------------------------------
		[System.Diagnostics.Conditional("ENABLE_DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
		static public void PrintLog(eMessageType eType, string szString, bool bLogToEditor = true)
		{
#if ENABLE_DEBUG || UNITY_EDITOR
			HTDebugLogCanvas.Logs(eType, szString);
			HTConsoleWindow.PrintConsoleText(eType, string.Format("[Log] {0}", szString));

			//-----
			if (bLogToEditor)
			{
				switch (eType)
				{
					case eMessageType.None:
					case eMessageType.Good:
						UnityEngine.Debug.Log(szString);
						break;

					case eMessageType.Warning:
						UnityEngine.Debug.LogWarning(szString);
						break;

					case eMessageType.Error:
						UnityEngine.Debug.LogError(szString);
						break;
				}
			}

			Utils.SafeInvoke(onLog, eType, szString);
#endif // ENABLE_DEBUG || UNITY_EDITOR
		}

		//---------------------------------------
		[System.Diagnostics.Conditional("ENABLE_DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
		static public void PureFunctionCall()
		{
#if ENABLE_DEBUG || UNITY_EDITOR
			PrintLog(eMessageType.Error, HTDebugInfo.Error_PureFunctionCall);
#endif // ENABLE_DEBUG || UNITY_EDITOR
		}

		//---------------------------------------
		static public void OnLogMessageRecived(string szLog, string szStack, LogType eType)
		{
#if ENABLE_DEBUG || UNITY_EDITOR
			eMessageType eMsgType = eMessageType.None;
			switch(eType)
			{
				case LogType.Warning:
					eMsgType = eMessageType.Warning;
					break;

				case LogType.Exception:
				case LogType.Error:
				case LogType.Assert:
					eMsgType = eMessageType.Error;
					break;
			}

			//-----
			PrintLog(eMsgType, szLog, false);

			if (eMsgType == eMessageType.Error)
			{
				string[] vStack = szStack.Split('\n');
				if (vStack != null && vStack.Length > 0)
				{
					int nStackLine = Mathf.Min(vStack.Length, 3);
					for (int nInd = 0; nInd < nStackLine; ++nInd)
						PrintLog(eMsgType, vStack[nInd], false);
					
					vStack = null;
				}
			}
#endif // ENABLE_DEBUG || UNITY_EDITOR
		}

		//---------------------------------------
		[System.Diagnostics.Conditional("ENABLE_DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
		static public void InvokeDebugLog(eMessageType eType, string szString)
		{
#if ENABLE_DEBUG || UNITY_EDITOR
			lock (_lockObject)
			{
				_invokedLogs.Add(new HTInvokedLog()
				{
					_type = eType,
					_desc = szString,
				});
			}
#endif // ENABLE_DEBUG || UNITY_EDITOR
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}