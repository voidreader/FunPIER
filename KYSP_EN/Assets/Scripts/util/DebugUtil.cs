using UnityEngine;
using System.Collections;

public static class DebugUtil {
	
	private const bool debug = false;
	
	public static void Log(string msg) {
		if (debug) {
			Debug.Log(msg);
		}
#if UNITY_EDITOR
		Debug.Log(msg);
#endif
	}
}