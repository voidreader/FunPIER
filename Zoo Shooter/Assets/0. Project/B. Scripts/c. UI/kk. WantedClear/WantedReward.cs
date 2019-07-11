using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;
using Doozy.Engine;

public class WantedReward : MonoBehaviour
{
    public List<WantedRewardCol> listRewards; // 각 컬럼 (수정하지 말것)
    public List<WantedRewardDataRow> listRewardData; // 기준정보 (무기)


    public Weapon _weapon; // 4개중에 1개는 무기보상 


    public List<WantedRewardCol> listSelect; // 선택용 임시 리스트 
    public GameObject btnNoThanks, btnAgain; // 버튼 2개
    WantedRewardCol selectedCol;
    public bool isGetFirstReward = false;


    /// <summary>
    /// 첫 시작
    /// </summary>
    public void OnView() {

        isGetFirstReward = false; 

        btnNoThanks.SetActive(false);
        btnAgain.SetActive(false);

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

        // 선택 연출 시작
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

            if (selectIndex >= listSelect.Count) // 개수 넘어가면 다시 0으로 인덱스 변경 
                selectIndex = 0;
        }

        // 연출 끝났으면 선택된.. 
        selectedCol = null;
        for(int i=0; i<listSelect.Count;i++) {
            if (listSelect[i].IsSelect())
                selectedCol = listSelect[i];
        }

        // 선택된 컬럼 연출 처리 
        // 재화 처리 
        if(selectedCol == null) {
            Debug.Log(">> selectedCol is null Error!");
            yield break;
        }

        selectedCol.SetSelectEffect();
        // 재화 획득처리 
        

        if(isGetFirstReward) { // 두번째 돌림 

            // 잠깐 대기탔다가 메인으로 돌아간다 
            yield return new WaitForSeconds(2);

            GameEventMessage.SendEvent("CallMainFromWantedReward");
        }
        else { // 첫 보상!
            // 버튼 두개 활성화
            btnAgain.SetActive(true);
            btnNoThanks.SetActive(true);
            isGetFirstReward = true; // 한번 받았음!

            // 보상을 받아야 리스트 증가 처리 
            PIER.CurrentList++;
            PIER.main.SaveData();
        }


    }

    /// <summary>
    /// 다시 돌리기 
    /// </summary>
    public void OnClickAgain() {
        // 
        listSelect.Remove(selectedCol); // 이미 받은건 제거

        // 
        OnCompleteWatch();
    }


    void OnCompleteWatch() {
        StartCoroutine(Selecting());
    }


    public void OnClickNoThanks() {
        // 메인으로 고고 
    }

}
