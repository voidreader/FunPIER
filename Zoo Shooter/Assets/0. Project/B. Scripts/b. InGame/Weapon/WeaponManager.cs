﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {

    public static bool isShooting = false;

    public bool isInit = false;
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

        if (isInit)
            return;

        isInit = true;

        EquipWeapon = Stocks.main.ListWeapons[0]; // 임시 
        GameManager.main.currentWeapon = EquipWeapon;
        CurentWeaponRenderer.sprite = EquipWeapon.WeaponSprite;

        this.transform.localPosition = EquipWeapon.posEquip; // 위치 설정 
        this.transform.localScale = EquipWeapon.posScale;
        GunpointTransform.transform.localPosition = EquipWeapon.posGunPoint; // 건포인트 설정

        // Aim 설정
        CurrentAim.Init(EquipWeapon);

        _direction = -1; // 방향은 -1로 고정이다. 
    }

    public void SetDrop(bool isLeft) {
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        this.GetComponent<Rigidbody2D>().isKinematic = false;

        if (isLeft)
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(UnityEngine.Random.Range(-150f, -20f), UnityEngine.Random.Range(100f, 250f)));
        else
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(UnityEngine.Random.Range(20f, 150f), UnityEngine.Random.Range(100f, 250f)));

        this.GetComponent<Rigidbody2D>().AddTorque(360, ForceMode2D.Impulse);
        this.gameObject.layer = 15;


        Destroy(this.gameObject, 4);
    }

    public void AfterShoot() {
        CurrentAim.ResetAim();
    }

    public void Shoot() {

        PlayerBullet.isHitEnemy = false;
        isShooting = true;

        switch (EquipWeapon.CurrentType) {
            case WeaponType.Gun:
                ShootWithGun();
                Invoke("Reload", 1f);
                break;
            case WeaponType.Shotgun:
                ShootWithShotgun();
                Invoke("Reload", 1f);
                break;
            case WeaponType.MachineGun:
                StartCoroutine(ShootWithMachineGun());
                break;
        }

        
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
        ShotBullet();

        // wait
        AimController.Wait = true; // 한방 쏘고 더이상 조준하지 않음.. 
        isShooting = false;
    }

    /// <summary>
    /// 샷건 발사 
    /// </summary>
    /// <returns></returns>
    private void ShootWithShotgun() {

        float randPos;

        // Audio
        for(int i =0; i< EquipWeapon.BulletsCount; i++) {
            randPos = UnityEngine.Random.Range(-0.1f, 0.1f);
            PlayerBullet b = GameObject.Instantiate(EquipWeapon.bullet.gameObject, null, false).GetComponent<PlayerBullet>();
            b.transform.position = new Vector2(GunpointTransform.position.x, GunpointTransform.position.y + randPos);
            b.transform.rotation = Quaternion.identity;
            b.AddBulletForce(transform, _direction);
        }

        AimController.Wait = true;
        isShooting = false;
    }

    /// <summary>
    /// 머신건 발사 
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShootWithMachineGun() {

        Debug.Log("ShootWithMachineGun :: " + EquipWeapon.BulletsCount);

        for (int i = 0; i < EquipWeapon.BulletsCount; i++) {

            ShotBullet();
            yield return new WaitForSeconds(0.1f);
        }

        AimController.Wait = true;

        yield return new WaitForSeconds(0.4f);
        isShooting = false;
        Reload();
    }

    /// <summary>
    /// 재장전(소리)
    /// </summary>
    private void Reload() {

        Debug.Log("Reload Check! : " + PlayerBullet.isHitEnemy);

        // AudioManager.Instance.PlayAudio(_currenWeapon.ReloadSound);
        if (!PlayerBullet.isHitEnemy) {
            GameManager.isMissed = true;
        }

    }
}
