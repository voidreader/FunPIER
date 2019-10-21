using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;
using Doozy.Engine.Progress;

public class UnitStoreColumn : MonoBehaviour
{
    
    
    public UnitDataRow row; // 기준 데이터 

    public Text textLevel, textName, textEarning, textPower, textRPM, textPrice;
    public Image spriteUnit;

    public Progressor barPower, barRPM; // 2개 게이지 

    [SerializeField] TweWeapon weapon;


    public GameObject Cover; // 커버
    public GameObject btnPurchase;
    
    
    [SerializeField] int _level;
    [SerializeField] long _price;
    [SerializeField] int _step; // 구매 단계
    [SerializeField] string _debugPrice = string.Empty;
    



    /// <summary>
    /// 유닛 상점 컬럼 초기화
    /// </summary>
    /// <param name="r"></param>
    /// <param name="p"></param>
    public void InitUnitStoreColumn(UnitDataRow r) {
        this.gameObject.SetActive(true);
        SetLock(false);

        row = r;

        _level = r._level;
        row = Stock.GetMergeItemData(_level);
        // _price = long.Parse(r._price);
        


        

        textName.text = row._displayname;
        textLevel.text = _level.ToString(); // 레벨 
        weapon = Stock.GetFriendlyUnitWeapon(row._weaponid);
        // 무기 관련 프로그레서 처리 
        // textPower.text = row._damage.ToString();
        // textRPM.text = row._firerate.ToString();

        SetCurrentPrice();
        SetProgressor();

        // 스파라이트
        spriteUnit.sprite = Stock.GetFriendlyUnitUI(row._spriteUI);


        // 초당 획득 코인 정보 
        textEarning.text = row._earning + " / sec";


        _debugPrice = PIER.GetBigNumber(_price);
    }


    /// <summary>
    /// Damage, RPM 프로그레서 처리 
    /// </summary>
    void SetProgressor() {


        float rpm, power;

        // barPower.SetValue(power);
        // barRPM.SetValue(rpm);

        // RPM은 1~11까지 값이 들어있다. 11단계로 나누어주어야 함 
        // 각 단계마다 0.091 (1을 11단계로 나눈다)
        // RPM 처리
        rpm = (float)weapon.fireRateGrade * 0.091f;

        if (weapon.fireRateGrade >= 11) { // 11단계 발사속도면 MAX 처리 
            barRPM.SetValue(1);
            textRPM.text = "MAX";
        }
        else {
            barRPM.SetValue(rpm);
            textRPM.text = weapon.fireRateGrade.ToString();
        }

        // Power는 RPM의 반대다. 
        power = (float)(12 - weapon.fireRateGrade) * 0.091f;

        if(weapon.fireRateGrade <= 1) {
            barPower.SetValue(1);
            textPower.text = "MAX";
        }
        else {
            barPower.SetValue(power);
            textPower.text = (12 - weapon.fireRateGrade).ToString();
        }

        

    }



    public void SetLock(bool flag) {

        Cover.SetActive(flag);
        btnPurchase.SetActive(!flag);


    }

    void SetCurrentPrice() {
        _price = Unit.GetUnitCurrentPrice(_level);
        textPrice.text = PIER.GetBigNumber(_price); // 가격.. step 계산 포함. 
    }

    public void OnClickGet() {
        PIER.main.SaveUnitPurchaseStep(_level);
        SetCurrentPrice();

    }
}
