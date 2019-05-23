using UnityEngine;
using System.Collections;

/// <summary>
/// 問題「あのダンス」のお前用スクリプト。
/// </summary>
public class KyDanceGame : KyScriptObject {

	protected override void Start() {
		base.Start();
		OmaeSprite = GetComponent<Sprite>();
		mInput.Threshold = 40;
	}
	
	protected override void UpdateCore() {
		if (!mEnabled) { return; }
		UpdateOmae();
	}

	private void UpdateOmae() {
		if (!CommandManager.PreviewMode) { mInput.Update(); }
		int animAdd = 0;
		if ((mInput.Slide & KyInputCrossKey.Direction.Left) != 0) {
			if (mOmaeState <= 1 || 7 <= mOmaeState) {
				animAdd = 1;
			} else if (3 <= mOmaeState && mOmaeState <= 5) {
				animAdd = -1;
			}
		}
		else if ((mInput.Slide & KyInputCrossKey.Direction.Down) != 0) {
			if (1 <= mOmaeState && mOmaeState <= 3) {
				animAdd = 1;
			} else if (5 <= mOmaeState && mOmaeState <= 7) {
				animAdd = -1;
			}
		}
		else if ((mInput.Slide & KyInputCrossKey.Direction.Right) != 0) {
			if (mOmaeState <= 1 || 7 <= mOmaeState) {
				animAdd = -1;
			} else if (3 <= mOmaeState && mOmaeState <= 5) {
				animAdd = 1;
			}
		}
		else if ((mInput.Slide & KyInputCrossKey.Direction.Up) != 0) {
			if (1 <= mOmaeState && mOmaeState <= 3) {
				animAdd = -1;
			} else if (5 <= mOmaeState && mOmaeState <= 7) {
				animAdd = 1;
			}
		}
		if (animAdd != 0) {
			mOmaeState = (mOmaeState + animAdd + 8) % 8;
			if (mOmaeState <= 4) {
				OmaeSprite.FrameIndex = mOmaeState;
				Vector3 scale = OmaeSprite.transform.localScale;
				scale.x = 1;
				OmaeSprite.transform.localScale = scale;
			} else {
				int state = 8 - mOmaeState;
				OmaeSprite.FrameIndex = state;
				Vector3 scale = OmaeSprite.transform.localScale;
				scale.x = -1;
				OmaeSprite.transform.localScale = scale;
			}
			UpdatePath(mOmaeState);
		}
	}

	private void UpdatePath(int state) {
		if (state == 0 && mDancePath[mPathIndex] != 0) {
			if (mPathIndex == 2) {
				if (mDancePath[1] == 1 && mDancePath[2] == 2) {
					mCounterClockwise++;
					DebugUtil.Log("counterclockwise:" + mCounterClockwise);
				} else if (mDancePath[1] == 2 && mDancePath[2] == 1) {
					mClockwise++;
					DebugUtil.Log("clockwise:" + mClockwise);
				}
			}
			mKomanechi--;
			mPathIndex = 0;
			mDancePath[0] = 0;
		} else if (state == 2 && mDancePath[mPathIndex] != 1 && mPathIndex < 3) {
			DebugUtil.Log("path1");
			mPathIndex++;
			mDancePath[mPathIndex] = 1;
		} else if (state == 6 && mDancePath[mPathIndex] != 2 && mPathIndex < 3) {
			DebugUtil.Log("path2");
			mPathIndex++;
			mDancePath[mPathIndex] = 2;
		}
		if (state == 4) {
			mKomanechi++;
			DebugUtil.Log("komanechi");
		}
	}

	public void OnEnabled() {
		mEnabled = true;
	}

	public void OnDisabled() {
		mEnabled = false;
	}

	public void OnJudge() {
		mEnabled = false;
		if (mCounterClockwise == 3 && mClockwise == 0) {
			CommandManager.SetVariable("result", 1);
		} else if (mClockwise >= 3) {
			CommandManager.SetVariable("result", 2);
		}
		if (mKomanechi >= 8 && mOmaeState == 4) {
			CommandManager.SetVariable("secret", 1);
		}
	}


	public Sprite OmaeSprite = null;
	public Sprite HitoSprite = null;   

	private KyInputCrossKey mInput = new KyInputCrossKey();
	private bool mEnabled = false;
	private int mOmaeState = 0;
	private int[] mDancePath = new int[4];
	private int mPathIndex = 0;
	private int mClockwise = 0;
	private int mCounterClockwise = 0;
	private int mKomanechi = 0;
}
