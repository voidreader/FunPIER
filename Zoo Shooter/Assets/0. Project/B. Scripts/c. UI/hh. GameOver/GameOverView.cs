using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{
    public Text _lblCurrentScore, _lblBestScore, _lblLevel;

    public void OnView() {

        Debug.Log(">> OnView in GameOverView ::  " + GameViewManager.main._currentScore);

        PIER.main.SaveBestScore(GameViewManager.main._currentScore);
        _lblLevel.text = GameViewManager.main._currentScore.ToString();
        _lblBestScore.text = PIER.main.BestScore.ToString();
        _lblLevel.text = "LEVEL : " + PIER.CurrentLevel.ToString();

    }
}
