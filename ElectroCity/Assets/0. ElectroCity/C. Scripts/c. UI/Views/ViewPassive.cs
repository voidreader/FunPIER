using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;

public class ViewPassive : MonoBehaviour
{
    List<PassiveDataRow> ListPassiveData;
    int DamageLevel, DiscountLevel;

    public void OnView() {

        // 리스트 가져오기 
        ListPassiveData = PassiveData.Instance.Rows;

        // DamageLevel = PIER.main

        
    }
}
