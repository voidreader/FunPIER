using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;

public class BlacklistManager : MonoBehaviour {

    public static bool isClaimAvailable = false;  //  쓸 필요가 있나.. 고민중 

    public int _currentLevel, _currentList;
    public List<BossPortrait> _listBoss; // 보스 목록 

    List<BossDataRow> list; // 기준정보 
    public GameObject _btnClaim, _btnBack; // 보상받기 버튼 
    public Text _lblList; // 타이틀 텍스트 
    


    public void OnView() {

        _btnClaim.SetActive(false);
        _btnBack.SetActive(true);

        _currentLevel = PIER.CurrentLevel;
        _currentList = PIER.CurrentList;

        InitList();
    }


    /// <summary>
    /// 리스트 초기화 
    /// </summary>
    void InitList() {

        _lblList.text = "LIST NO." + (PIER.CurrentList + 1).ToString();
        

        for (int i=0; i<_listBoss.Count; i++) {
            _listBoss[i].OffPortrait();
        }

        list = PIER.main.GetCurrentBlacklist();
        
        for(int i=0; i<list.Count;i++) {
            _listBoss[i].OnPortrait(list[i]);
        }

        // 보상 받을 수 있는지 체크 
        isClaimAvailable = true;
        for(int i=0; i<_listBoss.Count; i++) {

            if (!_listBoss[i].gameObject.activeSelf)
                continue;

            if(!_listBoss[i].IsCaptured()) {  // 잡혔는지 안잡혔는지 체크 
                isClaimAvailable = false; // 하나라도 안잡힌게 있으면. false 
                break;
            }
        }

        // 보상받을 수 있는 상태면 타이틀 바꾸고 버튼 상태 변경
        if(isClaimAvailable) {
            _btnBack.SetActive(false);
            _btnClaim.SetActive(true);
            _lblList.text = "List is completed!";
        }
    }


    /// <summary>
    /// 보상 받기 누르기 
    /// </summary>
    public void OnClickClaim() {
        
    }

    public void OnClickBack() {

    }

}
