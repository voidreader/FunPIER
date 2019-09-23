using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SA.Android.App;
using SA.Android.Content;
using SA.iOS.StoreKit;
// using SA.iOS.StoreKit;

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
        //_btnAds.SetActive(AdsManager.main.IsAvailableRewardAD());
        _btnAds.SetActive(true);
    }

    public void OnClickWatchAD() {

        if(!IAPControl.IsNetVerified()) {
            PIER.SetNotReachInternetText();
            return;
        }

        if(!AdsManager.main.IsAvailableRewardAD()) {
            PIER.SetNotAvailableAdvertisement();
            return;
        }

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
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.utplus.hitmonkey");





#elif UNITY_IOS
        // ISN_SKStoreReviewController.RequestReview();
        Application.OpenURL("https://itunes.apple.com/app/id1474934321");
#endif



    }

    public void OnClickLB() {
        PlatformManager.main.ShowLeaderBoardUI();
    }

    public void OnClickHome() {

        if (!IAPControl.main)
            return;

        /* 메인 화면으로 돌아갈때마다 SDK 및 IAP 체크 */
        // 인터넷 연결되어있는데 IAP 초기화가 안되어있는 경우 
        if (IAPControl.IsNetVerified() && !IAPControl.IsInitialized) {
            Debug.Log(">> Re-Init Billing System <<");
            IAPControl.main.InitBilling(); // 다시 초기화 시작 
        }

        if (IAPControl.IsNetVerified() && !AdsManager.IsAdsInit) {
            Debug.Log(">> Re-Init SDKs <<");
            AdsManager.main.InitializeSDKs();
        }
    }

}
