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

    /// <summary>
    /// 미니언 설정!
    /// </summary>
    /// <param name="id"></param>
    /// <param name="hp"></param>
    public void InitMinion(int id, long hp) {

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

        this.transform.DOMoveY(1.77f, 0.1f);
    }

    void SetProgressorHP() {
        hpvalue = (decimal)HP / (decimal)MaxHP;
        progressorHP.SetValue((float)hpvalue);
    }


    public void SetDamage(long d) {
        HP -= d;
        SetProgressorHP();

        if(HP <= 0) {
            GameManager.main.CurrentEnemy = null;
            BreakUnit();
        }
    }


    /// <summary>
    /// 파괴 처리 
    /// </summary>
    public void BreakUnit() {

        Destroy(this.gameObject);
    }

}
