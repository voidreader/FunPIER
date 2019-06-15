using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RPGTextMesh))]
public class RPGAniOutLineColor : MonoBehaviour
{

    public RPGTextMesh TextMesh;

    public float Duration = 1.0f;
    public Color StartColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    public Color EndColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    float time = 0.0f;

	// Use this for initialization
	void Start () {
        if (TextMesh == null)
            TextMesh = GetComponent<RPGTextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
        if (TextMesh == null)
            return;
        time += Time.deltaTime;
        if (time > Duration)
            time = 0.0f;

        float t = time / Duration;
        TextMesh.SetOutlineColor(Color.Lerp(StartColor, EndColor, t));
	}
}
