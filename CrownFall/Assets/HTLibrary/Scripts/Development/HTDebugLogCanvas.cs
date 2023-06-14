using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class HTDebugLogCanvas : MonoBehaviour
	{
		//---------------------------------------
		static public HTDebugLogCanvas _instance = null;

		//---------------------------------------
		[Header("DEFAULT SETTINGS")]
		[SerializeField]
		private Text[] _logs = null;
		[SerializeField]
		private float _defaultLifeTimes = 5.0f;

		[Header("FOLD")]
		[SerializeField]
		private RectTransform _foldRoot = null;
		[SerializeField]
		private Button _foldButton = null;

		[Header("COLOR SETTINGS")]
		[SerializeField]
		private Color _color_Normal = Color.white;
		[SerializeField]
		private Color _color_Good = Color.green;
		[SerializeField]
		private Color _color_Warnning = Color.yellow;
		[SerializeField]
		private Color _color_Error = Color.red;

		//---------------------------------------
		private float[] _logsLifeTime = null;
		private bool _isFold = false;

		//---------------------------------------
		private void Awake()
		{
#if ENABLE_DEBUG || UNITY_EDITOR
			_instance = this;
			//DontDestroyOnLoad(this);

			//-----
			_logsLifeTime = new float[_logs.Length];
			for (int nInd = 0; nInd < _logsLifeTime.Length; ++nInd)
				_logsLifeTime[nInd] = 0.0f;

			//-----
			_foldButton.onClick.AddListener(OnClickFold);

#else // ENABLE_DEBUG || UNITY_EDITOR
			_instance = null;
#endif // ENABLE_DEBUG || UNITY_EDITOR
		}

		private void OnDestroy()
		{
			_instance = null;
		}

		//---------------------------------------
		private void Update()
		{
			for (int nInd = 0; nInd < _logs.Length - 1; ++nInd)
			{
				if (string.IsNullOrEmpty(_logs[nInd].text))
					continue;

				_logsLifeTime[nInd] -= HT.TimeUtils.GameTime;
				if (_logsLifeTime[nInd] <= 0.0f)
					PopElement(nInd);
			}
		}

		//---------------------------------------
		private void OnClickFold()
		{
			_isFold = !_isFold;

			//-----
			bool bShowLogs = true;
			int nTransformHeight = 240;
			if (_isFold)
			{
				bShowLogs = false;
				nTransformHeight = 40;
			}

			//-----
			Vector2 vSize = _foldRoot.sizeDelta;
			vSize.y = nTransformHeight;
			_foldRoot.sizeDelta = vSize;

			for (int nInd = 0; nInd < _logs.Length; ++nInd)
				_logs[nInd].gameObject.SetActive(bShowLogs);
		}

		//---------------------------------------
		private void PopElement(int nStartIndex)
		{
			int nLastIndex = _logs.Length - 1;
			for (int nSwap = nStartIndex; nSwap < nLastIndex; ++nSwap)
			{
				_logs[nSwap].text = _logs[nSwap + 1].text;
				_logs[nSwap].color = _logs[nSwap + 1].color;
				_logsLifeTime[nSwap] = _logsLifeTime[nSwap + 1];

				RefreshTextBoxSize(_logs[nSwap]);
			}

			_logs[nLastIndex].text = string.Empty;
			_logs[nLastIndex].color = Color.white;
			_logsLifeTime[nLastIndex] = 0.0f;

			RefreshTextBoxSize(_logs[nLastIndex]);
		}

		//---------------------------------------
		public void PushLog(eMessageType eType, string szStr)
		{
			if (szStr == _logs[_logs.Length - 1].text)
				return;

			//-----
			int nIndex = -1;
			for (int nInd = _logs.Length - 1; nInd >= 0; --nInd)
			{
				if (nInd < 0)
					break;

				if (string.IsNullOrEmpty(_logs[nInd].text))
					nIndex = nInd;
			}

			if (nIndex > 0 && _logs[nIndex - 1].text == szStr)
				return;

			//-----
			if (nIndex < 0)
			{
				PopElement(0);
				nIndex = _logs.Length - 1;
			}

			//-----
			Color pColor = _color_Normal;
			switch(eType)
			{
				case eMessageType.Good:
					pColor = _color_Good;
					break;

				case eMessageType.Warning:
					pColor = _color_Warnning;
					break;

				case eMessageType.Error:
					pColor = _color_Error;
					break;
			}
			
			_logs[nIndex].color = pColor;
			_logs[nIndex].text = szStr;
			RefreshTextBoxSize(_logs[nIndex]);

			_logsLifeTime[nIndex] = _defaultLifeTimes;
		}

		public static void Logs(eMessageType eType, string szStr)
		{
			if (_instance == null)
				return;

			_instance.PushLog(eType, szStr);
		}

		private void RefreshTextBoxSize(Text pLogText)
		{
			//Vector2 vSizeDelta = pLogText.rectTransform.sizeDelta;
			//vSizeDelta.y = pLogText.preferredHeight;
			//pLogText.rectTransform.sizeDelta = vSizeDelta;
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}