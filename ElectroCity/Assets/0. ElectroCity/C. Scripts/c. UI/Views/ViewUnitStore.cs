using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;

public class ViewUnitStore : MonoBehaviour
{

    UnlockDataRow row; 
    List<UnitDataRow> ListUnitData;
    [SerializeField] List<UnitStoreColumn> ListColumns; // 컬럼 리스트 

    public ScrollRect SC;
    public Action OnPurchaseCallback = delegate { };


    public void OnView() {

        row = UnlockData.Instance.Rows[PIER.main.HighestUnitLevel - 1]; // 언락 데이터 
        ListUnitData = UnitData.Instance.Rows;

        // 처음에 모두 락 처리
        for(int i=0; i<ListColumns.Count;i++) {
            ListColumns[i].InitUnitStoreColumn(ListUnitData[i], StoreColumnType.Locked);
        }


        // 코인 구매 가능한 상한선까지 Available 처리
        for(int i=0; i<row._coinlimit;i++) {
            ListColumns[i].InitUnitStoreColumn(ListUnitData[i], StoreColumnType.Available);
        }

        // 젬 구매 상한선 처리 
        for(int i=0; i<row._gemlimit; i++) {
            if (ListColumns[i].colType == StoreColumnType.Available)
                continue;

            if (i == row._gemlimit-1 || i == row._gemlimit - 2)
                ListColumns[i].InitUnitStoreColumn(ListUnitData[i], StoreColumnType.Special);
        }


        // 광고 구매 가능 상품처리 
        if(row._adlimit > 0) {
            ListColumns[row._adlimit - 1].InitUnitStoreColumn(ListUnitData[row._adlimit - 1], StoreColumnType.AD);
        }

        // HighestColumn 처리
        // ListColumns[PIER.main.HighestUnitLevel-1].InitUnitStoreColumn(listunitdata)

    }
}
