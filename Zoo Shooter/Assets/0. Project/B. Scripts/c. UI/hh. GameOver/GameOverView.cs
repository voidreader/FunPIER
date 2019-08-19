using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{
    public Text _lblCurrentScore, _lblBestScore, _lblLevel;

    public GameObject _btnAds;

    public void OnView() {

        Debug.Log(">> OnView in GameOverView Current Score ::  " + PIER.main.CurrentScore);

        PIER.main.SaveBestScore(PIER.main.CurrentScore);

        _lblCurrentScore.text = PIER.main.CurrentScore.ToString(); // 현스코어 
        _lblBestScore.text = PIER.main.BestScore.ToString(); // 베스트 스코어 

        _lblLevel.text = "LEVEL : " + PIER.CurrentLevel.ToString();

        SetAdvertiseButton();

        // 게임오버 떴으면 CurrentScore 초기화
        PIER.main.SaveCurrentScore(0);


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
