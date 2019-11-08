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

    public void OnView() {

        Debug.Log("ViewBossCall OnView");

        // Common 2개, Rare 1개, Unique 1개, Legendary 1개 뽑는다
        PickNominee();

        MixBoss();

        StartCoroutine(Rolling());
    }


    IEnumerator Rolling() {
        int rollIndex = 0;
        int order = 0;
        float speed = 1.4f;

        while(true) {
            MoveNominee(rollIndex, ListRandomOrder[order], speed);


            rollIndex++;
            order++;

            if (order >= ListRandomOrder.Count)
                order = 0;

            if (rollIndex >= ListNomineeSprite.Count)
                rollIndex = 0;

            speed -= 0.15f;
            if (speed < 0.2f)
                speed = 0.2f;

            yield return new WaitForSeconds(speed * 0.5f);
        }

        
    }

    void MoveNominee(int i, BossDataRow row, float speed) {
        ListNomineeSprite[i].sprite = Stock.GetBossUI_Sprite(row._spriteUI);
        ListNomineeSprite[i].transform.localPosition = new Vector2(500, row._beltPosY); // 포지션 (Y값만 달라진다)
        ListNomineeSprite[i].transform.DOLocalMoveX(-500, speed).SetEase(Ease.Linear);
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
