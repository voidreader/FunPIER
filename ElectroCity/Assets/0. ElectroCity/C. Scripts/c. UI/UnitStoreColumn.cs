using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;
using Doozy.Engine.Progress;

public class UnitStoreColumn : MonoBehaviour
{
    public Action OnPurchaseCallback = delegate { };
    public Machine machine;
    public UnitDataRow row; // 기준 데이터 

    public Text textLevel, textName, textEarning, textPower, textRPM, textPrice;
    public Image spriteUnit;

    public Progressor barPower, barRPM; // 2개 게이지 
    
    


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
    public void InitUnitStoreColumn(UnitDataRow r, Action p) {
        this.gameObject.SetActive(true);
        SetLock(false);

        row = r;
        OnPurchaseCallback = p;

        
            

        _level = r._level;
        machine = Stock.GetMergeItemData(_level);
        _price = long.Parse(r._price);
        textLevel.text = "LV " + _level.ToString(); // 레벨 
        textPrice.text = PIER.GetBigNumber(_price); // 가격.. step 계산 포함. 
        

        textName.text = row._displayname;



        // 프로그레서 처리 
        textPower.text = row._damage.ToString();
        textRPM.text = row._firerate.ToString();
        SetProgressor();
        
        // 스파라이트
        spriteUnit.sprite = machine.SpriteMergeUI;


        // 초당 획득 코인 정보 
        textEarning.text = row._earning + " / sec";


        _debugPrice = PIER.GetBigNumber(_price);
    }


    /// <summary>
    /// Damage, RPM 프로그레서 처리 
    /// </summary>
    void SetProgressor() {

        float power = row._damage / 100f;
        float rpm = row._firerate / 20f;

        barPower.SetValue(power);
        barRPM.SetValue(rpm);

    }



    public void SetLock(bool flag) {

        Cover.SetActive(flag);
        btnPurchase.SetActive(!flag);


    }
}
