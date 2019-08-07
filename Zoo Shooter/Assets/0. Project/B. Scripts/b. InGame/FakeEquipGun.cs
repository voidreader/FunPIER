using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeEquipGun : MonoBehaviour
{
    public SpriteRenderer WeaponSprite;
    Weapon weapon;

    public void SetFakeEquipWeapon(Vector3 pos, Weapon w) {
        this.transform.position = new Vector3(pos.x - 0.8f, pos.y - 0.6f, 0);

        SetWeapon(w);

        this.gameObject.SetActive(true);


    }

    public void SetWeapon(Weapon w) {
        weapon = w;
        WeaponSprite.sprite = Stocks.GetWeaponInGameSprite(w);

        WeaponSprite.transform.localScale = w.posScale;
    }

    public void SetHide() {
        this.gameObject.SetActive(false);
    }

}
