using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwePlayerCombat : MonoBehaviour
{
    public TweWeapon weapon;
    public Transform aimPoint;
    private float shotTimer;
    private bool isFiring = false;
    public GameObject Camera;
    private TweCameraControl cc;

    private void Start()
    {
        cc = Camera.GetComponent<TweCameraControl>();

        if (weapon.usePool)
        {
            weapon.CreatePool();//spawn pool on start, if you dont do this everything will still work, but the pool will instantiate the first time the player shoots.
        }

    }

    void Update()
    {
        //this script fires a the weapon using the aimPoint's transform.


        if (Input.GetButtonDown("Fire1"))
        {
            isFiring = true;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            isFiring = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            weapon.CreatePool();
        }


        if (isFiring )
        {
            if (shotTimer <= 0)
            {
                weapon.FireWeapon(aimPoint,cc);
                //weapon.FireWeapon(aimPoint);
                shotTimer = weapon.fireRate;
                //cc.ScreenShake(weapon.screenShake);
            }
        }

        if (shotTimer > 0)
        {
            shotTimer -= Time.deltaTime;
        }


    }
}
