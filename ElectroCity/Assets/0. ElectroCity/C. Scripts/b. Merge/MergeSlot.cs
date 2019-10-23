﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MergeSlot : MonoBehaviour, IDropHandler {

    public MergeItem mergeItem = null;



    MergeItem GetNewMergeItem() {
        MergeItem item = GameObject.Instantiate(Stock.main.ObjectMergeItem, this.transform, true).GetComponent<MergeItem>();
        item.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        item.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);

        return item;
    }


    public void SpawnMergeUnitInstantly(int l) {

        MergeItem item = GetNewMergeItem();
        item.Slot = this;
        item.SetMergeItem(l);
        mergeItem = item;
    }

    /// <summary>
    /// 
    /// 박스 생성
    /// 
    /// </summary>
    /// <param name="l">유닛 레벨</param>
    /// <param name="isSpecialBox">스페셜 박스 여부</param>
    public void SpawnBox(bool isSpecialBox = false) {

        MergeItem item = GetNewMergeItem();
        item.SetMergeBox(this);

        mergeItem = item;
    }

    public void EmptyOutSlot() {
        mergeItem = null;
    }


    public void OnDrop(PointerEventData eventData) {

        Debug.Log(">> OnDrop :: " + this.name);
        

        // 아이템 존재하는 경우 
        if(mergeItem) {
            // 머지 체크 
            Debug.Log("OnDrop Not Empty! :: + " + this.gameObject.name);

            if(mergeItem.Level == MergeSystem.DraggingItem.Level) {
                // 머지!
                MergeUnit(mergeItem, MergeSystem.DraggingItem);
            }
            else {
                // .... 
                MergeSystem.main.SetTargetSlot(null);
                
            }

        }
        else { // 빈 슬롯 
            Debug.Log("OnDrop Empty! :: + " + this.gameObject.name);
            MergeSystem.main.SetTargetSlot(this);
        }


    }

    /// <summary>
    /// 두개의 머지 아이템 합치기 
    /// </summary>
    /// <param name="u1">가만히있던애</param>
    /// <param name="u2">드래그되던애</param>
    public void MergeUnit(MergeItem u1, MergeItem u2) {
        Debug.Log("MergeUnit is just called!");

        // 드래그되던 유닛의 Slot을 비워준다
        u2.Slot.mergeItem = null;

        MergeItem.IsMerging = true; // Merge 연출을 위한 거였는데.. 필요없어진듯..?

        OnMergeLevelUp(u1);
        OnMergeDestroy(u2);

        // u1.transform.SetParent(this.transform);
        // u2.transform.SetParent(this.transform);

        /*
        u1.transform.localPosition = new Vector3(-40, 0, 0);
        u2.transform.localPosition = new Vector3(40, 0, 0);

        u1.transform.DOLocalMoveX(0, 0.4f).SetEase(Ease.InBack).OnComplete(()=>OnMergeLevelUp(u1));
        u2.transform.DOLocalMoveX(0, 0.4f).SetEase(Ease.InBack).OnComplete(() => OnMergeDestroy(u2));
        */
    }

    void OnMergeLevelUp(MergeItem m) {
        m.LevelUp();
        MergeItem.IsMerging = false;
    }

    void OnMergeDestroy(MergeItem m) {
        Destroy(m.gameObject);
    }
}
