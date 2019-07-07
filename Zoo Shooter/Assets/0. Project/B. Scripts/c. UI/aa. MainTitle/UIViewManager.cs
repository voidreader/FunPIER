using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Doozy.Engine.Progress;

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



    private void Awake() {
        main = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
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

        
    }

    public void OnViewSubscribe() {
        _subAura1.transform.localEulerAngles = Vector3.zero;
        _subAura2.transform.localEulerAngles = Vector3.zero;

        _subAura1.transform.DOLocalRotate(new Vector3(0, 0, 720), 3, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        _subAura2.transform.DOLocalRotate(new Vector3(0, 0, -720), 3, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

    }
}
