using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RPGTextMesh))]
public class RPGAniScale : MonoBehaviour {

    private Transform _transform;

    public float Duration = 2.0f;
    public float StartScale = 1.0f;
    public float EndScale = 3.0f;

    float time = 0.0f;

    // Use this for initialization
    void Start()
    {
        if (_transform == null)
            _transform = transform;
    }

    // Update is called once per frame
    void Update() {
        time += Time.deltaTime;
        if (time > Duration)
            time = 0.0f;

        float t = time / Duration;
        float lerp = Mathf.Lerp(StartScale, EndScale, t);
        _transform.localScale = new Vector3(lerp, lerp, 1.0f);
	}
}
