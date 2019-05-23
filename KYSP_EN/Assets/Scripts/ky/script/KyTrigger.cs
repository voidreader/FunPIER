using UnityEngine;
using System.Collections;

public class KyTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	protected void OnTrigger() {
		if (Trigger != null) {
			Trigger(this, System.EventArgs.Empty);
		}
		if (AutoDestroy) {
			Destroy(gameObject);
		}
	}

	public bool Matched = false;
	public bool AutoDestroy = true;
	public int Priority = 0;
	public event System.EventHandler Trigger;
}
