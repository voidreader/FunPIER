using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RPGTextMesh))]
public class RPGAniGradientColor : MonoBehaviour
{

    public RPGTextMesh TextMesh;

    public float Duration = 1.0f;
    public Color StartTopColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    public Color EndTopColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public Color StartBottomColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public Color EndBottomColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

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
        TextMesh.SetColor(Color.Lerp(StartTopColor, EndTopColor, t), Color.Lerp(StartBottomColor, EndBottomColor, t));
	}
}
