using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;


/// <summary>
/// 게임내에서 Middle 관련 UI 제어와 연관이 깊다. 
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager main = null;

    [Header("Stage")]
    public int Stage = 1;
    public int KillCount = 200;

    int DPSLevel, DiscountLevel;

    // 캐릭터 장착 슬롯
    [Header("Unit Equipment")]
    public List<EquipSlot> ListEquipSlot;

    [Header("Kill Count")]
    public Text TextKillCount;

    [Header("Passive")]
    public GameObject PassiveDPS;
    public GameObject PassiveDiscount;
    public Text TextPassiveDPS, TextPassiveDiscount;


    const string KeyEquipSlot = "EquipSlot";
    const string KeyStage = "CurrentStage";
    const string KeyKillCountg = "CurrentKillCount";

    const int MaxKillCount = 200;

    void Awake() {
        main = this;
        PIER.OnRefreshPlayerInfo += RefreshInfo;
    }


    void Start() {

        for (int i = 0; i < ListEquipSlot.Count; i++) {
            ListEquipSlot[i].InitEquipSlot(i);
        }

        LoadStageMemory();
        LoadEquipSlotMemory();
    }

    void RefreshInfo() {

        DPSLevel = PIER.main.DamageLevel;
        DiscountLevel = PIER.main.DiscountLevel;

        if (DPSLevel == 1)
            PassiveDPS.SetActive(false);
        else {
            PassiveDPS.SetActive(true);
        }


        if (DiscountLevel == 1)
            PassiveDiscount.SetActive(false);
        else {
            PassiveDiscount.SetActive(true);
        }



    }

    #region Equip Slot, Data Save & Load

    /// <summary>
    /// 스테이지 진행도 불러오기 
    /// </summary>
    void LoadStageMemory() {
        Stage = PlayerPrefs.GetInt(KeyStage, 1);
        KillCount = PlayerPrefs.GetInt(KeyKillCountg, MaxKillCount);


        TextKillCount.text = "Destroy " + KillCount.ToString() + " Enemies";
    }

    /// <summary>
    /// 스테이지 진행도 저장 
    /// </summary>
    void SaveStageMemory() {
        PlayerPrefs.SetInt(KeyStage, Stage);
        PlayerPrefs.SetInt(KeyKillCountg, KillCount);
        PlayerPrefs.Save();
    }


    void LoadEquipSlotMemory() {

        int level;

        for(int i=0; i<ListEquipSlot.Count;i++) {
            level = PlayerPrefs.GetInt(KeyEquipSlot + i.ToString(), 0);

            if (level <= 0)
                continue;

            ListEquipSlot[i].EquipUnit(UnitData.Instance.Rows[level - 1]);
        }
    }

    void SaveEquipSlotMemory() {
        for(int i =0; i<ListEquipSlot.Count; i++) {
            if(ListEquipSlot[i].equipUnit == null)
                PlayerPrefs.SetInt(KeyEquipSlot + i.ToString(), 0);
            else 
                PlayerPrefs.SetInt(KeyEquipSlot + i.ToString(), ListEquipSlot[i].equipUnit._level);
        }

        PlayerPrefs.Save();

    }
    #endregion


    private void OnApplicationPause(bool pause) {
        if (pause) {
            SaveEquipSlotMemory();
            SaveStageMemory();
        }
    }

    private void OnApplicationQuit() {
        SaveEquipSlotMemory();
    }

}
