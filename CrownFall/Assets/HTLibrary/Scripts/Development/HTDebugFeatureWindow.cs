using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class HTDebugFeatureOrder
	{
		public string subject = null;
		public string order = null;
	}

	public class HTDebugFeatureContainer
	{
		//---------------------------------------
		static List<HTDebugFeatureOrder> _registedOrders = null;
		public static List<HTDebugFeatureOrder> RegistedOrders { get { return _registedOrders; } }

		//---------------------------------------
		[System.Diagnostics.Conditional("ENABLE_DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
		public static void RegistDebugFeature(string szSubject, string szOrder)
		{
			if (_registedOrders == null)
				_registedOrders = new List<HTDebugFeatureOrder>();

			_registedOrders.Add(new HTDebugFeatureOrder()
			{
				subject = szSubject,
				order = szOrder,
			});
		}

		//---------------------------------------
	}

	/////////////////////////////////////////
	//---------------------------------------
	public class HTDebugFeatureWindow : MonoBehaviour
	{
		//---------------------------------------
		static HTDebugFeatureWindow _instance = null;
		public static HTDebugFeatureWindow Instance
		{
			get
			{
#if ENABLE_DEBUG || UNITY_EDITOR
				return _instance;
#else // ENABLE_DEBUG || UNITY_EDITOR
				return null;
#endif // ENABLE_DEBUG || UNITY_EDITOR
			}
		}

		//---------------------------------------
		[Header("BASE ELEMENTS")]
		[SerializeField]
		private Image _mask = null;
		private RectTransform _maskRecttransform = null;
		[SerializeField]
		private RectTransform _contentRoot = null;
		[SerializeField]
		private HTDebugFeatureButton _contentInstance = null;

		[Header("EVENTER")]
		[SerializeField]
		private Button _closeButton = null;

		//---------------------------------------
		private LocalObjectPool<HTDebugFeatureButton> _objectPool = null;

		private static bool _isOpened = false;
		public static bool IsOpened { get { return _isOpened; } }

		//---------------------------------------
		private void Awake()
		{
#if ENABLE_DEBUG || UNITY_EDITOR
			_objectPool = new LocalObjectPool<HTDebugFeatureButton>();
			_objectPool.Init(_contentInstance, _contentRoot);

			_closeButton.onClick.AddListener(CloseWindow);

			_instance = this;
			DontDestroyOnLoad(_instance);

			OnEnable();
#else // ENABLE_DEBUG || UNITY_EDITOR
			_instance = null;
			Destroy(gameObject);
#endif // ENABLE_DEBUG || UNITY_EDITOR
		}

		void OnEnable()
		{
			if (_maskRecttransform == null)
				_maskRecttransform = _mask.GetComponent<RectTransform>();

			_maskRecttransform.sizeDelta = Vector2.zero;

			//-----
			_objectPool.ChangeObjectCount(HTDebugFeatureContainer.RegistedOrders.Count);
			for(int nInd = 0; nInd < HTDebugFeatureContainer.RegistedOrders.Count; ++nInd)
			{
				_objectPool.ObjectPoolList[nInd].Init(HTDebugFeatureContainer.RegistedOrders[nInd]);
			}

			//-----
			gameObject.transform.SetAsLastSibling();
		}

		//---------------------------------------
		[System.Diagnostics.Conditional("ENABLE_DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
		public static void OpenWindow()
		{
			if (_instance == null)
			{
				string szPrefabAddr = GEnv.HTPrefabFolder + GEnv.Prefab_DebugFeatureWindow;
				GameObject pObject = Utils.Instantiate(szPrefabAddr);
				if (pObject != null)
					_instance = pObject.GetComponent<HTDebugFeatureWindow>();
			}

			if (_instance != null)
			{
				_isOpened = true;
				Utils.SafeActive(_instance.gameObject, true);
			}
		}

		public static void CloseWindow()
		{
			if (_instance != null)
			{
				_isOpened = false;
				Utils.SafeActive(_instance.gameObject, false);
			}
		}

		//---------------------------------------
	}

	/////////////////////////////////////////
	//---------------------------------------
}