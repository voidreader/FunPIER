using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
    

public class BossDamageText : MonoBehaviour
{

    public Camera mainCamera;

    public Transform target;
    public Text lblDamage;

    public static float IncrementalV = 0;


    /// <summary>
    /// 데미지 표시기
    /// </summary>
    /// <param name="t"></param>
    /// <param name="damage"></param>
    /// <param name="isDouble"></param>
    public void SetDamage(Transform t, int damage, bool isDouble = false) {
        target = t;
        Vector3 pos = mainCamera.WorldToScreenPoint(target.position);
        // pos = new Vector3(pos.x + Random.Range(-20f, 20f), pos.y + Random.Range(-20f, 20f), 0);
        this.transform.position = pos;
        //this.transform.position = new Vector3(this.transform.position.x + Random.Range(-15f, 15f), this.transform.position.y + Random.Range(-15f, 15f), 0);

        if(GameManager.main.enemy.isLeft)
            this.transform.position = new Vector3(this.transform.position.x + Random.Range(5f,15f), this.transform.position.y + IncrementalV, 0);
        else
            this.transform.position = new Vector3(this.transform.position.x - Random.Range(5f, 15f), this.transform.position.y + IncrementalV, 0);

        // 위치 좌표 추가
        IncrementalV += 10f;




        if (isDouble) {

            lblDamage.text = "-" + (damage*2).ToString();
            lblDamage.fontSize = 50;
            lblDamage.color = Stocks.main.ColorHeadshotFont;
            
        }
        else {
            lblDamage.text = "-" + damage.ToString();
            lblDamage.fontSize = 40;
            lblDamage.color = Color.white;
        }

        this.gameObject.SetActive(true);
        this.transform.DOKill();
        this.transform.localScale = new Vector3(0.6f, 0.6f, 1);
        // this.transform.localScale = Vector3.one;
        // lblDamage.CrossFadeAlpha(0, 1f, true);
        lblDamage.DOColor(Stocks.main.ColorTransparent, 1.5f).SetEase(Ease.InQuad);
        this.transform.DOScale(1.2f, 0.8f).SetLoops(2, LoopType.Yoyo).OnComplete(OnComplteteScale1);

    }

    void OnComplteteScale1() {
        this.gameObject.SetActive(false);
    }



}
