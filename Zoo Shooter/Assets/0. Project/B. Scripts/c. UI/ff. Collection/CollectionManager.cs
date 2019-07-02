﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;

public class CollectionManager : MonoBehaviour
{
    public List<CollectionDataRow> ListCollection = null; // 기준정보 
    public List<Sprite> ListImages; // 이미지들. 


    public Image recordImage;

    public GameObject btnLeft, btnRight;
    public Text lblRecord;

    public int index = 0;


    public void OnViewCollection() {
        if (ListCollection == null)
            ListCollection = CollectionData.Instance.Rows;

        // 현 인덱스의 처리
        if (PlayerPrefs.HasKey(ConstBox.keyCurrentCollectionIndex))
            index = PlayerPrefs.GetInt(ConstBox.keyCurrentCollectionIndex);


        // PIER.CurrentList


        SetRecordInfo();
        SetButtonSide();
    }


    /// <summary>
    /// 레코드 이미지와 레이블 처리 
    /// </summary>
    void SetRecordInfo() {
        recordImage.sprite = ListImages[index];
        lblRecord.text = "RECORD." + string.Format("{0:00}", (index + 1).ToString());

        if(index == PIER.CurrentList) { // 현재 상태는. 검은색
            recordImage.color = Color.black;
        }
        else {
            recordImage.color = Color.white;
        }
    }

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


    public void OnClickRight() {
        if (index + 1 >= PIER.CurrentList)
            return;


        index++;

        SetRecordInfo();
        SetButtonSide();

        SaveCollectionIndex();
    }

    public void OnClickLeft() {
        if (index <= 0)
            return;

        index--;

        SetRecordInfo();
        SetButtonSide();

        SaveCollectionIndex();
    }

    void SaveCollectionIndex() {
        PlayerPrefs.SetInt(ConstBox.keyCurrentCollectionIndex, index);
    }

    

    
}