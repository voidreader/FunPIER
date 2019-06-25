using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Statistics : MonoBehaviour {
    public static Statistics _instance;

    public decimal money = 0; //Your MONEY / CURRENCY
    public Text moneyText, moneySuffixText; //Text to display money on the TOP
    public string moneyString; //for debug usage

    private void Awake() {
        _instance = this;
    }

    public Level[] levels;

    // Use this for initialization
    void Start() {
        LoadPrefs();

        AddMoney(0);

        SetMaxBallNum();
        SetBallDamage();
        SetBallSpeed();
        SetOfflineIncome();
        SetTapDamage();
        SetFireRate();

        StartCoroutine(DelayedSave());
        StartCoroutine(GetDay());

        StartCoroutine(CanBuyUpgradeCoroutine());
        StartCoroutine(SaveQuestProgression());
    }

    // Update is called once per frame
    void Update() {

    }

    IEnumerator SaveQuestProgression() {
        yield return new WaitForSeconds(0.5f);

        foreach (Level l in levels) {
            l.Setup();
        }

        yield return new WaitForSeconds(1f);

        while (true) {
            foreach (Level l in levels) {
                PlayerPrefs.SetInt("QUEST_PROGRESSION_" + l.id.ToString(), l.progression);
            }

            yield return new WaitForSeconds(7f);
        }
    }

    /// <summary>
    /// In every 0.5 seconds checking each upgrades if you can buy them
    /// </summary>
    /// <returns></returns>
    IEnumerator CanBuyUpgradeCoroutine() {
        while (true) {
            UpgradeButtonDisableCheck();

            yield return new WaitForSeconds(0.5f);
        }
    }

    public GameObject upgradeHolder;

    /// <summary>
    /// Graying buy upgrade button if you do not have enough money to buy it
    /// </summary>
    void UpgradeButtonDisableCheck() {
        if (!upgradeHolder.activeSelf) return;
        if (priceBallNum > money)
            maxBallPrice.GetComponentInParent<Image>().color = Color.gray;
        else maxBallPrice.GetComponentInParent<Image>().color = new Color(240f / 255, 207f / 255, 34f / 255);

        if (priceBallDamage > money)
            ballDamagePrice.GetComponentInParent<Image>().color = Color.gray;
        else ballDamagePrice.GetComponentInParent<Image>().color = new Color(240f / 255, 207f / 255, 34f / 255);

        if (priceTapDamage > money)
            tapDamagePrice.GetComponentInParent<Image>().color = Color.gray;
        else tapDamagePrice.GetComponentInParent<Image>().color = new Color(240f / 255, 207f / 255, 34f / 255);

        if (priceBallSpeed > money)
            ballSpeedPrice.GetComponentInParent<Image>().color = Color.gray;
        else ballSpeedPrice.GetComponentInParent<Image>().color = new Color(240f / 255, 207f / 255, 34f / 255);

        if (priceOfflineEarning > money)
            offlinePrice.GetComponentInParent<Image>().color = Color.gray;
        else offlinePrice.GetComponentInParent<Image>().color = new Color(240f / 255, 207f / 255, 34f / 255);

        if (priceFireRate > money)
            fireRatePrice.GetComponentInParent<Image>().color = Color.gray;
        else fireRatePrice.GetComponentInParent<Image>().color = new Color(240f / 255, 207f / 255, 34f / 255);
    }

    /// <summary>
    /// Saving game data in every 10 seconds (Playerprefs.Set stuffs method is really slow so it needs to be delayed to avoid lagging9
    /// </summary>
    /// <returns></returns>
    IEnumerator DelayedSave() {
        yield return new WaitForSeconds(10);

        while (true) {
            Debug.Log("DELAYED SAVE");

            PlayerPrefs.SetString("MONEY", money.ToString());

            PlayerPrefs.SetInt("DESTROYEDBLOCKS", LevelBar._instance.destroyedBlocks);

            yield return new WaitForSeconds(10f);
        }
    }

    /// <summary>
    /// Changing money value. ALWAYS use this method, it also changes the texts
    /// </summary>
    /// <param name="m"></param>
    public void AddMoney(decimal m) {
        money += m;

        moneyString = money.ToString();
        moneyText.text = moneyString;

        int db = 0;
        for (int i = moneyString.Length; i > 1; i--) {
            db++;
            if (db % 3 == 0) moneyText.text = moneyText.text.Insert(i - 1, ".");
        }

        moneySuffixText.text = GetSuffix(money);
    }

    /// <summary>
    /// Getting suffix string such as 19.5M (19.5 million)
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public string GetSuffix(decimal num) {

        string suffix = "";
        string s = num.ToString();

        if (num < 1000) return num.ToString();

        switch ((s.Length - 1) / 3) {
            case 0:
                suffix = string.Empty;
                break;
            case 1:
                suffix = "K";
                break;
            case 2:
                suffix = "M";
                break;
            case 3:
                suffix = "B";
                break;
            case 4:
                suffix = "T";
                break;
            case 5:
                suffix = "Qa";
                break;
            case 6:
                suffix = "Qi";
                break;
            case 7:
                suffix = "S";
                break;
        }

        if (s.Length % 3 == 1)
            return s.Substring(0, 1) + "." + s.Substring(1, 1) + s.Substring(2, 1) + suffix; // 4.35m

        if (s.Length % 3 == 2)
            return s.Substring(0, 1) + s.Substring(1, 1) + "." + s.Substring(2, 1) + suffix; // 4.35m

        if (s.Length % 3 == 0)
            return s.Substring(0, 1) + s.Substring(1, 1) + s.Substring(2, 1) + suffix; // 4.35m

        return "";
    }

    /// <summary>
    /// Method to use Mathf.Pow with decimal values
    /// </summary>
    /// <param name="num"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    decimal DecPow(decimal num, int p) {
        decimal d = num;
        for (int i = 0; i < p - 1; i++)
            d *= num;

        return d;
    }

    //UPGRADE RELATED METHODS AND VARIABLES

    #region MaxBallNum
    [Header("Max Ball Num")]
    public int maxBallNum, maxBallNumLevel;
    public decimal basePriceMaxBallNum = 60, priceBallNum;
    public Text maxBallPrice, maxBallPieces;

    public void UpgradeMaxBallNum() {
        if (money >= priceBallNum && maxBallNumLevel < 45) { //45 is the maximum level
            AddMoney(-priceBallNum); //Decreasing money
            maxBallNumLevel++;

            SetMaxBallNum();

            PlayerPrefs.SetInt("MAXBALLNUM", maxBallNumLevel); //aving
        }
    }

    void SetMaxBallNum() {
        priceBallNum = basePriceMaxBallNum * DecPow(maxBallNumLevel + 1, 3) / 2;
        maxBallPrice.text = GetSuffix(priceBallNum);

        maxBallNum = 10 + maxBallNumLevel;
        maxBallPieces.text = maxBallNum.ToString();
        maxBallPieces.GetComponentsInChildren<Text>()[1].text = maxBallNumLevel.ToString() + " lv";

        if (maxBallNumLevel == 45) maxBallPrice.text = "MAX";
    }
    #endregion

    #region BallDamage
    [Header("Ball Damage")]
    public int damageLevel;
    public decimal ballDamage;
    public decimal basePriceBallDamage = 150m, priceBallDamage;
    public Text ballDamagePrice, ballDamagePieces;

    public void UpgradeBallDamage() {
        if (money >= priceBallDamage) {
            AddMoney(-priceBallDamage);
            damageLevel++;

            SetBallDamage();

            PlayerPrefs.SetInt("BALLDAMAGE", damageLevel);
        }
    }

    void SetBallDamage() {
        priceBallDamage = basePriceBallDamage * DecPow(damageLevel + 1, 3);
        ballDamagePrice.text = GetSuffix(priceBallDamage);

        ballDamage = 1 + damageLevel + (damageLevel / 4) * 2 + (damageLevel / 8) * 3 + (damageLevel / 15) * 15 + (damageLevel / 30) * 30 + (damageLevel / 60) * 45 + (damageLevel / 100) * 110;
        ballDamagePieces.text = ballDamage.ToString();
        ballDamagePieces.GetComponentsInChildren<Text>()[1].text = damageLevel.ToString() + " lv";
    }
    #endregion

    #region TapDamage
    [Header("Tap Damage")]
    public int tapDamageLevel;
    public decimal tapDamage;
    public decimal basePriceTapDamage = 50, priceTapDamage;
    public Text tapDamagePrice, tapDamagePieces;

    public void UpgradeTapDamage() {
        if (money >= priceTapDamage) {
            AddMoney(-priceTapDamage);
            tapDamageLevel++;

            SetTapDamage();

            PlayerPrefs.SetInt("TAPDAMAGE", tapDamageLevel);
        }
    }

    void SetTapDamage() {
        priceTapDamage = basePriceTapDamage * DecPow(tapDamageLevel + 1, 2) + (basePriceTapDamage / 2) * DecPow(tapDamageLevel, 2);
        if (tapDamageLevel > 10) priceTapDamage += basePriceTapDamage * DecPow(tapDamageLevel + 1, 2);
        tapDamagePrice.text = GetSuffix(priceTapDamage);

        tapDamage = 1 + tapDamageLevel + (tapDamageLevel / 4);
        tapDamagePieces.text = tapDamage.ToString();
        tapDamagePieces.GetComponentsInChildren<Text>()[1].text = tapDamageLevel.ToString() + " lv";
    }
    #endregion

    #region BallSpeed
    [Header("Ball Speed")]
    public float ballSpeed;
    public int ballSpeedLevel;
    public decimal basePriceBallSpeed = 500, priceBallSpeed;
    public Text ballSpeedPrice, ballSpeedPieces;

    public void UpgradeBallSpeed() {
        if (money >= priceBallSpeed && ballSpeedLevel < 15) {
            AddMoney(-priceBallSpeed);
            ballSpeedLevel++;

            SetBallSpeed();

            PlayerPrefs.SetInt("BALLSPEED", ballSpeedLevel);
        }
    }

    void SetBallSpeed() {
        priceBallSpeed = basePriceBallSpeed * DecPow(ballSpeedLevel + 1, 4);
        ballSpeedPrice.text = GetSuffix(priceBallSpeed);

        ballSpeed = 6.5f + ballSpeedLevel / 5f;
        ballSpeedPieces.text = ballSpeed.ToString() + " m/s";
        ballSpeedPieces.GetComponentsInChildren<Text>()[1].text = ballSpeedLevel.ToString() + " lv";

        if (ballSpeedLevel == 15) ballSpeedPrice.text = "MAX";
    }
    #endregion

    #region OfflineEarning
    [Header("Offline Earning")]
    public int offlinePercent, offlineLevel;
    public Text offlinePrice, offlinePercentText;
    public decimal basePriceOfflinePercent = 750, priceOfflineEarning;

    public void UpgradeOfflineIncome() {
        if (money >= priceOfflineEarning && offlineLevel < 15) {
            AddMoney(-priceOfflineEarning);
            offlineLevel++;

            SetOfflineIncome();

            PlayerPrefs.SetInt("OFFLINE", offlineLevel);
        }
    }

    void SetOfflineIncome() {
        priceOfflineEarning = basePriceOfflinePercent * DecPow(offlineLevel + 1, 6);
        offlinePrice.text = GetSuffix(priceOfflineEarning);

        offlinePercent = offlineLevel * 5;
        offlinePercentText.text = offlinePercent.ToString() + "%";
        offlinePercentText.GetComponentsInChildren<Text>()[1].text = offlineLevel.ToString() + " lv";

        if (offlineLevel == 15) offlinePercentText.GetComponentsInChildren<Text>()[1].text = "MAX";
    }
    #endregion

    #region FireRate
    [Header("Fire Rate")]
    public float fireRate;
    public int fireRateLevel;
    public decimal basePriceFireRate = 50, priceFireRate;
    public Text fireRatePrice, fireRatePieces;

    public void UpgradeFireRate() {
        if (money >= priceFireRate && fireRateLevel < 21) {
            AddMoney(-priceFireRate);
            fireRateLevel++;

            SetFireRate();

            PlayerPrefs.SetInt("FIRERATE", fireRateLevel);
        }
    }

    void SetFireRate() {
        priceFireRate = basePriceFireRate * DecPow(fireRateLevel + 1, 4);
        fireRatePrice.text = GetSuffix(priceFireRate);

        fireRate = 2.0f + fireRateLevel / 2f;
        fireRatePieces.text = fireRate.ToString() + " /sec";
        fireRatePieces.GetComponentsInChildren<Text>()[1].text = fireRateLevel.ToString() + " lv";

        if (fireRateLevel == 21) fireRatePrice.text = "MAX";
    }
    #endregion

    /// <summary>
    /// Loading PlayerPrefs
    /// </summary>
    void LoadPrefs() {
        money = decimal.Parse(PlayerPrefs.GetString("MONEY", "0"));

        fireRateLevel = PlayerPrefs.GetInt("FIRERATE");
        damageLevel = PlayerPrefs.GetInt("BALLDAMAGE");
        ballSpeedLevel = PlayerPrefs.GetInt("BALLSPEED");
        maxBallNumLevel = PlayerPrefs.GetInt("MAXBALLNUM");
        tapDamageLevel = PlayerPrefs.GetInt("TAPDAMAGE");
        offlineLevel = PlayerPrefs.GetInt("OFFLINE");
    }

    public Text offlineIncomeText;

    /// <summary>
    /// Getting offline reward
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetDay() {
        //WWW www = new WWW("http://leatonm.net/wp-content/uploads/2017/candlepin/getdate.php");
        yield return null;

        //DateTime currentDateTime = DateTime.Parse(www.text.Replace('/', ' '));
        DateTime currentDateTime = DateTime.Now;

        if (!PlayerPrefs.HasKey("OLD_LOGIN_TIME") || offlinePercent == 0) {
            PlayerPrefs.SetString("OLD_LOGIN_TIME", currentDateTime.ToString());
            yield break;
        }


        DateTime oldDate = DateTime.Parse(PlayerPrefs.GetString("OLD_LOGIN_TIME"));
        TimeSpan sub = currentDateTime - oldDate;

        if (sub.TotalMinutes < 3) yield break;

        offlineIncomeText.transform.parent.parent.gameObject.SetActive(true);

        AddMoney(decimal.Round(DMin((int)sub.TotalMinutes, 12 * 60) * ballDamage * (int)fireRate * LevelBar._instance.level * offlinePercent / 40));

        offlineIncomeText.text = GetSuffix(decimal.Round((DMin((int)sub.TotalMinutes, 12 * 60) * ballDamage * (int)fireRate * LevelBar._instance.level * offlinePercent) / 40));
        Debug.Log("OFFLINE REWARD " + (int)sub.TotalMinutes);

        PlayerPrefs.SetString("OLD_LOGIN_TIME", currentDateTime.ToString());

        //In case of clicking on DOUBLE MONEY
        ADs._instance.moneyToAdd = decimal.Round(DMin((int)sub.TotalMinutes, 6 * 60) * ballDamage * (int)fireRate * LevelBar._instance.level * offlinePercent / 40);
    }

    /// <summary>
    /// Choosing minimum value of two decimals
    /// </summary>
    /// <param name="d1"></param>
    /// <param name="d2"></param>
    /// <returns></returns>
    public decimal DMin(decimal d1, decimal d2) {
        if (d1 < d2) return d1;
        else return d2;
    }

    /// <summary>
    /// Choosing maximum value of two decimals
    /// </summary>
    /// <param name="d1"></param>
    /// <param name="d2"></param>
    /// <returns></returns>
    public decimal DMax(decimal d1, decimal d2) {
        if (d1 > d2) return d1;
        else return d2;
    }

    public float blockSpeed = 0.25f;

    /// <summary>
    /// Setting blocks speed slider
    /// </summary>
    /// <param name="s"></param>
    public void SetBlockSpeed(Slider s) {
        blockSpeed = Mathf.Lerp(0.2f, 0.6f, s.value);
    }

    /// <summary>
    /// On Purchase Successful method for "Max firerate boost"
    /// </summary>
    public void MaxFireRate() {
        fireRateLevel = 20;
        PlayerPrefs.SetInt("FIRERATE", fireRateLevel);

        fireRate = 2.5f + fireRateLevel / 2;
        fireRatePieces.text = fireRate.ToString() + "/sec";
        fireRatePieces.GetComponentsInChildren<Text>()[1].text = fireRateLevel.ToString() + " lv";

        if (fireRateLevel == 20) fireRatePrice.text = "MAX";
    }

    public void TapDamageBoost() {
        tapDamageLevel = 20;
        PlayerPrefs.SetInt("TAPDAMAGE", tapDamageLevel);

        SetTapDamage();
    }

}
