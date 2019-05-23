using UnityEngine;
using System.Collections;

/// <summary>
/// 問題「くじ」のあみだくじ用スクリプト
/// </summary>
public class KyAmidaLine : KyScriptObject {

	enum StartPosition {
		None = 0,
		Left,
		Right,
	}

	protected override void Start() {
		base.Start();
		mState.ChangeState(StateWaitTouch);
	}
	
	protected override void UpdateCore() {
		if (!mEnabled) { return; }
		mState.Execute();
	}

	private int StateWaitTouch() {
		if (!CommandManager.PreviewMode) { mInput.Update(); }
		if (mInput.InputTrigger == KyInputDrag.Trigger.TouchDown) {
			if (StartRange.Contains(mInput.StartPosition)) {
				mStartPosition = StartPosition.Left;
			} else if (EndRange.Contains(mInput.StartPosition)) {
				mStartPosition = StartPosition.Right;
			}
			if (mStartPosition != StartPosition.None) {
				mState.ChangeState(StateDragging);
				KyAudioManager.Instance.PlayOneShot("se_move");
				mCurrentLine = CreateLine();
				mCurrentLine.transform.localPosition = mInput.StartPosition;
			}
		}
		return 0;
	}

	private int StateDragging() {
		if (!CommandManager.PreviewMode) { mInput.Update(); }
		if (mInput.InputTrigger == KyInputDrag.Trigger.TouchUp) {
			bool accept = false;
			if (mStartPosition == StartPosition.Left && EndRange.Contains(mInput.EndPosition)) {
				accept = true;
			}
			if (mStartPosition == StartPosition.Right && StartRange.Contains(mInput.EndPosition)) {
				accept = true;
			}
			if (accept) {
				KyAudioManager.Instance.PlayOneShot("se_move");
				mLineCount++;
				if (mLineCount >= 3) {
					CommandManager.MoveFrame(2000, 1);
				}
			} else {
				KyAudioManager.Instance.PlayOneShot("se_cancel");
				Destroy(mCurrentLine.gameObject);
			}
			mState.ChangeState(StateWaitTouch);
			mStartPosition = StartPosition.None;
		} else if (mInput.DeltaPosition != Vector3.zero) {
			float length = mInput.DragPosition.magnitude;
			float angle = Mathf.Atan2(mInput.DragPosition.y, mInput.DragPosition.x);
			mCurrentLine.Size.x = length;
			mCurrentLine.UpdateAll();
			mCurrentLine.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
		}
		return 0;
	}

	private SpriteSimple CreateLine() {
		GameObject go = CommandManager.CreateKyObject("line");
		go.transform.parent = transform;
		SpriteSimple sprite = go.AddComponent<SpriteSimple>();
		sprite.Size = new Vector2(6, 6);
		sprite.AnchorX = SpriteAnchor.Minimum;
		sprite.MainColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		sprite.UpdateAll();
		return sprite;
	}

	public void OnDisabled() {
		mEnabled = false;
	}

	public void OnJudge() {
		if (mLineCount % 2 == 1) {
			CommandManager.SetVariable("result", 1);
		}
		if (mLineCount >= 2) {
			CommandManager.SetVariable("result", 2);
		}
		if (mLineCount == 3) {
			CommandManager.SetVariable("secret", 1);
		}
	}

	private Rect StartRange = new Rect(-105, -110, 80, 110);
	private Rect EndRange = new Rect(60, -110, 80, 110);
	private KyInputDrag mInput = new KyInputDrag();
	private SpriteSimple mCurrentLine = null;
	private StartPosition mStartPosition = StartPosition.None;
	private int mLineCount = 0;
	private bool mEnabled = true;
	private KyStateManager mState = new KyStateManager();
}
