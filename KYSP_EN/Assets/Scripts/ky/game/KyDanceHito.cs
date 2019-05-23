using UnityEngine;
using System.Collections;

/// <summary>
/// 問題「あのダンス」のヒト用スクリプト
/// </summary>
public class KyDanceHito : KyScriptObject {

	protected override void Start() {
		base.Start();
		mSprite = GetComponent<SpriteTexAnim>();
		mScaleOrg = transform.localScale.x;
	}
	
	protected override void UpdateCore() {
		if (mDancing) {
			mElapsedTime += DeltaTime;
			if (mElapsedTime >= IntervalTime) {
				mElapsedTime = 0;
				mAnimIndex = (mAnimIndex + 1) % 8;
				if (mAnimIndex <= 4) {
					mSprite.FrameIndex = mAnimIndex;
					mSprite.UpdateFrame();
					Vector3 scale = mSprite.transform.localScale;
					scale.x = mScaleOrg;
					mSprite.transform.localScale = scale;
				} else {
					int state = 8 - mAnimIndex;
					mSprite.FrameIndex = state;
					mSprite.UpdateFrame();
					Vector3 scale = mSprite.transform.localScale;
					scale.x = -mScaleOrg;
					mSprite.transform.localScale = scale;
				}
				if (mAnimIndex == 0) {
					mDanceCount++;
				}
				if (mDanceCount >= DanceCountMax) {
					mDancing = false;
				}
			}
		}
	}

	public void OnBeginDance() {
		mDancing = true;
	}

	public float IntervalTime = 0.2f;
	public int DanceCountMax = 3;

	private SpriteTexAnim mSprite;
	private bool mDancing = false;
	private float mElapsedTime = 0;
	private int mAnimIndex = 0;
	private float mScaleOrg = 1.0f;
	private int mDanceCount = 0;
}
