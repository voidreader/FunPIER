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

    public Text _textLevel;
    public Text _textEXP;

    public Progressor _levelBar;

    public int PlayerLevel;
    public int EXP;
    public int MaxEXP;
    float levelValue;
    LevelDataRow currentLevelRow;

    private void Awake() {
        main = this;
        PIER.OnRefreshPlayerInfo += RefreshPlayerInfo;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }


    /// <summary>
    /// 
    /// </summary>
    void RefreshPlayerInfo() {


        SetLevelProgressor(true);

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

            
            StartCoroutine(DelaySetLevelProgressor());

        }
        else {

            PIER.main.EXP = EXP;
            PIER.main.SaveData(false); // 저장.
            SetLevelProgressor(false);
        }

    }

    IEnumerator DelaySetLevelProgressor() {
        yield return new WaitForSeconds(0.5f);
        SetLevelProgressor(false);

    }



}
