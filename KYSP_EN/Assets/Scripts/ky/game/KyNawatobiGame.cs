using UnityEngine;
using System.Collections;

/// <summary>
/// 問題「大縄跳び」のゲーム用スクリプト
/// </summary>
public class KyNawatobiGame : KyScriptObject {

	#region Inner Classes

	private enum OmaeState {
		Landing = 0,
		Jumping,
	}

	public class Stage {
		public int mStartCount = 0;
		public float mNawaRangeMin = 1.0f;
		public float mNawaRangeMax = 1.0f;
		public float mNawaSlopeMax = 1.0f;

		public Stage() {}

		public Stage(int startCount, float rangeMin, float rangeMax, float slopeMax) {
			mStartCount = startCount;
			mNawaRangeMin = rangeMin;
			mNawaRangeMax = rangeMax;
			mNawaSlopeMax = slopeMax;
		}
	}

	#endregion

	#region MonoBehaviour Methods

	protected override void Start() {
		base.Start();
		EndlessMode = GameEngine.EndlessMode;
		//TEST
		//EndlessMode = true;
		if (EndlessMode) {
			ConstantJump = false;
			KyUtil.SetText(gameObject, "ten", KyText.GetText(20051));
			KyUtil.SetText(gameObject, "score", mScore.ToString());

			mStages = new Stage[6];
			for (int i = 0; i < 6; ++i) {
				Stage stage = new Stage();
				int paramId = 1100 + i * 10;
				stage.mStartCount = KyDesignParams.GetParam(paramId + 0);
				stage.mNawaRangeMax = KyDesignParams.GetParam(paramId + 1) / 1000.0f;
				stage.mNawaRangeMin = KyDesignParams.GetParam(paramId + 2) / 1000.0f;
				stage.mNawaSlopeMax = KyDesignParams.GetParam(paramId + 3) / 1000.0f;
				mStages[i] = stage;
			}
			UpdateStage(0);
		}

		Vector3 scale = NawaObject.transform.localScale;
		scale.y = -0.3f;
		NawaObject.transform.localScale = scale;

		mHitoParent = CommandManager.CreateKyObject("hitoParent");
		mHitoParent.transform.parent = transform;
		for (int i = - HitoCount; i <= HitoCount; ++i) {
			if (i == 0) { continue; }
			GameObject hito = (GameObject)Instantiate(HitoPrefab);
			hito.name = "hito";
			hito.transform.localPosition = new Vector3(30 * i, 0);
			hito.transform.parent = mHitoParent.transform;
		}
		mHitoJumpTweener = mHitoParent.AddComponent<KyJumpTweener>();
		mHitoJumpTweener.JumpTime = 0.5f;
		mHitoJumpTweener.JumpHeight = 30.0f;

		mOmaeJumpTweener = OmaeObject.AddComponent<KyJumpTweener>();
		mOmaeJumpTweener.JumpTime = 0.1f;
		mOmaeJumpTweener.JumpHeight = 10.0f;

		mMawasuLSprite = MawasuLObject.GetComponent<Sprite>();
		mMawasuRSprite = MawasuRObject.GetComponent<Sprite>();
		mNawaTime = NawaTime;
	}

	protected override void UpdateCore() {
		mState.Execute();
	}

	#endregion

	#region State Methods

	private int StateReady() {
		mMawasuLSprite.AnimationIndex = 0;
		mMawasuRSprite.AnimationIndex = 0;
		mState.ChangeState(StatePlay);
		return 0;
	}

	private int StatePlay() {
		mElapsedTime += DeltaTime;
		mNawaSeqElapsed += DeltaTime;
		if (!CommandManager.PreviewMode) { mInput.Update(); }
		float t = mElapsedTime / mNawaTime;
		if (mNawaState == +1) {
			float u = t * t;
			float scaley = Mathf.Lerp(1.0f, -0.3f, u);
			Vector3 scale = NawaObject.transform.localScale;
			scale.y = scaley;
			NawaObject.transform.localScale = scale;
			//	ヒトがジャンプ
			if (t >= 0.5f && !mHitoJumpTweener.Jumping) {
				mHitoJumpTweener.JumpTime = mNawaTime / 2;
				mHitoJumpTweener.Jump();
			}
			//	回すヒト
			int frameIndex = 
				u <= 0.2f	? 0 :
				u <= 0.4f	? 1 :
 				u <= 0.6f	? 2 :
				u <= 0.8f	? 3 : 4;
			mMawasuLSprite.FrameIndex = frameIndex;
			mMawasuRSprite.FrameIndex = frameIndex;
		} else if (mNawaState == -1) {
			float u = Mathf.Sqrt(t);
			float scaley = Mathf.Lerp(-0.3f, 1.0f, t);
			Vector3 scale = NawaObject.transform.localScale;
			scale.y = scaley;
			NawaObject.transform.localScale = scale;
			//	回すヒト
			int frameIndex =
				u <= 0.2f ? 4 :
				u <= 0.4f ? 3 :
				u <= 0.6f ? 2 :
				u <= 0.8f ? 1 : 0;
			mMawasuLSprite.FrameIndex = frameIndex;
			mMawasuRSprite.FrameIndex = frameIndex;
		}

		//	「お前」のジャンプ処理
		if (ConstantJump) {
			//	固定ジャンプ
			if ((mInput.InputTrigger & KyInputDrag.Trigger.TouchDown) != 0 && !mOmaeJumpTweener.Jumping) {
				mOmaeJumpTweener.JumpHeight = 30.0f;
				mOmaeJumpTweener.JumpTime = 0.5f;
				mOmaeJumpTweener.Jump();
				if (mNawaState == +1 && 0.4f < t && t < 0.6f) {
				} else {
					mOnlyShortJump = false;
				}
			}
		} else {
			//	可変ジャンプ
			if ((mInput.InputTrigger & KyInputDrag.Trigger.TouchDown) != 0 && !mOmaeJumpTweener.Jumping) {
				mOmaeJumpTweener.Jump();
				mOmaeJumpTweener.JumpHeight = 10.0f;
				mOmaeJumpTweener.JumpTime = 0.1f;
				//mJumpingTime = 0;
				mJumpRelease = false;
				//KyAudioManager.Instance.PlayOneShot("se_jump");
			} else if (mInput.InputState == KyInputDrag.State.TouchDown && mOmaeJumpTweener.Jumping && !mJumpRelease) {
				mOmaeJumpTweener.JumpHeight += 80.0f * DeltaTime;
				mOmaeJumpTweener.JumpTime += 0.8f * DeltaTime;
				mOmaeJumpTweener.JumpHeight = Mathf.Clamp(mOmaeJumpTweener.JumpHeight, 0, 50.0f);
				mOmaeJumpTweener.JumpTime = Mathf.Clamp(mOmaeJumpTweener.JumpTime, 0, 0.5f);
				if (mOnlyShortJump && mOmaeJumpTweener.JumpHeight >= 30.0f) {
					mOnlyShortJump = false;
					//DebugUtil.Log("long jump!");
				}
			} else if (mInput.InputState == KyInputDrag.State.TouchUp && mOmaeJumpTweener.Jumping) {
				mJumpRelease = true;
			}
		}

		//	縄の挙動のスイッチング
		if (mElapsedTime >= mNawaTime) {
			mElapsedTime = 0;
			mNawaState *= -1;
			if (mNawaState == -1) {
				if (!mOmaeJumpTweener.Jumping) {
					mState.ChangeState(StateGameOver);
				} else {
					KyAudioManager.Instance.PlayOneShot("se_catch", 1.0f, 1.5f);
					mScore++;
					if (EndlessMode) {
						KyUtil.SetText(gameObject, "score", mScore.ToString());
						UpdateNawa();
					}
				}
			} else if (mNawaState == 1) {
				if (!EndlessMode) {
					if (mScore >= ClearCount) {
						if (mOnlyShortJump) {
							CommandManager.MoveFrame(3000, 0);
						} else {
							CommandManager.MoveFrame(2000, 0);
						}
						mState.ChangeState(null);
					}
				}
				KyAudioManager.Instance.PlayOneShot("se_pitching", 0.2f, 0.8f);
			}
			Vector3 pos = NawaObject.transform.localPosition;
			pos.z = -mNawaState;
			NawaObject.transform.localPosition = pos;
		}
		return 0;
	}

	private int StateGameOver() {
		if (mState.Sequence == 0) {
			Sprite sprite = OmaeObject.GetComponent<Sprite>();
			if (sprite) {
				sprite.AnimationIndex = 1;
			}
			KyAudioManager.Instance.PlayOneShot("se_down");
			mElapsedTime = 0;
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			mElapsedTime += DeltaTime;
			if (mElapsedTime >= 1.0f) {
				CommandManager.SetVariable("result", mScore);
				CommandManager.MoveFrame(1000, 0);
				mState.ChangeState(null);
			}
		}
		return 0;
	}

	#endregion

	#region Methods

	public void OnBeginPlay() {
		if (mState.State == null) {
			mState.ChangeState(StateReady);
		}
	}

	private void UpdateNawa() {
		//mNawaSeqElapsed += DeltaTime;
		//	シーケンス時間が経過した場合の状態変更
		if (mNawaSeqElapsed > mNawaSeqTime) {
			if (mStageCount < mStages.Length - 1) {
				if (mScore >= mStages[mStageCount + 1].mStartCount) {
					UpdateStage(mStageCount + 1);
				}
			}

			mNawaTempoState = 1 - mNawaTempoState;	//	テンポ状態のスイッチ
			if (mNawaTempoState == 0) {	//	テンポ一定
				mNawaTime = mNawaTimeEnd;
				//mNawaTime = 5.0f / mNawaSpeed;
			} else if (mNawaTempoState == 1) {	//	テンポ移動
				mNawaTimeBegin = mNawaTimeEnd;
				mNawaTimeEnd = Random.Range(mNawaRangeMin, mNawaRangeMax);
				mNawaTimeEnd = Mathf.Clamp(mNawaTimeEnd, mNawaTimeBegin - mNawaSlope, mNawaTimeBegin + mNawaSlope);
			}
			mNawaSeqElapsed = 0;
		}
		//	テンポ移動時の縄スピードの更新。
		if (mNawaTempoState == 1) {
			float t = mNawaSeqElapsed / mNawaSeqTime;
			mNawaTime = Mathf.Lerp(mNawaTimeBegin, mNawaTimeEnd, t);
			//mNawaTime = 5.0f / mNawaSpeed;
		}
		DebugUtil.Log("nawa time : " + mNawaTime);
	}

	private void UpdateStage(int stage) {
		mStageCount = stage;
		mNawaRangeMax = mStages[stage].mNawaRangeMax;
		mNawaRangeMin = mStages[stage].mNawaRangeMin;
		mNawaSlope = mStages[stage].mNawaSlopeMax;
		DebugUtil.Log("update stage : " + stage);
	}

	#endregion

	#region

	public GameObject NawaObject;
	public GameObject OmaeObject;
	public GameObject MawasuLObject;
	public GameObject MawasuRObject;
	public GameObject HitoPrefab;
	public int HitoCount = 0;
	public float JumpTime = 0.3f;
	public float NawaTime = 2.0f;
	public float JumpHeight = 20.0f;
	public int ClearCount = 5;
	public bool ConstantJump = true;
	public bool EndlessMode = false;

	private KyInputDrag mInput = new KyInputDrag();
	private int mNawaState = -1;
	private float mElapsedTime = 0;
	private int mScore = 0;
	private GameObject mHitoParent;
	private KyJumpTweener mHitoJumpTweener;
	private KyJumpTweener mOmaeJumpTweener;
	private Sprite mMawasuLSprite;
	private Sprite mMawasuRSprite;
	private bool mOnlyShortJump = true;
	private bool mJumpRelease = false;
	private KyStateManager mState = new KyStateManager();

	private Stage[] mStages = new Stage[6];
	//	大縄跳びゲームレベル情報
	private int mStageCount = 0;	//	現在のステージ番号。
	private float mNawaSeqElapsed = 0;	//	現シーケンスの経過時間。
	private float mNawaTempoState = 0;	//	縄のテンポが一定なら0，移動中なら1。
	private float mNawaTime = 1.0f;	//	縄が半周する時間。(スピード)
	private float mNawaTimeBegin = 1.0f;	//	縄スピードが移動する場合の最初のスピード。
	private float mNawaTimeEnd = 1.0f;	//	縄スピードが移動する場合の最後のスピード。
	private float mNawaRangeMin = 0.15f;	//	縄スピード乱数の下限
	private float mNawaRangeMax = 1.00f;	//	縄スピード乱数の上限
	private float mNawaSlope = 0.2f;
	private float mNawaSeqTime = 10.0f;	//	１シーケンスの長さ。

	#endregion

}
