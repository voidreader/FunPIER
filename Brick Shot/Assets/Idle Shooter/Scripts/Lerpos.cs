using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerpos : MonoBehaviour {

    public float min, max;

	// Use this for initialization
	void Start () {
        StartCoroutine(LateStart());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator LateStart() {
        yield return new WaitForEndOfFrame();
        transform.position = new Vector3(0, Mathf.Lerp(min, max, (Camera.main.orthographicSize - 4.65f) * 1.5f), 0);

    }
}
