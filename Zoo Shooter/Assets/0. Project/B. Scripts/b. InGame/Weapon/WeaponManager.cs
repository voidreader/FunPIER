using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {

    public static Action ShootAction;
    public static Action InitAction;

    private Weapon _currenWeapon;
    public AimController CurrentAim;
    public SpriteRenderer CurentWeaponRenderer;
    public Transform GunpointTransform;
    public GameObject BulletPrefab;

    private int _bulletsCount;
    private int _direction;

    public static float StartAimDistance;

    private void OnEnable() {
        ShootAction += Shoot;
        InitAction += Init;
    }

    private void OnDisable() {
        ShootAction -= Shoot;
        InitAction -= Init;
    }

    private void Start() {
        InitAction();
    }

    /// <summary>
    /// 무기 초기화 
    /// </summary>
    public void Init() {
        _currenWeapon = Stocks.main.ListWeapons[0];
        CurrentAim.AimSpeed = _currenWeapon.AimSpeed;
        CurrentAim.AimRange = _currenWeapon.WeaponRange;
        CurentWeaponRenderer.sprite = _currenWeapon.WeaponSprite;
        _bulletsCount = _currenWeapon.BulletsCount;
        StartAimDistance = _currenWeapon.StartAimLineDistance;
        CurrentAim.Init();
        _direction = -1;
    }

    public void Shoot() {
        switch (_currenWeapon.CurrentType) {
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
        PlayerBullet b = GameObject.Instantiate(BulletPrefab, null, false).GetComponent<PlayerBullet>();
        b.transform.position = GunpointTransform.position;
        b.transform.rotation = Quaternion.identity;
        // b.transform.rotation = this.transform.rotation;

        b.AddBulletForce(transform, _direction);
    }


    /// <summary>
    /// 단발형 총 발사 
    /// </summary>
    private void ShootWithGun() {
        _direction *= -1;
        // AudioManager.Instance.PlayAudio(_currenWeapon.ShootSound);
        // Bullet newBullet = Instantiate(BulletPrefab, null, false);
        ShotBullet();
        // newBullet.transform.position = GunpointTransform.position;
        // newBullet.AddBulletForce(transform, _direction);
        AimController.Wait = true;
    }

    /// <summary>
    /// 샷건 발사 
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShootWithShotgun() {
        _direction *= -1;
        // AudioManager.Instance.PlayAudio(_currenWeapon.ShootSound);
        for (int i = 0; i < _bulletsCount; i++) {
            /*
            float randPos = UnityEngine.Random.Range(-0.1f, 0.1f);
            Bullet newBullet = Instantiate(BulletPrefab, null, false);
            newBullet.transform.position = new Vector2(GunpointTransform.position.x, GunpointTransform.position.y + randPos * i);
            newBullet.AddBulletForce(transform, _direction);
            */
            ShotBullet();
            yield return new WaitForSeconds(0.01f);
        }
        AimController.Wait = true;
    }

    /// <summary>
    /// 머신건 발사 
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShootWithMachineGun() {
        _direction *= -1;
        for (int i = 0; i < _bulletsCount; i++) {
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
        AimController.Wait = true;
    }

    /// <summary>
    /// 재장전(소리)
    /// </summary>
    private void Reload() {
        // AudioManager.Instance.PlayAudio(_currenWeapon.ReloadSound);
    }
}
