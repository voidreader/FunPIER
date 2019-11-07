using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;

using Doozy.Engine.Progress;

/// <summary>
/// 플레이어 정보 
/// </summary>
public class PlayerInfo : MonoBehaviour
{

    public static PlayerInfo main = null;
    public static bool isOnLevelUp = false; // 레벨업중..? 

    
    public BottomButtonCtrl HomeButton;

    public Text _textLevel;
    public Text _textEXP;

    public Progressor _levelBar;

    public int PlayerLevel;
    public int EXP;
    public int MaxEXP;
    float levelValue;
    LevelDataRow currentLevelRow;

    [Header("Coin Bar")]
    public Text _textCoin;
    public long Coin;

    [Header("Gem Bar")]
    public Text _textGem;
    public int Gem;

    private void Awake() {
        main = this;
        PIER.OnRefreshPlayerInfo += RefreshPlayerInfo;
    }


    // Start is called before the first frame update
    void Start()
    {
        HomeButton.SetActive(true);
    }


    /// <summary>
    /// 
    /// </summary>
    void RefreshPlayerInfo() {

        SetLevelProgressor(true);
        SetCoinBar();
        SetGemBar();

    }

    void SetCoinBar() {
        Coin = PIER.main.Coin;

        if (Coin <= 0)
            _textCoin.text = "0";
        else 
            _textCoin.text = PIER.GetBigNumber(Coin);
    }

    void SetGemBar() {
        Gem = PIER.main.Gem;
        _textGem.text = Gem.ToString();
    }

    public void AddCoin(long c) {
        Coin += c;
        PIER.main.Coin = Coin;
        PIER.main.SaveData();
    }

    public void AddGem(int g) {
        Gem += g;
        PIER.main.Gem = Gem;
        PIER.main.SaveData();
    }




    /// <summary>
    /// 레벨 게이지 처리하기. 
    /// </summary>
    /// <param name="isInstant">바 연출 하지 않음 </param>
    void SetLevelProgressor(bool isInstant) {
        PlayerLevel = PIER.main.UserLevel;
        EXP = PIER.main.EXP;

        currentLevelRow = LevelData.Instance.Rows[PlayerLevel - 1];
        MaxEXP = currentLevelRow._needexp;

        levelValue = (float)EXP / (float)MaxEXP;

        if(isInstant)
            _levelBar.InstantSetValue(levelValue);
        else
            _levelBar.SetValue(levelValue);


        _textLevel.text = PlayerLevel.ToString();
        _textEXP.text = EXP.ToString() + "/" + MaxEXP.ToString();
    }

    /// <summary>
    /// 경험치 추가 
    /// </summary>
    /// <param name="xp"></param>
    public void AddExp(int xp) {

        EXP += xp;

        // 레벨업에 필요한 경험치와 수치 비교
        if (EXP >= MaxEXP) {
            // 레벨업 처리
            PlayerLevel++;
            EXP = EXP - MaxEXP;

            // 게이지가 차 오르는... 연출 후에 레벨업이 되어야 함. 
            PIER.main.UserLevel = PlayerLevel;
            PIER.main.EXP = EXP;
            PIER.main.SaveData(false);
            _levelBar.SetValue(1);

            isOnLevelUp = true; // Unlock 과 동시에 열리는 상황을 방지한다. 
            StartCoroutine(DelaySetLevelProgressor());

        }
        else {

            PIER.main.EXP = EXP;
            PIER.main.SaveData(false); // 저장.
            SetLevelProgressor(false);
        }

    }

    IEnumerator DelaySetLevelProgressor() {
        // 게이지가 줄어드는 시간을 준다. 
        yield return new WaitForSeconds(0.2f);
        SetLevelProgressor(false);
        Debug.Log(">> LevelUpEvent <<");
        Doozy.Engine.GameEventMessage.SendEvent("LevelUpEvent"); // 창 오픈!

    }


    /// <summary>
    /// 플레어이가 사용 가능한 머지공간 개수 
    /// </summary>
    /// <returns></returns>
    public static int GetAvailableMergeSpot() {

        int v = 0;

        for(int i=0; i<main.PlayerLevel; i++) {
            v += LevelData.Instance.Rows[i]._mergespot;
        }

        return v;
    }

    /// <summary>
    /// 플레이어가 사용 가능한 전투포지션 개수 
    /// </summary>
    /// <returns></returns>
    public static int GetAvailableBattleSpot() {
        int v = 0;

        for (int i = 0; i < main.PlayerLevel; i++) {
            v += LevelData.Instance.Rows[i]._battlespot;
        }

        return v;
    }

}
