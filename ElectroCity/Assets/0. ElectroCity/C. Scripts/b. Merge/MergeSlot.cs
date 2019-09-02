using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MergeSlot : MonoBehaviour, IDropHandler {

    public MergeItem mergeItem = null;



    /// <summary>
    /// 박스 생성 
    /// </summary>
    public void SpawnBox(int l, bool isSpecialBox = false) {

        MergeItem item = GameObject.Instantiate(Stock.main.ObjectMergeItem, this.transform, true).GetComponent<MergeItem>();
        item.SetMergeBox(l);

        mergeItem = item;
    }

    public void EmptyOutSlot() {
        mergeItem = null;
    }


    public void OnDrop(PointerEventData eventData) {
        Debug.Log("OnDrop!");
    }
}
