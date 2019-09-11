﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
    

public class MergeItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public static bool IsMerging = false;

    public int Level = 0;
    public Machine ItemData; // 데이터 ScriptableObject

    public Image ImageItem; // 머지 아이템 이미지 
    public GameObject LevelSign;
    public Text TextLevel; // 단계 

    public bool IsPacked = false; // 상자 상태여부 


    // 드래그 시작시 정보 
    [SerializeField] Vector3 StartDragPosition;
    [SerializeField] Transform StartDragParent;



    /// <summary>
    /// 박스 처리 
    /// </summary>
    /// <param name="isSpecialBox"></param>
    public void SetMergeBox(int l, bool isSpecialBox = false) {
        IsPacked = true;
        ImageItem.sprite = Stock.main.SpriteBox;
        ImageItem.SetNativeSize();


        Level = l;
        ItemData = Stock.GetMergeItemData(Level); // 데이터 가져오기 

        HideLevel();

        this.transform.localPosition = Vector3.zero;
        this.transform.localScale = Vector3.zero;
        this.transform.DOScale(1, 0.25f).SetEase(Ease.OutBack);
    }


    /// <summary>
    /// 공개된 머지아이템 세팅 
    /// </summary>
    /// <param name="l"></param>
    public void SetMergeItem(int l) {

        Debug.Log(">> SetMergeItem :: " + l);

        Level = l;
        ItemData = Stock.GetMergeItemData(Level);
        ImageItem.sprite = ItemData.SpriteMergeUI;
        ImageItem.SetNativeSize();
        this.transform.localScale = Vector3.zero;
        this.transform.DOScale(1, 0.25f).SetEase(Ease.OutBack);

        IsPacked = false;

        SetLevel(Level);

    }

    /// <summary>
    /// 아이템 터치 
    /// </summary>
    public void OnClickItem() {

        // 개봉되었으면 return 
        if (!IsPacked)
            return;

        // 
        SetMergeItem(Level);

    }

    void SetLevel(int l) {
        LevelSign.SetActive(true);
        TextLevel.text = l.ToString();
    }

    void HideLevel() {
        LevelSign.SetActive(false);
    }


    /// <summary>
    /// 유닛 레벨업!
    /// </summary>
    public void LevelUp() {
        Level++;

        SetMergeItem(Level);
    }


    #region Drag & Drop

    public void OnBeginDrag(PointerEventData eventData) {

        if (IsPacked)
            return;

        MergeSystem.DraggingItem = this;
        StartDragPosition = this.transform.position;
        StartDragParent = this.transform.parent;

        GetComponent<CanvasGroup>().blocksRaycasts = false;

        transform.SetParent(MergeSystem.main.DragParent);
        LevelSign.SetActive(false);

    }

    public void OnDrag(PointerEventData eventData) {

        if (IsPacked)
            return;

        /*
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position += Vector3.back * transform.position.z;
        */
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {

        if (IsPacked)
            return;

        Debug.Log("OnEndDrop!");

        MergeSystem.DraggingItem = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        LevelSign.SetActive(true);


        if (IsMerging)
            return;



        
        // 올바르지 않은 위치, 같은 슬롯이면 원상 복구 
        if (MergeSystem.main.TargetSlot == null || StartDragParent == MergeSystem.main.TargetSlot.transform) {

            Debug.Log("Restore Position");

            this.transform.SetParent(StartDragParent);
            this.transform.localPosition = Vector3.zero;
        }
        else { // 빈 공간으로의 이동 

            Debug.Log("Move Position");

            // MergeItem 처리 
            StartDragParent.GetComponent<MergeSlot>().mergeItem = null;
            MergeSystem.main.TargetSlot.mergeItem = this; 

            this.transform.SetParent(MergeSystem.main.TargetSlot.transform); 
            this.transform.localPosition = Vector3.zero;

            this.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            this.transform.DOKill();
            this.transform.DOScale(1, 0.25f).SetEase(Ease.OutBack);
        }
        
        MergeSystem.main.SetTargetSlot(null);
        
    }

    #endregion
}