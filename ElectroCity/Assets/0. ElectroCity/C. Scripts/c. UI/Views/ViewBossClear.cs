using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBossClear : MonoBehaviour
{
    public Text TextDoubleReward, TextReward;
    public long RewardCoin = 0;
    public BigAura Aura;

    public void OnView() {
        RewardCoin = GameManager.main.EarningCoin * 40;
        TextDoubleReward.text = PIER.GetBigNumber(RewardCoin * 2);
        TextReward.text = PIER.GetBigNumber(RewardCoin);

        Aura.PlayAura();

    }

    public void OnClickDoubleReward() {
        // 코인 획득 처리 
        GameManager.main.OnCompleteBossClearEvent();
    }

    public void OnClickJustReward() {
        // 코인 획득 처리
        GameManager.main.OnCompleteBossClearEvent();
    }
}
