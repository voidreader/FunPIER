using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.Progress;

public class ContinueView : MonoBehaviour {

    public GameObject _btnNoThanks;
    public Progressor _timer;
    float maxSec = 6f;

    public bool isContinue = false;

    public void OnView() {
        Debug.Log("OnView in ContinueView");

        isContinue = false;
        _timer.InstantSetValue(1);
        _btnNoThanks.SetActive(false);


        StartCoroutine(TimerRoutine());
        StartCoroutine(NoThanksRoutine());
    }

    IEnumerator NoThanksRoutine() {
        float elapsed = 0;
        while(elapsed < 2) {
            elapsed += Time.deltaTime;
            yield return null;
                 
        }

        _btnNoThanks.SetActive(true);
    }

    IEnumerator TimerRoutine() {
        float elapsed = maxSec;
        float v;

        // AudioAssistant.main.PlayLoopingSFX("Tick");

        while(elapsed >= 0) {
            elapsed -= Time.deltaTime;
            v = elapsed / maxSec;
            _timer.SetValue(v);

            yield return null;
        }

        _timer.SetValue(0); // 끝나면 다음 화면 호출
        // Doozy.Engine.GameEventMessage.SendEvent("GameOverEvent");

        Debug.Log("TIME OVER... ");

        // 광고보기 눌렀으면 끝. 
        if (isContinue)
            yield break;

        GameManager.main.GameOver();


    }

    public void OnClickWatchAD() {

        if (_timer.Value <= 0)
            return;

        Debug.Log("WatchAD! Continue...... ");

        // 컨티뉴 눌렀다..!
        isContinue = true;

        if(Application.isEditor) {
            OnCompletedWathADContinue();
            return;
        }





        AdsManager.main.OpenRewardAd(OnCompletedWathADContinue);
        
    }

    void OnCompletedWathADContinue() {
        Debug.Log("WatchAD! OnCompletedWathADContinue...... ");
        // 다시 게임으로 돌아가서.. 부활시켜야 된다. 
        Doozy.Engine.GameEventMessage.SendEvent("ReviveEvent");
        GameManager.main.Revive();
    }

    public void OnClickNoThanks() {
        Debug.Log("No Thanks!");
        GameManager.main.CleanGameObjects();
    }

}
