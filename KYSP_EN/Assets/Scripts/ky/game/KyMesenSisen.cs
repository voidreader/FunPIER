using UnityEngine;
using System.Collections;

/// <summary>
/// 問題「目線」の目線オブジェクト用スクリプト。
/// </summary>
public class KyMesenSisen : MonoBehaviour {

	void Start () {
		if (transform.parent != null) {
			mEngine = transform.parent.GetComponent<KyGameEngine>();
		}
	}
	
	void Update () {
		if (KyScriptTime.DeltaTime == 0) { return; }
		Vector3 dir = (GoalPosition - transform.localPosition).normalized;
		transform.localPosition += dir * (Speed * KyScriptTime.DeltaTime);
		if (RangeHot.Contains(transform.localPosition)) {
			mTotalTimeHot += KyScriptTime.DeltaTime;
		}
		if (RangeCool.Contains(transform.localPosition)) {
			mTotalTimeCool += KyScriptTime.DeltaTime;
		}
	}

	protected void OnDrawGizmos() {
		Gizmos.color = Color.green;
		GizmosUtil.DrawRect(RangeHot);
		GizmosUtil.DrawRect(RangeCool);
	}

	public void OnJudge() {
		if (mEngine == null) { return; }
		if (mTotalTimeCool >= ThresholdSecret) {
			mEngine.CommandManager.SetVariable("secret", 1);
		}
		if (mTotalTimeHot < ThresholdYometa) {
			mEngine.CommandManager.MoveFrame(1000, 0);
		} else if (ThresholdYommanai < mTotalTimeHot) {
			mEngine.CommandManager.MoveFrame(2000, 0);
		} else {
			mEngine.CommandManager.MoveFrame(3000, 0);
		}
	}

	public Vector3 GoalPosition = new Vector3(0, 0, 0);	//	勝手に目線が行ってしまう位置。
	public Rect RangeHot = new Rect();			//	気になる範囲。
	public Rect RangeCool = new Rect();			//	反対車線の範囲。
	public float Speed = 4.0f;
	public float ThresholdYometa = 3.0f;			//	空気読めた判定となる時間。
	public float ThresholdYommanai = 7.0f;			//	空気読まない判定となる時間。
	public float ThresholdSecret = 5.0f;			//	シークレット判定となる時間。

	private float mTotalTimeHot = 0.0f;			//	気になる範囲に目線を入れていた総時間。
	private float mTotalTimeCool = 0.0f;		//	反対車線の範囲に目線を入れていた総時間。
	private KyGameEngine mEngine = null;
}
