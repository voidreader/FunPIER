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
    
    public int _currentScore = 0;
    public int _currentBossHP = 0;

    public int _getScoreIndex = 0;
    public List<PlusScore> ListGetScores; // 스코어 획득 표시 Pooling 용도 
    public List<PlusScore> ListActiveScores; // 액티브된 스코어 획득 표시들.. (여러개 표시될 수 있다)


    /* 인피니트 모드 */
    public GameObject _infiniteGroup;
    public Text _textKillCount, _textInfiniteBossName; // 킬카운트, 보스 이름 
    public Transform _infiniteBox1, _infiniteBox2, _infiniteBox3, _infiniteText;
    public Progressor _InfiniteHP; // 인피니트 모드 보스 HP 

    private void Awake() {
        main = this;
    }


    public void OnView() {

        // 광고를 보고 돌아온 경우는 초기화 하지 않는다. 
        if (isContinueWatchAD) 
            return;


        _bossGroup.SetActive(false);
        _infiniteGroup.SetActive(false);

        if (PIER.main.InfiniteMode) {
            // 아무것도 하지 않음.
        }
        else {
            // 보스 데이터 처리 
            _bossData = BossData.Instance.Rows[PIER.CurrentLevel];
            _currentBossHP = _bossData._hp;

            Debug.Log(">> GameViewManager BossHP :: " + _currentBossHP);


            _lblBossName1.text = _bossData._name;
            _lblBossName2.text = _bossData._name;

            _portrait.sprite = PIER.main.GetBossPortraitSprite(_bossData._portrait);


            // 스코어 및 ... 등등 
            _lblScore.text = "0";
            _lblLevel.text = "Level " + (PIER.CurrentLevel + 1).ToString();
            _levelProgressor.gameObject.SetActive(true);
            _levelProgressor.InstantSetValue(0);
            _currentScore = 0;
        }





        AudioAssistant.main.PlayMusic("Battle");

    }

    #region 인피니트 모드 


    /// <summary>
    /// 인피니트 모드 보스 정보 세팅 
    /// </summary>
    /// <param name="bossIndex"></param>
    public void SetInfiniteBossInfo(int bossIndex) {


        // 보스 HP 등장 
        _InfiniteHP.gameObject.SetActive(true);
        _InfiniteHP.InstantSetValue(1); // HP 만피로 설정 

        _bossData = BossData.Instance.Rows[bossIndex];
        _currentBossHP = _bossData._hp;

        _textInfiniteBossName.text = _bossData._name;
        

    }

    /// <summary>
    /// 인피니트 모드 시작 
    /// </summary>
    public void ShowInfiniteStart() {
        Debug.Log("ShowInfiniteStart..!!");
        GameManager.isWait = true;

        _infiniteBox1.localPosition = new Vector3(-750, 490, 0);
        _infiniteBox2.localPosition = new Vector3(-750, 416, 0);
        _infiniteBox3.localPosition = new Vector3(-750, 342, 0);
        _infiniteText.localPosition = new Vector3(-750, 415, 0);

        _infiniteGroup.SetActive(true); // 인피니티 그룹만 사용한다.
        _InfiniteHP.gameObject.SetActive(false);
        _bossHP.gameObject.SetActive(false);
        _levelProgressor.gameObject.SetActive(false);

        // 무브무브!
        _infiniteBox1.DOLocalMoveX(0, 0.8f);
        _infiniteBox2.DOLocalMoveX(0, 0.7f).SetDelay(0.1f);
        _infiniteBox3.DOLocalMoveX(0, 0.6f).SetDelay(0.2f);

        _infiniteText.DOLocalMoveX(0, 0.7f).SetDelay(0.2f).OnComplete(OnCompleteInfiniteFirstMove);
    }

    void OnCompleteInfiniteFirstMove() {
        Debug.Log("OnCompleteInfiniteFirstMove..!!");

        // 박스들이 중앙에 도착하면 인피니트 보스 HP 세팅
        SetInfiniteBossInfo(0); // 첫번째 보스. 


        Invoke("SetInfiniteBoxOut", 1);
    }

    void SetInfiniteBoxOut() {
        // 박스 빠지고 
        _infiniteBox1.DOLocalMoveX(750, 0.6f);
        _infiniteBox2.DOLocalMoveX(750, 0.6f).SetDelay(0.1f);
        _infiniteBox3.DOLocalMoveX(750, 0.6f).SetDelay(0.2f);

        // 텍스트 빠지고 
        _infiniteText.DOLocalMoveX(750, 0.6f).OnComplete(OnCompleteAppear);
        // _lblBossName1.transform.DOLocalMoveX(_lblBossName1.transform.localPosition.x + 750, 0.6f).OnComplete(OnCompleteAppear);
    }


    #endregion

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
        _bossHP.gameObject.SetActive(false);

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

        // 이거 인피니트랑 공유중이니까 주의할것. 
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


        ListGetScores[_getScoreIndex++].GetScore(s, ListActiveScores.Count, isDouble);

        if (_getScoreIndex >= ListGetScores.Count)
            _getScoreIndex = 0;


        // _lblScore.text =
        //_plusScore.GetScore(s, isDouble);
    }


    /// <summary>
    /// 보스 체력 게이지 처리 
    /// </summary>
    /// <param name="damage"></param>
    public void CalcBossHP(int damage, bool isHeadShot = false) {
        float v;

        _currentBossHP -= damage;
        if(isHeadShot)
            _currentBossHP -= damage;

        if (_currentBossHP <= 0) {
            _bossHP.SetValue(0);
            return;
        }

        v = (float)_currentBossHP / (float)_bossData._hp;

        _bossHP.SetValue(v);


    }

}
