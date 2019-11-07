using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public enum LevelReward {
    Gem,
    MergeSpot,
    BattleSpot
}

public class LevelRewardColumn : MonoBehaviour
{

    [SerializeField] LevelReward rewardKind;
    [SerializeField] int count = 0;


    /// <summary>
    /// 보상 설정
    /// </summary>
    /// <param name="t"></param>
    /// <param name="cnt"></param>
    public void SetRewardCol(LevelReward t, int cnt, float delayTime = 0) {
        rewardKind = t;
        count = cnt;

        this.transform.localScale = Vector3.zero;
        this.gameObject.SetActive(true);
        this.transform.DOScale(1, 0.4f).SetEase(Ease.OutBounce).SetDelay(delayTime);
    }
}
