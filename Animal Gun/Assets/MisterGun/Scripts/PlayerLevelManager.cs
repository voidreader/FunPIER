using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelManager : MonoBehaviour {

    public static Action ChangeKillsAction;

    public Slider LevelSlider;
    public Text LevelText;
    public Text LevelTextInGameOver;
    public GameManager GameManager;

    [HideInInspector]
    public int CurrentLevel;

    private int _currentKills;

    private void OnEnable()
    {
        ChangeKillsAction += ChangeKills;
    }
    private void OnDisable()
    {
        ChangeKillsAction -= ChangeKills;
    }

    private void Start () {
        CurrentLevel = PlayerPrefs.GetInt("PlayerLevel");
        _currentKills = 0;
        LevelSlider.maxValue = GameManager.MissionKillsCount;
        LevelTextInGameOver.text = LevelText.text = "LEVEL " + CurrentLevel;
    }

    private void ChangeKills()
    {
        _currentKills++;

        if (_currentKills < GameManager.MissionKillsCount)
        {
            LevelSlider.value = _currentKills;
        }
        else
        {
            LevelSlider.value = _currentKills + 1;
            ChangeLevel();
        }
    }

    private void ChangeLevel()
    {
        GameManager.IsGameOver = true;
        PlayerPrefs.SetInt("PlayerLevel", CurrentLevel + 1);
        GameManager.EnableBlackoutAnim();
    }
}
