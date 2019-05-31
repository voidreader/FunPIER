using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager Instance { get; private set; }

    public Text ScoreText;
    public Text BestScoreInMenu;
    public Text BestScoreInGameOver;

    private int _scoreValue;
    private int _bestScore;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _scoreValue = 0;
        _bestScore = PlayerPrefs.GetInt("BestScore");
        UpdateScoreUI();
        UpdateBestScoreUI();
    }

    public void AddScore(int value)
    {
        _scoreValue += value;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        ScoreText.text = _scoreValue.ToString();
    }

    public void UpdateBestScoreUI()
    {
        _bestScore += _scoreValue;
        PlayerPrefs.SetInt("BestScore", _bestScore);
        BestScoreInMenu.text = "BEST: " + _bestScore;
        BestScoreInGameOver.text = _bestScore.ToString();
    }
}
