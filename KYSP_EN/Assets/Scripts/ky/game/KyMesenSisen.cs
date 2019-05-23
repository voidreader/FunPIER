using UnityEngine;
using System.Collections;

/// <summary>
/// ���u�ڐ��v�̖ڐ��I�u�W�F�N�g�p�X�N���v�g�B
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

	public Vector3 GoalPosition = new Vector3(0, 0, 0);	//	����ɖڐ����s���Ă��܂��ʒu�B
	public Rect RangeHot = new Rect();			//	�C�ɂȂ�͈́B
	public Rect RangeCool = new Rect();			//	���ΎԐ��͈̔́B
	public float Speed = 4.0f;
	public float ThresholdYometa = 3.0f;			//	��C�ǂ߂�����ƂȂ鎞�ԁB
	public float ThresholdYommanai = 7.0f;			//	��C�ǂ܂Ȃ�����ƂȂ鎞�ԁB
	public float ThresholdSecret = 5.0f;			//	�V�[�N���b�g����ƂȂ鎞�ԁB

	private float mTotalTimeHot = 0.0f;			//	�C�ɂȂ�͈͂ɖڐ������Ă��������ԁB
	private float mTotalTimeCool = 0.0f;		//	���ΎԐ��͈̔͂ɖڐ������Ă��������ԁB
	private KyGameEngine mEngine = null;
}
