using UnityEngine;
using System.Collections;

public class KyTriggerObjectPosition : KyTrigger {

	void Start () {
		mTargetObject = GameObject.Find(TargetName);
	}
	
	void Update () {
		if (KyScriptTime.DeltaTime == 0) {
			return;
		}
		if (WhenReleased && Input.GetMouseButton(0)) {
			return;
		}
		if (mTargetObject != null) {
			if (Range.Contains(mTargetObject.transform.position)) {
				Matched = true;
				OnTrigger();
			}
		}
	}

	protected void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(
			new Vector3((Range.xMax + Range.xMin) / 2, (Range.yMax + Range.yMin) / 2, 0),
			new Vector3(Range.width, Range.height, 1));
	}

	public Rect Range;
	public string TargetName;
	public bool WhenReleased;

	private GameObject mTargetObject;
}
