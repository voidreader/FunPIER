using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Google2u;

public class ViewBossCall : MonoBehaviour
{
    public List<BossDataRow> ListBoss = new List<BossDataRow>(); // 5개의 보스 픽 
    public List<BossDataRow> ListTemp = new List<BossDataRow>();

    public List<BossDataRow> ListRandomOrder = new List<BossDataRow>(); // 랜덤 order.

    public List<Image> ListNomineeSprite = new List<Image>();

    public List<Image> ListFrame; // 상단 초상화

    public bool isStopped = false;


    // 선택된 보스 정보 
    public Text TextBossName;
    public Image GradeTag; 

    [Header("Moving Objects")]
    public Transform MommyHead; // 엄마 머리
    public Image LightOn; // 우측 불빛
    public GameObject ButtonStopOn; // STOP 버튼

    [SerializeField] int SelectedIndex = 0;
    [SerializeField] BossDataRow SelectedBossData;
    [SerializeField] float nextAlpha  = 0f;
    [SerializeField] float incrementalValue;

    public void OnView() {

        Debug.Log("ViewBossCall OnView");

        InitMovingObject();

        // Common 2개, Rare 1개, Unique 1개, Legendary 1개 뽑는다
        PickNominee();

        MixBoss();

        StartCoroutine(Rolling());
    }

    /// <summary>
    /// 시작 전 초기화 
    /// </summary>
    void InitMovingObject() {
        ButtonStopOn.SetActive(false);
        // LightOn.gameObject.SetActive(false);
        LightOn.DOKill();
        LightOn.color = new Color(1, 1, 1, 0);

        MommyHead.DOKill();
        MommyHead.localEulerAngles = new Vector3(0, 0, -10);
        MommyHead.transform.DOLocalRotate(new Vector3(0, 0, 10), 1.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            
        

        TextBossName.text = string.Empty;
        GradeTag.gameObject.SetActive(false);



    }


    /// <summary>
    /// 보스 굴리기!
    /// </summary>
    /// <returns></returns>
    IEnumerator Rolling() {
        int rollIndex = 0;
        int order = 0;
        int moveCount = 0;
        float speed = 1.4f;


        isStopped = false;

        while (!isStopped) {
            MoveNominee(rollIndex, ListRandomOrder[order], speed);

            moveCount++;
            rollIndex++;
            order++;

            if (order >= ListRandomOrder.Count)
                order = 0;

            if (rollIndex >= ListNomineeSprite.Count)
                rollIndex = 0;

            speed -= 0.15f;
            if (speed < 0.2f)
                speed = 0.2f;

            if(moveCount > 12) {
                SetReadyStop();
            }

            yield return new WaitForSeconds(speed * 0.5f);
        }

        
    }


    


    /// <summary>
    /// Stop 버튼 누르기 
    /// </summary>
    public void OnClickStop() {

        isStopped = true;
        ButtonStopOn.SetActive(false);

        // 멈추기!
        for(int i=0; i<ListNomineeSprite.Count;i++) {
            ListNomineeSprite[i].transform.DOKill(); // 다 멈추고. 
        }

        ListNomineeSprite[SelectedIndex].transform.DOLocalMoveX(0, 1f).SetDelay(0.5f).OnComplete(OnCompletedStop);

        if (SelectedIndex - 1 < 0)
            ListNomineeSprite[ListNomineeSprite.Count - 1].transform.DOLocalMoveX(-500, 0.5f).SetDelay(0.5f);
        else
            ListNomineeSprite[SelectedIndex - 1].transform.DOLocalMoveX(-500, 0.5f).SetDelay(0.5f);
    }

    void OnCompletedStop() {
        ViewBossCallResult.row = SelectedBossData;
        Doozy.Engine.GameEventMessage.SendEvent("BossCallResultEvent");
    }


    /// <summary>
    /// 버튼 및 불 활성화
    /// </summary>
    void SetReadyStop() {
        ButtonStopOn.SetActive(true);
        // LightOn.DOColor(new Color(1, 1, 1, 1), 1).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        // LightOn.DOFade(1, 1).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);

        StartCoroutine(Lighting());
    }

    IEnumerator Lighting() {
        Color c = LightOn.color;

        incrementalValue = 0.01f;

        nextAlpha = 0;
        c.a = nextAlpha;
        LightOn.color = c;

        while(!isStopped) {

            if(nextAlpha > 1) {
                nextAlpha = 1;
                c.a = nextAlpha;
                incrementalValue = -1 * incrementalValue;
                Debug.Log("Lighting Backward!");
            }
            else if(nextAlpha < 0) {
                nextAlpha = 0;
                c.a = nextAlpha;
                incrementalValue = -1 * incrementalValue;
                Debug.Log("Lighting Forward!");
            }

            nextAlpha += incrementalValue;
            c.a = nextAlpha;


            
            LightOn.color = c;
            yield return null;


        }

        LightOn.color = new Color(1, 1, 1, 1);
    }


    /// <summary>
    /// 하나씩 좌로 움직이기.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="row"></param>
    /// <param name="speed"></param>
    void MoveNominee(int i, BossDataRow row, float speed) {

        if (isStopped)
            return;

        ListNomineeSprite[i].sprite = Stock.GetBossUI_Sprite(row._spriteUI);
        ListNomineeSprite[i].transform.localPosition = new Vector2(500, row._beltPosY); // 포지션 (Y값만 달라진다)
        ListNomineeSprite[i].transform.DOLocalMoveX(-500, speed).SetEase(Ease.Linear);

        // Stop 버튼을 눌러야 하기 때문에, 누르는 순간의 보스 정보를 정해놓는다.
        SelectedIndex = i;
        SelectedBossData = row;
    }

    void OnMoveComplete(Transform t) {
        
    }



    void MixBoss() {
        int index;
        ListRandomOrder.Clear();
        ListTemp.Clear();

        // 일단 5개를 다 집어넣고
        for(int i=0; i<ListBoss.Count;i++) {
            ListTemp.Add(ListBoss[i]);
        }

        for(int i=0; i<ListBoss.Count;i++) {
            index = Random.Range(0, ListTemp.Count);
            ListRandomOrder.Add(ListTemp[index]);
            ListTemp.RemoveAt(index);
        } // 섞기 끝. 
    }



    /// <summary>
    /// 5개의 보스 후보군 선택 
    /// </summary>
    void PickNominee() {

        int index;

        ListBoss.Clear();
        AddBossDataByGradeToTemp("Common");

        // Common에서 2개 뽑는다 
        index = Random.Range(0, ListTemp.Count);
        ListBoss.Add(ListTemp[index]);
        ListTemp.RemoveAt(index);

        index = Random.Range(0, ListTemp.Count);
        ListBoss.Add(ListTemp[index]);
        ListTemp.RemoveAt(index);


        // Rare에서 1개.
        AddBossDataByGradeToTemp("Rare");
        index = Random.Range(0, ListTemp.Count);
        ListBoss.Add(ListTemp[index]);
        ListTemp.RemoveAt(index);

        // Unique 1개
        AddBossDataByGradeToTemp("Unique");
        index = Random.Range(0, ListTemp.Count);
        ListBoss.Add(ListTemp[index]);
        ListTemp.RemoveAt(index);

        // Legendary 1개
        AddBossDataByGradeToTemp("Legendary");
        index = Random.Range(0, ListTemp.Count);
        ListBoss.Add(ListTemp[index]);
        ListTemp.RemoveAt(index);


        // Frame !
        for(int i=0; i<ListBoss.Count;i++) {
            ListFrame[i].sprite = Stock.GetBossFrame_Sprite(ListBoss[i]._FrameSprite);
        }

    }

    void AddBossDataByGradeToTemp(string g) {
        ListTemp.Clear();
        for (int i = 0; i < BossData.Instance.Rows.Count; i++) {
            if (BossData.Instance.Rows[i]._grade == g) {
                ListTemp.Add(BossData.Instance.Rows[i]);
            }
        }
    }

}
