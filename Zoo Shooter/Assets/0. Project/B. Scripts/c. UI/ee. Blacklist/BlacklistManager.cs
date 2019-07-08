using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;

public class BlacklistManager : MonoBehaviour {

    public int _currentLevel, _currentList;
    public List<BossPortrait> _listBoss;

    List<BossDataRow> list;

    
    

    public Text _lblList;
    


    public void OnView() {

        

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
    }

}
