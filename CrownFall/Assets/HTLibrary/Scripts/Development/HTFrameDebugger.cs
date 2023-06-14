using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class HTFrameDebugger : MonoBehaviour
	{
		//---------------------------------------
		[Header("CONSOLE")]
		[SerializeField]
		private Button _consoleButton = null;
		[SerializeField]
		private Button _debugFeatureButtons = null;

		[Header("FRAME PER SEC")]
		[SerializeField]
		private Text _framePerSec = null;
		
		[Header("FRAME GRAPH")]
		[SerializeField]
		private Image _frameDrawGraph = null;
		[SerializeField]
		private Color _frameDrawGraph_Color = Color.white;

		//---------------------------------------
		private int _lastDeltaTimeLogedIndex = 0;
		private const int FrameAverageSampleCount = 30;
		private float[] _deltaTimes = new float[FrameAverageSampleCount];

		private const string FramePerSecFormat = "FPS : {0:F2} [{1:F2} ... {2:F2}]";

		//---------------------------------------
		private Texture2D _frameDrawTex = null;
		private Sprite _frameDrawSprite = null;

		private const int _frameDraw_AverageCount = 5;
		private float[] _frameDraw_Average = new float[_frameDraw_AverageCount];

		//---------------------------------------
		private void Awake()
		{
			_consoleButton.onClick.AddListener(OnClickConsoleButton);
			_debugFeatureButtons.onClick.AddListener(OnClickDebugFeature);

			//-----
			int nWidth = (int)_frameDrawGraph.rectTransform.sizeDelta.x;
			int nHeight = (int)_frameDrawGraph.rectTransform.sizeDelta.y;
			_frameDrawTex = new Texture2D(nWidth, nHeight, TextureFormat.ARGB32, false);
			_frameDrawTex.filterMode = FilterMode.Point;
			_frameDrawTex.SetPixels(new Color[nWidth * nHeight]);

			Rect pSpriteRect = new Rect(0.0f, 0.0f, nWidth, nHeight);
			_frameDrawSprite = Sprite.Create(_frameDrawTex, pSpriteRect, new Vector2(0.5f, 0.5f));
			_frameDrawGraph.sprite = _frameDrawSprite;
			_frameDrawGraph.color = Color.white;

			//-----
			_frameDraw_Average.Memset(0.0f);
		}

		private void Update()
		{
			float fDeltaTime = TimeUtils.RealTime;

			//-----
			_deltaTimes[_lastDeltaTimeLogedIndex++] = fDeltaTime;
			if (_lastDeltaTimeLogedIndex >= FrameAverageSampleCount)
				_lastDeltaTimeLogedIndex = 0;

			int nSampleCount = 0;
			float fTotalDeltas = 0.0f;
			float fMinDelta = float.MaxValue;
			float fMaxDelta = float.MinValue;
			for (int nInd = 0; nInd < _deltaTimes.Length; ++nInd)
			{
				if (_deltaTimes[nInd] <= 0.0f)
					continue;

				++nSampleCount;

				float fCurDelta = _deltaTimes[nInd];
				fTotalDeltas += fCurDelta;
				fMinDelta = Mathf.Min(fMinDelta, fCurDelta);
				fMaxDelta = Mathf.Max(fMaxDelta, fCurDelta);
			}

			fTotalDeltas = 1.0f / (fTotalDeltas / nSampleCount);
			fMinDelta = 1.0f / fMinDelta;
			fMaxDelta = 1.0f / fMaxDelta;
			_framePerSec.text = string.Format(FramePerSecFormat, fTotalDeltas, fMaxDelta, fMinDelta);

			//-----
			int nCurIndex = 0;
			for (nCurIndex = 0; nCurIndex < _frameDraw_Average.Length; ++nCurIndex)
				if (_frameDraw_Average[nCurIndex] <= 0.0f)
					break;

			if (nCurIndex >= _frameDraw_Average.Length)
			{
				float fAverage = 0.0f;
				for (int nInd = 0; nInd < _frameDraw_Average.Length; ++nInd)
					fAverage += _frameDraw_Average[nInd];

				fAverage /= _frameDraw_Average.Length;
				fAverage = Mathf.Clamp(fAverage, 0.0f, 60.0f);

				//-----
				Color[] vSwapPixel = _frameDrawTex.GetPixels(1, 0, _frameDrawTex.width - 1, _frameDrawTex.height);
				_frameDrawTex.SetPixels(0, 0, _frameDrawTex.width - 1, _frameDrawTex.height, vSwapPixel);

				Color[] vFramePixel = _frameDrawTex.GetPixels(_frameDrawTex.width - 1, 0, 1, _frameDrawTex.height);
				vFramePixel.Memset(new Color());

				int nGraphHeight = (int)(_frameDrawTex.height * (fAverage / 60.0f));
				for (int nInd = 0; nInd < nGraphHeight; ++nInd)
					vFramePixel[nInd] = _frameDrawGraph_Color;

				_frameDrawTex.SetPixels(_frameDrawTex.width - 1, 0, 1, _frameDrawTex.height, vFramePixel);
				_frameDrawTex.Apply();

				//-----
				nCurIndex = 0;
				_frameDraw_Average.Memset(0.0f);
			}

			_frameDraw_Average[nCurIndex] = 1.0f / fDeltaTime;
		}

		//---------------------------------------
		private void OnClickConsoleButton()
		{
			HTConsoleWindow.OpenWindow();
		}

		private void OnClickDebugFeature()
		{
			HTDebugFeatureWindow.OpenWindow();
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}