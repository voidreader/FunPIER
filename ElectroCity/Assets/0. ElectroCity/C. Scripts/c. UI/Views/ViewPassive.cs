using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;

public class ViewPassive : MonoBehaviour
{
    List<PassiveDataRow> ListPassiveData;
    List<PassiveDataRow> ListDPS = new List<PassiveDataRow>(); // DPS 기준정보 리스트 
    List<PassiveDataRow> ListDiscount = new List<PassiveDataRow>(); // Discount 기준정보 리스트 

    [SerializeField] List<PassiveColumn> ListColumns; // 컬럼 리스트 

    int DamageLevel, DiscountLevel;

    public void OnView() {

        int dpsIndex, discountIndex;

        for(int i=0; i<ListColumns.Count; i++) {
            ListColumns[i].gameObject.SetActive(false);
        }

        // 리스트 가져오기 
        ListPassiveData = PassiveData.Instance.Rows;

        ListDPS.Clear();
        ListDiscount.Clear();


        // DPS와 Discount로 분류
        for (int i = 0; i < ListPassiveData.Count; i++) {

            if (ListPassiveData[i]._rid.Contains("DAMAGE"))
                ListDPS.Add(ListPassiveData[i]);
            else
                ListDiscount.Add(ListPassiveData[i]);
        }


        dpsIndex = 0;
        discountIndex = 0;

        for(int i=0; i<20; i++) {
            // 번갈아가면서 나오도록 처리
            if (i % 2 == 0)
                ListColumns[i].InitPassiveColumn(ListDPS[dpsIndex++]);
            else
                ListColumns[i].InitPassiveColumn(ListDiscount[discountIndex++]);
        }

        // DamageLevel = PIER.main

        // 리스트의 순서는 데미지, 할인이 번갈아 나오도록 처리 한다. 
        // 데이터는 0~9까지 10~19까지 할인. 

        // ListColumns[0].InitPassiveColumn(ListPassiveData[0]);


    }
}
