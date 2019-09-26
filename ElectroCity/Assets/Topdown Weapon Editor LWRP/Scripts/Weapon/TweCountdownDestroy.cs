using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweCountdownDestroy : MonoBehaviour
{
    public float timer = 0;
    public float particleTimer = 0;
    private bool counting = false;
    int poolID = 0;
    TweWeapon weapon;
    ParticleSystem pt;
    bool usingTrail;
    bool clear = false;

    public void BeginCountdown(float time, int id, TweWeapon input)
    {
        timer = time;
        counting = true;
        poolID = id;
        weapon = input;
        usingTrail = true;
    }
    public void BeginCountdown(float time, int id, TweWeapon input, ParticleSystem ps)
    {
        pt = ps;
        particleTimer = ps.main.startLifetime.constantMax;
        timer = time;
        counting = true;
        poolID = id;
        weapon = input;
        usingTrail = true;
    }
    public void BeginCountdown(int id, TweWeapon input, ParticleSystem ps)
    {
        pt = ps;
        particleTimer = ps.main.startLifetime.constantMax;
        timer = 0;
        counting = true;
        poolID = id;
        weapon = input;
        usingTrail = false;
    }


    void Update()
    {
        

        if (counting)
        {
            particleTimer -= Time.deltaTime;
            timer -= Time.deltaTime;

            if (usingTrail && timer <= 0 && !clear)
            {
                gameObject.GetComponent<TrailRenderer>().Clear();//clear trails points. so basically brand new trail on restart
                clear = true;
            }


            if (timer <= 0 && particleTimer <=0)
            {
                if (weapon.usePool && poolID == weapon.poolID)
                {
                    weapon.objectPool.Enqueue(gameObject);
                    gameObject.SetActive(false);
                    Destroy(this);
                }
                else if (weapon.usePool && poolID == 0)
                {
                    Debug.LogError("Weapon is using pool but poolID is not being given to CountdownDestroy.cs make sure you are using the correct BeginCountdown overload. This error gets thrown because a Projectile had a poolID of zero");
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }              
            }
        }
    }
}