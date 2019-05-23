using UnityEngine;
using System.Collections;

public class KyTriggerAnimEnd : KyTrigger {

	void Start () {
		mTargetObject = GameObject.Find(TargetName);
	}
	
	void Update () {
		if (KyScriptTime.DeltaTime == 0) {
			return;
		}
		if (mTargetObject != null) {
			if (mTargetObject.GetComponent<KyTweener>() == null) {
				Matched = true;
				OnTrigger();
			}
		}
	}

	public string TargetName;

	private GameObject mTargetObject;
}
