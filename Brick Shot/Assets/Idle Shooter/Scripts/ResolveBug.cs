using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// There is a weird bug (Unity editor related) with The bottom holder scrollviews
/// </summary>
public class ResolveBug : MonoBehaviour {

    public GameObject a, b, c, content; //disabling upgrades panel at launch bug

    float timer = 60;

    // Use this for initialization
    void Start() {
        a.SetActive(false);
        b.SetActive(true);
        b.SetActive(false);
        a.SetActive(true);

        content.transform.localPosition = new Vector3(350, 0, 0);
    }

    private void Update() {
        if (b.activeSelf) timer = 0;
        timer += Time.deltaTime;

        if (timer > 180) {
            timer = 0;
            b.SetActive(true);
            a.SetActive(false);
            c.SetActive(false);
        }
    }
}
