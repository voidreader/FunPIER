using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayFX : MonoBehaviour
{
    public static Action<Collision2D, int> EnableFxAction;

    // [0] - body, [1] - headshot, [2] - dead
    public ParticleSystem[] Fx;

    private void OnEnable()
    {
        EnableFxAction += EnableFx;
    }
    private void OnDisable()
    {
        EnableFxAction -= EnableFx;
    }

    public virtual void EnableFx(Collision2D collision, int particleId)
    {
        transform.position = collision.transform.position;
        Fx[particleId].Play();
    }
}
