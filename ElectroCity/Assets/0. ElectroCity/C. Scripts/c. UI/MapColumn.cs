using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Google2u;

public class MapColumn : MonoBehaviour
{
    public Image Frame;
    public GameObject GroupLock, GroupUnlock, GroupAvailable;
    public Text TextLockAreaName, TextAvailableAreaName;
    public Text TextUnlockValue, TextAvailableValue, TextLockValue;
    public Text TextAvailableNeedCoin, TextLockNeedCoin;

    public int StageNum = 0; // 스테이지 번호 

    StageDataRow row;
    long bignumber;


    
    void InitGroup() {
        GroupLock.SetActive(false);
        GroupUnlock.SetActive(false);
        GroupAvailable.SetActive(false);
    }

    /// <summary>
    /// 맵 초기화
    /// </summary>
    /// <param name="index"></param>
    public void InitMap(int index) {

        InitGroup();

        StageNum = index + 1;

        Frame.gameObject.SetActive(true);
        Frame.sprite = Stock.main.SpriteMapLock;
        row = StageData.Instance.Rows[index];

        GroupLock.SetActive(true);

        SetInfo();
    }

    /// <summary>
    /// Available 처리
    /// </summary>
    public void SetCurrentMap() {
        
        Frame.sprite = Stock.main.SpriteMapCurrent;
        InitGroup();
        GroupAvailable.SetActive(true);
    }

    /// <summary>
    /// Unlock 처리
    /// </summary>
    public void SetClearMap() {
        InitGroup();
        GroupUnlock.SetActive(true);
        Frame.sprite = Stock.main.SpriteMapUnlock;

    }

    void SetInfo() {

        // 지역 이름 
        TextAvailableAreaName.text = row._displayname;
        TextLockAreaName.text = row._displayname;

        // 필요 코인 
        TextAvailableNeedCoin.text = PIER.GetBigNumber(long.Parse(row._needcoin));
        TextLockNeedCoin.text = PIER.GetBigNumber(long.Parse(row._needcoin));


        TextLockValue.text = "x" + row._coinfactor.ToString();
        TextUnlockValue.text = "x" + row._coinfactor.ToString();
        TextAvailableValue.text = "x" + row._coinfactor.ToString();


    }

}
