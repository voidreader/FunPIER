using UnityEngine;
using System.Collections;

public class KyScriptObject : MonoBehaviour {

	protected virtual void Start() {
		if (transform.parent != null) {
			mGameEngine = transform.parent.GetComponent<KyGameEngine>();
		}
	}

	protected void Update() {
		if (DeltaTime == 0) { return; }
		UpdateCore();
	}

	protected virtual void UpdateCore() {
	}

	public KyGameEngine GameEngine {
		get { return mGameEngine; }
	}

	public KyCommandManager CommandManager {
		get {
			if (mGameEngine != null) { 
				return mGameEngine.CommandManager; 
			} else { 
				return null; 
			}
		}
	}

	protected float DeltaTime {
		get {
			if (mScriptDriven) {
				return KyScriptTime.DeltaTime;
			} else {
				return Time.deltaTime;
			}
		}
	}

	protected KyGameEngine mGameEngine = null;
	protected bool mScriptDriven = true;
}
