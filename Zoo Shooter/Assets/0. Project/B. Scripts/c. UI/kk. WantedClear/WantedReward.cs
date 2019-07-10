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

    public List<WantedRewardCol> listSelect;


    public void OnView() {

        listRewardData = WantedRewardData.Instance.Rows;
        listSelect = new List<WantedRewardCol>();

        // 설정용
        List<WantedRewardCol> list = new List<WantedRewardCol>();

        for(int i=0; i< listRewards.Count;i++) {
            list.Add(listRewards[i]);
            listSelect.Add(listRewards[i]);
        }

        // 
        for (int i=0; i<listRewardData.Count; i++) {

            _weapon = Stocks.GetWeaponByID(listRewardData[i]._weaponid);
            if (!PIER.main.HasGun(_weapon)) {
                break;// 보유하고 있지 않은 무기로 설정 
            }
        }

        WantedRewardCol col = list[Random.Range(0, list.Count)];
        col.SetSpecialReward(_weapon); // 스페셜 리워드 설정
        list.Remove(col);
        
        
        // 코인 25, 50, 75 
        col = list[Random.Range(0, list.Count)];
        col.SetCommonReward(25);
        list.Remove(col);
        

        col = list[Random.Range(0, list.Count)];
        col.SetCommonReward(50);
        list.Remove(col);

        col = list[Random.Range(0, list.Count)];
        col.SetCommonReward(75);
        list.Remove(col);


        StartCoroutine(Selecting());
        
    }

    IEnumerator Selecting() {

        yield return new WaitForSeconds(1);

        int max = Random.Range(18, 32);
        
        int selectIndex = 0;

        for(int i=0; i<max;i++) {

            // 모두 다 Unselect 
            for (int j = 0; j < listSelect.Count; j++) {
                listSelect[j].SetUnselect();
            }

            // 순서대로 select 처리 
            listSelect[selectIndex++].SetSelect();

            yield return new WaitForSeconds(0.1f);

            if (selectIndex >= listSelect.Count)
                selectIndex = 0;

        }



    }

}
