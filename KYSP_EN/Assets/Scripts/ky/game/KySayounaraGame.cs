using UnityEngine;
using System.Collections;

public class KySayounaraGame : KyScriptObject {

	protected override void Start() {
		base.Start();
		mNextSprite = KyUtil.FindSibling(gameObject, "next").GetComponent<Sprite>();
		mMessageSprite = KyUtil.FindSibling(gameObject, "message").GetComponent<SpriteTextCustom>();
		mTextId = TextIdBase;
		mState.ChangeState(StateSpeaking);
	}
	
	protected override void UpdateCore() {
		mState.Execute();
	}

	private int StateSpeaking() {
		if (mState.Sequence == 0) {
			mMessageText = KyText.GetText(mTextId);
			mCharIndex = 0;
			mElapsedTime = 0;
			mMessageSprite.Text = "";
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			//	ÇPï∂éöÇ∏Ç¬?é¶Ç∑ÇÈÅB
			if (!CommandManager.PreviewMode) { mInput.Update(); }
			if (mCharIndex < mMessageText.Length) {
				mElapsedTime += DeltaTime;
				if (mElapsedTime >= CharTimespan) {
					mMessageSprite.Text += mMessageText[mCharIndex];
					mMessageSprite.UpdateAll();
					mCharIndex++;
					mElapsedTime = 0;
					if (mCharIndex == mMessageText.Length) {
						mNextSprite.AnimationIndex = 1;
						mState.Sequence++;
					}
				}
			}
			if (mInput.InputState == KyInputDrag.State.TouchDown) {
				KyAudioManager.Instance.PlayOneShot("se_cancel");
				mState.Sequence = 3;
				mSkipCount++;
			}
		} else if (mState.Sequence == 2) {
			if (!CommandManager.PreviewMode) { mInput.Update(); }
			mSpeakElapsedTime += DeltaTime;
			if (mSpeakElapsedTime >= SpeakTimespan) {
				mState.Sequence++;
			}
			if (mNextSprite.AnimationIndex == 1 && mInput.InputState == KyInputDrag.State.TouchDown) {
				KyAudioManager.Instance.PlayOneShot("se_cancel");
				mMessageSprite.Text = "";
				mMessageSprite.UpdateAll();
				mNextSprite.AnimationIndex = 0;
				mYomuCount++;
			}
		} else if (mState.Sequence == 3) {
			mMessageSprite.Text = "";
			mMessageSprite.UpdateAll();
			mNextSprite.AnimationIndex = 0;
			mElapsedTime = 0;
			mSpeakElapsedTime = 0;
			mState.Sequence++;
		} else if (mState.Sequence == 4) {
			//	ê√Ç©Ç»éûä‘
			mElapsedTime += DeltaTime;
			if (mElapsedTime >= SilentTimespan) {
				mTextId++;
				if (mTextId >= TextIdBase + TextCount) {
					mState.ChangeState(StateFinish);
				} else {
					mState.Sequence = 0;
				}
			}
		}
		return 0;
	}

	private int StateFinish() {
		if (mYomuCount >= TextCount-2) {
			CommandManager.SetVariable("result", 1);
		}
		if (mSkipCount >= TextCount-2) {
			CommandManager.SetVariable("secret", 1);
		}
		CommandManager.MoveFrame(3000, 1);
		mState.ChangeState(null);
		return 0;
	}

	public int TextIdBase = 1400;
	public int TextCount = 29;
	public float SpeakTimespan = 3.0f;
	public float SilentTimespan = 1.0f;
	public float CharTimespan = 0.1f;

	private SpriteTextCustom mMessageSprite = null;
	private Sprite mNextSprite = null;
	private string mMessageText = "";
	private int mTextId = 0;
	private int mCharIndex = 0;
	private int mSkipCount = 0;
	private int mYomuCount = 0;
	private float mElapsedTime = 0;
	private float mSpeakElapsedTime = 0;

	private KyInputDrag mInput = new KyInputDrag();
	private KyStateManager mState = new KyStateManager();
}
