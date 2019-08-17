using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;
using DG.Tweening;



public class DailyRewardView : MonoBehaviour
{
    public static DailyRewardView main = null;
    public List<DailyRewardRow> ListRows;
    public List<DailyRewardDataRow> ListDailyRewardData;
    public List<Sprite> ListRowSprite; // 각 행들 바탕 스프라이트 

    public Weapon rewardWeapon; // 5일차 보상 무기

    public Button btnOK;
    public int currentDay = 0;
         
    private void Awake() {
        main = this;
    }

    public void OnView() {

        btnOK.gameObject.SetActive(false);
        SetRewardWeapon(); // 보상 5일차 무기 처리

        for(int i =0; i<ListRows.Count;i++) {

            if (i < 4)
                ListRows[i].SetRow(i);
            else
                ListRows[i].SetLastRow(rewardWeapon);
        }

        // 오늘이 몇일차인지 가져온다
        currentDay = PIER.main.DailyRewardDay;

        for(int i=0; i<currentDay; i++) { // 오늘 이전날까지는 받음 처리 
            ListRows[i].SetTakenRow();
        }

        Invoke("InvokedTakeTodayReward", 1.5f);
    }

    void InvokedTakeTodayReward() {
        ListRows[currentDay].TakeReward(); // 보상 받기 
    }

    public void OnCompleteTake() {
        btnOK.transform.localScale = Vector3.zero;
        btnOK.gameObject.SetActive(true);
        btnOK.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);

        currentDay++;
        PIER.main.SaveDailyRewardDay(currentDay); // 보상받았으면 일자 정보 저장 
    }

    


    /// <summary>
    /// 새로운 무기가 없으면, 출석체크 창은 더이상 띄우지 않는다.
    /// </summary>
    /// <returns></returns>
    public static bool CheckNewDailyRewardWeapon() {
        return main.SetRewardWeapon();
    }

    public static bool CheckNewDay() {

        int today = System.DateTime.Now.DayOfYear;
        if (today != PIER.main.DayOfYear) // 새로운 날! -- 출첵 창 오픈 가능 
            return true;
        else {

            Debug.Log("Already get today daily reward!!");

            return false; // 같은 날!
        }
    }

    bool SetRewardWeapon() {
        // 이번 리스트 보상 찾기 
        ListDailyRewardData = DailyRewardData.Instance.Rows;
        for (int i = 0; i < ListDailyRewardData.Count; i++) {
            rewardWeapon = Stocks.GetWeaponByID(ListDailyRewardData[i]._weaponid);

            if (!PIER.main.HasGun(rewardWeapon)) {
                return true;
            }

        }

        rewardWeapon = null; // 다 받아서 보상이 음슴.
        Debug.Log("There no more daily reward weapon!!");
        return false;
    }
}
