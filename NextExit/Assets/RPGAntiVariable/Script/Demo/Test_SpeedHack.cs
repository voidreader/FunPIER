using UnityEngine;
using System.Collections;
using RPG.AntiVariable;

public class Test_SpeedHack : MonoBehaviour {

    public Transform Cube1;
    public Transform Cube2;

    bool isSpeedHack = false;

    // Update is called once per frame
    void Update()
    {
        Cube1.Rotate(Vector3.up, Time.deltaTime * 60f);
        Cube2.Rotate(Vector3.up, Time.deltaTime * 60f);
    }

    void OnDetection()
    {
        isSpeedHack = true;
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.width - 20));
        {
            GUILayout.Label("GameSpeed : x" + AntiManager.Instance.Speed);
            GUILayout.Label("isSpeedHack : " + isSpeedHack);
            GUILayout.Label("System.DateTime.Now.Ticks : " + System.DateTime.Now.Ticks);
            GUILayout.Label("unscaledTime : " + Time.unscaledTime);
            GUILayout.Label("deltaTime : " + Time.deltaTime);
            GUILayout.Label("timeScale : " + Time.timeScale);
        }
        GUILayout.EndArea();
    }
}
