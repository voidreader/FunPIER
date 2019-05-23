using UnityEngine;
using System.Collections;

/// <summary>
/// 問題「ペンライト」のゲーム用スクリプト。
/// </summary>
public class KyPenlightGame : KyScriptObject {

	enum HitoState {
		None,
		Ready,
		BeforeRight,
		MoveRight,
		BeforeLeft,
		MoveLeft,
	}

	protected override void Start() {
		base.Start();
		SetupHitoPenlights();
		//ChangeState(StateMove);
	}
	
	protected override void UpdateCore() {
		if (State != null) {
			State();
		}
	}

	private int StateMove() {
		if (mSequence == 0) {
			mElapsedTime = 0;
			if (mMoveCount >= MoveCountMax) {
				ChangeState(StateFinish);
			} else {
				MoveHitoPenlights(mHitoDir);
				mSequence++;
			}
		} else if (mSequence == 1) {
			mElapsedTime += DeltaTime;
			if (mElapsedTime >= MovingTime) {
				mElapsedTime = 0;
				mSequence++;
			}
		} else if (mSequence == 2) {
			mElapsedTime += DeltaTime;
			if (mElapsedTime >= WaitingTime) {
				mElapsedTime = 0;
				mSequence++;
			}
			if (!mGood) {
				if ((mHitoDir < 0 && mOmaeAngle < RightAngle + 10) ||
					(mHitoDir > 0 && mOmaeAngle > LeftAngle - 10)) {
					mGood = true;
				}
			}
			if (!mCounter) {
				if ((mHitoDir < 0 && mOmaeAngle > LeftAngle - 10) ||
					(mHitoDir > 0 && mOmaeAngle < RightAngle + 10)) {
					mCounter = true;
				}
			}
		} else if (mSequence == 3) {
			mHitoDir *= -1;
			mSequence = 0;
			if (mGood) {
				mGoodCount++;
			}
			if (mCounter) {
				mCounterCount++;
			}
			if (mGood && mCounter) {
				mSecretCount++;
			}
			mCounter = false;
			mGood = false;
			mMoveCount++;
		}
		if (!CommandManager.PreviewMode) { mInput.Update(); }
		if (mInput.InputState == KyInputDrag.State.TouchDown) {
			float dx = mInput.DeltaPosition.x;
			mOmaeAngle += -dx / 2;
			mOmaeAngle = Mathf.Clamp(mOmaeAngle, RightAngle, LeftAngle);
			mOmaePenlight.transform.rotation = Quaternion.Euler(0, 0, mOmaeAngle);
		}
		return 0;
	}

	private int StateFinish() {
		if (mSecretCount >= 3) {
			//	シークレット
			CommandManager.SetVariable("secret", 1);
		}
		if (mCounterCount >= 5) {
			//	読まない
			CommandManager.SetVariable("result", 2);
		} else if (mGoodCount >= 5 && mCounterCount <= 1) {
			//	読めた
			CommandManager.SetVariable("result", 1);
		} else {
			//	読めない
			CommandManager.SetVariable("result", 0);
		}
		CommandManager.MoveFrame(1000, 0);
		ChangeState(null);
		return 0;
	}

	private void SetupHitoPenlights() {
		mHitoPenlights = new GameObject[20];
		int num = 0;
		for (int i = 0; i <= 6; ++i) {
			if (i == 3) { continue; }
			GameObject go = (GameObject)Instantiate(HitoPrefab);
			go.transform.localPosition = new Vector3(80 * i - 240, 0, 0);
			mHitoPenlights[num++] = go;
		}
		for (int i = 0; i <= 6; ++i) {
			GameObject go = (GameObject)Instantiate(HitoPrefab);
			go.transform.localPosition = new Vector3(60 * i - 180, 80, 0);
			go.transform.localScale = new Vector3(0.8f, 0.8f, 1);
			mHitoPenlights[num++] = go;
		}
		for (int i = 0; i <= 6; ++i) {
			GameObject go = (GameObject)Instantiate(HitoPrefab);
			go.transform.localPosition = new Vector3(50 * i - 150, 140, 0);
			go.transform.localScale = new Vector3(0.7f, 0.7f, 1);
			mHitoPenlights[num++] = go;
		}
		foreach (GameObject go in mHitoPenlights) {
			KyTweener tweener = go.GetComponent<KyTweener>();
			go.transform.rotation = Quaternion.Euler(0, 0, LeftAngle);
			tweener.UseFlag = KyTweener.UseFlags.UseRotation | KyTweener.UseFlags.UseStart;
			tweener.StartRotation = new Vector3(0, 0, LeftAngle);
			tweener.EndRotation = new Vector3(0, 0, LeftAngle);
			tweener.Play(false);
			go.transform.parent = transform;
		}

		mOmaeAngle = LeftAngle;
		mOmaePenlight = (GameObject)Instantiate(OmaePrefab);
		mOmaePenlight.transform.localPosition = new Vector3(0, 0, -1);
		mOmaePenlight.transform.rotation = Quaternion.Euler(0, 0, mOmaeAngle);
		mOmaePenlight.transform.parent = transform;
	}

	private void UpdateHitoPenlights(float angle) {
		Quaternion rot = Quaternion.Euler(0, 0, angle);
		foreach (GameObject hito in mHitoPenlights) {
			hito.transform.rotation = rot;
		}
	}

	private void MoveHitoPenlights(int dir) {
		foreach (GameObject hito in mHitoPenlights) {
			KyTweener tweener = hito.GetComponent<KyTweener>();
			tweener.StartRotation = tweener.EndRotation;
			if (dir < 0) {
				tweener.EndRotation = new Vector3(0, 0, RightAngle);
			} else {
				tweener.EndRotation = new Vector3(0, 0, LeftAngle);
			}
			tweener.Duration = MovingTime + Random.Range(-0.1f, +0.1f);
			tweener.StartTween(Random.Range(-0.1f, 0));
		}
	}

	public void OnBegin() {
		ChangeState(StateMove);
	}

	public GameObject OmaePrefab = null;
	public GameObject HitoPrefab = null;

	public float LeftAngle = 60.0f;
	public float RightAngle = -60.0f;
	public float MovingTime = 1.0f;
	public float WaitingTime = 1.0f;
	public int MoveCountMax = 6;

	private GameObject[] mHitoPenlights = null;
	private GameObject mOmaePenlight = null;
	private KyInputDrag mInput = new KyInputDrag();
	private float mElapsedTime = 0;
	private float mOmaeAngle = 0;
	private int mHitoDir = -1;
	private bool mGood = false;
	private bool mCounter = false;
	private int mGoodCount = 0;
	private int mCounterCount = 0;
	private int mSecretCount = 0;
	private int mMoveCount = 0;

	private System.Func<int> State = null;
	private int mSequence = 0;
	private void ChangeState(System.Func<int> state) {
		State = state;
		mSequence = 0;
	}
	
}
