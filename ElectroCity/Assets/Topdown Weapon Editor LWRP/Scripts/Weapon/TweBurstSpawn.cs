using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweBurstSpawn : MonoBehaviour
{

    public float burstTime;
    public TweWeapon weapon;
    public int bulletNum;

    public TweBurstSpawn Initialize(float bt, TweWeapon w, int bn)
    {
        burstTime = bt;
        weapon = w;
        bulletNum = bn;
        return this;
    }

    void Update()
    {
        burstTime -= Time.deltaTime;
        if (burstTime <= 0)
        {
            weapon.ScreenShake();

            if (weapon.fireParticle != null)
            {
                Instantiate(weapon.fireParticle, transform.position, transform.rotation);//we do this here on burst weapons instead of inside the weapon.cs
            }

            if (weapon.fireSound != null)
            {
                AudioSource.PlayClipAtPoint(weapon.fireSound, transform.position);
            }

            if (weapon.usePool)
            {
                if (weapon.objectPool.Count == 0)
                {
                    weapon.AddToPool();
                }

                GameObject spawn = weapon.objectPool.Dequeue();

                spawn.transform.position = transform.position;
                spawn.transform.rotation = transform.rotation;
                TweProjectileControl pc = spawn.GetComponent<TweProjectileControl>();
                pc.aimpoint = transform;
                pc.bulletNum = bulletNum;
                spawn.SetActive(true);
                Destroy(this);
            }
            else
            {
                GameObject spawn = weapon.Projectile();

                spawn.transform.position = transform.position;
                spawn.transform.rotation = transform.rotation;
                TweProjectileControl pc = spawn.GetComponent<TweProjectileControl>();
                pc.aimpoint = transform;
                pc.bulletNum = bulletNum;
                spawn.SetActive(true);
                Destroy(this);
            }
        }
    }
}
