using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;
using DG.Tweening;

public class ViewLevelUp : MonoBehaviour
{

    public int RewardCount = 0; // 레벨업하면서 받는 보상 개수 
    public List<LevelRewardColumn> ListRewards;
    
    public Button btnBack;

    // 보상
    public int CountMergeSpot, CountBattleSpot, CountGem;
    

    LevelDataRow row;

    public void OnView() {

        btnBack.gameObject.SetActive(false);
        row = LevelData.Instance.Rows[PlayerInfo.main.PlayerLevel - 1]; // 현재 레벨의 보상 데이터 

        // 카운트 체크
        if(row._mergespot > 0) {
            RewardCount++;
            CountMergeSpot = row._mergespot;
        }

        if(row._battlespot > 0) {
            RewardCount++;
            CountBattleSpot = row._battlespot;
        }

        if(row._gem > 0) {
            RewardCount++;
            CountGem = row._gem;
        }

        InitRewardCols();
        StartCoroutine(Waiting());

        // 기본적으로 보석 보상은 레벨업마다 있다.
        
        // 보상개수에 따른 위치 
        switch(RewardCount) {
            case 1:
                ListRewards[0].transform.localPosition = Vector3.zero;

                // 1개짜리는 다 보석
                ListRewards[0].SetRewardCol(LevelReward.Gem, CountGem, 0.4f);

                break;

            case 2:
                ListRewards[0].transform.localPosition = new Vector2(-100, 0);
                ListRewards[1].transform.localPosition = new Vector2(100, 0);

                // 머지스팟과 배틀스팟 둘중에 한개. 
                if(row._mergespot > 0) {
                    ListRewards[0].SetRewardCol(LevelReward.MergeSpot, 1, 0.4f);
                }
                else {
                    ListRewards[0].SetRewardCol(LevelReward.BattleSpot, 1, 0.4f);
                }

                ListRewards[1].SetRewardCol(LevelReward.Gem, CountGem, 0.8f);

                break;

            case 3:
                ListRewards[0].transform.localPosition = new Vector2(-200, 0);
                ListRewards[1].transform.localPosition = Vector3.zero;
                ListRewards[2].transform.localPosition = new Vector2(200, 0);
                

                ListRewards[0].SetRewardCol(LevelReward.MergeSpot, 1, 0.4f);
                ListRewards[1].SetRewardCol(LevelReward.BattleSpot, 1, 0.8f);
                ListRewards[2].SetRewardCol(LevelReward.Gem, CountGem, 1.2f);

                break;
        }

        

        // 레벨업 하고, 스팟 조정 
        MergeSystem.main.SetAvailableMergeSpot();
        GameManager.main.InitEquipUnitPosition();
        
    }

    void InitRewardCols() {
        for(int i=0; i<ListRewards.Count;i++) {
            ListRewards[i].gameObject.SetActive(false);
        }
    }

    IEnumerator Waiting() {
        for(int i=0; i<RewardCount;i++) {
            yield return new WaitForSeconds(0.8f);
        }

        btnBack.gameObject.SetActive(true);
    }

    public void OnClose() {
        Debug.Log(">> ViewLevelUp OnClose <<");

        PlayerInfo.isOnLevelUp = false;


    }

}
