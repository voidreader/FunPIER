using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;
using Doozy.Engine.Progress;

public enum StoreColumnType {
    Available,
    Locked,
    Special,
    AD
}

public class UnitStoreColumn : MonoBehaviour
{

    public StoreColumnType colType = StoreColumnType.Locked;
    public UnitDataRow row; // 기준 데이터 

    public Text textLevel, textName, textPrice, textDPS;
    // public Text textEarning, textPower, textRPM;

    public Image spriteFrame;
    public Image spriteUnit;
    public Image spriteLevelSign;

    // public Progressor barPower, barRPM; // 2개 게이지 

    [SerializeField] TweWeapon weapon;
    public Image btnPurchase;
    public Image btnIcon;

    // 하단 버튼에 들어가는 텍스트 + Icon 그룹 

    public GameObject GroupLock, GroupAvailable, GroupAD, GroupSpecial;
    public Text textGemPrice;
    
    
    [SerializeField] int _level;
    [SerializeField] long _price;
    [SerializeField] int _gemPrice;
    [SerializeField] int _step; // 구매 단계
    [SerializeField] string _debugPrice = string.Empty;
    



    /// <summary>
    /// 유닛 상점 컬럼 초기화
    /// </summary>
    /// <param name="r"></param>
    /// <param name="p"></param>
    public void InitUnitStoreColumn(UnitDataRow r, StoreColumnType t) {
        this.gameObject.SetActive(true);
        InitGroup();

        colType = t;
        row = r;

        _level = r._level;
        row = Stock.GetMergeItemData(_level);
        // _price = long.Parse(r._price);
       

        textName.text = row._displayname;
        textLevel.text = _level.ToString(); // 레벨 
        weapon = Stock.GetFriendlyUnitWeapon(row._weaponid);

        // 스파라이트0
        spriteUnit.sprite = Stock.GetFriendlyUnitUI(row._spriteUI);
        spriteUnit.color = Color.white;
        _debugPrice = PIER.GetBigNumber(_price);

        switch(colType) {
            case StoreColumnType.Available:
                SetAvailableColumn();
                break;
            case StoreColumnType.Locked:
                SetLockedColumn();
                break;
            case StoreColumnType.AD:
                SetADColumn();
                break;
            case StoreColumnType.Special:
                SetSpecialColumn();
                break;
        }

    }

    void InitGroup() {
        GroupAvailable.SetActive(false);
        GroupLock.SetActive(false);
        GroupSpecial.SetActive(false);
        GroupAD.SetActive(false);

    }

    /// <summary>
    /// Lock 상태의 컬럼 처리 
    /// </summary>
    void SetLockedColumn() {
        spriteFrame.sprite = Stock.main.SpriteStoreLocked;
        spriteLevelSign.sprite = Stock.main.SpriteLevelSignLocked;

        textName.text = "???";
        textDPS.text = "???";
        textDPS.color = Stock.main.ColorStoreLockedDPS;

        spriteUnit.color = Color.black;

        btnPurchase.sprite = Stock.main.SpriteStoreButtonLocked;
        GroupLock.SetActive(true);
    }

    /// <summary>
    /// 일반 코인 구매 상품 
    /// </summary>
    void SetAvailableColumn() {
        spriteFrame.sprite = Stock.main.SpriteStoreAvailable;
        spriteLevelSign.sprite = Stock.main.SpriteLevelSignAvailable;

        SetDPS();

        btnPurchase.sprite = Stock.main.SpriteStoreButtonAvailable;
        GroupAvailable.SetActive(true);

        SetCoinPrice();
    }

    /// <summary>
    /// 광고 유닛 
    /// </summary>
    void SetADColumn() {
        spriteFrame.sprite = Stock.main.SpriteStoreSpecial;
        spriteLevelSign.sprite = Stock.main.SpriteLevelSignSpecial;

        SetDPS();

        btnPurchase.sprite = Stock.main.SpriteStoreButtonAD;
        GroupAD.SetActive(true);
    }

    /// <summary>
    /// 보석 유닛
    /// </summary>
    void SetSpecialColumn() {
        spriteFrame.sprite = Stock.main.SpriteStoreSpecial;
        spriteLevelSign.sprite = Stock.main.SpriteLevelSignSpecial;

        SetDPS();

        btnPurchase.sprite = Stock.main.SpriteStoreButtonSpecial;
        GroupSpecial.SetActive(true);

        SetGemPrice();
    }




    /// <summary>
    /// DPS 처리
    /// </summary>
    void SetDPS() {

        if (colType == StoreColumnType.Available)
            textDPS.color = Stock.main.ColorStoreAvailableDPS;
        else if (colType == StoreColumnType.Locked)
            textDPS.color = Stock.main.ColorStoreLockedDPS;
        else
            textDPS.color = Stock.main.ColorStoreSpecialDPS;

        textDPS.text = Unit.GetDPS(row, weapon);
    }

    void SetCoinPrice() {
        _price = Unit.GetUnitCurrentPrice(_level);
        textPrice.text = PIER.GetBigNumber(_price); // 가격.. step 계산 포함. 
    }

    /// <summary>
    /// 보석 가격 설정(스페셜)
    /// </summary>
    void SetGemPrice() {
        if (row._level + 2 == PIER.main.HighestUnitLevel) {
            textGemPrice.text = "20";
            _gemPrice = 20;
        }
        else {
            textGemPrice.text = "10";
            _gemPrice = 10;
        }
    }

    public void OnClickGet() {

        if (colType == StoreColumnType.Available) {
            PIER.main.SaveUnitPurchaseStep(_level);
            SetCoinPrice();
        }

    }




    /*
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
*/
}
