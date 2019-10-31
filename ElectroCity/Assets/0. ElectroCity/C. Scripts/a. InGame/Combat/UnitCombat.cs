using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombat : MonoBehaviour
{

    public TweWeapon weapon;
    public Transform aimPoint;
    private float shotTimer = 0; // 발사세트 간격 
    float bulletTimer = 0; // 한 세트에서의 총알 발사 간격 
    private bool isFiring = false;

    int muzzleIndex = 0;
    public Unit unit;


    // Start is called before the first frame update
    void Start() {

        unit = this.gameObject.GetComponent<Unit>();


        if (weapon.usePool) {
            weapon.CreatePool();//spawn pool on start, if you dont do this everything will still work, but the pool will instantiate the first time the player shoots.
        }

        isFiring = true;
    }

    // Update is called once per frame
    void Update()
    {
        //this script fires a the weapon using the aimPoint's transform.

        if (!isFiring)
            return;

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

        // 샷건스타일이거나 단발 형식.
        if(weapon.bullets == 1 || weapon.bulletRate == 0 ) { 
            ShootSingle();
        }
        else {
            StartCoroutine(ShootMulti());
        }


    }

    void ShootSingle() {
        if (shotTimer <= 0) {
            
            // weapon.FireWeapon(GetAimPoint(), null);
            weapon.FireWeapon(GetAimPoint(), long.Parse(unit._data._attackfactor));
            shotTimer = weapon.fireRate;
            //cc.ScreenShake(weapon.screenShake);
        }

        if (shotTimer > 0) {
            shotTimer -= Time.deltaTime;
        }
    }

    IEnumerator ShootMulti() {
        isFiring = false;
        

        

        for (int i=0;i<weapon.bullets; i++) {

            while(bulletTimer > 0) {
                bulletTimer -= Time.deltaTime;
                yield return null;
            }

            weapon.FireWeapon(GetAimPoint(), long.Parse(unit._data._attackfactor));
            // weapon.FireWeapon(GetAimPoint(), null);
            bulletTimer = weapon.bulletRate;

        }


        yield return new WaitForSeconds(weapon.fireRate);
        // shotTimer = 0;

        isFiring = true;
    }





    Transform GetAimPoint() {
        

        // 총구가 여러개인 무기는 교차해서 발사 
        if (muzzleIndex >= weapon.muzzles)
            muzzleIndex = 0;

        return unit.GetAimPoint(muzzleIndex++);


        
    }


}
