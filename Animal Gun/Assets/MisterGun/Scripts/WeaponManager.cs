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
    public Bullet BulletPrefab;

    private int _bulletsCount;
    private int _direction;

    public static float StartAimDistance;

    private void OnEnable()
    {
        ShootAction += Shoot;
        InitAction += Init;
    }

    private void OnDisable()
    {
        ShootAction -= Shoot;
        InitAction -= Init;
    }

    private void Start () {
        InitAction();
    }

    public void Init()
    {
        // _currenWeapon = Resources.Load<Weapon>("Weapons/Gun_" + PlayerPrefs.GetInt("CurrenWeapon"));
        _currenWeapon = Resources.Load<Weapon>("Weapons/Gun_6");
        CurrentAim.AimSpeed = _currenWeapon.AimSpeed;
        CurrentAim.AimRange = _currenWeapon.WeaponRange;
        CurentWeaponRenderer.sprite = _currenWeapon.WeaponSprite;
        _bulletsCount = _currenWeapon.BulletsCount;
        StartAimDistance = _currenWeapon.StartAimLineDistance;
        CurrentAim.Init();
        _direction = -1;
    }

    private void Shoot()
    {
        switch (_currenWeapon.CurrentType)
        {
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

    private void ShootWithGun()
    {
        _direction *= -1;
        AudioManager.Instance.PlayAudio(_currenWeapon.ShootSound);
        Bullet newBullet = Instantiate(BulletPrefab, null, false);
        newBullet.transform.position = GunpointTransform.position;
        newBullet.AddBulletForce(transform, _direction);
        AimController.Wait = true;
    }
    private IEnumerator ShootWithShotgun()
    {
        _direction *= -1;
        AudioManager.Instance.PlayAudio(_currenWeapon.ShootSound);
        for (int i = 0; i < _bulletsCount; i++)
        {
            float randPos = UnityEngine.Random.Range(-0.1f, 0.1f);
            Bullet newBullet = Instantiate(BulletPrefab, null, false);
            newBullet.transform.position = new Vector2(GunpointTransform.position.x, GunpointTransform.position.y + randPos * i);
            newBullet.AddBulletForce(transform, _direction);
            yield return new WaitForSeconds(0.01f);
        }
        AimController.Wait = true;
    }
    private IEnumerator ShootWithMachineGun()
    {
        _direction *= -1;
        for (int i = 0; i < _bulletsCount; i++)
        {
            AudioManager.Instance.PlayAudio(_currenWeapon.ShootSound);
            Bullet newBullet = Instantiate(BulletPrefab, null, false);
            newBullet.transform.position = GunpointTransform.position;
            newBullet.AddBulletForce(transform, _direction);
            yield return new WaitForSeconds(0.08f);
        }
        yield return new WaitForSeconds(0.2f);
        AimController.Wait = true;
    }
    private void Reload()
    {
        AudioManager.Instance.PlayAudio(_currenWeapon.ReloadSound);
    }
}
