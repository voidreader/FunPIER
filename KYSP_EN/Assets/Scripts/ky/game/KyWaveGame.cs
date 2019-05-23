using UnityEngine;
using System.Collections;

/// <summary>
/// 問題「ウェーブ」のゲーム用スクリプト
/// </summary>
public class KyWaveGame : KyScriptObject {

	[System.Serializable]
	public class StageSettings {
		public int StartCount;
		public float SpeedMin;
		public float SpeedMax;
		public float IntervalMin;
		public float IntervalMax;
		public int FakeRate;
		public float WaveWidthRange;
	}

	#region MonoBehaviour Methods

	protected override void Start() {
		base.Start();
		Assert.AssertNotNull(HitoPrefab);
		Assert.AssertNotNull(OmaePrefab);
		Assert.AssertTrue(PeopleCount > 0);
		Assert.AssertTrue(MyselfIndex >= 0);

		EndlessMode = GameEngine.EndlessMode;
		if (EndlessMode) {
			KyUtil.SetText(gameObject, "ten", KyText.GetText(20051));
			KyUtil.SetText(gameObject, "score", mScore.ToString());
		}

		mSprites = new KySpriteAnimation[PeopleCount];
		for (int i = 0; i < PeopleCount; ++i) {
			GameObject obj =
				i != MyselfIndex ? CommandManager.CreateKyObject("wave/KyWaveHito", null) :
				CommandManager.CreateKyObject("wave/KyWaveOmae", null);
			mSprites[i] = obj.GetComponent<KySpriteAnimation>();
			Assert.AssertNotNull(mSprites[i]);
			mSprites[i].AnimationIndex = 0;
			Vector3 pos = obj.transform.position;
			pos.x = -(PeopleCount - 1 - 2 * i) * PeopleInterval / 2;
			obj.transform.position = pos;
			obj.transform.parent = transform;
		}

		//mSpriteScore = KyUtil.GetComponentInChild<KySpriteNumber>(gameObject, "score");
		//Assert.AssertNotNull(mSpriteScore);
		mButtonFire = KyUtil.GetComponentInChild<KyButton>(gameObject, "btnFire");
		Assert.AssertNotNull(mButtonFire);

		/*if (!EndlessMode) {
			mSpriteScore.gameObject.SetActiveRecursively(false);
			Transform ten = KyUtil.GetComponentInChild<Transform>(gameObject, "ten");
			if (ten != null) {
				ten.gameObject.active = false;
			}
		}*/
		mState.ChangeState(StateWait);
	}

	protected override void UpdateCore() {
		mState.Execute();
	}

	#endregion

	#region State

	private int StateWait() {
		mElapsedTime += DeltaTime;
		if (mElapsedTime >= WaveInterval) {
			mState.ChangeState(StateWaving);
		}
		return 0;
	}

	private int StateWaving() {
		if (mState.Sequence == 0) {
			mEvaluated = false;
			mElapsedTime = 0;
			SetupWave();
			if (mFake) {
				mSprites[mFakePeople].AnimationIndex = 1;
				mSprites[mFakePeople].Restart();
				mState.Sequence = 4;
			} else {
				mState.Sequence++;
			}
		} else if (mState.Sequence == 1) {
			mElapsedTime += KyScriptTime.DeltaTime;
			UpdateWave();
			if (mWavePos >= PeopleCount || 0 > mWavePos) {
				mElapsedTime = 0;
				mState.Sequence++;
			}
		} else if (mState.Sequence == 2) {
			mElapsedTime += KyScriptTime.DeltaTime;
			if (mElapsedTime >= WaveInterval + mAnimInterval * 6) {
				CheckScore();
				if (EndlessMode) {
					KyUtil.SetText(gameObject, "score", mScore.ToString());
				}
				//mSpriteScore.Number = mScore;
				if (!EndlessMode && mWaveCount >= MaxWaveCount) {
					mElapsedTime = 0;
					mState.Sequence++;
				} else if (EndlessMode && (!mEvaluated)) {
					mElapsedTime = 0;
					mState.Sequence++;
				} else {
					mState.Sequence = 0;
				}
			}
		} else if (mState.Sequence == 3) {
			mElapsedTime += KyScriptTime.DeltaTime;
			if (mElapsedTime >= 1.0f) {
				mState.ChangeState(StateFinish);
			}
		} else if (mState.Sequence == 4) {
			mElapsedTime += KyScriptTime.DeltaTime;
			if (mElapsedTime >= WaveInterval + mAnimInterval * 6) {
				mState.Sequence = 0;
			}
		}
		if (!CommandManager.PreviewMode) {
			if ((mButtonFire.EventFlag & KyButton.EventType.ButtonDown) != 0) {
				if (!mSprites[MyselfIndex].AnimRunning) {
					mSprites[MyselfIndex].AnimationIndex = 1;
					mSprites[MyselfIndex].Restart();
					Evaluate();
				}
			}
		}
		return 0;
	}

	private int StateFinish() {
		if (mMedachiCount >= 5) {
			CommandManager.SetVariable("secret", 1);
		}
		if (EndlessMode) {
			CommandManager.SetVariable("result", mScore);
		} else {
			if (mScore >= 3) {
				CommandManager.SetVariable("result", 1);
			} else {
				CommandManager.SetVariable("result", 0);
			}
		}
		CommandManager.MoveFrame(1000, 0);
		mState.ChangeState(null);
		return 0;
	}

	#endregion

	#region Methods

	private void SetupWave() {
		if (EndlessMode) {
			if (mStageCount < Stages.Length - 1) {
				if (mWaveCount >= Stages[mStageCount + 1].StartCount) {
					mStageCount++;
				}
			}
			StageSettings stage = Stages[mStageCount];
			WaveSpeed = Random.Range(stage.SpeedMin, stage.SpeedMax);
			mWaveUpdateTime = 1.0f / WaveSpeed;
			float animRate = Random.Range(1.5f - stage.WaveWidthRange, 1.5f + stage.WaveWidthRange);
			mAnimInterval = Mathf.Clamp(mWaveUpdateTime / animRate, 0, 0.2f);
			UpdateAnimationInterval(mAnimInterval);

			WaveInterval = Random.Range(stage.IntervalMin, stage.IntervalMax);
			WaveDirection = Random.Range(0, 2) == 0 ? -1 : 1;
			mWavePos = WaveDirection > 0 ? 0 : PeopleCount - 1;
			if (stage.FakeRate > 0) {
				mFake = Random.Range(0, stage.FakeRate) == 0 ? true : false;
			} else {
				mFake = false;
			}
			if (mFake) {
				int index = Random.Range(0, mFakePeopleTable.Length);
				mFakePeople = mFakePeopleTable[index];
				if (mFakePeople >= PeopleCount) {
					mFakePeople = 0;
				}
			}
			mOldWavePos = -2;
			mWaveCount++;
		} else {
			WaveSpeed = 5.0f;
			mWaveUpdateTime = 1.0f / WaveSpeed;
			mAnimInterval = mWaveUpdateTime / 1.5f;
			UpdateAnimationInterval(mAnimInterval);
			WaveInterval = 1.0f;
			WaveDirection = 1;
			mWavePos = 0;
			mOldWavePos = -2;
			mWaveCount++;
		}
	}

	private void UpdateWave() {
		if (mElapsedTime >= mWaveUpdateTime) {
			mWavePos += WaveDirection;
			mElapsedTime = 0;
		}
		if (mWavePos != mOldWavePos) {
			mOldWavePos = mWavePos;
			if (mWavePos != MyselfIndex && 0 <= mWavePos && mWavePos < PeopleCount) {
				mSprites[mWavePos].AnimationIndex = 1;
				mSprites[mWavePos].Restart();
			}
		}
	}

	private void UpdateAnimationInterval(float interval) {
		foreach (KySpriteAnimation sprite in mSprites) {
			sprite.Animations[1].Interval = interval;
		}
	}

	private void Evaluate() {
		if (MyselfIndex - 1 <= mWavePos && mWavePos <= MyselfIndex + 1) {
			mEvaluated = true;
			mMedachiCount = 0;
		} else {
			mPenalty++;
		}
		if (mWavePos <= 2 || PeopleCount - 2 <= mWavePos) {
			mMedachiCount++;
		}
	}

	private void CheckScore() {
		if (mPenalty >= 2) {
			mEvaluated = false;
		}
		if (mEvaluated) {
			mScore++;
			mPenalty = 0;
		}
	}

	#endregion

	#region Fields

	public bool EndlessMode = false;
	public int PeopleCount = 17;
	public int MyselfIndex = 7;
	public GameObject HitoPrefab = null;
	public GameObject OmaePrefab = null;
	public int PeopleInterval = 32;
	public float WaveInterval = 0;
	public float WaveSpeed = 0.5f;
	public int WaveDirection = 1;
	public int MaxWaveCount = 3;
	public StageSettings[] Stages;

	private KySpriteAnimation[] mSprites;
	//private KySpriteNumber mSpriteScore;
	private KyButton mButtonFire;
	private int mWavePos = 0;
	private int mOldWavePos = -1;
	private float mElapsedTime = 0;
	private bool mEvaluated = false;
	private int mPenalty = 0;
	private int mScore = 0;
	private int mWaveCount = 0;
	private int mStageCount = 0;
	private int mMedachiCount = 0;
	private float mWaveUpdateTime = 0;
	private bool mFake = false;
	private int mFakePeople = 0;
	private int[] mFakePeopleTable = { 0, 1, 15, 16 };
	private float mAnimInterval = 0;
	private KyStateManager mState = new KyStateManager();

	#endregion
}
