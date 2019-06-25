using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Laser block
/// </summary>
public class Laser : MonoBehaviour {

    GameManager gm;

    public SpriteRenderer sp1, sp2;
    public LineRenderer lineRenderer; //visualizing the efect with LineRenderer

    float lineLength = 0;

    public GameObject collider;

    bool inUsage = false;

    public bool isVertical = false; //if the laser hits the blocks in a row or in a column

    // Use this for initialization
    void Start() {
        gm = FindObjectOfType<GameManager>();

        int colorN = Random.Range(0, gm.colors.Length);
        sp1.color = sp2.color = lineRenderer.startColor = lineRenderer.endColor = gm.colors[colorN]; //Setting colors randomly
    }

    // Update is called once per frame
    void Update() {
        if (!isVertical) {
            lineRenderer.SetPosition(0, new Vector3(-lineLength + transform.position.x, transform.position.y, 0));
            lineRenderer.SetPosition(1, new Vector3(lineLength + transform.position.x, transform.position.y, 0));

            collider.transform.localScale = new Vector3(lineLength * 40, 1, 1);
        } else {
            lineRenderer.SetPosition(0, new Vector3(transform.position.x, -lineLength + transform.position.y, 0));
            lineRenderer.SetPosition(1, new Vector3(transform.position.x, lineLength + transform.position.y, 0));

            collider.transform.localScale = new Vector3(1, lineLength * 40, 1);
        }
    }

    /// <summary>
    /// If the laser is hit by a ball then the effect starts
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Ball") {
            if (!inUsage)
                StartCoroutine(LaserUp());
        }
    }

    /// <summary>
    /// Starting the laser effect
    /// </summary>
    /// <returns></returns>
    IEnumerator LaserUp() {
        inUsage = true;
        float t = 0;

        while (t <= 7) {
            t += Time.deltaTime * 80;
            lineLength = t;

            yield return null;
        }

        lineLength = t;

        while (t > 0) {
            t -= Time.deltaTime * 80;
            lineLength = t;

            yield return null;
        }

        lineLength = 0;

        inUsage = false;
    }
}
