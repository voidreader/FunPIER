using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;


public class DailyRewardView : MonoBehaviour
{
    public static DailyRewardView main = null;
    public List<DailyRewardRow> ListRows;
    public List<DailyRewardDataRow> ListDailyRewardData;
    public List<Sprite> ListRowSprite; // 각 행들 바탕 스프라이트 

    public Weapon rewardWeapon; // 5일차 보상 무기

         
    private void Awake() {
        main = this;
    }

    public void OnView() {
        SetRewardWeapon(); // 보상 5일차 무기 처리

        for(int i =0; i<ListRows.Count;i++) {

            if (i < 4)
                ListRows[i].SetRow(i);
            else
                ListRows[i].SetLastRow(rewardWeapon);
        }
    }

    /// <summary>
    /// 새로운 무기가 없으면, 출석체크 창은 더이상 띄우지 않는다.
    /// </summary>
    /// <returns></returns>
    public static bool CheckNewDailyRewardWeapon() {
        return main.SetRewardWeapon();
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
        return false;
    }
}
