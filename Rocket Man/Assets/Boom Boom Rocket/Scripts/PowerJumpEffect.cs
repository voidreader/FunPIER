using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerJumpEffect : MonoBehaviour {

	GameObject PlayerObj;


	void Start () {
		PlayerObj = GameObject.Find("Player");
		
	}
	
	void Update () {
		transform.position = PlayerObj.transform.position;
	}
}
