﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Google2u;
using DanielLochner.Assets.SimpleScrollSnap;
using Doozy.Engine;

public class CollectionManager : MonoBehaviour
{

    public static CollectionManager main = null;
    public static int TargetPanel = -1;

    public List<CollectionDataRow> ListCollection = null; // 기준정보 
    public List<Sprite> ListImages; // 이미지들. 
    public List<PrisonCinema> ListCinemas;
    public SimpleScrollSnap _sc;


    public Image NewPoster;

    public GameObject btnLeft, btnRight;
    public GameObject InvisibleCover;
    public Text lblRecord;

    public int index = 0;

    private void Awake() {
        main = this;
    }


    #region OnStart
    /// <summary>
    /// 
    /// </summary>
    public void OnViewCollection() {

        InvisibleCover.SetActive(false);

        if (ListCollection == null)
            ListCollection = CollectionData.Instance.Rows;

        // 현 인덱스의 처리
        if (PlayerPrefs.HasKey(ConstBox.keyCurrentCollectionIndex))
            index = PlayerPrefs.GetInt(ConstBox.keyCurrentCollectionIndex);
        else
            index = 0;

        Debug.Log("List & Level & index :: " + PIER.CurrentList + "/" + PIER.CurrentLevel +"/" + index);

        for(int i=0; i<ListCinemas.Count;i++) {
            ListCinemas[i].SetCinema();
        }

        // 타겟 패널은 신규 포스터 발생시, 값이 입력된다 .
        if(TargetPanel >= 0) {
            StartCoroutine(NewPosterRoutine());
        }

    }
    #endregion




    /// <summary>
    /// 사이드 버튼 체크 
    /// </summary>
    void SetButtonSide() {
        // 버튼 체크 
        btnLeft.SetActive(false);
        btnRight.SetActive(false);

        if (PIER.CurrentList == 0)
            return;

        if (index > 0)
            btnLeft.SetActive(true);

        if (index + 1 < PIER.CurrentList)
            btnRight.SetActive(true);
    }




    void SaveCollectionIndex() {
        PlayerPrefs.SetInt(ConstBox.keyCurrentCollectionIndex, index);
    }

    public void PanelChanged() {

        /*
        Debug.Log("PanelChanged :: " + _sc.TargetPanel);
        

        if(_sc.TargetPanel == 0) {
            btnLeft.SetActive(false);
            btnRight.SetActive(true);
        }
        else if(_sc.TargetPanel == 3) {
            btnLeft.SetActive(true);
            btnRight.SetActive(false);
        }
        else {
            btnLeft.SetActive(true);
            btnRight.SetActive(true);
        }
        */
    }
    

    IEnumerator NewPosterRoutine() {

        Debug.Log("New Poster Routine go!");

        // 입력막기용 오브젝트 활성화
        InvisibleCover.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        // 목표 패널로 이동 
        _sc.GoToPanel(TargetPanel);
        yield return new WaitForSeconds(0.5f);

        // 신규 포스터 오픈
        // !! 아직 CurrentList를 + 하기 전이다. 
        // CurrentList는 실제 보상을 받고 나서 증가한다.
        // CurrentList는 현재 진행중인 현상금 수배 리스트다. 
        NewPoster.sprite = Stocks.GetPosterSprite(PIER.CurrentList);

        NewPoster.transform.localEulerAngles = new Vector3(0, 0, 719);
        NewPoster.transform.localScale = new Vector3(0, 0, 1);
        NewPoster.color = new Color(1, 1, 1, 0);
        NewPoster.gameObject.SetActive(true);

        NewPoster.transform.DORotate(new Vector3(0, 0, 0), 0.4f, RotateMode.FastBeyond360).SetEase(Ease.InSine);
        NewPoster.transform.DOScale(1.2f, 0.4f).SetEase(Ease.InOutExpo);
        NewPoster.DOFade(1, 0.4f).SetEase(Ease.Linear);
        // NewPoster.transform.doc

        yield return new WaitForSeconds(1);

        // 지금 리스트가 몇번째 인덱스인지 찾는다.
        Transform target = null;

        for(int i=0; i<5;i++) {
            if(ListCinemas[TargetPanel].ListPosters[i].GetComponent<CinemaPoster>()._posterID == PIER.CurrentList) {
                target = ListCinemas[TargetPanel].ListPosters[i].transform;
            }
        }
        
        if(target) {
            NewPoster.transform.DOMove(target.position, 1f).OnComplete(() => OnCompleteNewPosterArrive(target));
            NewPoster.transform.DOScale(0.4f, 1f);
        }

        yield return new WaitForSeconds(1.2f);
        InvisibleCover.SetActive(false);

        // 보상화면 오픈 
        GameEventMessage.SendEvent("CallWantedReward");
    }

    void OnCompleteNewPosterArrive(Transform target) {
        NewPoster.gameObject.SetActive(false);
        target.GetComponent<CinemaPoster>().SetPoster(PIER.CurrentList);
        TargetPanel = -1;

        ListCinemas[TargetPanel].SetOpen();
        
    }

    
}
