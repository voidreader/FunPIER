using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MuseumStruct : MonoBehaviour {

    public int maxStep = 0;
    public int currentStep = 0;

    public Theme theme = Theme.Car;

    public float standardScale;
    public Transform _baseTransform; // 회전용 기본 모델



    public List<Transform> listStep1 = new List<Transform>();
    public List<Transform> listStep2 = new List<Transform>();
    public List<Transform> listStep3 = new List<Transform>();
    public List<Transform> listStep4 = new List<Transform>();
    public List<Transform> listStep5 = new List<Transform>();
    public List<Transform> listStep6 = new List<Transform>();
    public List<Transform> listStep7 = new List<Transform>();
    public List<Transform> listStep8 = new List<Transform>();
    public List<Transform> listStep9 = new List<Transform>();
    public List<Transform> listStep10 = new List<Transform>();
    public List<Transform> listStep11 = new List<Transform>();
    public List<Transform> listStep12 = new List<Transform>();
    public List<Transform> listStep13 = new List<Transform>();
    public List<Transform> listStep14 = new List<Transform>();

    public List<List<Transform>> listStepObjects = new List<List<Transform>>();

    // Start is called before the first frame update
    void Start()  {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    /// <param name="m"></param>
    public void InitMuseumStep(int c, int m) {
        maxStep = m;
        currentStep = c;

        Debug.Log("InitMuseumStep :: " + theme);

        // 14단계 입력
        listStepObjects.Add(listStep1);
        listStepObjects.Add(listStep2);
        listStepObjects.Add(listStep3);
        listStepObjects.Add(listStep4);
        listStepObjects.Add(listStep5);
        listStepObjects.Add(listStep6);
        listStepObjects.Add(listStep7);
        listStepObjects.Add(listStep8);
        listStepObjects.Add(listStep9);
        listStepObjects.Add(listStep10);
        listStepObjects.Add(listStep11);
        listStepObjects.Add(listStep12);
        listStepObjects.Add(listStep13);
        listStepObjects.Add(listStep14);

        // 일단 다 비활성화
        for (int i = 0; i < maxStep; i++) {
            for (int j = 0; j < listStepObjects[i].Count; j++) {
                listStepObjects[i][j].gameObject.SetActive(false);
            }
        }

        this.gameObject.SetActive(true);

        IdleMoving(0); 


        // 현재 스텝까지만 보여주게 처리 
        for (int i=0; i<currentStep; i++) {
            for(int j=0; j<listStepObjects[i].Count; j++) {
                listStepObjects[i][j].gameObject.SetActive(true);
            }

            IdleMoving(i+1);
        }
    }

    #region InitObjectsTransform() 

    /// <summary>
    /// 오브젝트들의 회전값, 위치 등 초기화 
    /// </summary>
    void InitObjectsTransform() {

        _baseTransform.DOKill();

        switch(theme) {
            case Theme.Car:
                // _baseTransform.localEulerAngles = new Vector3(0, 0, 0);
                _baseTransform.localEulerAngles = new Vector3(0, 50, 0);



                listStep2[0].transform.DOKill();
                listStep2[0].transform.localPosition = new Vector3(2.09504f, 5.568235f, 0.12691f);
                listStep2[0].transform.eulerAngles = new Vector3(-90, 0, 0);

                listStep2[1].transform.DOKill();
                listStep2[1].transform.eulerAngles = new Vector3(-90, 0, 0);
                listStep2[1].transform.localPosition = new Vector3(0.32586f, 5.233376f, 3.07556f);

                listStep3[0].transform.DOKill();
                listStep3[0].transform.localPosition = new Vector3(-0.0166f, 4.657842f, -3.88278f);
                listStep3[1].transform.DOKill();
                listStep3[1].transform.localEulerAngles = Vector3.zero;
                break;

            case Theme.Wine:
                // _baseTransform.localEulerAngles = new Vector3(-90, 0, 0);
                _baseTransform.localEulerAngles = new Vector3(-90, 0, 0);

                listStep1[1].transform.DOKill();
                listStep1[1].transform.localPosition = new Vector3(0.02036037f, -0.01167562f, -0.002455181f);
                listStep1[2].transform.DOKill();
                listStep1[2].transform.localPosition = new Vector3(0.01072971f, 0.02231506f, -0.002455181f);
                listStep3[0].transform.DOKill();
                listStep3[0].transform.localPosition = new Vector3(-0.025891f, -0.004980526f, 0.001420102f);
                listStep4[1].transform.DOKill();
                listStep4[1].transform.localEulerAngles = Vector3.zero;
                listStep8[0].transform.DOKill();
                listStep8[0].transform.localEulerAngles = Vector3.zero;
                listStep10[0].transform.DOKill();
                listStep10[0].transform.localPosition = new Vector3(-0.01280104f, 0.0004438782f, 0.08470623f);

                listStep5[1].transform.DOKill();
                listStep5[1].transform.localEulerAngles = Vector3.zero;
                listStep7[0].transform.DOKill();
                listStep7[0].transform.localEulerAngles = Vector3.zero;

                break;

            case Theme.Viking:
                // _baseTransform.localEulerAngles = new Vector3(-90, 0, 0);
                _baseTransform.localEulerAngles = new Vector3(-90, 20, 0);

                listStep1[1].transform.DOKill();
                listStep1[1].transform.localPosition = new Vector3(-0.01997795f, -0.02931089f, 0.007697856f);

                listStep2[1].transform.DOKill();
                listStep2[1].transform.localPosition = new Vector3(-0.02914677f, -0.0004445457f, 0.02021695f);

                listStep3[1].transform.DOKill();
                listStep3[1].transform.localPosition = new Vector3(0.03229805f, 0.0245434f, 0.004921727f);

                listStep4[1].transform.DOKill();
                listStep4[1].transform.localPosition = new Vector3(-0.03460052f, -0.02160149f, 0.02402929f);

                listStep5[0].transform.DOKill();
                listStep5[0].transform.localEulerAngles = Vector3.zero;

                listStep5[1].transform.DOKill();
                listStep5[1].transform.localPosition = new Vector3(0.01546141f, 0.04023971f, 0.07328384f);

                listStep10[0].transform.DOKill();
                listStep10[0].transform.localEulerAngles = Vector3.zero;

                break;

        }

    }

    #endregion


    /// <summary>
    /// 박물관 구조물 업그레이드
    /// </summary>
    /// <param name="s">인게임의 current step</param>
    public void UpgradeMuseum(int s) {

        // 현재 단계가 인게임 마지막 플레이 단계보다 크거나 같으면 return. 
        if (currentStep >= s)
            return;

        Debug.Log("Current Step :: " + currentStep);
        Debug.Log("Next Step :: " + s);

        StartCoroutine(UpgradeRoutine(s));
    }

    IEnumerator UpgradeRoutine(int s) {

        Debug.Log(">> UpgradeRoutine :: " + s + "/" + maxStep);
        LobbyManager.isAnimation = true;
        InitObjectsTransform();

        int upgradeIndex = 0;

        yield return new WaitForSeconds(1);

        LobbyManager.main.ShotFireworks(); // 불꽃놀이 시작 


        // 업적 체크
        if (s == maxStep && theme == Theme.Car)
            PlatformManager.main.UnlockAchievements(MIMAchievement.CompleteCar);
        else if (s == maxStep && theme == Theme.Wine)
            PlatformManager.main.UnlockAchievements(MIMAchievement.CompleteWine);
        else if (s == maxStep && theme == Theme.Viking)
            PlatformManager.main.UnlockAchievements(MIMAchievement.CompleteViking);



        for (int i = currentStep; i < s; i++) {


            if (i > maxStep)
                break;

            for (int j = 0; j < listStepObjects[i].Count; j++) {
                listStepObjects[i][j].transform.localScale = Vector3.zero;
                listStepObjects[i][j].gameObject.SetActive(true);
                listStepObjects[i][j].transform.DOScale(standardScale, 0.4f).SetEase(Ease.OutBack);

                // 파티클 이펙트 추가
                LobbyManager.main.ShowUpgradeStar(listStepObjects[i][j].transform.position);
                PlayUpgradeSound(upgradeIndex++);



                Debug.Log("Struct upgrade! step :: " + i);

                yield return new WaitForSeconds(0.3f);

            }


        }

        AudioAssistant.Shot("ComboFinal");
        yield return new WaitForSeconds(1);

        /*
        for(int i=currentStep; i<=s; i++) {
            IdleMoving(i);
        }
        */


        IdleMoving(0);
        for (int i = 1; i <= s; i++) {
            IdleMoving(i);
        }


        currentStep = s; // 업그레이드 하고 currentstep 정보 저장 
        LobbyManager.isAnimation = false;

    }

    /// <summary>
    /// 업그레이드 사운드 
    /// </summary>
    /// <param name="index"></param>
    void PlayUpgradeSound(int index) {
        if (index == 0)
            AudioAssistant.Shot("Combo1");
        else if (index == 1)
            AudioAssistant.Shot("Combo2");
        else if (index == 2)
            AudioAssistant.Shot("Combo3");
        else if (index == 3)
            AudioAssistant.Shot("Combo4");
        else if (index == 4)
            AudioAssistant.Shot("Combo5");
        else
            AudioAssistant.Shot("Combo5");
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="step"></param>
    public void IdleMoving(int step) {

        //Debug.Log("IdleMoving! :: " + step);

        // 기본 모델 회전 

        if (step == 0) {
            //Sequence baseSeq = DOTween.Sequence();
            //baseSeq.Append(_baseTransform.DOLocalRotate(new Vector3(_baseTransform.localEulerAngles.x, 50, 0), Random.Range(8f, 14f), RotateMode.Fast).SetEase(Ease.Linear));
            //baseSeq.Append(_baseTransform.DOLocalRotate(new Vector3(_baseTransform.localEulerAngles.x, 50, 0), Random.Range(8f, 14f), RotateMode.Fast).SetEase(Ease.Linear));

            _baseTransform.DOKill();

            /*
            if(theme == Theme.Car)
                _baseTransform.localEulerAngles = new Vector3(0, 0, 0);
            else
                _baseTransform.localEulerAngles = new Vector3(-90, 0, 0);

            _baseTransform.DOLocalRotate(new Vector3(_baseTransform.localEulerAngles.x, 360, 0), Random.Range(12f, 16f), RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
            */

            if (theme == Theme.Car) {
                _baseTransform.localEulerAngles = new Vector3(0, 50, 0);
                _baseTransform.DOLocalRotate(new Vector3(_baseTransform.localEulerAngles.x, -50, 0), Random.Range(8f, 15f), RotateMode.Fast).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            }
            else if (theme == Theme.Wine) {
                _baseTransform.localEulerAngles = new Vector3(-90, 0, 0);
                _baseTransform.DOLocalRotate(new Vector3(_baseTransform.localEulerAngles.x, -60, 0), Random.Range(8f, 15f), RotateMode.Fast).SetEase(Ease.Linear).OnComplete(OnLoopRotateWine);
                //_baseTransform.DOLocalRotate(new Vector3(_baseTransform.localEulerAngles.x, 40, 0), Random.Range(8f, 15f), RotateMode.Fast).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            }
            else if (theme == Theme.Viking) {
                _baseTransform.localEulerAngles = new Vector3(-90, 20, 0);
                _baseTransform.DOLocalRotate(new Vector3(_baseTransform.localEulerAngles.x, -70, 0), Random.Range(10f, 16f), RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            }

            

        }

        switch (theme) {
            
            case Theme.Car:
                #region Theme Car
                if (step == 2) {
                    if(listStep2.Count > 0 && listStep2[0].gameObject.activeSelf) {
                        listStep2[0].transform.DOKill();
                        listStep2[0].transform.localPosition = new Vector3(2.09504f, 5.568235f, 0.12691f);
                        listStep2[0].transform.eulerAngles = new Vector3(-90, 0, 0);
                        listStep2[0].transform.DOLocalMoveY(6.5f, 4).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                        // listStep2[0].GetComponent<Rigidbody>().DORotate(new Vector3(-90, 360, 0), 4, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
                        // RotateObject(listStep2[0].transform, false);

                        // listStep2[0].transform.DOLocalRotate(new Vector3(-90, 360, 0), 4, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
                    }

                    if (listStep2.Count > 1 && listStep2[1].gameObject.activeSelf) {
                        listStep2[1].transform.DOKill();
                        listStep2[1].transform.eulerAngles = new Vector3(-90, 0, 0);
                        listStep2[1].transform.localPosition = new Vector3(0.32586f, 5.233376f, 3.07556f);

                        listStep2[1].transform.DOLocalMoveY(6.2f, 1.8f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                        // listStep2[1].transform.rot
                        // RotateObject(listStep2[1].transform, false);
                        //listStep2[1].transform.DORotate(new Vector3(-90, -360, 0), 4, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
                    }
                }

                if(step == 3) {
                    if (listStep3.Count > 0 && listStep3[0].gameObject.activeSelf) {
                        listStep3[0].transform.DOKill();
                        // listStep3[0].transform.localEulerAngles = new Vector3(0, -90, -90);
                        listStep3[0].transform.localPosition = new Vector3(-0.0166f, 4.657842f, -3.88278f);
                        listStep3[0].transform.DOLocalMoveY(6.5f, 2).SetLoops(-1, LoopType.Yoyo);
                        // listStep3[0].transform.DOLocalRotate(new Vector3(360, -90, 90), 5, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
                        // listStep3[0].transform.DOLocalRotate(new Vector3(360, -90, -90), 5, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
                        // RotateObject(listStep3[0].transform, true);
                    }

                    if (listStep3.Count > 1 && listStep3[1].gameObject.activeSelf) {
                        listStep3[1].transform.DOKill();
                        listStep3[1].transform.localEulerAngles = Vector3.zero;
                        listStep3[1].transform.DOLocalRotate(new Vector3(360, 0, 0), 5, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
                    }

                }
                #endregion
                break;

            case Theme.Wine:
                #region 
                if(step == 1) {
                    listStep1[1].transform.DOKill();
                    listStep1[2].transform.DOKill();

                    listStep1[1].transform.localPosition = new Vector3(0.02036037f, -0.01167562f, -0.002455181f);
                    listStep1[2].transform.localPosition = new Vector3(0.01072971f, 0.02231506f, -0.002455181f);

                    listStep1[1].transform.DOLocalMoveZ(-0.001f, 0.8f).SetLoops(-1, LoopType.Yoyo);
                    listStep1[2].transform.DOLocalMoveZ(-0.00122f, 0.8f).SetLoops(-1, LoopType.Yoyo);

                }

                if(step == 3) {
                    Debug.Log("Wine Step 3 D!");
                    listStep3[0].transform.DOKill();
                    listStep3[0].transform.localPosition = new Vector3(-0.025891f, -0.004980526f, 0.001420102f);
                    listStep3[0].transform.DOLocalMoveZ(0.003f, 1).SetLoops(-1, LoopType.Yoyo);
                }

                if(step == 4) {
                    listStep4[1].transform.DOKill();
                    listStep4[1].transform.localEulerAngles = Vector3.zero;
                    listStep4[1].transform.DOLocalRotate(new Vector3(0, 0, -720), 8, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
                }

                if(step == 5) {
                    listStep5[1].transform.DOKill();
                    listStep5[1].transform.localEulerAngles = Vector3.zero;
                    listStep5[1].transform.DOLocalRotate(new Vector3(0, 0, -720), 10, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
                }

                if (step == 7) {
                    listStep7[0].transform.DOKill();
                    listStep7[0].transform.localEulerAngles = Vector3.zero;
                    listStep7[0].transform.DOLocalRotate(new Vector3(0, 0, -720), 10, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
                }


                if (step == 8) {
                    listStep8[0].transform.DOKill();
                    listStep8[0].transform.localEulerAngles = Vector3.zero;
                    listStep8[0].transform.DOLocalRotate(new Vector3(0, 0, -720), 10, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
                }

                if(step == 10) {
                    listStep10[0].transform.DOKill();
                    listStep10[0].transform.localPosition = new Vector3(-0.01280104f, 0.0004438782f, 0.08470623f);
                    listStep10[0].transform.DOLocalMoveZ(0.09f, 4).SetLoops(-1, LoopType.Yoyo);
                        
                }
                #endregion

                break;

            case Theme.Viking:

                if(step == 1) {
                    listStep1[1].transform.DOKill();
                    listStep1[1].transform.localPosition = new Vector3(-0.01997795f, -0.02931089f, 0.007697856f);
                    listStep1[1].transform.DOLocalMoveZ(0.0018f, 3f).SetLoops(-1, LoopType.Yoyo);
                }

                if (step == 2) {
                    listStep2[1].transform.DOKill();
                    listStep2[1].transform.localPosition = new Vector3(-0.02914677f, -0.0004445457f, 0.02021695f);
                    listStep2[1].transform.DOLocalMoveZ(0.0631f, 2.5f).SetLoops(-1, LoopType.Yoyo);
                }

                if (step == 3) {
                    listStep3[1].transform.DOKill();
                    listStep3[1].transform.localPosition = new Vector3(0.03229805f, 0.0245434f, 0.004921727f);
                    listStep3[1].transform.DOLocalMoveZ(-0.0019f, 1.5f).SetLoops(-1, LoopType.Yoyo);
                }

                if (step == 4) {
                    listStep4[1].transform.DOKill();
                    listStep4[1].transform.localPosition = new Vector3(-0.03460052f, -0.02160149f, 0.02402929f);
                    listStep4[1].transform.DOLocalMoveZ(0.032f, 2f).SetLoops(-1, LoopType.Yoyo);
                    // 0.032
                }


                if(step == 5) {
                    listStep5[0].transform.DOKill();
                    listStep5[0].transform.localEulerAngles = Vector3.zero;
                    listStep5[0].transform.DOLocalRotate(new Vector3(0, 0, 360), 4, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);

                    listStep5[1].transform.DOKill();
                    listStep5[1].transform.localPosition = new Vector3(0.01546141f, 0.04023971f, 0.07328384f);
                    listStep5[1].transform.DOLocalMoveZ(0.083f, 3f).SetLoops(-1, LoopType.Yoyo);
                }

                if(step == 10) {
                    listStep10[0].transform.DOKill();
                    listStep10[0].transform.localEulerAngles = new Vector3(-10, 0, 0);
                    listStep10[0].transform.DOLocalRotate(new Vector3(10, 0, 0), 2, RotateMode.Fast).SetLoops(-1, LoopType.Yoyo);
                }


                break;
        }

        
        
        

    }

    void OnLoopRotateWine() {
        _baseTransform.DOLocalRotate(new Vector3(_baseTransform.localEulerAngles.x, 40, 0), Random.Range(8f, 15f), RotateMode.Fast).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }


    public void RotateObject(Transform t, bool isX) {
        StartCoroutine(Rotating(t, isX));

    }

    IEnumerator Rotating(Transform t, bool isX) {
        Vector3 originRot = t.localEulerAngles;
        float axis = 0;

        while(true) {

            

            if(isX) {
                // t.localEulerAngles = new Vector3(axis, originRot.y, originRot.z);
                t.Rotate(360 * Time.deltaTime, originRot.y, originRot.z, Space.Self);
            }
            else { // Y 축 기준
                // t.localEulerAngles = new Vector3(originRot.x, axis, originRot.z);
                t.Rotate(originRot.x, 360 * Time.deltaTime, originRot.z, Space.Self);
            }

            /*
            axis += 10;

            if (axis >= 360)
                axis = 0;
            */


            // yield return new WaitForSeconds(0.05f);
            yield return null;
        }
    }

    
}
