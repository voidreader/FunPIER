using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunColumn : MonoBehaviour
{

    public Text _lblHowToGet, _lblGunName;
    public Image _gunImage;

    public GameObject _activeBG;
    public GameObject _lockCover;
    public Weapon _weapon;
    

    public void SetGunProduct(Weapon data) {
        this.gameObject.SetActive(true);

        _weapon = data;
        

        // 무기 이미지 
        _gunImage.sprite = Stocks.GetWeaponStoreSprite(_weapon);
        _gunImage.SetNativeSize();



        if (PIER.main.HasGun(_weapon)) {
            _lockCover.SetActive(false);
            _lblHowToGet.gameObject.SetActive(false);
            _lblGunName.gameObject.SetActive(true);
            _lblGunName.text = _weapon.DisplayName;
            _gunImage.color = Color.white;
        }
        else {
            _lockCover.SetActive(true);
            

            SetHowToGet(_weapon.HowToGet);

            _lblGunName.gameObject.SetActive(false);
            _gunImage.color = Color.black;

        }

        // 장착 무기 체크
        if(PIER.main.CurrentWeapon.name == _weapon.name) {
            _activeBG.SetActive(true);
        }
        else {
            _activeBG.SetActive(false);
        }

    }

    void SetHowToGet(WeaponGetType t) {

        _lblHowToGet.gameObject.SetActive(true);

        switch (t) {
            
            case WeaponGetType.Unlock250:
                _lblHowToGet.color = Stocks.main.ColorUnlock250;
                _lblHowToGet.text = "250\nCoins";
                break;

            case WeaponGetType.Unlock500:
                _lblHowToGet.color = Stocks.main.ColorUnlock500;
                _lblHowToGet.text = "500\nCoins";
                break;

            case WeaponGetType.Daily:
                _lblHowToGet.color = Stocks.main.ColorDailyReward;
                _lblHowToGet.text = "DAILY\nREWARD";
                break;

            case WeaponGetType.Wanted:
                _lblHowToGet.color = Stocks.main.ColorWantedReward;
                _lblHowToGet.text = "WANTED\nREWARD";
                break;

            case WeaponGetType.Specialist:
                _lblHowToGet.color = Stocks.main.ColorSpecialist;
                _lblHowToGet.text = "SPECIALIST";
                break;

        }
    }

   
}
