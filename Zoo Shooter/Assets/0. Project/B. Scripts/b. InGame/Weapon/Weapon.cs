using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public enum WeaponType {
        Gun,
        Shotgun,
        MachineGun
    }

    public WeaponType CurrentType;
    public string WeaponID;
    public string DisplayName;


    public Sprite WeaponSprite;
    public AudioClip ShootSound;
    public AudioClip ReloadSound;

    public int BulletsCount;
    

    public float WeaponRange;
    public float AimSpeed;
    public float StartAimLineDistance;
}
