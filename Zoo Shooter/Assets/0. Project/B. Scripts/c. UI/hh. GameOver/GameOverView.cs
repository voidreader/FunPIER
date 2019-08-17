using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{
    public Text _lblCurrentScore, _lblBestScore, _lblLevel;

    public GameObject _btnAds;

    public void OnView() {

        Debug.Log(">> OnView in GameOverView ::  " + GameViewManager.main._currentScore);

        PIER.main.SaveBestScore(GameViewManager.main._currentScore);
        _lblLevel.text = GameViewManager.main._currentScore.ToString();
        _lblBestScore.text = PIER.main.BestScore.ToString();
        _lblLevel.text = "LEVEL : " + PIER.CurrentLevel.ToString();

        SetAdvertiseButton();

    }

    /// <summary>
    /// 광고버튼 활성화 여부 
    /// </summary>
    /// <returns></returns>
    void SetAdvertiseButton() {
        _btnAds.SetActive(AdsManager.main.IsAvailableRewardAD());
    }

    public void OnClickWatchAD() {
        AdsManager.main.OpenRewardAd(CallbackWatchAD);
    }

    void CallbackWatchAD() {
        PIER.main.AddCoin(35); // 코인 35개 
        _btnAds.SetActive(false);  // 광고버튼 비활성화
    }
}
