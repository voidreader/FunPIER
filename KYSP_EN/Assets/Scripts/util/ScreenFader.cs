using UnityEngine;
using System.Collections;

//namespace GameLib.Util {

	public class ScreenFader : MonoBehaviour {

		#region MonoBehaviour Methods

		void Awake() {
			mFadeTexture = new Texture2D(1, 1);
			mBackgroundStyle = new GUIStyle();
			mBackgroundStyle.normal.background = mFadeTexture;
			mFadeTexture.SetPixel(0, 0, new Color(1, 1, 1, 1));
			mFadeTexture.Apply();
			if (InitialFade) {
				mCurrentColor = FadeColor;
			}
		}

		void Update() {
			if (mFadeRunning) {
				float rate = Mathf.Clamp01(mCurrentDuration / mFadeDuration);
				mCurrentColor = Color.Lerp(mStartColor, mEndColor, rate);
				if (mCurrentDuration >= mFadeDuration) {
					mFadeRunning = false;
				} else {
					mCurrentDuration += Time.deltaTime;
				}
			}
		}

		void OnGUI() {
			if (mCurrentColor.a > 0.0f) {
				GUI.color = mCurrentColor;
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), mFadeTexture);
			}
		}

		#endregion

		#region Methods

		public void Fade(Color startColor, Color endColor, float duration) {
			mStartColor = startColor;
			mEndColor = endColor;
			mFadeRunning = true;
			mFadeDuration = duration;
			mCurrentDuration = 0.0f;
			mCurrentColor = startColor;
		}

		public void FadeOut(float duration) {
			Color startColor = FadeColor;
			startColor.a = 0;
			Fade(startColor, FadeColor, duration);
		}

		public void FadeOut() {
			FadeOut(DefaultFadeDuration);
		}

		public void FadeIn(float duration) {
			Color endColor = FadeColor;
			endColor.a = 0;
			Fade(FadeColor, endColor, duration);
		}

		public void FadeIn() {
			FadeIn(DefaultFadeDuration);
		}

		public void EndFade() {
			mFadeRunning = false;
		}

		public static void SetColorMain(Color color) {
			ScreenFader fader = Main;
			if (fader != null) {
				fader.mCurrentColor = color;
			}
		}	

		protected void SetOverlayColor(Color color) {
			//mCurrentOverlayColor = color;
			mFadeTexture.SetPixel(0, 0, color);
			mFadeTexture.Apply();
		}

		#endregion

		#region Properties

		public bool FadeRunning {
			get { return mFadeRunning; }
		}

		public Color Color {
			get { return mCurrentColor; }
			set { mCurrentColor = value; }
		}

		public static ScreenFader Main {
			get {
				if (mMainFader == null) {
					mMainFader = (ScreenFader)Camera.main.GetComponent("ScreenFader");
				}
				return mMainFader;
			}
		}

		#endregion

		#region Fields

		public float FadeDuration = 0.5f;
		public Color FadeColor = new Color(0, 0, 0, 0);
		public int GuiDepth = -1000;
		public float DefaultFadeDuration = 1.0f;
		public bool InitialFade = true;

		private Texture2D mFadeTexture;
		private GUIStyle mBackgroundStyle;

		private float mFadeDuration;
		private Color mStartColor;
		private Color mEndColor;
		private Color mCurrentColor;
		private Color mOldColor;
		
		[SerializeField] private bool mFadeRunning;
		private float mCurrentDuration;
		private static ScreenFader mMainFader;

		#endregion

	}

//}