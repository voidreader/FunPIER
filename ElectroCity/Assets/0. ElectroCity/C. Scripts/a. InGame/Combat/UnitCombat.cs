using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombat : MonoBehaviour
{

    public TweWeapon weapon;
    public Transform aimPoint;
    private float shotTimer = 0;
    private bool isFiring = false;

    // Start is called before the first frame update
    void Start() {
        if (weapon.usePool) {
            weapon.CreatePool();//spawn pool on start, if you dont do this everything will still work, but the pool will instantiate the first time the player shoots.
        }

        isFiring = true;
    }

    // Update is called once per frame
    void Update()
    {
        //this script fires a the weapon using the aimPoint's transform.

        /*
        if (Input.GetButtonDown("Fire1")) {
            isFiring = true;
        }
        if (Input.GetButtonUp("Fire1")) {
            isFiring = false;
        }

        if (Input.GetButtonDown("Jump")) {
            weapon.CreatePool();
        }
        */


        if (isFiring) {
            if (shotTimer <= 0) {
                weapon.FireWeapon(aimPoint, null);
                //weapon.FireWeapon(aimPoint);
                shotTimer = weapon.fireRate;
                //cc.ScreenShake(weapon.screenShake);
            }
        }

        if (shotTimer > 0) {
            shotTimer -= Time.deltaTime;
        }

    }
}
