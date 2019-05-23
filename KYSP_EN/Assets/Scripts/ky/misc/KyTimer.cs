using UnityEngine;
using System.Collections;

public class KyTimer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		DebugUtil.Log("Update : " + Time.deltaTime);
	}

	void FixedUpdate() {
		DebugUtil.Log("FixedUpdate : " + Time.deltaTime);
	}
}
