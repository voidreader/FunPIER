using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;


public class PlayManager : MonoBehaviour
{
    public GameObject GameoverPanel;

    public GameObject PlayerObj;

    int currentScore = 0;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI best;


    void Start()
    {
        Time.timeScale = 1f;
        currentScoreText.text = currentScore.ToString();
        bestScoreText.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
    }


    void Update()
    {
        currentScore = (int)PlayerObj.transform.position.y / 2;
        currentScoreText.text = currentScore.ToString();

        if (currentScore > PlayerPrefs.GetInt("BestScore", 0))
        {
            PlayerPrefs.SetInt("BestScore", currentScore);
            bestScoreText.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
        }
    }


    public void GameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }


    IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        Time.timeScale = 0.005f;
        GameoverPanel.SetActive(true);
        ChangeColorToWhite();

        yield break;
    }


    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void ChangeColorToWhite()
    {
        currentScoreText.color = Color.white;
        bestScoreText.color = Color.white;
        best.color = Color.white;
    }

}
