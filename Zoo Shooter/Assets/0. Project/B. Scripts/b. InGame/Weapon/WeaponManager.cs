﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WeaponManager : MonoBehaviour {

    public static bool isShooting = false;

    public bool isInit = false;
    public Weapon EquipWeapon; // 장착한 무기 
    public AimController CurrentAim;  // Aim Controller 
    public SpriteRenderer CurentWeaponRenderer;


    public Transform GunpointTransform;
    public GameObject BulletPrefab;

    RaycastHit2D hit;
    [SerializeField] Transform hitObject = null;
    
    private int _direction;

    public static float StartAimDistance;



    /// <summary>
    /// 무기 초기화 
    /// </summary>
    public void Init(bool force = false) {

        if (force)
            isInit = false;

        if (isInit)
            return;

        isInit = true;

        // EquipWeapon = Stocks.main.ListWeapons[0]; // 임시 
        EquipWeapon = PIER.main.CurrentWeapon;

        GameManager.main.currentWeapon = EquipWeapon;
        CurentWeaponRenderer.sprite = EquipWeapon.WeaponSprite;

        this.transform.localPosition = EquipWeapon.posEquip; // 위치 설정 
        this.transform.localScale = EquipWeapon.posScale;
        GunpointTransform.transform.localPosition = EquipWeapon.posGunPoint; // 건포인트 설정

        // Aim 설정
        CurrentAim.Init(EquipWeapon);

        _direction = -1; // 방향은 -1로 고정이다. 
    }


    void Update() {
        RayGunPoint();    
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
                StartCoroutine(ShootWithShotgun());
                Invoke("Reload", 1f);
                break;
            case WeaponType.MachineGun:
                StartCoroutine(ShootWithMachineGun());
                break;
        }

        

    }

    /// <summary>
    /// 
    /// </summary>
    public void RayGunPoint() {
        hit = Physics2D.Raycast(GunpointTransform.position, GunpointTransform.right * -1);

        if (hit.collider != null) {
            hitObject = hit.collider.transform;
            Debug.DrawLine(GunpointTransform.position, hit.collider.transform.position, Color.red);
        }


    }

    void ShotBullet(bool isMute = false) {
        // PlayerBullet b = GameObject.Instantiate(BulletPrefab, null, false).GetComponent<PlayerBullet>();
        PlayerBullet b = GameObject.Instantiate(EquipWeapon.bullet.gameObject, null, false).GetComponent<PlayerBullet>();
        b.transform.position = GunpointTransform.position;
        b.transform.rotation = Quaternion.identity;
        

        b.AddBulletForce(transform, _direction);

        // Shoot 재생 
        if (isMute)
            return;

        PlayShootSound();
    }

    void PlayShootSound() {
        if (EquipWeapon.ShootSound) {
            AudioAssistant.Shot(EquipWeapon.ShootSound);
        }
    }

    /// <summary>
    /// 총기 흔들림효과 
    /// </summary>
    void ShakeAimRotation() {
        float acc = 1 - (EquipWeapon.Accuracy / 100);
        // Debug.Log("ShakeAimRotate :: " + acc);
        transform.Rotate(0, 0, 1 * UnityEngine.Random.Range(-acc, acc) * 10);

    }


    /// <summary>
    /// 단발형 총 발사 
    /// </summary>
    private void ShootWithGun() {

        Debug.Log("Shoot With Gun");
        ShotBullet();
        this.transform.DOLocalMove(Vector3.forward * 8, 0.15f).SetLoops(2, LoopType.Yoyo);

        // wait
        AimController.Wait = true; // 한방 쏘고 더이상 조준하지 않음.. 
        isShooting = false;
    }

    /// <summary>
    /// 샷건 발사 
    /// </summary>
    /// <returns></returns>
    IEnumerator ShootWithShotgun() {

        Quaternion originalRot = this.transform.rotation; // 이전 총구 회전값을 가지고 있는다.
        AimController.Wait = true;

        // Audio
        for (int i =0; i< EquipWeapon.BulletsCount; i++) {
            // 가장 첫번째 총알은 조준한대로 발사된다. 
            if (i == 0)
                ShotBullet();
            else {
                this.transform.rotation = originalRot; 
                ShakeAimRotation(); // 2번째 총알부터는 정확성에 따라 발사. 
                ShotBullet(true);
            }

            yield return new WaitForSeconds(0.01f);
        }

        this.transform.DOLocalMove(Vector3.forward * 10, 0.15f).SetLoops(2, LoopType.Yoyo);
        isShooting = false;
    }



    /// <summary>
    /// 머신건 발사 
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShootWithMachineGun() {

        Debug.Log("ShootWithMachineGun :: " + EquipWeapon.BulletsCount);
        AimController.Wait = true;

        for (int i = 0; i < EquipWeapon.BulletsCount; i++) {

            ShotBullet();
            // 쏘고 나서 정확도 만큼 틀어지게 만든다. 
            ShakeAimRotation();

            yield return new WaitForSeconds(EquipWeapon.FireRate);
        }

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
        else { // 맞췄을때만 리로드 사운드 
            if (EquipWeapon.ReloadSound) {
                AudioAssistant.Shot(EquipWeapon.ReloadSound);
            }
        }

    }
}
