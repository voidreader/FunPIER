using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeSystem : MonoBehaviour
{
    public static MergeSystem main = null;

    [Header(" - Drag Item - ")]
    public static MergeItem DraggingItem;

    public List<MergeSlot> ListSlots;
    public List<MergeSlot> ListEmptySlots;

    public float SecondSpawnBox;

    public Transform DragParent;
    public MergeSlot TargetSlot; // 드래그 하는 머지아이템이 이동할 슬롯 
    
    MergeSlot slot;

    private void Awake() {
        main = this;
    }

    void Start() {
        ListEmptySlots = new List<MergeSlot>();

        //StartCoroutine(MergeRoutine());

    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.A)) {
            slot = GetRandomEmptySlot();
            if (slot != null) {
                slot.SpawnBox(1);
            }
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
                    slot.SpawnBox(1);
                }

                time = 0;
            }

            yield return null;
        }
    }


    /// <summary>
    /// 랜덤 빈 슬롯 
    /// </summary>
    /// <returns></returns>
    MergeSlot GetRandomEmptySlot() {

        ListEmptySlots.Clear();

        // 비어있는 모든 슬롯 수집
        for(int i =0;i <ListSlots.Count;i++) {


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

        for(int i=0; i<ListSlots.Count;i++) {
            
            if (i == 0) {
                slot = ListSlots[i];
                distance = Vector2.Distance(ListSlots[i].transform.position, pos);
                continue;
            }

        }

        return null;
    }

    
}
