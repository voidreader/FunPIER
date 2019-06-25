using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour {

    public Image fillBar;
    public GameObject locked;
    public bool isUnlocked = false;

    public int id, levelToUnlock;
    public int progression, allProgression;

    public enum Type { destroyBlock, completeLevel, tapDamage, useBoosters, laserDestroy, destroyBoss, tapLevel, perfectWave, fireRateLevel};
    public Type type;

    public GameObject completedIndicator;

    bool rewardCollected = false;

    public Button button;
    // Use this for initialization
    void Start() {
        
    }

    public void Setup() {
        completedIndicator.SetActive(false);

        progression = PlayerPrefs.GetInt("QUEST_PROGRESSION_" + id.ToString());

        if (PlayerPrefs.GetInt("QUEST_COLLECTED_" + id.ToString()) == 1) {
            SetCompletedStatus();
            progression = 1000000;
        }

        locked.SetActive(true);

        if (LevelBar._instance.level >= levelToUnlock) {
            isUnlocked = true;
            locked.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update() {
        if (!isUnlocked) {
            progression = 0;
            if (LevelBar._instance.level >= levelToUnlock) {
                isUnlocked = true;
                locked.SetActive(false);
            }
            return;
        }

        if (type == Type.tapLevel) progression = Statistics._instance.tapDamageLevel;
        if (type == Type.fireRateLevel) progression = Statistics._instance.fireRateLevel;


        fillBar.fillAmount = (float)progression / allProgression;

        if (fillBar.fillAmount < 1) button.gameObject.SetActive(false);
        else if (!rewardCollected) button.gameObject.SetActive(true);
    }

    public void CollectMoney(int amount) {
        if (fillBar.fillAmount < 1 || rewardCollected) return;

        Toast.instance.ShowMessage("Reward Collected", 1f);
        Statistics._instance.AddMoney(amount);

        SetCompletedStatus();
    }

    void SetCompletedStatus() {
        rewardCollected = true;
        completedIndicator.SetActive(true);
        GetComponentInChildren<Button>().gameObject.SetActive(false);
        PlayerPrefs.SetInt("QUEST_COLLECTED_" + id.ToString(), 1);
    }

    public void ActivateBooster() {
        if (fillBar.fillAmount < 1 || rewardCollected) return;

        GameManager._instance.boostTime = 120;
        Toast.instance.ShowMessage("Boost activated");

        SetCompletedStatus();
    }

    public void UnlockProfileIcon(int newMax) {
        if (fillBar.fillAmount < 1 || rewardCollected) return;

        Toast.instance.ShowMessage("Reward Collected");
        SetCompletedStatus();
    }
}
