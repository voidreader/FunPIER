using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;
using SimpleJSON;
using System;


public class PIER : MonoBehaviour {

    public static PIER main = null;
    public static int CurrentList = 0;
    public static int CurrentLevel = 0;

    public static Action CoinRefresh;

    public int Coin = 0; // 보유 코인 
    public int NoAds = 0; // false. 광고 

    public List<BossDataRow> ListBossData;
    public List<Sprite> ListBossPortrait;
    



    void Awake() {
        main = this;
    }

    void Start() {
        ListBossData = BossData.Instance.Rows;
        
    }


    #region 저장된 데이터 로드 
    /// <summary>
    /// 데이터 로드 
    /// </summary>
    void LoadData() {

        CurrentList = 0;
        CurrentLevel = 0;

        if(PlayerPrefs.HasKey(ConstBox.keyCurrentList))
            CurrentList = PlayerPrefs.GetInt(ConstBox.keyCurrentList);

        if (PlayerPrefs.HasKey(ConstBox.keyCurrentLevel))
            CurrentLevel = PlayerPrefs.GetInt(ConstBox.keyCurrentLevel);

        if (PlayerPrefs.HasKey(ConstBox.keyCurrentCoin))
            Coin = PlayerPrefs.GetInt(ConstBox.keyCurrentCoin); // 코인 


    }

    /// <summary>
    /// 데이터 저장 
    /// </summary>
    void SaveData() {

    }

    #endregion

    #region 보스 데이터 가져오기

    /// <summary>
    /// 현 보스 블랙리스트 가져오기 
    /// </summary>
    /// <returns></returns>
    public List<BossDataRow> GetCurrentBlacklist() {
        List<BossDataRow> r = new List<BossDataRow>();

        for(int i =0;i<ListBossData.Count;i++) {

            if(ListBossData[i]._list == CurrentList) //current list 맞는것만 가져온다.
                r.Add(ListBossData[i]);
        }

        return r;
    }

    /// <summary>
    /// 보스 초상화 스프라이트 가져오기 
    /// </summary>
    /// <param name="portrait"></param>
    /// <returns></returns>
    public Sprite GetBossPortraitSprite(string p) {
        for(int i =0; i<ListBossPortrait.Count;i++) {
            if (ListBossPortrait[i].name == p)
                return ListBossPortrait[i];
        }

        return null;
    }

    

    #endregion

}
