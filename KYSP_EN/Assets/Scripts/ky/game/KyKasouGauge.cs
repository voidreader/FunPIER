using UnityEngine;
using System.Collections;

/// <summary>
/// ���u�������v�p�X�R�A�Q�[�W�X�N���v�g�B
/// </summary>
public class KyKasouGauge : MonoBehaviour {

	void Start () {
		if (transform.parent != null) {
			mEngine = transform.parent.GetComponent<KyGameEngine>();
		}
		GaugeLightSprite.Size.y = 0;
		GaugeLightSprite.UpdateAll();
	}
	
	void Update () {
		if (KyScriptTime.DeltaTime == 0) { return; }
		if (mGoukakuEnabled) {
			mElapsedTime += KyScriptTime.DeltaTime;
			if (mElapsedTime >= 0.3f) {
				mElapsedTime = 0;
				mGaugeRed = !mGaugeRed;
				if (mGaugeRed) {
					GaugeLightSprite.MainColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
				} else {
					GaugeLightSprite.MainColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				}
				GaugeLightSprite.UpdateAll();
			}
		} else if (mTouchEnabled && Input.GetMouseButtonDown(0)) {
			Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (mHitRect.Contains(wp)) {
				OnScoreUp();
				mTouchEnabled = false;
			}
		}
	}

	public void OnScoreUp() {
		if (mScore >= GaugeMax) { return; }
		mScore++;
		GaugeLightSprite.Size.y = GaugeStride * mScore;
		GaugeLightSprite.UpdateAll();
		//float pitch = Mathf.Lerp(PitchMin, PitchMax, (float)mScore / GaugeMax);
		KyAudioManager.Instance.PlayOneShot("se_cursor", 1, mPitch);
		mPitch *= 1.13f;
		if (mEngine != null) {
			mEngine.CommandManager.SetVariable("score", mScore);
		}
	}

	public void OnStartTouch() {
		mTouchEnabled = true;
	}

	public void OnEndTouch() {
		mTouchEnabled = false;
	}

	public void OnGoukaku() {
		mGoukakuEnabled = true;
	}

	public SpriteSimple GaugeLightSprite = null;
	public SpriteSimple GaugeBackSprite = null;
	public int[] GaugeUpTime = new int[] {
		60,60,60,60,60,60,60,60,60,60,
		60,60,60,60,
	};

	private const int GaugeMax = 20;
	private const int GaugeOthers = 14;
	private const float GaugeWidth = 60;
	private const float GaugeHeight = 20;
	private const float GaugeStride = 16;
	private const float PitchMin = 0.2f;
	private const float PitchMax = 2.0f;

	private int mScore = 0;
	private float mPitch = PitchMin;
	private Rect mHitRect = new Rect(-240, -240, 480, 480);
	private bool mTouchEnabled = false;
	private bool mGoukakuEnabled = false;
	private float mElapsedTime = 0;
	private bool mGaugeRed = false;

	private KyGameEngine mEngine = null;
}
