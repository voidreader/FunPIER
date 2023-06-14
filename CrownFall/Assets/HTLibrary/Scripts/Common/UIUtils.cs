using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class UIUtils
	{
		//---------------------------------------
		static public void SafeAddClickEvent(Button pButton, UnityEngine.Events.UnityAction pAction)
		{
			if (pButton == null || pAction == null)
				return;

			pButton.onClick.AddListener(pAction);
		}

		static public void SafeAddClickEvent(Toggle pToggle, UnityEngine.Events.UnityAction<bool> pAction)
		{
			if (pToggle == null || pAction == null)
				return;

			pToggle.onValueChanged.AddListener(pAction);
		}

		static public void SafeSetInteractable(Button pButton, bool bInteractable)
		{
			if (pButton == null)
				return;

			pButton.interactable = bInteractable;
		}
		
		static public void SafeSetInteractable(Toggle pToggle, bool bInteractable)
		{
			if (pToggle == null)
				return;

			pToggle.interactable = bInteractable;
		}

		//---------------------------------------
		static public void SetPreferredSize(Text pText, bool bWidth, bool bHeight)
		{
			if (pText == null || (bWidth == false && bHeight == false))
				return;

			Vector3 vOrigSize = pText.rectTransform.sizeDelta;
			float fWidth = (bWidth) ? pText.preferredWidth + 5.0f : vOrigSize.x;
			float fHeight = (bHeight) ? pText.preferredHeight : vOrigSize.y;
			pText.rectTransform.sizeDelta = new Vector2(fWidth, fHeight);
		}

		//---------------------------------------
		/// <summary>
		/// UI 기준에서 작동되는 Particle System을 생성합니다.
		/// 실제 UI에 귀속되는 것이 아닌 Camer의 앞에 위치합니다.
		/// TODO : 아직 검증되지 않았습니다.
		/// </summary>
		/// <param name="pInstance">Particle System의 Instance</param>
		/// <param name="vScreenPos">화면 위치</param>
		static public ParticleSystem CreateUIParticle(ParticleSystem pInstance, Vector3 vScreenPos)
		{
			ParticleSystem pParticle = pInstance.CreateInstance();

			Camera pCurCamera = Camera.current;
			vScreenPos = pCurCamera.ScreenToWorldPoint(vScreenPos);

			pParticle.transform.position = vScreenPos;
			pParticle.transform.SetParent(pCurCamera.transform);

			return pParticle;
		}

		/// <summary>
		/// UI 기준에서 작동되는 Particle System을 Object Pool을 사용해 생성합니다.
		/// 실제 UI에 귀속되는 것이 아닌 Camer의 앞에 위치합니다.
		/// TODO : 아직 검증되지 않았습니다.
		/// </summary>
		/// <param name="pInstance">Particle System의 Instance</param>
		/// <param name="vScreenPos">화면 위치</param>
		static public ParticleSystem CreateUIParticleFromPool(ParticleSystem pInstance, Vector3 vScreenPos)
		{
			ParticleSystem pParticle = pInstance.CreateInstance();

			Camera pCurCamera = Camera.current;
			vScreenPos = pCurCamera.ScreenToWorldPoint(vScreenPos);

			pParticle.transform.position = vScreenPos;
			pParticle.transform.SetParent(pCurCamera.transform);

			return pParticle;
		}

		//---------------------------------------
		/// <summary>
		/// 3D Text Mesh의 Font를 변경합니다.
		/// </summary>
		static public void ChangeTextmeshFont(TextMesh pMesh, Font pFont, Renderer pRenderer = null)
		{
			pMesh.font = pFont;

			if (pRenderer == null)
				pRenderer = pMesh.GetComponent<Renderer>();

			pRenderer.material = pFont.material;
		}

		static public void SafeTextSet(Text pText, string szString)
		{
			if (pText == null)
				return;

			pText.text = szString;
		}

		//---------------------------------------
		static public void SafeSetCanvasAlpha(CanvasGroup pCanvas, float fAlpha)
		{
			if (pCanvas == null)
				return;

			pCanvas.alpha = fAlpha;
		}

		//---------------------------------------
		static public bool RectangleContainsScreenPoint(RectTransform pRectTransform, Vector2 vScreenPos)
		{
			if (ParentIsWorldCanvas(pRectTransform))
				return RectTransformUtility.RectangleContainsScreenPoint(pRectTransform, vScreenPos, HTFramework.LastCamera);

			else
				return RectTransformUtility.RectangleContainsScreenPoint(pRectTransform, vScreenPos);
		}

		static public bool ParentIsWorldCanvas(RectTransform pRectTransform)
		{
			if (pRectTransform == null)
				return false;

			Canvas pParentCanvas = null;
			Transform pParentTransform = pRectTransform;
			while (true)
			{
				pParentTransform = pParentTransform.parent;
				if (pParentTransform == null)
					break;

				pParentCanvas = pParentTransform.GetComponent<Canvas>();
				if (pParentCanvas != null)
					break;
			}

			if (pParentCanvas != null && pParentCanvas.renderMode == RenderMode.WorldSpace)
				return true;

			return false;
		}

		static public Canvas GetRootCanvas(RectTransform pRectTransform)
		{
			if (pRectTransform == null)
				return null;

			Canvas pParentCanvas = null;
			RectTransform pParentTransform = pRectTransform;
			while (true)
			{
				pParentTransform = pParentTransform.parent as RectTransform;
				if (pParentTransform == null)
					break;

				pParentCanvas = pParentTransform.GetComponent<Canvas>();
				if (pParentCanvas != null)
					break;

				pParentCanvas = GetRootCanvas(pParentTransform);
			}

			return pParentCanvas;
		}

		//---------------------------------------
		static public Vector2 ClampScreenPosByRectTransform(RectTransform pRectTransform, Vector2 vScreenPos)
		{
			//Vector2 vWorldOffset = GetRectTransformWorldPosition(pRectTransform).xy();
			//Vector2 vScreenCenter = GEnv.Get()._ui_ScaleWithSizeResolution * 0.5f;

			Vector2 vHalfSize = pRectTransform.rect.size;
			vScreenPos = vScreenPos.Clamp(Vector2.zero, vHalfSize);// + (vWorldOffset - vScreenCenter);
			return vScreenPos;
		}

		static public Vector3 GetRectTransformWorldPosition(RectTransform pRectTransform)
		{
			Vector3 vWorldPos = pRectTransform.position;
			Vector3 vScaled = HTGlobalUI.Instance.CanvasScaled;
			vWorldPos.x /= vScaled.x;
			vWorldPos.y /= vScaled.y;
			vWorldPos.z /= vScaled.z;
			return vWorldPos;
		}



		/////////////////////////////////////////
		//---------------------------------------
		private class KoreanJosa
		{
			private class KoreanJosaPair
			{
				public KoreanJosaPair(string szJosaR, string szJosaL)
				{
					_josaR = szJosaR;
					_josaL = szJosaL;
				}

				private string _josaR = null;
				public string JosaR { get { return _josaR; } }

				private string _josaL = null;
				public string JosaL { get { return _josaL; } }
			}

			//---------------------------------------
			private static bool _initialized = false;
			private static Regex _josaRegex = new Regex(@"\(이\)가|\(와\)과|\(을\)를|\(은\)는|\(아\)야|\(이\)여|\(으\)로|\(이\)라");
			private static Dictionary<string, KoreanJosaPair> _josaPatternPaird = new Dictionary<string, KoreanJosaPair>();

			//---------------------------------------
			private static void Initialize()
			{
				if (_initialized)
					return;

				_initialized = true;

				//-----
				_josaPatternPaird.Add("(이)가", new KoreanJosaPair("이", "가"));
				_josaPatternPaird.Add("(와)과", new KoreanJosaPair("과", "와"));
				_josaPatternPaird.Add("(을)를", new KoreanJosaPair("을", "를"));
				_josaPatternPaird.Add("(은)는", new KoreanJosaPair("은", "는"));
				_josaPatternPaird.Add("(아)야", new KoreanJosaPair("아", "야"));
				_josaPatternPaird.Add("(이)여", new KoreanJosaPair("이여", "여"));
				_josaPatternPaird.Add("(으)로", new KoreanJosaPair("으로", "로"));
				_josaPatternPaird.Add("(이)라", new KoreanJosaPair("이라", "라"));
			}
			
			public static string Replace(string src)
			{
				Initialize();

				//-----
				StringBuilder pStrBuilder = new StringBuilder(src.Length);
				MatchCollection pJosaMatches = _josaRegex.Matches(src);
				int nLastHeadIndex = 0;

				//-----
				foreach (Match pJosaMatch in pJosaMatches)
				{
					KoreanJosaPair pJosaPair = _josaPatternPaird[pJosaMatch.Value];

					//-----
					pStrBuilder.Append(src, nLastHeadIndex, pJosaMatch.Index - nLastHeadIndex);
					if (pJosaMatch.Index > 0)
					{
						var prevChar = src[pJosaMatch.Index - 1];
						if ((HasJong(prevChar) && pJosaMatch.Value != "(으)로") ||
							(HasJongExceptRieul(prevChar) && pJosaMatch.Value == "(으)로"))
						{
							pStrBuilder.Append(pJosaPair.JosaR);
						}
						else
							pStrBuilder.Append(pJosaPair.JosaL);
					}
					else
						pStrBuilder.Append(pJosaPair.JosaR);

					//-----
					nLastHeadIndex = pJosaMatch.Index + pJosaMatch.Length;
				}

				//-----
				pStrBuilder.Append(src, nLastHeadIndex, src.Length - nLastHeadIndex);
				return pStrBuilder.ToString();
			}

			private static bool HasJong(char inChar)
			{
				if (inChar >= 0xAC00 && inChar <= 0xD7A3) // 가 ~ 힣
				{
					int localCode = inChar - 0xAC00; // 가~ 이후 로컬 코드 
					int jongCode = localCode % 28;
					if (jongCode > 0)
						return true;
				}

				//-----
				return false;
			}

			private static bool HasJongExceptRieul(char inChar)
			{
				if (inChar >= 0xAC00 && inChar <= 0xD7A3)
				{
					int localCode = inChar - 0xAC00;
					int jongCode = localCode % 28;
					if (jongCode == 8 || jongCode == 0)
						return false;
				}

				//-----
				return false;
			}
		}

		//---------------------------------------
		public static string ReplaceKoreanJosa(string szSrc)
		{
			return KoreanJosa.Replace(szSrc);
		}
		
		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}
