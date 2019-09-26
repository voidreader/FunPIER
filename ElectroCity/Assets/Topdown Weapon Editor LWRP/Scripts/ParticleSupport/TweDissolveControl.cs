using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweDissolveControl : MonoBehaviour
{
    //This script is used to create the dissolve effects on certain particle 

    ParticleSystemRenderer psr;
    ParticleSystem ps;

    public float maxTime;
    public float timer;
    public float currentTime;

    void Start()
    {
        psr = GetComponent<ParticleSystemRenderer>();
        ps = GetComponent<ParticleSystem>();
        maxTime = ps.main.startLifetime.constant + ps.main.startDelay.constant;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (psr == null)
        {
            Debug.Log("DissoveControl script on an object with no particle system FIX");
        }
        currentTime = timer / maxTime;
        psr.material.SetFloat("_DISSOLVETIME", currentTime);


        if (ps.isStopped)
        {
            timer = 0;
            currentTime = 0;
        }

    }
}
