using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour{

	public GameObject game;
	// Use this for initialization
	void Start (){
		StartCoroutine(StartGame());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator StartGame(){
		yield return new WaitForSeconds(2.6f);
		game.SetActive(true);
		yield return new WaitForSeconds(0.4f);
		gameObject.SetActive(false);
	}
}
