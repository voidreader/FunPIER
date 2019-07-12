using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WeaponType {
    Gun,
    Shotgun,
    MachineGun
}

public enum WeaponGetType {
    Unlock250,
    Daily,
    Wanted,
    Specialist,
    Unlock500
}
[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject {


    public WeaponType CurrentType;
    public WeaponGetType HowToGet;

    public string WeaponID;

    [TextArea]
    public string DisplayName;


    public Sprite WeaponSprite;
    public AudioClip ShootSound; // 발사 사운드 
    public AudioClip ReloadSound; // 재장전 사운드
    public PlayerBullet bullet;
    public int BulletsCount; // 총알 수
    public int Damage; // 데미지
    public float FireRate = 0.1f;
    

    public float AimSpeed; // Aim Speed
    public float AimRange; // Aim 길이

    public Vector3 posEquip; // 장착 위치 
    public Vector3 posGunPoint; // 발사위치
    public Vector3 posScale; // 장착 크기 
}
