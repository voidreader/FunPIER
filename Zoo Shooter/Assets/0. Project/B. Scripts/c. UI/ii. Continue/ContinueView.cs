using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.Progress;

public class ContinueView : MonoBehaviour {

    public GameObject _btnNoThanks;
    public Progressor _timer;
    float maxSec = 6f;

    public void OnView() {
        Debug.Log("OnView in ContinueView");

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

        while(elapsed >= 0) {
            elapsed -= Time.deltaTime;
            v = elapsed / maxSec;
            _timer.SetValue(v);

            yield return null;
        }

        _timer.SetValue(0); // 끝나면 다음 화면 호출
        // Doozy.Engine.GameEventMessage.SendEvent("GameOverEvent");
        GameManager.main.GameOver();

    }

    public void OnClickWatchAD() {
        Debug.Log("WatchAD!");

        // 다시 게임으로 돌아가서.. 부활시켜야 된다. 
        Doozy.Engine.GameEventMessage.SendEvent("ReviveEvent");
        GameManager.main.Revive();
    }

    public void OnClickNoThanks() {
        Debug.Log("No Thanks!");
        GameManager.main.CleanGameObjects();
    }

}
