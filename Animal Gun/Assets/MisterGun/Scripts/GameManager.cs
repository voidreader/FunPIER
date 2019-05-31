using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static Action GameOverAction;

    public Camera MainCamera;
    public GameObject MenuPanel;
    public GameObject GamePanel;
    public GameObject GameOverPanel;
    public GameObject LevelSlider;

    public Animator BlackOutAnim;
    public Image SoundImage;

    [HideInInspector]
    public int StairsCount;
    [HideInInspector]
    public int MissionKillsCount;

    public static bool IsStartGame;
    public static bool IsGameOver;

    private void OnEnable()
    {
        GameOverAction += GameOver;
    }
    private void OnDisable()
    {
        GameOverAction -= GameOver;
    }

    private void Awake()
    {
        Application.targetFrameRate = 600;
        IsStartGame = false;
        IsGameOver = false;
        StairsCount = 10 + PlayerPrefs.GetInt("PlayerLevel"); // 10 is default stairs value, you can change her
        MissionKillsCount = StairsCount / 2;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("Sounds") == 0)
        {
            AudioManager.Instance.SoundSource.mute = false;
            AudioManager.Instance.MusicSource.mute = false;
            SoundImage.sprite = AudioManager.Instance.SoundsSprites[PlayerPrefs.GetInt("Sounds")];
        }
        if (PlayerPrefs.GetInt("Sounds") == 1)
        {
            AudioManager.Instance.SoundSource.mute = true;
            AudioManager.Instance.MusicSource.mute = true;
            SoundImage.sprite = AudioManager.Instance.SoundsSprites[PlayerPrefs.GetInt("Sounds")];
        }
    }

    public void StartGame()
    {
        MenuPanel.SetActive(!MenuPanel.activeSelf);
        GamePanel.SetActive(!GamePanel.activeSelf);
        IsStartGame = true;
        MainCamera.backgroundColor = Color.black;
        MainCamera.GetComponent<Animator>().enabled = false;
    }
    public void GameOver()
    {
        GameOverPanel.SetActive(!GameOverPanel.activeSelf);
        LevelSlider.SetActive(false);
        IsGameOver = true;
        ScoreManager.Instance.UpdateBestScoreUI();
    }

    public void EnableBlackoutAnim()
    {
        BlackOutAnim.SetTrigger("GameOver");
        Invoke("RestartLevel", 2f);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene("Main");
    }
}
