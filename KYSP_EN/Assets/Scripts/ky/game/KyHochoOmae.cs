using UnityEngine;
using System.Collections;

/// <summary>
/// ���u�����v�p�X�N���v�g�B
/// </summary>
public class KyHochoOmae : KyScriptObject {
	
	protected override void UpdateCore () {
		//mInput.Update();
		Vector3 dir = (GoalPosition - transform.localPosition).normalized;
		transform.localPosition += dir * (Speed * KyScriptTime.DeltaTime);
	}

	public void OnJudge() {
		float y = transform.localPosition.y;
		if (220 < y && y < 300) {
			CommandManager.SetVariable("result", 1);
		} else if (-20 < y && y < 40) {
			CommandManager.SetVariable("secret", 1);
		}
	}

	public Vector3 GoalPosition = new Vector3(0, 300, 0);	//	�����ڕW�ʒu�B
	public float Speed = 4.0f;
}
