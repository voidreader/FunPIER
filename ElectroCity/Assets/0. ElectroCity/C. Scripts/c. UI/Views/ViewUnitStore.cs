using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;

public class ViewUnitStore : MonoBehaviour
{
    List<UnitDataRow> ListUnitData;
    [SerializeField] List<UnitStoreColumn> ListColumns; // 컬럼 리스트 

    public ScrollRect SC;
    public Action OnPurchaseCallback = delegate { };


    public void OnView() {

        ListUnitData = UnitData.Instance.Rows;

        for (int i = 0; i < ListUnitData.Count; i++) {

            ListColumns[i].InitUnitStoreColumn(ListUnitData[i]);
        }

    }
}
