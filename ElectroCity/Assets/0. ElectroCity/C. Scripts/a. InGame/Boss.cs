using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;
using DG.Tweening;

public class Boss : MonoBehaviour
{
    public long HP = 0, MaxHP = 0;
    public BossDataRow row;
    public SpriteRenderer body, leg, head;

    public SpriteRenderer secondHead, armLeft, armRight, otherParts; // 기타 부품들

    public BoxCollider2D col;
    public EnemyInfo info;

    /// <summary>
    /// 미니언 설정!
    /// </summary>
    /// <param name="id"></param>
    /// <param name="hp"></param>
    public void InitBoss(int id, long hp) {

        row = Stock.GetBossData(id);
        HP = hp;
        MaxHP = hp;

        body.sprite = Stock.GetBossBody(row._spriteBody);
        head.sprite = Stock.GetBossHead(row._spriteHead);
        leg.sprite = Stock.GetBossLeg(row._spriteLeg);


        col.offset = new Vector2(row._boxoffsetx, row._boxoffsety);
        col.size = new Vector2(row._boxsizex, row._boxsizey);
        info.IsBoss = true;


        // 기타 부품(팔, 고글, 바퀴.. 등)
        // 두번째 머리
        if(row._spriteHead2 == "0") {
            secondHead.gameObject.SetActive(false);
        } 

        // 팔 
        if(row._spriteArmLeft == "0") {
            armLeft.gameObject.SetActive(false);
            armRight.gameObject.SetActive(false);
        }

        // 기타 부품 
        if(row._spriteOtherParts == "0") {
            otherParts.gameObject.SetActive(false);
        }




        // 자리 위치 
        this.transform.DOMoveY(1.77f, 0.1f);

    }

    public void SetDamage(long d) {
        HP -= d;

        // 보스는 HP가 개체마다 종속되지 않아서 게임매니저한테 전달.
        GameManager.main.SetValueBossHP(HP, MaxHP);

        if (HP <= 0) {
            GameManager.main.CurrentEnemy = null;
            BreakUnit();
        }
    }


    /// <summary>
    /// 파괴 처리 
    /// </summary>
    public void BreakUnit() {

        GameManager.main.ClearBoss();

        Destroy(this.gameObject);
    }
}
