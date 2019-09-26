using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweCameraControl : MonoBehaviour
{
    public float cameraStretch = 1;//how far the camer moves

    public Vector3 cameraLocaiton;
    private Vector3 originalLocation;
    public float cameraSpeed;

    private Vector2 mouseRatio;

    public float baseDirectionalShake = .2f;//the default directional influence of shake

    void Start()
    {
        cameraLocaiton = transform.localPosition;
        originalLocation = cameraLocaiton;
    }

    void Update()
    {
        mouseRatio.x = (Input.mousePosition.x - (Screen.width / 2)) / (Screen.width / 2);
        mouseRatio.y = (Input.mousePosition.y - (Screen.width / 2)) / (Screen.width / 2);

        mouseRatio.x = Mathf.Clamp(mouseRatio.x, -1f, 1f);
        mouseRatio.y = Mathf.Clamp(mouseRatio.y, -1f, 1f);

        cameraLocaiton.x = originalLocation.x + (mouseRatio.x * cameraStretch);
        cameraLocaiton.z = originalLocation.z + (mouseRatio.y * cameraStretch);


        transform.localPosition = Vector3.Lerp(transform.localPosition, cameraLocaiton, cameraSpeed * Time.deltaTime);
    }

    public void ScreenShake(float shake)//keeping it very simple
    {


        Vector3 screenShake;

        screenShake.x = Random.Range(-shake,shake);
        screenShake.z = Random.Range(-shake, shake);
        screenShake.y = 0f;//no y, this is meant for top down,
        transform.localPosition += screenShake;
    }
}
