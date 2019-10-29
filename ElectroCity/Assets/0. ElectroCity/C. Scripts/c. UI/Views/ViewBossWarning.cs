using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Google2u;
    

public class ViewBossWarning : MonoBehaviour
{
    public static int warningBossID = 0;

    public Text textBossName;
    public Image AuraEffect;
    public Image ButtonBack;

    public BossDataRow row;


    public void OnView() {
        row = BossData.Instance.Rows[warningBossID - 1];

        ButtonBack.gameObject.SetActive(false);
        textBossName.text = row._displayname;

        AuraEffect.color = new Color(1, 1, 1, 0);
        AuraEffect.DOColor(new Color(1, 1, 1, 1), 2).SetLoops(-1, LoopType.Yoyo);

        Invoke("InvokedView", 1);
    }

    void InvokedView() {
        ButtonBack.gameObject.SetActive(true);
    }


    public void OnExit() {
        // GameManager.main.SetBossMode();
    }
}
