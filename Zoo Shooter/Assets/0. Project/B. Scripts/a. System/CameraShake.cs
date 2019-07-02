﻿ using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static bool isShaking = false;
    public static CameraShake main = null;


    private void Awake() {
        main = this;
    }

    public void ShakeOnce(float duration, float magnitude) {
        if (isShaking)
            return;

        StartCoroutine(Shake(duration, magnitude));
    }

    public void ShakeOnceWithVertical(float duration, float magnitude) {
        if (isShaking)
            return;

        StartCoroutine(Shake(duration, magnitude, true));
    }


    IEnumerator Shake(float duration, float magnitude, bool isVertical = false) {


        Vector3 originalPos = transform.localPosition;
        isShaking = true;

        float elapsed = 0.0f;
        float x, y;
        while(elapsed < duration) {
            x = Random.Range(-1f, 1f) * magnitude;
            // y = Random.Range(-1f, 1f) * magnitude;

            if(isVertical) {
                y = Random.Range(-1f, 1f) * magnitude;
                transform.localPosition = new Vector3(x, y, originalPos.z);
            }
            else {
                transform.localPosition = new Vector3(x, originalPos.y, originalPos.z);
            }


            // transform.localPosition = new Vector3(x, y, originalPos.z);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        isShaking = false;
    }
}