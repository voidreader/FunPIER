using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SA.Android.App;
using SA.Android.Content;

public class GameOverView : MonoBehaviour
{
    public Text _lblCurrentScore, _lblBestScore, _lblLevel;
    public GameObject _reviewCoin;
    public GameObject _btnAds;
    public int _reviewBonusGetCount = 0;

    public void OnView() {

        Debug.Log(">> OnView in GameOverView Current Score ::  " + PIER.main.CurrentScore);

        PIER.main.SaveBestScore(PIER.main.CurrentScore);

        _lblCurrentScore.text = PIER.main.CurrentScore.ToString(); // 현스코어 
        _lblBestScore.text = PIER.main.BestScore.ToString(); // 베스트 스코어 

        _lblLevel.text = "LEVEL : " + PIER.CurrentLevel.ToString();

        SetAdvertiseButton();
        SetReviewBonus();

        // 게임오버 떴으면 CurrentScore 초기화
        PIER.main.SaveCurrentScore(0);


    }



    /// <summary>
    /// 리뷰 보너스 처리 
    /// </summary>
    void SetReviewBonus() {
        if (PlayerPrefs.HasKey("KeyReviewBonus"))
            _reviewBonusGetCount = PlayerPrefs.GetInt("KeyReviewBonus");
        else
            _reviewBonusGetCount = 0;

        if(_reviewBonusGetCount == 0) {
            _reviewCoin.SetActive(true);
        }
        else {
            _reviewCoin.SetActive(false);
        }


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

    public void OnClickReview() {
        if(_reviewCoin.activeSelf) {
            PIER.main.AddCoin(25);
            _reviewBonusGetCount = 1;
            PlayerPrefs.SetInt("KeyReviewBonus", _reviewBonusGetCount);
            PlayerPrefs.Save();

            _reviewCoin.SetActive(false);
        }


#if UNITY_ANDROID
        System.Uri uri = new System.Uri("market://details?id=" + Application.identifier);
        AN_Intent viewIntent = new AN_Intent(AN_Intent.ACTION_VIEW, uri);
        AN_MainActivity.Instance.StartActivity(viewIntent);
#endif

    }

    public void OnClickLB() {
        PlatformManager.main.ShowLeaderBoardUI();
    }

}
