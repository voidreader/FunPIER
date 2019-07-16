using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunDetailView : MonoBehaviour
{

    public Text lblType, lblName, lblDamage, lblBullets;
    public Image weaponSprite;
    public Weapon weapon; 


    public void OnView() {

        weapon = PIER.main.CurrentWeapon;

        lblName.text = weapon.DisplayName;
        lblDamage.text = weapon.Damage.ToString();
        lblBullets.text = weapon.BulletsCount.ToString();

        weaponSprite.sprite = Stocks.GetWeaponStoreSprite(weapon);
        weaponSprite.SetNativeSize();
        weaponSprite.transform.localScale = new Vector3(1.5f, 1.5f, 1);

        switch(weapon.CurrentType) {
            case WeaponType.Gun:
                lblType.text = "SINGLE SHOT";
                break;

            case WeaponType.Shotgun:
                lblType.text = "SHOTGUN";
                break;

            case WeaponType.MachineGun:
                lblType.text = "MACHINE GUN";
                break;
        }


    }

}
