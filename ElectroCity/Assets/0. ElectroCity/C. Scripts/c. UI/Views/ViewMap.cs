using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ViewMap : MonoBehaviour
{
    public List<MapColumn> ListMaps;

    public float _AnimTime = 0.15f;
    public bool _Snap = false;
    public bool _LockX = true;
    public bool _LockY = false;
    public RectTransform _MaskTransform;
    public ScrollRect mScrollRect;
    private RectTransform mScrollTransform;
    private RectTransform mContent;

    private void Awake() {
        // mScrollRect = GetComponent<ScrollRect>();
        mScrollTransform = mScrollRect.GetComponent<RectTransform>();
        mContent = mScrollRect.content;
    }


    public void OnView() {

        // PIER.main.StageNum = 5;

        // Vertical, 아래부터 항목이 추가되기 때문에, List 순서가 거꾸로다. 
        int index = 0;
        int currentStageIndex = ListMaps.Count - PIER.main.StageNum;

        // 맵 초기화
        for (int i=ListMaps.Count-1; i >= 0;i--) { // 크기는 정해져 있다.
            ListMaps[i].InitMap(index++);
        }

        // 11 → 1 스테이지 10 → 2 스테이지... 
        // 현재 스테이지 처리 
        ListMaps[currentStageIndex].SetCurrentMap();


        index = 1;
        while(index < PIER.main.StageNum) {
            ListMaps[ListMaps.Count - index].SetClearMap();
            index++;
        }
    }


}
