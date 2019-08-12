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
        this.transform.position = new Vector3(this.transform.position.x + Random.Range(-10f, 10f), this.transform.position.y + Random.Range(-10f, 10f), 0);
        lblDamage.color = Color.white;
        



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
        this.transform.localScale = Vector3.one;
        // lblDamage.CrossFadeAlpha(0, 1f, true);
        this.transform.DOScale(1.1f, 0.4f).SetLoops(2, LoopType.Yoyo).OnComplete(OnComplteteScale1);

    }

    void OnComplteteScale1() {
        this.gameObject.SetActive(false);
    }



}
