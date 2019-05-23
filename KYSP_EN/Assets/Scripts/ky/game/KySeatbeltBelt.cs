using UnityEngine;
using System.Collections;

/// <summary>
/// 問題「シートベルト」のシートベルト用スクリプト
/// </summary>
public class KySeatbeltBelt : KyScriptObject {

	protected override void Start() {
		base.Start();
		BeltKeyHomePosition = BeltKeyObject.transform.localPosition;
		mBeltKeyPos = BeltKeyHomePosition;
		UpdateBelt();
		BeltTomeRange = new Rect(BeltTomePosition.x - 10, BeltTomePosition.y - 10, 20, 20);
		BeltHitoRange = new Rect(BeltHitoPosition.x - 10, BeltHitoPosition.y - 10, 20, 20);
	}

	protected override void UpdateCore() {
		if (State != null) {
			State();
		}
		UpdateBelt();
	}

	private int StateWaitTouch() {
		if (!CommandManager.PreviewMode) { mInput.Update(); }
		if (mInput.InputTrigger == KyInputDrag.Trigger.TouchDown) {
			Rect range = new Rect(BeltKeyHomePosition.x - 20, BeltKeyHomePosition.y - 20, 40, 40);
			if (range.Contains(mInput.StartPosition)) {
				KyAudioManager.Instance.PlayOneShot("se_cursor");
				mBeltKeyPos = mInput.StartPosition;
				ChangeState(StateKeyDrag);
			}
		}
		return 0;
	}

	private int StateKeyDrag() {
		mInput.Update();
		if (mInput.InputTrigger == KyInputDrag.Trigger.TouchUp) {
			ChangeState(StateKeyReturn);
		} else if (mInput.DeltaPosition != Vector3.zero) {
			//mBeltKeyPos = mInput.EndPosition;
			mBeltKeyPos = KyUtil.ClampByRect(mInput.EndPosition, DragRange);
			if (BeltTomeRange.Contains(mInput.EndPosition)) {
				mBeltKeyPos = BeltTomePosition;
				Sprite sprite = BeltKeyObject.GetComponent<Sprite>();
				sprite.FrameIndex = 1;
				KyAudioManager.Instance.PlayOneShot("se_click");
				ChangeState(null);
				CommandManager.MoveFrame(2000, 0);
			} else if (BeltHitoRange.Contains(mInput.EndPosition)) {
				mBeltKeyPos = BeltHitoPosition;
				Sprite sprite = BeltKeyObject.GetComponent<Sprite>();
				sprite.FrameIndex = 1;
				BeltKeyObject.transform.rotation = Quaternion.Euler(0, 0, 40);
				KyAudioManager.Instance.PlayOneShot("se_click");
				ChangeState(null);
				CommandManager.MoveFrame(3000, 0);
			}
		}
		return 0;
	}

	private int StateKeyReturn() {
		mBeltKeyPos = BeltKeyHomePosition;
		ChangeState(StateWaitTouch);
		return 0;
	}

	private int StateKeySet() {
		KyAudioManager.Instance.PlayOneShot("se_click");
		ChangeState(null);
		return 0;
	}

	private void UpdateBelt() {
		if (mBeltKeyPos != mBeltKeyPosOld) {
			BeltKeyObject.transform.localPosition = mBeltKeyPos;
			mBeltKeyPosOld = mBeltKeyPos;
			UpBelt.Vertices[0] = new Vector3(UpBeltPosition.x + 8, UpBeltPosition.y, 0);
			UpBelt.Vertices[1] = new Vector3(UpBeltPosition.x - 8, UpBeltPosition.y, 0);
			UpBelt.Vertices[2] = new Vector3(mBeltKeyPos.x - 8, mBeltKeyPos.y, 0);
			UpBelt.Vertices[3] = new Vector3(mBeltKeyPos.x + 8, mBeltKeyPos.y, 0);
			UpBelt.UpdateVertices();

			DownBelt.Vertices[0] = new Vector3(DownBeltPosition.x + 8, DownBeltPosition.y, 0);
			DownBelt.Vertices[1] = new Vector3(DownBeltPosition.x - 8, DownBeltPosition.y, 0);
			DownBelt.Vertices[2] = new Vector3(mBeltKeyPos.x - 8, mBeltKeyPos.y, 0);
			DownBelt.Vertices[3] = new Vector3(mBeltKeyPos.x + 8, mBeltKeyPos.y, 0);
			DownBelt.UpdateVertices();
		}
	}

	public void OnBegin() {
		ChangeState(StateWaitTouch);
	}

	public GameObject BeltKeyObject = null;
	public Vector3 BeltKeyHomePosition = Vector3.zero;
	public Vector3 BeltTomePosition = Vector3.zero;
	public Vector3 BeltHitoPosition = Vector3.zero;
	public Vector3 UpBeltPosition = Vector3.zero;
	public Vector3 DownBeltPosition = Vector3.zero;
	public Rect BeltTomeRange = new Rect(60, -120, 40, 40);
	public Rect BeltHitoRange = new Rect(0, 0, 40, 40);
	public Rect DragRange = new Rect(-240, -180, 480, 360);
	public SpritePolygon UpBelt = null;
	public SpritePolygon DownBelt = null;

	private KyInputDrag mInput = new KyInputDrag();
	private Vector3 mBeltKeyPos = Vector3.zero;
	private Vector3 mBeltKeyPosOld = Vector3.zero;

	private System.Func<int> State = null;
	//private int mSequence = 0;
	private void ChangeState(System.Func<int> state) {
		State = state;
		//mSequence = 0;
	}
}
