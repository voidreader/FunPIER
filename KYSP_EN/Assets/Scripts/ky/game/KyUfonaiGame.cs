using UnityEngine;
using System.Collections;

/// <summary>
/// ���u�t�e�n��v�̃Q�[���p�X�N���v�g
/// </summary>
public class KyUfonaiGame : KyScriptObject {

	#region MonoBehaviour Methods

	protected override void Start() {
		base.Start();
		mSpeakerSprite = KyUtil.FindSibling(gameObject, "speaker").GetComponent<SpriteTextCustom>();
		mMessageSprite = KyUtil.FindSibling(gameObject, "message").GetComponent<SpriteTextCustom>();
		mTextIndex = TextIdBase;
		ChangeState(StateSpeaking);
	}
	
	protected override void UpdateCore() {
		if (State != null) {
			State();
		}
	}

	#endregion

	#region Methods

	private int StateSpeaking() {
		if (mSequence == 0) {
			mMessageText = KyText.GetText(mTextIndex);
			mAnswer = mMessageText[mMessageText.Length - 1];
			mMessageText = mMessageText.Substring(0, mMessageText.Length - 1);
			DebugUtil.Log("answer is :" + mAnswer);
			mCharIndex = 0;
			mElapsedTime = 0;
			mMessageSprite.Text = "";
			mMessageSprite.UpdateAll();
			mSpeakerSprite.GetComponent<Renderer>().enabled = true;
			mSequence++;
		} else if (mSequence == 1) {
			//	�P�������\������B
			mSpeakElapsedTime += DeltaTime;
			if (mCharIndex < mMessageText.Length) {
				mElapsedTime += DeltaTime;
				if (mElapsedTime >= CharTimespan) {
					mMessageSprite.Text += mMessageText[mCharIndex];
					mMessageSprite.UpdateAll();
					mCharIndex++;
					mElapsedTime = 0;
				}
			}
			if (mSpeakElapsedTime >= SpeakTimespan) {
				mMessageSprite.Text = "";
				mMessageSprite.UpdateAll();
				mSpeakerSprite.GetComponent<Renderer>().enabled = false;

				mSequence++;
				mElapsedTime = 0;
				mSpeakElapsedTime = 0;
			}
		} else if (mSequence == 2) {
			//	�Â��Ȏ���
			mElapsedTime += DeltaTime;
			if (mElapsedTime >= SilentTimespan) {
				JudgeAnswer();
				mTextIndex++;
				if (mTextIndex >= TextIdBase + TextCount) {
					ChangeState(StateFinish);
				} else {
					mSequence = 0;
				}
			}
		}

		return 0;
	}

	private int StateFinish() {
		if (mSequence == 0) {
			if (mScore == mQuest) {
				CommandManager.SetVariable("result", 1);
			} else if (mScore == -mQuest) {
				CommandManager.SetVariable("result", 2);
			}
			if (mYeahCount == mQuest) {
				CommandManager.SetVariable("secret", 1);
			}
			CommandManager.MoveFrame(3000, 1);
			ChangeState(null);
		}
		return 0;
	}

	private void JudgeAnswer() {
		if (mAnswer != 'E') {
			char yourAnswer = CommandManager.GetVariable("yeah") == 1 ? 'Y' : 'N';
			mScore += mAnswer == yourAnswer ? 1 : -1;
			if (yourAnswer == 'Y') {
				mYeahCount++;
			}
			mQuest++;
		}
	}

	public void OnBegin() {
		ChangeState(StateSpeaking);
	}

	#endregion

	#region Fields

	public int TextIdBase = 1000;
	public int TextCount = 10;
	public float SpeakTimespan = 5.0f;
	public float SilentTimespan = 1f;
	public float CharTimespan = 0.025f;

	private SpriteTextCustom mMessageSprite = null;
	private SpriteTextCustom mSpeakerSprite = null;
	private string mMessageText = "";
	private float mElapsedTime = 0;
	private float mSpeakElapsedTime = 0;
	private int mTextIndex = 0;
	private int mCharIndex = 0;
	private int mYeahCount = 0;

	private char mAnswer = 'E';
	private int mScore = 0;
	private int mQuest = 0;

	#endregion

	private System.Func<int> State = null;
	private int mSequence = 0;
	private void ChangeState(System.Func<int> state) {
		State = state;
		mSequence = 0;
	}
}
