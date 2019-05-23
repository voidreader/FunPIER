using UnityEngine;
using System.Collections;

/// <summary>
/// ���u�n�����v�̃Q�[���p�X�v���C�g�B
/// </summary>
public class KyBaseballGame : KyScriptObject {

	#region Inner Classes

	public enum BattingResult {
		None = 0,
		Strike,
		Foul,
		Hit,
		Homerun,
	}

	public class Stage {
		public int mStartCount = 0;
		public float mGravityMin = 0;
		public float mGravityMax = 0;
		public float mFlyingTimeMin = 0;
		public float mFlyingTimeMax = 0;
		public float mPitchTimeMin = 0;
		public float mPitchTimeMax = 0;
		public float mCriticalRate = 0;
	}

	#endregion

	#region MonoBehaviour Methods

	protected override void Start() {
		base.Start();
		EndlessMode = GameEngine.EndlessMode;

		if (EndlessMode) {
			KyUtil.SetText(gameObject, "ten", KyText.GetText(20051));
			KyUtil.SetText(gameObject, "score", mScore.ToString());

			mStages = new Stage[6];
			for (int i = 0; i < 6; ++i) {
				Stage stage = new Stage();
				int paramId = 1200 + i * 10;
				stage.mStartCount = KyDesignParams.GetParam(paramId + 0);
				stage.mGravityMin = KyDesignParams.GetParam(paramId + 1);
				stage.mGravityMax = KyDesignParams.GetParam(paramId + 2);
				stage.mFlyingTimeMax = KyDesignParams.GetParam(paramId + 3) / 1000.0f;
				stage.mFlyingTimeMin = KyDesignParams.GetParam(paramId + 4) / 1000.0f;
				stage.mPitchTimeMax = KyDesignParams.GetParam(paramId + 5) / 1000.0f;
				stage.mPitchTimeMin = KyDesignParams.GetParam(paramId + 6) / 1000.0f;
				stage.mCriticalRate = KyDesignParams.GetParam(paramId + 7) / 100.0f;
				mStages[i] = stage;
			}
		}

		Transform pitcher = transform.parent.Find(PitcherName);
		Assert.AssertNotNull(pitcher);
		mPitcherSprite = pitcher.GetComponent<Sprite>();
		Assert.AssertNotNull(mPitcherSprite);

		Transform batter = transform.parent.Find(BatterName);
		Assert.AssertNotNull(batter);
		mBatterSprite = batter.GetComponent<Sprite>();
		Assert.AssertNotNull(mBatterSprite);

		Assert.AssertNotNull(BallObject);
		Assert.AssertNotNull(ShadowObject);
		BallObject.GetComponent<Renderer>().enabled = false;
		ShadowObject.GetComponent<Renderer>().enabled = false;
	}
	
	protected override void UpdateCore() {
		mState.Execute();
	}

	#endregion

	#region State Methods

	private int StateBegin() {
		return 0;
	}

	private int StateBeforePitch() {
		if (mState.Sequence == 0) {
			mElapsedTime = 0;
			if (EndlessMode) {
				UpdateParam();
			}
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			mElapsedTime += DeltaTime;
			if (mElapsedTime > PitchTime) {
				mState.Sequence++;
			}
		} else if (mState.Sequence == 2) {
			//	�s�b�`���[�̃s�b�`�A�j���[�V�����J�n�B
			mPitcherSprite.AnimationIndex = 1;
			mPitcherSprite.UpdateAll();
			mElapsedTime = 0;
			mState.Sequence++;
		} else if (mState.Sequence == 3) {
			//	�s�b�`�A�j���[�V�����҂��B
			mElapsedTime += DeltaTime;
			if (mElapsedTime >= PitchAnimTime) {
				mElapsedTime = 0;
				mState.ChangeState(StatePitch);
			}
		}
		return 0;
	}

	private int StatePitch() {
		if (mState.Sequence == 0) {
			//	�{�[���̈ʒu�������B
			BallObject.transform.localPosition = StartPosition;
			mBallHeight = 0;
			mState.Sequence = 1;
			mElapsedTime = 0;
			mSwung = false;
			//	�{�[���̉����������x�Z�o
			mBallHeightSpeed = Gravity * FlyingTime / 2.0f;
			//	�{�[������
			BallObject.GetComponent<Renderer>().enabled = true;
			ShadowObject.GetComponent<Renderer>().enabled = true;
			KyAudioManager.Instance.PlayOneShot("se_laser", 0.8f, 0.6f);
			
		}
		if (mState.Sequence == 1) {
			if (!CommandManager.PreviewMode) { mInput.Update(); }
			mElapsedTime += DeltaTime;
			//	�{�[���̈ʒu�X�V�B
			//	��ʒu�Z�o�B
			float t = mElapsedTime / FlyingTime;
			Vector2 pos = Vector2.Lerp(StartPosition, EndPosition, t);
			//	�e�̈ʒu�B
			Vector2 shadowpos = pos;
			shadowpos.y -= ShadowHeight;
			ShadowObject.transform.localPosition = shadowpos;
			//	�����ψʎZ�o(�����铊���グ�^��)�B
			mBallHeight = mBallHeightSpeed * mElapsedTime - Gravity * mElapsedTime * mElapsedTime / 2.0f;
			Vector2 ballpos = pos;
			ballpos.y += mBallHeight;
			BallObject.transform.localPosition = ballpos;
			//	�X�P�[���Z�o�B
			float scale = Mathf.Lerp(0.6f, 1.0f, mBallHeight / 200);
			BallObject.transform.localScale = new Vector3(scale, scale, 1.0f);
			float shadowScale = 0.6f / (mBallHeight + 300) * 300;
			ShadowObject.transform.localScale = new Vector3(shadowScale, shadowScale, 1.0f);

			//	�X�g���C�N����B
			if (mElapsedTime >= FlyingTime) {
				mState.Sequence = 2;
			} else if (!mSwung && mInput.InputState == KyInputDrag.State.TouchDown) {
				KyAudioManager.Instance.PlayOneShot("se_pitching");
				mBatterSprite.AnimationIndex = 1;
				mBatterSprite.UpdateAll();
				mSwung = true;
				if (t >= HitRate) {
					HitStartPosition = pos;
					float rate = (t - HitRate) / (1 - HitRate);
					HitEndPosition = Vector2.Lerp(HitLeftPosition, HitRightPosition, rate);
					if (!EndlessMode && (rate <= 0.1f || 0.9f <= rate)) {
						//	�t�@�E��
						CommandManager.SetVariable("game", 3);
						KyAudioManager.Instance.PlayOneShot("se_homerun", 1.0f, 0.5f);
						mBattingResult = BattingResult.Foul;
					} else if (0.4f <= rate && rate <= 0.6f) {
						//	�z�[������
						CommandManager.SetVariable("game", 11);
						KyAudioManager.Instance.PlayOneShot("se_homerun", 1.0f, 1.0f);
						mBattingResult = BattingResult.Homerun;
					} else {
						//	�q�b�g
						CommandManager.SetVariable("game", 10);
						KyAudioManager.Instance.PlayOneShot("se_homerun", 1.0f, 0.8f);
						mBattingResult = BattingResult.Hit;
					}
					mState.ChangeState(StateHit);
				} else {
					//	��U��
					CommandManager.SetVariable("game", 1);
					mBattingResult = BattingResult.Strike;
				}
			}
		} else if (mState.Sequence == 2) {
			KyAudioManager.Instance.PlayOneShot("se_catch", 1.0f, 0.5f);
			BallObject.GetComponent<Renderer>().enabled = false;
			ShadowObject.GetComponent<Renderer>().enabled = false;
			mElapsedTime = 0;
			mState.Sequence = 3;
		} else if (mState.Sequence == 3) {
			if (!CommandManager.PreviewMode) { mInput.Update(); }
			mElapsedTime += DeltaTime;
			if (mElapsedTime >= 0.5f) {
				mState.Sequence = 4;
			} else if (!mSwung && mInput.InputState == KyInputDrag.State.TouchDown) {
				KyAudioManager.Instance.PlayOneShot("se_pitching");
				mBatterSprite.AnimationIndex = 1;
				mBatterSprite.UpdateAll();
				mSwung = true;
				//�@�U��x��
				CommandManager.SetVariable("game", 2);
				mState.Sequence = 4;
			}
		} else if (mState.Sequence == 4) {
			mBattingResult = BattingResult.Strike;
			mState.ChangeState(StateFinish);
		}
		return 0;
	}

	private int StateHit() {
		if (mState.Sequence == 0) {
			//	�{�[���̈ʒu�������B
			BallObject.transform.localPosition = HitStartPosition;
			mBallHeight = 0;
			mState.Sequence = 1;
			mElapsedTime = 0;
			mSwung = false;
		} else if (mState.Sequence == 1) {
			mElapsedTime += DeltaTime;
			//	�{�[���̈ʒu�X�V�B
			float t = mElapsedTime / HitTime;
			Vector2 pos = Vector2.Lerp(HitStartPosition, HitEndPosition, t);
			//	�e�̈ʒu�B
			Vector2 shadowpos = pos;
			shadowpos.y -= ShadowHeight;
			ShadowObject.transform.localPosition = shadowpos;
			//	�����ψʎZ�o(�����铊���グ�^��)�B
			mBallHeight = Mathf.Lerp(0.0f, 120.0f, t);
			pos.y += mBallHeight;
			BallObject.transform.localPosition = pos;
			//	�X�P�[���Z�o�B
			float scale = Mathf.Lerp(0.6f, 1.0f, mBallHeight / 200);
			BallObject.transform.localScale = new Vector3(scale, scale, 1.0f);
			ShadowObject.transform.localScale = new Vector3(0.6f, 0.6f, 1.0f);
			if (mElapsedTime >= HitTime) {
				mState.Sequence = 2;
			}
		} else if (mState.Sequence == 2) {
			//CommandManager.MoveFrame(3000, 0);
			//BallObject.renderer.enabled = false;
			//ShadowObject.renderer.enabled = false;
			mState.ChangeState(StateFinish);
		}
		return 0;
	}

	private int StateFinish() {
		if (mState.Sequence == 0) {
			BallObject.GetComponent<Renderer>().enabled = false;
			ShadowObject.GetComponent<Renderer>().enabled = false;
			if (EndlessMode) {
				//	�G���h���X���[�h�̏I�������B
				if (mBattingResult == BattingResult.Strike) {
					CommandManager.SetVariable("result", mScore);
					CommandManager.MoveFrame(3000, 0);
					mState.ChangeState(null);
				} else {
					mScore++;
					mState.Sequence++;
					mElapsedTime = 0;
				}
			} else {
				CommandManager.MoveFrame(3000, 0);
				mState.ChangeState(null);
			}
		} else if (mState.Sequence == 1) {
			mElapsedTime += DeltaTime;
			if (mElapsedTime > 1.0f) {
				KyUtil.SetText(gameObject, "score", mScore.ToString());
				mState.ChangeState(StateBeforePitch);
				mBatterSprite.AnimationIndex = 0;
				mBatterSprite.UpdateAll();
				mPitcherSprite.AnimationIndex = 0;
				mPitcherSprite.UpdateAll();
				mElapsedTime = 0;
			}
		}
		return 0;
	}

	#endregion

	#region Methods

	public void OnBeginPitch() {
		if (mState.State == null) {
			mState.ChangeState(StateBeforePitch);
		}
	}

	private void UpdateParam() {
		//	�ŏI�X�e�[�W�����ŁA���̃X�e�[�W���n�܂�X�R�A���ǂ������������B
		if (mStageCount < mStages.Length - 1) {
			if (mScore >= mStages[mStageCount + 1].mStartCount) {
				mStageCount++;
			}
		}
		Stage stage = mStages[mStageCount];
		bool critical = Random.Range(0.0f, 1.0f) <= stage.mCriticalRate;
		if (critical) {
			Gravity = stage.mGravityMax;
			FlyingTime = stage.mFlyingTimeMin;
		} else {
			Gravity = Random.Range(stage.mGravityMin, stage.mGravityMax);
			FlyingTime = Random.Range(stage.mFlyingTimeMin, stage.mFlyingTimeMax);
		}
		PitchTime = Random.Range(stage.mPitchTimeMin, stage.mPitchTimeMax);
		DebugUtil.Log("gravity : " + Gravity);
		DebugUtil.Log("flying time : " + FlyingTime);
	}

	#endregion

	#region Fields

	public float Gravity = 1.0f;
	public float FlyingTime = 2.0f;
	public float PitchTime = 1.0f;
	public float HitTime = 2.0f;
	public Vector2 StartPosition = Vector2.zero;
	public Vector2 EndPosition = Vector2.zero;
	public Vector2 HitStartPosition = Vector2.zero;
	public Vector2 HitEndPosition = Vector2.zero;
	public Vector2 HitLeftPosition = new Vector2(-240, 240);
	public Vector2 HitRightPosition = new Vector2(240, 240);
	public string PitcherName = "pitcher";
	public string BatterName = "batter";
	public float PitchAnimTime = 1.0f;
	public float ShadowHeight = 10.0f;
	public float HitRate = 0.85f;
	public int HomerunEventFrame = 0;
	public GameObject BallObject = null;
	public GameObject ShadowObject = null;
	public bool EndlessMode = false;

	private KyStateManager mState = new KyStateManager();
	private KyInputDrag mInput = new KyInputDrag();
	private float mBallHeight = 0.0f;
	private float mBallHeightSpeed = 0.0f;
	private float mElapsedTime = 0.0f;
	private bool mSwung = false;
	private Sprite mPitcherSprite = null;
	private Sprite mBatterSprite = null;
	private BattingResult mBattingResult = BattingResult.None;
	private int mScore = 0;

	//	�싅�Q�[�����x�����
	private Stage[] mStages = new Stage[6];
	private int mStageCount = 0;

	#endregion
}
