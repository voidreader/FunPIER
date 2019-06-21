using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {


    public Weapon EquipWeapon; // 장착한 무기 
    public AimController CurrentAim;  // Aim Controller 
    public SpriteRenderer CurentWeaponRenderer;


    public Transform GunpointTransform;
    public GameObject BulletPrefab;

    
    private int _direction;

    public static float StartAimDistance;



    /// <summary>
    /// 무기 초기화 
    /// </summary>
    public void Init() {
        EquipWeapon = Stocks.main.ListWeapons[0]; // 임시 
        CurentWeaponRenderer.sprite = EquipWeapon.WeaponSprite;

        this.transform.localPosition = EquipWeapon.posEquip; // 위치 설정 
        GunpointTransform.transform.localPosition = EquipWeapon.posGunPoint; // 건포인트 설정

        // Aim 설정
        CurrentAim.Init(EquipWeapon);


        _direction = 1;
    }

    public void Shoot() {
        switch (EquipWeapon.CurrentType) {
            case Weapon.WeaponType.Gun:
                ShootWithGun();
                break;
            case Weapon.WeaponType.Shotgun:
                StartCoroutine(ShootWithShotgun());
                break;
            case Weapon.WeaponType.MachineGun:
                StartCoroutine(ShootWithMachineGun());
                break;
        }

        Invoke("Reload", 1f);
    }

    void ShotBullet() {
        // PlayerBullet b = GameObject.Instantiate(BulletPrefab, null, false).GetComponent<PlayerBullet>();
        PlayerBullet b = GameObject.Instantiate(EquipWeapon.bullet.gameObject, null, false).GetComponent<PlayerBullet>();
        b.transform.position = GunpointTransform.position;
        b.transform.rotation = Quaternion.identity;
        

        b.AddBulletForce(transform, _direction);
    }


    /// <summary>
    /// 단발형 총 발사 
    /// </summary>
    private void ShootWithGun() {

        Debug.Log("Shoot With Gun");

        _direction *= -1;
        // AudioManager.Instance.PlayAudio(_currenWeapon.ShootSound);
        // Bullet newBullet = Instantiate(BulletPrefab, null, false);
        ShotBullet();
        // newBullet.transform.position = GunpointTransform.position;
        // newBullet.AddBulletForce(transform, _direction);

        // wait
        // AimController.Wait = true;
    }

    /// <summary>
    /// 샷건 발사 
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShootWithShotgun() {
        _direction *= -1;
        // AudioManager.Instance.PlayAudio(_currenWeapon.ShootSound);
        for (int i = 0; i < EquipWeapon.BulletsCount; i++) {
            /*
            float randPos = UnityEngine.Random.Range(-0.1f, 0.1f);
            Bullet newBullet = Instantiate(BulletPrefab, null, false);
            newBullet.transform.position = new Vector2(GunpointTransform.position.x, GunpointTransform.position.y + randPos * i);
            newBullet.AddBulletForce(transform, _direction);
            */
            ShotBullet();
            yield return new WaitForSeconds(0.01f);
        }
        // AimController.Wait = true;
    }

    /// <summary>
    /// 머신건 발사 
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShootWithMachineGun() {
        _direction *= -1;
        for (int i = 0; i < EquipWeapon.BulletsCount; i++) {
            // AudioManager.Instance.PlayAudio(_currenWeapon.ShootSound);
            /*
            Bullet newBullet = Instantiate(BulletPrefab, null, false);
            newBullet.transform.position = GunpointTransform.position;
            newBullet.AddBulletForce(transform, _direction);
            */

            ShotBullet();
            yield return new WaitForSeconds(0.08f);
        }
        yield return new WaitForSeconds(0.2f);
        // AimController.Wait = true;
    }

    /// <summary>
    /// 재장전(소리)
    /// </summary>
    private void Reload() {
        // AudioManager.Instance.PlayAudio(_currenWeapon.ReloadSound);
    }
}
