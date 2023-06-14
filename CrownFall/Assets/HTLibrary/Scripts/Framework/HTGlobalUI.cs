using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	/// <summary>
	/// UIPopup, Fade Mask, Indicator 등을 관리하는 Behaviour입니다.
	/// HTFramework에 의해 자동 생성됩니다.
	/// </summary>
	public class HTGlobalUI : MonoBehaviour
	{
		[Header("DEVELOPMENT")]
		[SerializeField]
		private GameObject _developmentRoot = null;

		[Header("GLOBAL MASK")]
		[SerializeField]
		private CanvasGroup _globalMask = null;
		[SerializeField]
		private Image _maskComponent = null;

		[Header("INDICATOR")]
		[SerializeField]
		private CanvasGroup _indicator = null;
		[SerializeField]
		private Image _indicator_RayCastPreventer = null;

		[Header("KEY DOWN WAITER")]
		[SerializeField]
		private RectTransform _keyDownWaiter = null;
		[SerializeField]
		private Text _keyDownWaiter_Desc = null;

		//---------------------------------------
		private Coroutine _proc_MaskFade = null;
		private Coroutine _proc_Indicator = null;
		private Coroutine _proc_AnyKeyDown = null;

		//---------------------------------------
		private Canvas _mainCanvas = null;
		public Canvas MainCanvas
		{
			get
			{
				if (_mainCanvas == null)
					_mainCanvas = GetComponent<Canvas>();

				return _mainCanvas;
			}
		}
		
		public Vector3 CanvasScaled { get { return MainCanvas.transform.localScale; } }

		//---------------------------------------
		private static HTGlobalUI _instanced = null;
		public static HTGlobalUI Instance
		{
			get
			{
				if (_instanced == null)
				{
					string szPrefabAddr = GEnv.HTPrefabFolder + GEnv.Prefab_GlobalUI;
					GameObject pObject = Utils.InstantiateFromPool(szPrefabAddr);
					if (pObject != null)
						_instanced = pObject.GetComponent<HTGlobalUI>();
				}
				return _instanced;
			}
		}

		//---------------------------------------
		void Awake()
		{
			_globalMask.alpha = 0.0f;
			Utils.SafeActive(_keyDownWaiter, false);

			//-----
			CanvasScaler pScale = MainCanvas.GetComponent<CanvasScaler>();
			pScale.referenceResolution = GEnv.Get()._ui_ScaleWithSizeResolution;

			//-----
#if ENABLE_DEBUG || UNITY_EDITOR
			Utils.SafeActive(_developmentRoot, true);
#else // ENABLE_DEBUG || UNITY_EDITOR
			Utils.SafeActive(_developmentRoot, false);
#endif // ENABLE_DEBUG || UNITY_EDITOR
		}

		void Update()
		{
		}

		/////////////////////////////////////////
		//---------------------------------------
		/// <summary>
		/// 화면을 Fade 합니다.
		/// </summary>
		/// <param name="bFadeIn">Fade In / Out을 결정합니다</param>
		/// <param name="fTime">Fade 되는 시간</param>
		public void MaskFade(bool bFadeIn, float fTime = GEnv.GLOBALUI_DEFAULT_FADETIME)
		{
			Utils.SafeStopCorutine(this, ref _proc_MaskFade);
			_proc_MaskFade = StartCoroutine(MaskFade_Internal(bFadeIn, fTime));
		}

		/// <summary>
		/// 강제로 현재 Fade Ratio를 결정합니다.
		/// </summary>
		/// <param name="fRatio"></param>
		public void SetMaskFadeRatio(float fRatio)
		{
			_globalMask.alpha = fRatio;
		}

		IEnumerator MaskFade_Internal(bool bFadeIn, float fTime)
		{
			_maskComponent.raycastTarget = true;

			while (true)
			{
				float fDeltaTime = HT.TimeUtils.RealTime * (1.0f / fTime);
				_globalMask.alpha += (bFadeIn)? -fDeltaTime : fDeltaTime;
				if ((bFadeIn && _globalMask.alpha <= 0.0f) || (bFadeIn == false && _globalMask.alpha >= 1.0f))
					break;

				yield return new WaitForEndOfFrame();
			}
			
			if (bFadeIn)
				_maskComponent.raycastTarget = false;

			_proc_MaskFade = null;
		}

		//---------------------------------------
		/// <summary>
		/// Indicator를 표시합니다.
		/// UI Click을 방지 할 수 있습니다.
		/// </summary>
		/// <param name="bStopGame">Game을 일시 정지 할 지 결정합니다</param>
		public void ShowIndicator(bool bStopGame)
		{
			Utils.SafeStopCorutine(this, ref _proc_Indicator);
			_proc_Indicator = StartCoroutine(Indicator_Internal(true));

			if (bStopGame)
				TimeUtils.SetTimeScale(0.0f, GEnv.TIMELAYER_INDICATOR);
		}

		/// <summary>
		/// Indicator를 숨깁니다.
		/// Indicator에 의해 Game이 멈췄을 경우 자동으로 정지를 해제합니다.
		/// </summary>
		public void HideIndicator(bool bInstantly = false)
		{
			Utils.SafeStopCorutine(this, ref _proc_Indicator);
			if (bInstantly)
			{
				_indicator.alpha = 0.0f;
				_indicator_RayCastPreventer.raycastTarget = false;

				HT.HTUIManager.EnableHardwareButton = true;
			}
			else
				_proc_Indicator = StartCoroutine(Indicator_Internal(false));

			TimeUtils.SetTimeScale(1.0f, GEnv.TIMELAYER_INDICATOR);
		}

		IEnumerator Indicator_Internal(bool bShow)
		{
			_indicator_RayCastPreventer.raycastTarget = true;
			HT.HTUIManager.EnableHardwareButton = (bShow)? false : true;

			while (true)
			{
				float fDelta = HT.TimeUtils.RealTime;

				if (bShow)
					_indicator.alpha = Mathf.Min(_indicator.alpha + (fDelta / GEnv.GLOBALUI_DEFAULT_FADETIME), 1.0f);
				else
					_indicator.alpha = Mathf.Max(_indicator.alpha - (fDelta / GEnv.GLOBALUI_DEFAULT_FADETIME), 0.0f);

				if ((bShow && _indicator.alpha >= 1.0f) || (bShow == false && _indicator.alpha <= 0.0f))
					break;

				yield return new WaitForEndOfFrame();
			}

			if (bShow == false)
				_indicator_RayCastPreventer.raycastTarget = false;

			_proc_Indicator = null;
		}

		
		//---------------------------------------
		public void DoWaitForAnyKey(string szDescription, Action<KeyCode> pOnKeyDown)
		{
			_keyDownWaiter_Desc.text = szDescription;

			Utils.SafeStopCorutine(this, ref _proc_AnyKeyDown);
			_proc_AnyKeyDown = StartCoroutine(DoWaitForAnyKey_Internal(pOnKeyDown));
		}

		private IEnumerator DoWaitForAnyKey_Internal(Action<KeyCode> pOnKeyDown)
		{
			Utils.SafeActive(_keyDownWaiter, true);
			HT.HTUIManager.EnableHardwareButton = false;

			//-----
			while (true)
			{
				KeyCode eDownKey = 0;
				for(KeyCode eCode = 0; eCode < KeyCode.Joystick8Button19; ++eCode)
				{
					if (eCode == 0)
						continue;

					if (HTVirtualInput.GetKeyDown(eCode) || Input.GetKey(eCode))
					{
						if (eCode == KeyCode.Escape)
							break;

						eDownKey = eCode;
						break;
					}
				}

				if (eDownKey != 0)
				{
					Utils.SafeInvoke(pOnKeyDown, eDownKey);
					break;
				}

				yield return new WaitForEndOfFrame();
			}

			//-----
			Utils.SafeActive(_keyDownWaiter, false);
			HT.HTUIManager.EnableHardwareButton = true;
		}

		
		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}
