using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Doozy.Engine.Progress;

using Doozy.Engine;

/// <summary>
/// View - Main 만을 담당. 
/// </summary>
public class UIViewManager : MonoBehaviour
{

    public static UIViewManager main = null;

    // 타이틀의 개체들
    public Transform _titleTop, _titleBottom;
    public Image _progressBlacklist;
    public Text _bestScore, _currentLevel;


    // 구독
    public Transform _subAura1, _subAura2;
    public Progressor _wantedProgressor;


    public GameObject _infiniteSign;
    public Transform _btnSubscribe, _btnWanted, _btnCinema, _btnGunshop;


    private void Awake() {
        main = this;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update() {

        
        if(Input.GetKeyDown(KeyCode.F)) {
            // GameEventMessage.SendEvent("FakeWantedReward");
            CheckDailyReward();
        }
        
        
    }


    /// <summary>
    /// 인게임 플레이 후 데일리 리워드 체크.. 여기에 두는게 맞나싶다. 
    /// </summary>
    public void CheckDailyReward() {
        StartCoroutine(DailyRewardCheckRoutine());
    }

    IEnumerator DailyRewardCheckRoutine() {
        yield return new WaitForSeconds(0.5f); // 트윈이 있어서 대기한다. 

        if(DailyRewardView.CheckNewDailyRewardWeapon() && DailyRewardView.CheckNewDay()) {
            GameEventMessage.SendEvent("DailyRewardEvent"); // 무기 보상있고, 새로운 날이면 고고!
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public void OnViewMain() {

        _currentLevel.text = "Level " + (PIER.CurrentLevel + 1).ToString();
        _bestScore.text = "BEST : " + PIER.main.BestScore.ToString();

        // _progressBlacklist.f
        _titleTop.transform.localPosition = new Vector3(-700, _titleTop.transform.localPosition.y, 0);
        _titleBottom.transform.localPosition = new Vector3(700, _titleBottom.transform.localPosition.y, 0);


        _titleTop.transform.DOLocalMoveX(0, 0.5f);
        _titleBottom.transform.DOLocalMoveX(0, 0.5f);

        // Wanted 게이지 처리 
        _wantedProgressor.InstantSetValue(0);
        _wantedProgressor.SetValue(PIER.main.GetWantedListProgressValue());


        // 인피니트 모드 여부 , 버튼 배열 처리 
        if (PIER.main.InfiniteMode) {
            _infiniteSign.SetActive(true);
            _btnWanted.gameObject.SetActive(false);

            // 3버튼(wanted 없음)
            _btnSubscribe.localPosition = new Vector3(-160f, -405f, 0);
            _btnCinema.localPosition = new Vector3(0f, -405f, 0);
            _btnGunshop.localPosition = new Vector3(160f, -405f, 0);


        }
        else {
            _infiniteSign.SetActive(false);
            _btnWanted.gameObject.SetActive(true);

            // 4버튼
            _btnSubscribe.localPosition = new Vector3(-240f, -405f, 0);
            _btnWanted.localPosition = new Vector3(-80f, -405f, 0);
            _btnCinema.localPosition = new Vector3(80f, -405f, 0);
            _btnGunshop.localPosition = new Vector3(240f, -405f, 0);
        }

        


        AudioAssistant.main.PlayMusic("Main");
    }

    public void OnViewSubscribe() {
        _subAura1.transform.localEulerAngles = Vector3.zero;
        _subAura2.transform.localEulerAngles = Vector3.zero;

        _subAura1.transform.DOLocalRotate(new Vector3(0, 0, 720), 3, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        _subAura2.transform.DOLocalRotate(new Vector3(0, 0, -720), 3, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

    }



    /// <summary>
    /// Tap to start 터치 시작. 
    /// </summary>
    public void OnClickGamePlay() {

        if(PIER.main.InfiniteMode) {
            GameEventMessage.SendEvent("GamePlayEvent"); // 받을 보상 없으면 고고 
            GameManager.main.OnClickPlay(); // 게임플레이 시작.
            return;
        }

        // 보상 받다가, 게임 꺼진 경우를 대비한다. 
        if(PIER.main.HasWantedReward()) {
            GameEventMessage.SendEvent("GameClearEvent");
        }
        else {
            GameEventMessage.SendEvent("GamePlayEvent"); // 받을 보상 없으면 고고 
            GameManager.main.OnClickPlay(); // 게임플레이 시작.
        }
    }

}
