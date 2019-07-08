using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.Progress;
using DG.Tweening;
using Google2u;

public class GameViewManager : MonoBehaviour {

    public static GameViewManager main = null;
    public static bool isContinueWatchAD = false;


    public Text _lblScore, _lblLevel;
    public Text _lblBossName1, _lblBossName2;
    public Progressor _levelProgressor; // 스테이지 Progressor
    public Progressor _bossHP; // 보스 HP 

    public GameObject _bossGroup; // 보스 그룹들.. 

    public Transform _redBox1, _redBox2, _redBox3, _frame, _warning;
    public Image _portrait; // 보스 초상화 

    public BossDataRow _bossData; // 보스데이터
    public PlusScore _plusScore; // 스코어처리
    public int _currentScore = 0;
    public int _currentBossHP = 0;

    private void Awake() {
        main = this;
    }


    public void OnView() {

        // 광고를 보고 돌아온 경우는 초기화 하지 않는다. 
        if (isContinueWatchAD) 
            return; 


        // 보스 데이터 처리 
        _bossData = BossData.Instance.Rows[PIER.CurrentLevel];
        _currentBossHP = _bossData._hp;

        Debug.Log(">> GameViewManager BossHP :: " + _currentBossHP);

        _bossGroup.SetActive(false);
        _lblBossName1.text = _bossData._name;
        _lblBossName2.text = _bossData._name;
        
        _portrait.sprite = PIER.main.GetBossPortraitSprite(_bossData._portrait);


        // 스코어 및 ... 등등 
        _lblScore.text = "0";
        _lblLevel.text = "Level " + (PIER.CurrentLevel + 1).ToString();
        _levelProgressor.InstantSetValue(0);
        _currentScore = 0;


    }


    /// <summary>
    /// 보스 등장 처리 
    /// </summary>
    public void AppearBoss() {

        Debug.Log("Appear Boss..!!");
        GameManager.isWait = true; // 연출 끝날때까지 대기 

        

        _redBox1.localPosition = new Vector3(-750, 490, 0);
        _redBox2.localPosition = new Vector3(-750, 416, 0);
        _redBox3.localPosition = new Vector3(-750, 342, 0);

        _frame.localPosition = new Vector3(-925, 416, 0);
        _warning.localPosition = new Vector3(-608, 447, 0);
        _lblBossName1.transform.localPosition = new Vector3(-609, 380, 0);

        _bossGroup.SetActive(true);

        // 무브무브!
        _redBox1.DOLocalMoveX(0, 0.8f);
        _redBox2.DOLocalMoveX(0, 0.7f).SetDelay(0.1f);
        _redBox3.DOLocalMoveX(0, 0.6f).SetDelay(0.2f);

        _frame.DOLocalMoveX(-175, 0.7f).SetDelay(0.2f);
        _warning.DOLocalMoveX(142, 0.7f).SetDelay(0.2f);
        _lblBossName1.transform.DOLocalMoveX(141, 0.7f).SetDelay(0.2f).OnComplete(OnCompleteFirstMove);
    }

    void OnCompleteFirstMove() {
        _bossHP.gameObject.SetActive(true);
        _levelProgressor.gameObject.SetActive(false);
        _bossHP.InstantSetValue(1);
        Invoke("AppearBossStep2", 1);
    }

    void AppearBossStep2() {
        _redBox1.DOLocalMoveX(750, 0.6f);
        _redBox2.DOLocalMoveX(750, 0.6f).SetDelay(0.1f);
        _redBox3.DOLocalMoveX(750, 0.6f).SetDelay(0.2f);

        _frame.DOLocalMoveX(_frame.localPosition.x + 750, 0.6f);
        _warning.DOLocalMoveX(_warning.localPosition.x + 750, 0.6f);
        _lblBossName1.transform.DOLocalMoveX(_lblBossName1.transform.localPosition.x + 750, 0.6f).OnComplete(OnCompleteAppear);
    }

    void OnCompleteAppear() {
        GameManager.isWait = false;
    }

    /// <summary>
    /// 레벨 프로그레서 
    /// </summary>
    /// <param name="v"></param>
    public void SetLevelProgressor(float v) {
        _levelProgressor.SetValue(v);
    }

    /// <summary>
    /// 보스 HP
    /// </summary>
    /// <param name="v"></param>
    public void SetBossHP(float v) {
        _bossHP.SetValue(v);
    }

    public void AddScore(int s, bool isDouble = false) {

        _currentScore += s;
        _lblScore.text = _currentScore.ToString();

        // _lblScore.text =
        _plusScore.GetScore(s, isDouble);
    }


    /// <summary>
    /// 보스 체력 게이지 처리 
    /// </summary>
    /// <param name="damage"></param>
    public void CalcBossHP(int damage) {
        float v;
        _currentBossHP -= damage;

        if (_currentBossHP <= 0) {
            _bossHP.SetValue(0);
            return;
        }

        v = (float)_currentBossHP / (float)_bossData._hp;

        _bossHP.SetValue(v);


    }

}
