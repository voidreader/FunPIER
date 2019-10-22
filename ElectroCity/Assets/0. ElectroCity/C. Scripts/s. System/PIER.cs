using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;

public class PIER : MonoBehaviour
{
    public static PIER main = null;
    public string[] numberSymbol = { "K", "M", "G", "T", "P", "E", "Z" };

    /// <summary>
    /// 플레이어 데이터 Refresh 
    /// </summary>
    public static Action OnRefreshPlayerInfo = delegate { };

    /// <summary>
    /// 유저 정보
    /// </summary>
    public int UserLevel, EXP, DamageLevel, DiscountLevel;
    public int HighestUnitLevel; // 개방한 최고 레벨 유닛 
    public List<int> ListUnitPurchaseStep = new List<int>();
    // public List<int> ListMergeSpotMemory = new List<int>();
    public int[] ArrSpotMemory = new int[16];

    const string KeyUnitPurchaseStep = "UnitPurchaseStep";
    const string KeySpot = "Spot";

    void Awake() {
        main = this;

    }

    // Start is called before the first frame update
    void Start() {
        LoadData();

        OnRefreshPlayerInfo();
    }

    #region 데이터 Save & Load 

    public void SaveData(bool refresh = true) {
        PlayerPrefs.SetInt("UserLevel", UserLevel);
        PlayerPrefs.SetInt("EXP", EXP);
        PlayerPrefs.SetInt("DamageLevel", DamageLevel);
        PlayerPrefs.SetInt("DiscountLevel", DiscountLevel);
        PlayerPrefs.SetInt("HighestUnitLevel", HighestUnitLevel);

        PlayerPrefs.Save();

        if(refresh)
            OnRefreshPlayerInfo();
    }


    /// <summary>
    /// 데이터 로드
    /// </summary>
    void LoadData() {
        UserLevel = PlayerPrefs.GetInt("UserLevel", 1);
        EXP = PlayerPrefs.GetInt("EXP", 0);
        DamageLevel = PlayerPrefs.GetInt("DamageLevel", 1);
        DiscountLevel = PlayerPrefs.GetInt("DiscountLevel", 1);
        HighestUnitLevel = PlayerPrefs.GetInt("HighestUnitLevel", 1);

        LoadUnitPurchaseStep();
        LoadMergeSpotMemory();
    }


    #region 머지 스팟 위치 기억 LoadMergeSpotMemory & SaveMergeSpotMemory

    /// <summary>
    /// 머지 스팟 기억 불러오기 
    /// </summary>
    public void LoadMergeSpotMemory() {
        
        int l;

        // Spot 정보는 -2 : 스페셜 박스, -1 : 걍 박스, 0 : 비었음. 
        for (int i =0; i< 16; i++) { // 최대 16자리라고 가정 한다.

            l = PlayerPrefs.GetInt(KeySpot + i.ToString(), 0);
            ArrSpotMemory[i] = l; // 배열로 저장 

        }
    }

    /// <summary>
    /// 머지 스팟 위치 저장 
    /// </summary>
    public void SaveMergeSpotMemory() {
        for (int i = 0; i < MergeSystem.main.ListSlots.Count; i++) {

            if (MergeSystem.main.ListSlots[i].mergeItem == null)
                continue;

            PlayerPrefs.SetInt(KeySpot + i.ToString(), MergeSystem.main.ListSlots[i].mergeItem.Level);
        }

        PlayerPrefs.Save();
    }

    #endregion


    #region 유닛 구매 단계  LoadUnitPurchaseStep, SaveUnitPurchaseStep

    void LoadUnitPurchaseStep() {
        ListUnitPurchaseStep.Clear();

        int step;

        // 40가지의 유닛이 존재한다. 
        for(int i = 0; i < 40; i++) {
            step = PlayerPrefs.GetInt(KeyUnitPurchaseStep + (i + 1).ToString(), 0);
            ListUnitPurchaseStep.Add(step);
        }
    }

    public void SaveUnitPurchaseStep(int unitLevel) {
        // 1 더한다. 
        ListUnitPurchaseStep[unitLevel - 1] = ListUnitPurchaseStep[unitLevel - 1] + 1;

        // 개별저장 
        PlayerPrefs.SetInt(KeyUnitPurchaseStep + unitLevel.ToString(), ListUnitPurchaseStep[unitLevel - 1]);
        PlayerPrefs.Save();
    }

    #endregion




    public void SetDamageLevel(int l) {
        DamageLevel = l + 1;
        SaveData();
    }

    public void SetDiscountLevel(int l) {
        DiscountLevel = l + 1;
        SaveData();
    }

    #endregion



    /// <summary>
    /// 단위 붙여서 숫자 표시 
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string GetBigNumber(long num) {
        // 999,999 까진 그냥 숫자로 표시
        // 1,000,000 부터는(자리수 7자리) => 1,000 K 로 표시 
        // 999,999K 까지 표시..1,000,000 K(자리수 7자리) => 1,000M
        // K(7~9), M(10~12), G(13~15), T(16~18), P(19~21)의 순서다. 
        // 1,000,000 => 1,000K.   1,000,000,000 => 1,000M. 1,000,000,000,000 => G. 1,000,000,000,000,000 => T
        //  28 -> B 
        int leng = num.ToString().Length;
        string result = string.Empty;
        // 가장 높은 단위부터 체크 ..
         
        if(leng > 21) {
            return string.Empty;
        }

        if (leng <= 21 && leng > 18) { // P (페타) - 10의 15승 
            // 앞 최소 4자리, 최대 6자리까지 잘라낸다. 
            // result = num.ToString().Substring(0, leng - 15);
            result = string.Format("{0:#,###}", GetCutNumberString(num.ToString(), 15));
            result += "P";
        }
        else if (leng <= 18 && leng > 15) { // T테라 - 10의 12승 
            result = string.Format("{0:#,###}", GetCutNumberString(num.ToString(), 12));
            result += "T";
        }
        else if (leng <= 15 && leng > 12) { // G - 10의 9승 
            result = string.Format("{0:#,###}", GetCutNumberString(num.ToString(), 9));
            result += "G";
        }
        else if (leng <= 12 && leng > 9) { // M - 10의 6승 
            result = string.Format("{0:#,###}", GetCutNumberString(num.ToString(), 6));
            result += "M";
        }
        else if (leng <= 9 && leng > 6) { // K - 10의 3승 
            result = string.Format("{0:#,###}", GetCutNumberString(num.ToString(), 3));
            result += "K";
        }
        else {
            result = string.Format("{0:#,###}", num);
        }

        return result;


    }

    static long GetCutNumberString(string r, int cut) {
        return long.Parse(r.Substring(0, r.Length - cut));
    }



    /// <summary>
    /// 데미지 패시브 계수 구하기 
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static string GetDamageLevelFactor(int level) {
        if (level <= 1)
            return "0%";

        for (int i=0; i<PassiveData.Instance.Rows.Count; i++) {

            if (!PassiveData.Instance.Rows[i]._rid.Contains("DAMAGE")) {
                continue;
            }

            if(PassiveData.Instance.Rows[i]._level == level-1) {
                return PassiveData.Instance.Rows[i]._factor + "%";
            }
        }


        return "0%";
        
    }

    /// <summary>
    /// 디스카운트 패시브 레벨 계수 구하기 
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static string GetDiscountLevelFactor(int level) {
        if (level <= 1)
            return "0%";

        for (int i = 0; i < PassiveData.Instance.Rows.Count; i++) {

            if (!PassiveData.Instance.Rows[i]._rid.Contains("DISCOUNT")) {
                continue;
            }

            if (PassiveData.Instance.Rows[i]._level == level - 1) {
                return PassiveData.Instance.Rows[i]._factor + "%";
            }
        }


        return "0%";

    }

}
