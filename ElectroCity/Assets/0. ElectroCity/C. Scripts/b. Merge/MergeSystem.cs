using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.Events;
using Google2u;

public class MergeSystem : MonoBehaviour
{
    public static MergeSystem main = null;
    public static int MergeIncrementalID = 0; // 머지 유닛 생성때마다 발급 ID, BattlePosition - MergeItem 간의 연결고리! 너와 나의! 연결... :(
    public static bool isInitialized = false; // 초기화 여부 - GameManager에서 참조

    const string KeyMergeID = "KeyMergeID";

    [Header(" - Drag Item - ")]
    public static MergeItem DraggingItem;


    [Header( "- Slots - ")]
    public List<MergeSlot> ListSlots;
    public List<MergeSlot> ListEmptySlots;
    public int AvailableMergeSlotCount = 0;

    public float SecondSpawnBox;

    public Transform DragParent;
    public MergeSlot TargetSlot; // 드래그 하는 머지아이템이 이동할 슬롯 

    static UnlockDataRow CurrentUnlockData = null;

    MergeSlot slot;
    public float SpawnBoxTime = 0;

    [Header(" - Instant Purchase Unit -")]
    public Image SpriteInstantUnit;
    public Text TextInstantPrice;
    public long PriceInstantPurchase;

    

    #region static method GetNewUnitLevel

    /// <summary>
    /// Merge ID  가져오기 
    /// </summary>
    /// <returns></returns>
    public static int GetMergeIncrementalID() {

        MergeIncrementalID = PlayerPrefs.GetInt(KeyMergeID, 0);

        if (MergeIncrementalID > 60000)
            MergeIncrementalID = 0;

        MergeIncrementalID++;
        PlayerPrefs.SetInt(KeyMergeID, MergeIncrementalID); // 가장 마지막에 사용된 ID를 저장.
        PlayerPrefs.Save();

        return MergeIncrementalID;
    }

    /// <summary>
    /// 랜덤박스 유닛 레벨 가져오기 
    /// </summary>
    /// <returns></returns>
    public static int GetRandomBoxUnitLevel() {

        int limitLevel = 0;
        int rand = Random.Range(0, 100);
        

        CurrentUnlockData = UnlockData.Instance.Rows[PIER.main.HighestUnitLevel - 1];
        limitLevel = CurrentUnlockData._boxlimit;

        // limitLevel -3 부터 차례대로 10% 40% 40% 10% 로 받는다 
        // 그런데 -3 이 1보다 작을 수도 있음 

        if (limitLevel == 1)
            return 1;
        else if (limitLevel == 2) {
            if (rand < 60)
                return 1;
            else
                return 2;
        }
        else if (limitLevel == 3) {
            if (rand < 40)
                return 1;
            else if (rand >= 40 && rand < 80)
                return 2;
            else
                return 3;

        }
        else {

            if (rand < 10)
                return limitLevel-3;
            else if (rand >= 10 && rand < 50)
                return limitLevel-2;
            else if (rand >= 50 && rand < 90)
                return limitLevel-1;
            else
                return limitLevel;
        }

    }

    /// <summary>
    /// 스페셜 박스 유닛 레벨 가져오기 
    /// </summary>
    /// <returns></returns>
    public static int GetSpecialBoxUnitLevel() {
        return UnlockData.Instance.Rows[PIER.main.HighestUnitLevel - 1]._sbox;
    }

    #endregion


    private void Awake() {
        main = this;
    }

    IEnumerator Start() {


        while (!PIER.isInitialized)
            yield return null;

        // Start 메소드의 경우 순서가 필요하다.
        // MergeSystem 먼저, 그다음이 GameManager
        ListEmptySlots = new List<MergeSlot>();
        SetAvailableMergeSpot(); // 레벨에 따른 사용가능 머지 공간 
        SetMergeSpotMemory(); // 포지션 불러오기 
        SetInstantPurchaseUnit(); // 빠른구매 설정 

        isInitialized = true;

    }

    private void Update() {

        SpawnBoxTime += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.A)) {
            SpawnBoxToEmptySlot();
        }


        if(SpawnBoxTime > 60) {
            SpawnBoxTime = 0;
            SpawnBoxToEmptySlot();
        }

    }

    /// <summary>
    /// 플레이어 레벨에 따른 머지 공간 개수 설정
    /// </summary>
    public void SetAvailableMergeSpot() {
        AvailableMergeSlotCount = PlayerInfo.GetAvailableMergeSpot();

        // 초기화 후 
        for(int i =0; i<ListSlots.Count;i++) {
            ListSlots[i].IsAvailable = false;
        }


        // 사용 가능 개수만큼 불러오기 
        for(int i=0; i<AvailableMergeSlotCount;i++) {
            ListSlots[i].IsAvailable = true;
        }
    }


    /// <summary>
    /// 저장된 머지 공간 정보 세팅 
    /// </summary>
    public void SetMergeSpotMemory() {


        for (int i = 0; i < AvailableMergeSlotCount; i++) {

            // Spot 정보는 -2 : 스페셜 박스, -1 : 걍 박스, 0 : 비었음. 
            switch (PIER.main.ArrSpotMemory[i]) {
                case -2:
                    ListSlots[i].SpawnBox(true);
                    ListSlots[i].mergeItem.SetMergeIncrementalID(PIER.main.ArrSpotIncrementalIDMemory[i]);
                    break;
                case -1:
                    ListSlots[i].SpawnBox(false);
                    ListSlots[i].mergeItem.SetMergeIncrementalID(PIER.main.ArrSpotIncrementalIDMemory[i]);
                    break;

                case 0:
                    break;

                default:
                    ListSlots[i].SpawnMergeUnitInstantly(PIER.main.ArrSpotMemory[i]);
                    ListSlots[i].mergeItem.SetMergeIncrementalID(PIER.main.ArrSpotIncrementalIDMemory[i]);
                    break;

                   


            }
        }
    }



    void SpawnBoxToEmptySlot() { 

        // 슬롯이 꽉차있을때.. 빈자리가 나자마자 바로 생성되는 처리 필요
        slot = GetRandomEmptySlot();
        if (slot != null) {
            slot.SpawnBox();
        }
    }


    /// <summary>
    /// 머지 루틴..!
    /// </summary>
    /// <returns></returns>
    IEnumerator MergeRoutine() {
        float time = 0;
        MergeSlot slot;

        while(true) {
            time += Time.deltaTime;

            if(time > SecondSpawnBox) {

                // 랜덤한 slot 위치에 박스 생성 
                slot = null;
                slot = GetRandomEmptySlot();
                if(slot != null) {
                    slot.SpawnBox();
                }

                time = 0;
            }

            yield return null;
        }
    }



    /// <summary>
    /// MergeIncremental ID로 MergeItem 찾기!
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public MergeItem FindMergeItemByIncrementalID(int id) {

        for(int i=0; i<ListSlots.Count;i++) {

            if (!ListSlots[i].IsAvailable)
                continue;

            if (ListSlots[i].mergeItem == null)
                continue;

            if (ListSlots[i].mergeItem.MergeIncrementalID == id)
                return ListSlots[i].mergeItem;

        }

        return null;

    }

    /// <summary>
    /// 랜덤 빈 슬롯 
    /// </summary>
    /// <returns></returns>
    MergeSlot GetRandomEmptySlot() {

        ListEmptySlots.Clear();

        // 비어있는 모든 슬롯 수집
        for(int i =0;i <AvailableMergeSlotCount;i++) {


            if (ListSlots[i].mergeItem == null)
                ListEmptySlots.Add(ListSlots[i]);

        }

        if (ListEmptySlots.Count == 0)
            return null;

        return ListEmptySlots[Random.Range(0, ListEmptySlots.Count)];

    }


    /// <summary>
    /// 타겟 슬록 설정
    /// </summary>
    /// <param name="s"></param>
    public void SetTargetSlot(MergeSlot s) {
        TargetSlot = s;
    }

    public MergeSlot GetNearestSlot(Vector3 pos) {

        MergeSlot slot = null;
        float distance = 0;

        for(int i=0; i<AvailableMergeSlotCount;i++) {
            
            if (i == 0) {
                slot = ListSlots[i];
                distance = Vector2.Distance(ListSlots[i].transform.position, pos);
                continue;
            }

        }

        return null;
    }



    #region UNLOCK

    /// <summary>
    /// 유닛 언락 여부 체크
    /// </summary>
    /// <param name="unit"></param>
    public void CheckUnlock(UnitDataRow unit) {

        Debug.Log(">> CheckUnlock unitLevel(" + unit._level + ") / HighestLevel(" + PIER.main.HighestUnitLevel + ")");

        // 신규 유닛 체크 
        if(unit._level > PIER.main.HighestUnitLevel) {
            PIER.main.HighestUnitLevel = unit._level;
            PIER.main.SaveData(false);

            ViewUnlock.unlockUnit = unit;

            Doozy.Engine.GameEventMessage.SendEvent("UnlockEvent");
            CurrentUnlockData = UnlockData.Instance.Rows[PIER.main.HighestUnitLevel - 1];
            SetInstantPurchaseUnit();
        }

    }




    #endregion

    #region Instant Purchase

    /// <summary>
    /// 빠른구매 유닛 설정 
    /// </summary>
    public void SetInstantPurchaseUnit() {

        CurrentUnlockData = UnlockData.Instance.Rows[PIER.main.HighestUnitLevel - 1];

        int level = CurrentUnlockData._quick;
        UnitDataRow r = UnitData.Instance.Rows[level - 1];

        SpriteInstantUnit.sprite = Stock.GetFriendlyUnitUI(r._spriteUI);
        PriceInstantPurchase = Unit.GetUnitCurrentPrice(level);
        TextInstantPrice.text = PIER.GetBigNumber(PriceInstantPurchase);


    }

    /// <summary>
    /// 빠른 구매!
    /// </summary>
    public void OnClickInstantUnitPurchase() {
        MergeSlot s = GetRandomEmptySlot();
        if(s == null) {
            FloatingMessage.ShowMessage("NO SLOT AVAILABLE!");
            return;
        }

        // if(PriceInstantPurchase)
        // 금액 체크
        // ListSlots[i].SpawnMergeUnitInstantly(PIER.main.ArrSpotMemory[i]);
        s.SpawnMergeUnitInstantly(CurrentUnlockData._quick);

        PIER.main.SaveUnitPurchaseStep(CurrentUnlockData._quick);
        SetInstantPurchaseUnit();


    }

    #endregion

    private void OnApplicationPause(bool pause) {
        if (pause)
            PIER.main.SaveMergeSpotMemory();
    }

    private void OnApplicationQuit() {
        PIER.main.SaveMergeSpotMemory();
    }

}
