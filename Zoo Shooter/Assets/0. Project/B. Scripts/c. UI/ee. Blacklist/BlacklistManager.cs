using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;

public class BlacklistManager : MonoBehaviour {

    public int _currentLevel, _currentList;
    public List<BossPortrait> _listBoss;

    List<BossDataRow> list;

    public bool isRewardable = false;
    public Image btnReward;

    public Sprite _rewardableSprite;
    public Sprite _unrewardableSprite;


    public void OnView() {

        isRewardable = false;

        _currentLevel = PIER.CurrentLevel;
        _currentList = PIER.CurrentList;

        InitList();
    }


    /// <summary>
    /// 리스트 초기화 
    /// </summary>
    void InitList() {

        isRewardable = true;

        for (int i=0; i<_listBoss.Count; i++) {
            _listBoss[i].OffPortrait();
        }

        list = PIER.main.GetCurrentBlacklist();
        
        for(int i=0; i<list.Count;i++) {
            _listBoss[i].OnPortrait(list[i]);

            if (list[i]._level <= PIER.CurrentLevel)
                isRewardable = false;
        }

        // 보상받을 수 있는지 체크
        if (isRewardable)
            btnReward.sprite = _rewardableSprite;
        else
            btnReward.sprite = _unrewardableSprite;
        

    }

}
