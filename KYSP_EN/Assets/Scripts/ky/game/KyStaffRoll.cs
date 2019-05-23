using UnityEngine;
using System.Collections;

/// <summary>
/// 問題「さようなら」のスタッフロール用スクリプト
/// </summary>
public class KyStaffRoll : KyScriptObject {

	#region MonoBehaviour Methods

	protected override void Start() {
		base.Start();
		mNextTextId = TextIdBase;
		mState.ChangeState(StateStaffRoll);
	}
	
	protected override void UpdateCore() {
		mState.Execute();
	}

	#endregion

	#region State Methods

	private int StateStaffRoll() {
		if (mState.Sequence == 0) {
			if (!CheckStaffNames(mPage)) {
				mState.ChangeState(null);
			} else {
				//SetSpriteColors(new Color(0, 0, 0, 0));
				SetStaffNames(mPage);
				SetSpriteColors(new Color(0, 0, 0, 0));
				mElapsedTime = 0;
				mState.Sequence++;
			}
		} else if (mState.Sequence == 1) {
			//	テキストフェードイン
			mElapsedTime += DeltaTime;
			Color color = new Color(0, 0, 0, Mathf.Clamp01(mElapsedTime / TimeFadeIn));
			SetSpriteColors(color);
			if (mElapsedTime >= TimeFadeIn) {
				mElapsedTime = 0;
				if (mRollId > 0 && mKeepRoll == false) {
					mKeepRoll = true;
				}
				mState.Sequence++;
			}
		} else if (mState.Sequence == 2) {
			//	テキスト表示中
			mElapsedTime += DeltaTime;
			if (mElapsedTime >= TimeShown) {
				mElapsedTime = 0;
				if (mNeedToFadeOutWithRoll) {
					mKeepRoll = false;
					mRollId = 0;
				}
				mState.Sequence++;
			}
		} else if (mState.Sequence == 3) {
			//	テキストフェードアウト
			mElapsedTime += DeltaTime;
			Color color = new Color(0, 0, 0, 1 - Mathf.Clamp01(mElapsedTime / TimeFadeOut));
			SetSpriteColors(color);
			if (mElapsedTime >= TimeFadeOut) {
				mElapsedTime = 0;
				mState.Sequence++;
			}
		} else if (mState.Sequence == 4) {
			//	テキスト表示中
			mElapsedTime += DeltaTime;
			if (mElapsedTime >= TimeShown) {
				mElapsedTime = 0;
				mState.Sequence = 0;
				mPage++;
			}
		}
		return 0;
	}

	#endregion

	#region Methods

	private void SetSpriteColors(Color color) {
		for (int i = 0; i < Sprites.Length; ++i) {
			if (i == 0 && mRollId > 0 && mKeepRoll == true) {
				continue;
			}
			SpriteUtil.SetVerticesColor(Sprites[i].gameObject, color);
		}
	}

	private void SetStaffNames(int page) {
		int i = 0;
		
		string temp = KyText.GetText(mNextTextId);
		if (temp.StartsWith("◆")) {
			mRollId = mNextTextId;
			mNeedToFadeOutWithRoll = false;
		} else if (mRollId > 0) {
			i = 1;
		}
		
		for (; i < Sprites.Length; ++i) {
			string text = KyText.GetText(mNextTextId);
			if (string.IsNullOrEmpty(text) || text == "EOP") {
				Sprites[0].Text = "";
				mNextTextId++;
				mNeedToFadeOutWithRoll = true;
				break;
			}
			Sprites[i].Text = text;
			Sprites[i].UpdateAll();
			mNextTextId++;
		}
		for (; i < Sprites.Length; ++i) {
			Sprites[i].Text = "";
			Sprites[i].UpdateAll();
		}
		
		temp = KyText.GetText(mNextTextId);
		if (string.IsNullOrEmpty(temp) || temp == "EOP") {
			//Sprites[0].Text = "";
			//mNextTextId++;
			mNeedToFadeOutWithRoll = true;
		}
	}

	private bool CheckStaffNames(int page) {
		return !string.IsNullOrEmpty(KyText.GetText(mNextTextId));
	}

	private void StartStaffRoll() {
		mState.ChangeState(StateStaffRoll);
	}

	#endregion

	#region Fields

	public SpriteTextCustom[] Sprites = null;
	private const float TimeFadeIn = 0.8f;
	private const float TimeFadeOut = 0.8f;
	private const float TimeShown = 2.3f;
	private const float TimeHidden = 0.5f;
	private const int TextIdBase = 2000;
	private const int TextIdPage = 10;
	private KyStateManager mState = new KyStateManager();
	private float mElapsedTime = 0;
	private int mPage = 0;
	private int mNextTextId = 0;
	private int mRollId = 0;
	private bool mKeepRoll = false;
	private bool mNeedToFadeOutWithRoll = false;

	#endregion
}
