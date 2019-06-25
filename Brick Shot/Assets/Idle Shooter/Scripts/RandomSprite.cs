using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RandomSprite : MonoBehaviour {

    public Sprite[] sprites;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnEnable() {
        GetComponent<Image>().sprite = sprites[Random.Range(0, sprites.Length)];
    }
}
