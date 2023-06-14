using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class HTDebugger : MonoBehaviour
	{
		//---------------------------------------
		[SerializeField]
		private GameObject _debugger_Network = null;
		[SerializeField]
		private GameObject _debugger_DebugLogView = null;

		/////////////////////////////////////////
		//---------------------------------------
		private void Awake()
		{
#if ENABLE_DEBUG || UNITY_EDITOR
			InitializeDebugger();
#endif // ENABLE_DEBUG || UNITY_EDITOR
		}


		//---------------------------------------
		[System.Diagnostics.Conditional("ENABLE_DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
		private void InitializeDebugger()
		{
			HTConsoleOrderContainer.RegistConsoleOrder("HUD_Enable", "Show debug window (0 ~ 1)", OnHUDEnable);
			HTConsoleOrderContainer.RegistConsoleOrder("HUD_Network", "Show network debug window (0 ~ 1)", OnHUDRender);
			HTConsoleOrderContainer.RegistConsoleOrder("HUD_LogViewer", "Show log viewer window (0 ~ 1)", OnDebugLogView);
			
			HTConsoleOrderContainer.RegistConsoleOrder("EFFECT_LineTail_Debug", "Show linetail effect debug (0 ~ 1)", OnEffectLineTailDebug);
		}


		/////////////////////////////////////////
		//---------------------------------------
		private void OnHUDRender(int value)
		{
			_debugger_Network.SetActive((value != 0)? true : false);
		}

		private void OnDebugLogView(int value)
		{
			_debugger_DebugLogView.SetActive((value != 0) ? true : false);
		}

		private void OnHUDEnable(int value)
		{
			gameObject.SetActive((value != 0) ? true : false);
		}


		/////////////////////////////////////////
		//---------------------------------------
		private void OnEffectLineTailDebug(int value)
		{
#if ENABLE_DEBUG || UNITY_EDITOR
			HTLineTailEffect.DebugEnabled = (value != 0) ? true : false;
#endif // ENABLE_DEBUG || UNITY_EDITOR
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}
