using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Google2u;
using PathologicalGames;

public class GameOverCtrl : MonoBehaviour {

    public GameObject _rate; // 평가 부분
    public UILabel _lblCong; // 축하합니다!
    public UILabel _lblResultInfo; // 추가 정보
    public UILabel _lblFinalScore; // 최종 스코어
    public UIButton _btnReplay; // 다시하기 
    public UIButton _btnHome; // 돌아가기 

    public GameObject _upperStar1, _upperStar2, _upperStar3; // 레이블 주위 별똥별.
    bool isUpgrade = false; // 박물관 진행(업그레이드) 여부 

    public ParticleSystem _confettiRainbow;
    public ParticleSystem _confettiGreenYellow;
    public ParticleSystem _confettiOrangeGreen;
    public ParticleSystem _poof;

    // Start is called before the first frame update
    void Start() {
        
    }

    void Init() {
        this.gameObject.SetActive(true);
        isUpgrade = false;

        _lblCong.gameObject.SetActive(false);
        _lblFinalScore.gameObject.SetActive(false);
        _lblResultInfo.gameObject.SetActive(false);

        _upperStar1.gameObject.SetActive(false);
        _upperStar2.gameObject.SetActive(false);
        _upperStar3.gameObject.SetActive(false);

        _btnHome.gameObject.SetActive(false);
        _btnReplay.gameObject.SetActive(false);

        _rate.SetActive(false);

        InGame.main.DisappearRedMoon();
    }


    /// <summary>
    /// 게임 클리어 처리 
    /// </summary>
    public void OpenGameClear() {
        Init();


        Debug.Log("Game Clear!!!!");

        _lblCong.text = PierSystem.GetLocalizedText(MLocal.rowIds.TEXT19);
        _lblResultInfo.text = PierSystem.GetLocalizedText(MLocal.rowIds.TEXT34);

        isUpgrade = true;


        StartCoroutine(GameOverRoutine());

    }

    /// <summary>
    /// 게임 오버 
    /// </summary>
    public void OpenGameOver() {


        Init();



        // 게임을 플레이 하기 전, PreviousStep과 비교해서 축하를 할지 위로를 할지. 
        if (PierSystem.main.GetPreviousProgress(InGame.currentTheme) < InGame.main.currentThemeStep) { // 축하 

            Debug.Log("Congratulation!!");

            AudioAssistant.main.PlayMusic("Win", false);
            

            _lblCong.text = PierSystem.GetLocalizedText(MLocal.rowIds.TEXT19);
            _lblResultInfo.text = PierSystem.GetLocalizedText(MLocal.rowIds.TEXT20);

            isUpgrade = true;
        }
        else { // 더 진행하지 못하고 Game Over

            Debug.Log("Game Over!!");
            AudioAssistant.main.PlayMusic("Lose", false);
            _lblCong.text = PierSystem.GetLocalizedText(MLocal.rowIds.TEXT27);
            _lblResultInfo.text = PierSystem.GetLocalizedText(MLocal.rowIds.TEXT28);

            isUpgrade = false;
        }


        StartCoroutine(GameOverRoutine());

    }

    IEnumerator GameOverRoutine() {

        Debug.Log(" >> GameOverRoutine Start , highscore :: " + PierSystem.main.GetHighScore(InGame.currentTheme));
        Debug.Log(" >> GameOverRoutine Start , currentScore :: " + InGame.main.currentScore);

        PlatformManager.main.SubmitScore(InGame.currentTheme, InGame.main.currentScore);

        // 하이스코어 처리
        if (PierSystem.main.GetHighScore(InGame.currentTheme) < InGame.main.currentScore) {
            PierSystem.main.SetHighScore(InGame.currentTheme, InGame.main.currentScore);
        }
        

        LobbyManager.isAnimation = true;

        yield return null;

        _lblCong.transform.localPosition = new Vector3(0, 420, 0);
        _lblCong.gameObject.SetActive(true);
        _lblCong.transform.DOLocalMoveY(460, 0.5f);

        _lblResultInfo.transform.localPosition = new Vector3(0, 350, 0);
        _lblResultInfo.gameObject.SetActive(true);
        _lblResultInfo.transform.DOLocalMoveY(380, 0.5f).SetDelay(0.2f);

        yield return new WaitForSeconds(0.5f);

        // if(isUpgrade) {
            
        // }


        // 스코어링 시작 
        StartCoroutine(Scoring());

        // 별 등장
        _upperStar1.transform.localPosition = new Vector3(0, 555, 0);
        _upperStar1.SetActive(true);
        _upperStar1.transform.DOLocalMoveY(575, 0.2f);

        _upperStar2.transform.localPosition = new Vector3(-220, 485, 0);
        _upperStar2.SetActive(true);
        _upperStar2.transform.DOLocalMoveY(505, 0.2f).SetDelay(0.1f);

        _upperStar3.transform.localPosition = new Vector3(220, 485, 0);
        _upperStar3.SetActive(true);
        _upperStar3.transform.DOLocalMoveY(505, 0.2f).SetDelay(0.2f);

        // 평가
        _rate.transform.localPosition = new Vector3(0, -1000, 0);
        _rate.SetActive(true);
        _rate.transform.DOLocalMoveY(-420, 1);

        // 다시하기 버튼 & 홈 버튼 

        if (isUpgrade) {
            // 업그레이드가 된 경우는 다시하기 버튼 나오지 않음
            _btnHome.transform.localPosition = new Vector3(0, 140, 0);
            _btnHome.gameObject.SetActive(true);
            // _btnHome.transform.DOLocalMoveX(160, 0.4f);

            StartCoroutine(ConfettiRoutine());

        }
        else {

            _btnReplay.transform.localPosition = new Vector3(-170, 160, 0);
            _btnReplay.gameObject.SetActive(true);
            _btnReplay.transform.DOLocalMoveX(-150, 0.4f);

            _btnHome.transform.localPosition = new Vector3(170, 160, 0);
            _btnHome.gameObject.SetActive(true);
            _btnHome.transform.DOLocalMoveX(150, 0.4f).SetDelay(0.2f);

            // StartCoroutine(PoofRoutine());
        }

        yield return new WaitForSeconds(0.5f);

        // 모든 연출 종료 
        LobbyManager.isAnimation = false;

    }

    /// <summary>
    /// 스코어 연출
    /// </summary>
    /// <returns></returns>
    IEnumerator Scoring() {

        int s = 0;
        int t = InGame.main.currentScore / 20;
        _lblFinalScore.gameObject.SetActive(true);

        AudioAssistant.Shot("Scoring");

        for (int i=0; i<20; i++) {
            _lblFinalScore.text = string.Format("{0:n0}", s);
            yield return new WaitForSeconds(0.05f);

            s += t;
        }

        _lblFinalScore.text = string.Format("{0:n0}", InGame.main.currentScore);
    }

    /// <summary>
    /// 별점주기
    /// </summary>
    public void OnClickRate() {
        AdsControl.main.IsCoolingPauseAds = true;
        Application.OpenURL("http://onelink.to/azvmtn");
    }

    /// <summary>
    /// 다시하기
    /// </summary>
    public void OnClickReplay() {
        this.gameObject.SetActive(false);
        InGame.main.RestartSession();

        // 업그레이드 요소가 없으면 전면 배너 오픈 
        if(!isUpgrade) {
            AdsControl.main.ShowInterstitial();
        }

    }

    /// <summary>
    /// 홈 
    /// </summary>
    public void OnClickHome() {
        this.gameObject.SetActive(false);
        InGame.main.CloseSession();


        // 업그레이드 요소가 없으면 전면 배너 오픈 
        if (!isUpgrade) {
            AdsControl.main.ShowInterstitial();
        }
    }


    /// <summary>
    ///  축하용 파티클 연출
    /// </summary>
    /// <returns></returns>
    IEnumerator ConfettiRoutine() {

        yield return new WaitForSeconds(1);

        AudioAssistant.Shot("Confetti");
        PoolManager.Pools[ConstBox.poolIngame].Spawn(_confettiRainbow, new Vector3(-2f, 4f), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);

        AudioAssistant.Shot("Confetti");
        PoolManager.Pools[ConstBox.poolIngame].Spawn(_confettiOrangeGreen, new Vector3(2.5f, 4.5f), Quaternion.identity);
        yield return new WaitForSeconds(0.35f);

        AudioAssistant.Shot("Confetti");
        PoolManager.Pools[ConstBox.poolIngame].Spawn(_confettiOrangeGreen, new Vector3(-1.5f, -0.5f), Quaternion.identity);
        yield return new WaitForSeconds(0.4f);

        AudioAssistant.Shot("Confetti");
        PoolManager.Pools[ConstBox.poolIngame].Spawn(_confettiRainbow, new Vector3(2.2f, 2f), Quaternion.identity);
    }

    IEnumerator PoofRoutine() {
        //DustDirtyPoof4

        yield return new WaitForSeconds(1);

        AudioAssistant.Shot("Confetti");
        PoolManager.Pools[ConstBox.poolIngame].Spawn(_poof, new Vector3(-2f, 4f), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);

        AudioAssistant.Shot("Confetti");
        PoolManager.Pools[ConstBox.poolIngame].Spawn(_poof, new Vector3(2.5f, 4.5f), Quaternion.identity);
        yield return new WaitForSeconds(0.35f);

        AudioAssistant.Shot("Confetti");
        PoolManager.Pools[ConstBox.poolIngame].Spawn(_poof, new Vector3(-1.5f, -0.5f), Quaternion.identity);
        yield return new WaitForSeconds(0.4f);

        AudioAssistant.Shot("Confetti");
        PoolManager.Pools[ConstBox.poolIngame].Spawn(_poof, new Vector3(2.2f, 2f), Quaternion.identity);

    }
}

