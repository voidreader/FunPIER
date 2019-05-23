using UnityEngine;
using System.Collections;

public class KyBlink : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		Color color = GetComponent<Renderer>().material.GetColor("_TintColor");
		color.a = Mathf.PingPong(Time.time, Duration) / (2 * Duration);
		GetComponent<Renderer>().material.SetColor("_TintColor", color);
	}

	public float Duration = 2.0f;
}
