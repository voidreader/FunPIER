using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;

public class WantedReward : MonoBehaviour
{
    public List<WantedRewardCol> listRewards;
    
    public List<WantedRewardDataRow> listRewardData;
    public Weapon _weapon; 


    public void OnView() {

        listRewardData = WantedRewardData.Instance.Rows;

        List<WantedRewardCol> list = new List<WantedRewardCol>();
        for(int i=0; i< listRewards.Count;i++) {
            list.Add(listRewards[i]);
        }



        // 
        for(int i=0; i<listRewardData.Count; i++) {

            _weapon = Stocks.GetWeaponByID(listRewardData[i]._weaponid);
            if (!PIER.main.HasGun(_weapon)) {
                break;// 보유하고 있지 않은 무기로 설정 
            }
        }

        WantedRewardCol col = list[Random.Range(0, list.Count)];

        // 




    }
}
