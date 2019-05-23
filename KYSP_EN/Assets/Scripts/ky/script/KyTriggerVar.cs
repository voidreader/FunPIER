using UnityEngine;
using System.Collections;

public class KyTriggerVar : KyTrigger {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (KyScriptTime.DeltaTime == 0) {
			return;
		}
		if (Cond != null) {
			if (Cond(CommandManager.GetEvalValue(Lhs), CommandManager.GetEvalValue(Rhs))) {
				Matched = true;
				OnTrigger();
			}
		} else {
			Destroy(gameObject);
		}
	}

	public delegate bool Condition(float lhs, float rhs);

	public KyCommandManager CommandManager = null;
	public KyVariable Lhs = null;
	public KyVariable Rhs = null;
	public Condition Cond = null;

}
