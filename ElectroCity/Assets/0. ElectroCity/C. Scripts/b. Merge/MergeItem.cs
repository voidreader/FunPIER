using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Google2u;
    

public class MergeItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public static bool IsMerging = false;

    public bool IsSpecialBox = false;
    public int Level = 0;
    
    public UnitDataRow unitRow;

    public Image ImageItem; // 머지 아이템 이미지 
    public GameObject LevelSign;
    public Text TextLevel; // 단계 

    public bool IsPacked = false; // 상자 상태여부 
    public bool IsBattle = false; // 전투 중 여부 


    // 드래그 시작시 정보 
    [SerializeField] Vector3 StartDragPosition;
    [SerializeField] Transform StartDragParent;

    public MergeSlot Slot; // 위치한 슬롯

    [Header("Back Light Effect")]
    public GameObject GroupBackLight;
    public Transform BackCircle, BackFat, BackThin;

    /// <summary>
    /// 박스 처리 
    /// </summary>
    /// <param name="isSpecialBox"></param>
    public void SetMergeBox(MergeSlot s, bool isSpecialBox = false) {
        IsPacked = true;
        ImageItem.sprite = Stock.main.SpriteBox;
        ImageItem.SetNativeSize();
        IsSpecialBox = isSpecialBox;

        ImageItem.transform.localScale = new Vector3(0.55f, 0.55f, 1); // 

        Slot = s;

        
        unitRow = Stock.GetMergeItemData(Level);

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
        unitRow = Stock.GetMergeItemData(Level);


        ImageItem.sprite = Stock.GetFriendlyUnitUI(unitRow._spriteUI);
        ImageItem.SetNativeSize();
        ImageItem.transform.localScale = new Vector3(0.55f, 0.55f, 1); // 머지 유닛의 크기는 0.55

        this.transform.localPosition = Vector3.zero;
        this.transform.localScale = Vector3.zero;
        this.transform.DOScale(1, 0.25f).SetEase(Ease.OutBack);

        IsPacked = false;

        SetLevel(Level);

    }

    /// <summary>
    /// 아이템 터치 
    /// </summary>
    public void OnClickItem() {

        // 전투에서 불러오기 
        if (IsBattle) {
            GameManager.main.CallbackBattleUnit(this);
            return;
        }

        // 개봉되었으면 return 
        if (!IsPacked)
            return;


        // 
        if(IsSpecialBox)
            SetMergeItem(MergeSystem.GetSpecialBoxUnitLevel());
        else
            SetMergeItem(MergeSystem.GetRandomBoxUnitLevel());


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

        Debug.Log(">> Unit Merge !! :: " + Level);

        MergeSystem.main.CheckUnlock(unitRow); // UNLOCK 체크 호출
        PlayerInfo.main.AddExp(Level); // 경험치 추가

    }


    /// <summary>
    /// 드래그할때 희미하게 처리 
    /// </summary>
    /// <param name="flag"></param>
    public void SetDragDim(bool flag) {

        if(IsBattle) {
            TextLevel.gameObject.SetActive(true);
            return;
        }

        if (flag) {
            ImageItem.color = new Color(1, 1, 1, 0.6f);
            TextLevel.gameObject.SetActive(false);
        }
        else {
            ImageItem.color = new Color(1, 1, 1, 1);
            TextLevel.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 유닛이 필드에 나갈때 실행
    /// </summary>
    /// <param name="flag"></param>
    public void SetBattle(bool flag) {

        // 숫자는 그대로고, 이미지만 
        IsBattle = flag;
        if(flag) {
            
            ImageItem.color = new Color(1, 1, 1, 0.6f); 
        }
        else {
            ImageItem.color = new Color(1, 1, 1, 1);
        }
    }



    public void OnBackLight() {
        GroupBackLight.SetActive(true);

        // 
    }


    #region Drag & Drop

    public void OnBeginDrag(PointerEventData eventData) {

        if (IsPacked)
            return;

        MergeSystem.DraggingItem = this;
        StartDragPosition = this.transform.position;
        StartDragParent = this.transform.parent;

        GetComponent<CanvasGroup>().blocksRaycasts = false;

        // 화면상 순서때문에 다른 Parent를 Set 
        transform.SetParent(MergeSystem.main.DragParent);
        LevelSign.SetActive(false);

        SetDragDim(true);

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

        Debug.Log(">> MergeItem OnEndDrop :: " + this.name);

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
        SetDragDim(false);
        
    }

    #endregion
}
