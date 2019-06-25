using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBar : MonoBehaviour {

    public static LevelBar _instance;

    public Image fillImage; //Fill Bar on the top
    public Text lv1; // level text

    public GameObject upgradeButton;

    public int destroyedBlocks, level;

    public int allBlocksToUpgrade = 10; // The neccessary destroyed blocks to level up

    public GameObject upgradePanel, upgradeEffect, gameOverPanel, gameOverEffect;
    public Text upgradeBonus, progress;

    public GameObject share;

    private void Awake() {
        _instance = this;
    }

    // Use this for initialization
    void Start() {
        fillImage.fillAmount = 0;
        upgradeButton.SetActive(false);

        level = PlayerPrefs.GetInt(PlayerPrefsStatic.CURRENT_LEVEL);
        lv1.text = "LEVEL  " + level.ToString();
        destroyedBlocks = PlayerPrefs.GetInt("DESTROYEDBLOCKS");

        allBlocksToUpgrade = 100 + Mathf.Min(level * 10 + (level / 15) * 15, 750);
    }

    // Update is called once per frame
    void Update() {
        fillImage.fillAmount = destroyedBlocks / (float)allBlocksToUpgrade;

        if (fillImage.fillAmount >= 1 && !upgradeButton.activeSelf) { //if the fillbar is full, level up
            level++;
            upgradeBonus.text = Statistics._instance.GetSuffix(Statistics._instance.DMax(850 * DecPow(level, 2), 800));
            OnUpgrade();
            Time.timeScale = 0;
        }

        // if the fillbar is under 0 (destroyed blocks < 0) -> Game Over
        if (destroyedBlocks < 0) {
            destroyedBlocks = 0;
            Time.timeScale = 0; //Pausing the game
            OnGameOver();
        }

        progress.text = destroyedBlocks + " / " + allBlocksToUpgrade.ToString();
    }

    /// <summary>
    /// Called on Upgrade, activating upgrade panel
    /// </summary>
    public void OnUpgrade() {
        upgradeEffect.SetActive(true);

        upgradePanel.SetActive(true);
        upgradeButton.SetActive(false);


        Statistics._instance.AddMoney(Statistics._instance.DMax(850 * DecPow(level, 2), 800));

        fillImage.fillAmount = 0;
        destroyedBlocks = 0;

        lv1.text = "LEVEL  " + (level).ToString();

        PlayerPrefs.SetInt("DESTROYEDBLOCKS", 0);
        PlayerPrefs.SetInt(PlayerPrefsStatic.CURRENT_LEVEL, level);

        allBlocksToUpgrade = 100 + Mathf.Min(level * 10 + (level / 15) * 15, 750);

        foreach (Level l in Statistics._instance.levels) {
            if (l.type == Level.Type.completeLevel)
                l.progression++;
        }

        if (level == 5 || level == 10) share.SetActive(true);
    }

    bool sharing = false;
    public void Share() {
        /*
        new NativeShare().SetSubject("Try out this game!").SetText("https://play.google.com/store/apps/details?id=com.fmgames.idleballz").Share();
        sharing = true;
        share.SetActive(false);
        */
    }

    private void OnApplicationPause(bool pause) {
        if (sharing) {
            sharing = false;
            Statistics._instance.AddMoney(25000);
        }
    }

    decimal DecPow(decimal num, int p) {
        decimal d = num;
        for (int i = 0; i < p - 1; i++)
            d *= num;

        return d;
    }

    /// <summary>
    /// On Click listener for Double Money button on upgrade panel
    /// </summary>
    public void DoubleMoney() {        
        ADs._instance.moneyToAdd = Statistics._instance.DMax(850 * DecPow(level - 1, 2), 800);
        ADs._instance.ShowRewardedGetMoney();
    }

    public void SetTimeScaleBack() {
        Time.timeScale = 1;
    }

    /// <summary>
    /// Game Over
    /// </summary>
    void OnGameOver() {
        destroyedBlocks = 5;

        gameOverPanel.SetActive(true);
        gameOverEffect.SetActive(true);

        ADs._instance.adTimer = 0f;
    }
}
