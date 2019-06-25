using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonGroup : MonoBehaviour {


    public GameObject[] buttons;

    public bool sizeChange;
    public float smallWidth;
    public float smallHeight;
    public float bigWidth;
    public float bigHeight;

    public bool colorChange;
    public bool textChange;

    public Color color1;

    void Start () {
        foreach (GameObject b in buttons) {
            b.GetComponent<Button>().onClick.AddListener(delegate { click(b); });
            notSelected(b);
        }

        selected(buttons[0]);
    }

    public void click(GameObject button) {
        foreach (GameObject b in buttons) {
            notSelected(b);
        }
        selected(button);
    }

    private void notSelected(GameObject b) {
        if (colorChange) {
            b.GetComponent<Image>().color = color1;
        }

        if(sizeChange)
            b.GetComponent<RectTransform>().sizeDelta = new Vector2(smallWidth, smallHeight);

        if (textChange)
            b.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 0.8f);
    }

    private void selected(GameObject b) {
        if (colorChange) {
            b.GetComponent<Image>().color = Color.black;
        }

        if(sizeChange)
            b.GetComponent<RectTransform>().sizeDelta = new Vector2(bigWidth, bigHeight);

        if (textChange)
            b.GetComponentInChildren<Text>().color = new Color(1,1,1,1);
    }
}
