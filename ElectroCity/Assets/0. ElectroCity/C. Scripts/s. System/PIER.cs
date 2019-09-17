using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIER : MonoBehaviour
{
    public static PIER main = null;

    /// <summary>
    /// 유저 정보
    /// </summary>
    public int UserLevel, EXP, DamageLevel, DiscountLevel;


    void Awake() {
        main = this;
    }

    // Start is called before the first frame update
    void Start() {
        LoadData(); 
    }

    #region 데이터 Save & Load 

    public void SaveData() {
        PlayerPrefs.SetInt("UserLevel", UserLevel);
        PlayerPrefs.SetInt("EXP", EXP);
        PlayerPrefs.SetInt("DamageLevel", DamageLevel);
        PlayerPrefs.SetInt("DiscountLevel", DiscountLevel);

        PlayerPrefs.Save();
    }


    /// <summary>
    /// 데이터 로드
    /// </summary>
    void LoadData() {
        UserLevel = PlayerPrefs.GetInt("UserLevel", 1);
        EXP = PlayerPrefs.GetInt("EXP", 1);
        DamageLevel = PlayerPrefs.GetInt("DamageLevel", 1);
        DiscountLevel = PlayerPrefs.GetInt("DiscountLevel", 1);

    }

    #endregion

}
