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

    // 보상
    public int CountMergeSpot, CountBattleSpot, CountGem;

    LevelDataRow row;

    public void OnView() {

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
    }
}
