using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweParticleDestory : MonoBehaviour
{
    //this script deletes the partice systems game object when it has finished. For performance
    public ParticleSystem ps;

    void Start()
    {
        if (ps == null)
        {
            ps = GetComponent<ParticleSystem>();
        }

        if (ps == null)
        {
            Debug.LogError("ParticleDestory.cs on an object that does not have a particle system");
        }
    }

    void FixedUpdate()
    {
        if (ps)
        {
            if (!ps.IsAlive())
            {
                Destroy(gameObject);
                Destroy(this);
            }
        }
    }
}
