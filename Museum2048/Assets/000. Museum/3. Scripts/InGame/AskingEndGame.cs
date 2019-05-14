using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AskingEndGame : MonoBehaviour {

    public static AskingEndGame main = null;

    public UISprite _frame;
    public UILabel _lblComment;

    private void Awake() {
        main = this;
    }

    /// <summary>
    /// 오픈!
    /// </summary>
    public void OpenAsking() {
        this.gameObject.SetActive(true);
        LobbyManager.isAnimation = true;
        _frame.gameObject.SetActive(false);
        InGame.main.ShowInGameUIs(false);

        StartCoroutine(OpenRoutine());
    }

    IEnumerator OpenRoutine() {
        _lblComment.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT25) + "\n\n" + PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT26);

        _frame.alpha = 0;
        _frame.gameObject.SetActive(true);
        for(int i =0; i<20; i++) {
            _frame.alpha += 0.05f;
            yield return new WaitForSeconds(0.1f);
        }

        _frame.alpha = 1;

        LobbyManager.isAnimation = false;

    }

    /// <summary>
    /// 게임 계속하기 
    /// </summary>
    public void ContinueGame() {
        InGame.main.ShowInGameUIs(true);
        this.gameObject.SetActive(false);

        InGame.main.OnCloseAsking();
    }


    /// <summary>
    /// 게임 종료! 게임오버 
    /// </summary>
    public void OnClickFinishGame() {
        InGame.main.GameOver();
        this.gameObject.SetActive(false);
    }

    public void OnClickAlign() {
        Debug.Log("OnClickAlign");
        AdsControl.main.ShowWatchAd(OnWatchedAlighAD);
    }

    void OnWatchedAlighAD() {
        Debug.Log(">> OnWatchedAlighAD <<");
        ContinueGame();
        InGame.main.AlignBlocks();
    }

}
