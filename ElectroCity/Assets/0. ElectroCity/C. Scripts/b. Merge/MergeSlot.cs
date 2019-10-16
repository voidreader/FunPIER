using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MergeSlot : MonoBehaviour, IDropHandler {

    public MergeItem mergeItem = null;



    /// <summary>
    /// 
    /// 박스 생성
    /// 
    /// </summary>
    /// <param name="l">유닛 레벨</param>
    /// <param name="isSpecialBox">스페셜 박스 여부</param>
    public void SpawnBox(int l, bool isSpecialBox = false) {

        MergeItem item = GameObject.Instantiate(Stock.main.ObjectMergeItem, this.transform, true).GetComponent<MergeItem>();
        item.SetMergeBox(l);

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
    /// <param name="u2">드래그하던애</param>
    public void MergeUnit(MergeItem u1, MergeItem u2) {
        Debug.Log("MergeUnit is just called!");
        MergeItem.IsMerging = true;

        u1.transform.SetParent(this.transform);
        u2.transform.SetParent(this.transform);

        u1.transform.localPosition = new Vector3(-40, 0, 0);
        u2.transform.localPosition = new Vector3(40, 0, 0);

        u1.transform.DOLocalMoveX(0, 0.4f).SetEase(Ease.InBack).OnComplete(()=>OnMergeLevelUp(u1));
        u2.transform.DOLocalMoveX(0, 0.4f).SetEase(Ease.InBack).OnComplete(() => OnMergeDestroy(u2));
    }

    void OnMergeLevelUp(MergeItem m) {
        m.LevelUp();
        MergeItem.IsMerging = false;
    }

    void OnMergeDestroy(MergeItem m) {
        Destroy(m.gameObject);
    }
}
