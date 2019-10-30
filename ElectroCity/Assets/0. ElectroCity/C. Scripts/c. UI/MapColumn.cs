using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Google2u;

public class MapColumn : MonoBehaviour
{
    public Image Frame, BossImage, BrokenImage;
    public GameObject Cover, UnderFrame;

    public int StageNum = 0; // 스테이지 번호 
    StageDataRow row;
    
    public void InitMap(int index) {
        StageNum = index + 1;

        Frame.gameObject.SetActive(true);
        Frame.sprite = Stock.main.SpriteMapNext;

        Cover.SetActive(false);
        BossImage.gameObject.SetActive(false);
        BrokenImage.gameObject.SetActive(false);

        row = StageData.Instance.Rows[index];
    }

    public void SetCurrentMap() {

        Frame.sprite = Stock.main.SpriteMapCurrent;
        BrokenImage.gameObject.SetActive(false);
        Cover.SetActive(false);

        BossImage.gameObject.SetActive(true);
        BossImage.sprite = Stock.GetBossUI_Sprite(BossData.Instance.Rows[row._bid - 1]._spriteUI); // 보스 이미지 처리 
        BossImage.SetNativeSize();
    }

    public void SetClearMap() {
        BossImage.gameObject.SetActive(false);
        BrokenImage.gameObject.SetActive(true); // 브로큰 이미지
        BrokenImage.sprite = Stock.GetBossBrokenSprite(BossData.Instance.Rows[row._bid - 1]._spriteBroken);
        BrokenImage.SetNativeSize();

        Cover.SetActive(true);
        Frame.gameObject.SetActive(false);

    }

}
