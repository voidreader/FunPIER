using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KySharedRes : MonoBehaviour {

	[System.Serializable]
	public class ResEntry {
		public string name;
		public Object res;
	}

	void Awake() {
		if (sInstance == null) {
			sInstance = this;
		} else {
			Assert.AssertTrue(false);
		}
	}

	public Object GetResource(string name) {
		foreach (Object res in Resources) {
			if (res.name == name) { return res; }
		}
		return null;
	}

	public Object[] Resources;

	public static KySharedRes Instance {
		get { return sInstance; }
	}

	protected static KySharedRes sInstance;
}
