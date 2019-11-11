using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Google2u;

public class ViewBossCallResult : MonoBehaviour
{
    public static BossDataRow row;
    public static long getCoin;

    public string grade;

    public string getCoinString;

    public Image SpriteBG, SpriteBoss, SpritePanel, SpriteTag;
    public Text TextName, TextGrade, TextCoin;

    public GameObject ButtonBack;

    public void OnShowStart() {
        InitView();
    }

    public void OnShowFinish() {
        StartCoroutine(Waiting());
    }

    void InitView() {
        TextName.text = string.Empty;
        TextGrade.text = string.Empty;
        TextCoin.text = string.Empty;

        SpriteBoss.sprite = Stock.GetBossUI_Sprite(row._spriteUI);
        TextName.text = row._displayname;

        if(GameManager.main) {
            getCoin = GameManager.main.EarningCoin;
            getCoinString = PIER.GetBigNumber(getCoin);
        }
        else {
            getCoin = 0;
            getCoinString = "879.123K";
        }

        SetGradeInfo();
        ButtonBack.SetActive(false);


    }


    /// <summary>
    /// 등급에 따른 배경및 기타 정보 
    /// </summary>
    void SetGradeInfo() {
        grade = row._grade;

        switch(grade) {
            case "Common":
                SpriteBG.sprite = Stock.main.ListBossCallBGSprite[0];
                SpritePanel.sprite = Stock.main.ListBossCallPanelSprite[0];
                SpriteTag.sprite = Stock.main.ListBossCallNameTagSprite[0];
                TextGrade.text = grade;
                break;

            case "Rare":
                SpriteBG.sprite = Stock.main.ListBossCallBGSprite[1];
                SpritePanel.sprite = Stock.main.ListBossCallPanelSprite[1];
                SpriteTag.sprite = Stock.main.ListBossCallNameTagSprite[1];
                TextGrade.text = grade;
                break;

            case "Unique":
                SpriteBG.sprite = Stock.main.ListBossCallBGSprite[2];
                SpritePanel.sprite = Stock.main.ListBossCallPanelSprite[2];
                SpriteTag.sprite = Stock.main.ListBossCallNameTagSprite[2];
                TextGrade.text = grade;
                break;

            case "Legendary":
                SpriteBG.sprite = Stock.main.ListBossCallBGSprite[3];
                SpritePanel.sprite = Stock.main.ListBossCallPanelSprite[3];
                SpriteTag.sprite = Stock.main.ListBossCallNameTagSprite[3];
                TextGrade.text = grade;
                break;
        }
    }

    IEnumerator Waiting() {
        yield return new WaitForSeconds(2);
        ButtonBack.SetActive(true);
    }

    public void OnClose() {
        /*
        if(GameManager.main) {
            GameManager.main.CallFieldBoss();
        }
        */
    }
}
