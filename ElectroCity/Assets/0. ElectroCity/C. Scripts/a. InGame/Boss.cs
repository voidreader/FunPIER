using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;

public class Boss : MonoBehaviour
{
    public long HP = 0;
    public BossDataRow row;
    public SpriteRenderer body, leg;
    public BoxCollider2D col;
    public EnemyInfo info;

    /// <summary>
    /// 미니언 설정!
    /// </summary>
    /// <param name="id"></param>
    /// <param name="hp"></param>
    public void InitBoss(int id, long hp) {

        /*
        row = Stock.GetMinionData(id);
        HP = hp;

        body.sprite = Stock.GetMinionBody(row._spriteBody);
        leg.sprite = Stock.GetMinionLeg(row._spriteLeg);
        leg.transform.localPosition = new Vector2(row._legX, row._legY);

        col.offset = new Vector2(row._boxoffsetx, row._boxoffsety);
        col.size = new Vector2(row._boxsizex, row._boxsizey);

        info.IsBoss = false;

        this.transform.DOMoveY(1.77f, 0.1f);
        */
    }

    public void SetDamage(long d) {
        HP -= d;

        if (HP <= 0) {
            GameManager.main.CurrentEnemy = null;
            Destroy(this.gameObject);
        }
    }
}
