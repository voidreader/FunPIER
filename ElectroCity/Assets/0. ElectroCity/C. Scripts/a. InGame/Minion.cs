using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Google2u;
using Doozy.Engine.Progress;

public class Minion : MonoBehaviour
{
    public long HP, MaxHP = 0;
    public MinionDataRow row;
    public SpriteRenderer body,leg;
    public BoxCollider2D col;
    public EnemyInfo info;
    public Doozy.Engine.Progress.Progressor progressorHP;
    decimal hpvalue;

    public Animator anim;

    bool isReady = false;

    /// <summary>
    /// 미니언 설정!
    /// </summary>
    /// <param name="id"></param>
    /// <param name="hp"></param>
    public void InitMinion(int id, long hp) {

        Debug.Log(">> InitMinion hp :: " + hp.ToString());

        // anim.SetBool("isAlive", true);
        row = Stock.GetMinionData(id);
        HP = hp;
        MaxHP = hp;
        progressorHP.InstantSetValue(1);

        body.sprite = Stock.GetMinionBody(row._spriteBody);
        leg.sprite = Stock.GetMinionLeg(row._spriteLeg);
        leg.transform.localPosition = new Vector2(row._legX, row._legY);

        col.offset = new Vector2(row._boxoffsetx, row._boxoffsety);
        col.size = new Vector2(row._boxsizex, row._boxsizey);

        info.IsBoss = false;
        isReady = false;

        this.transform.DOMoveY(1.77f, 0.1f).OnComplete(OnReady);
    }

    void OnReady() {
        isReady = true;
    }


    void SetProgressorHP() {

        

        hpvalue = (decimal)HP / (decimal)MaxHP;
        progressorHP.SetValue((float)hpvalue);
    }


    public void SetDamage(long d) {

        if (!isReady)
            return;

        HP -= d;
        

        if(HP <= 0) {
            progressorHP.SetValue(0);
            BreakUnit();
            return;
        }

        SetProgressorHP();
    }


    /// <summary>
    /// 파괴 처리 
    /// </summary>
    public void BreakUnit() {
        

        if (GameManager.main != null) {
            GameManager.main.GetMinionKillCoin();
            GameManager.main.DecreaseKillCount();
            
        }

        anim.SetBool("isAlive", false);
        ReadyToDestroy();

        // Destroy(this.gameObject);
    }


    void ReadyToDestroy() {
        // progressorHP.transform.DOScale(Vector3.zero, 0.2f);
        progressorHP.gameObject.SetActive(false);
        col.enabled = false;

        this.transform.DOLocalMoveX(3, 0.6f);
    }

    public void DestroyMinion() {

        Debug.Log("<< DestroyMinion >>");
        anim.SetBool("isAlive", true);
        GameManager.main.CurrentEnemy = null;

        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

}
