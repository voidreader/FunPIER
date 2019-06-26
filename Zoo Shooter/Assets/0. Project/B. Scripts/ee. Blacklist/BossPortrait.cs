using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;

public class BossPortrait : MonoBehaviour {
    public Sprite _captureSprite;
    public Sprite _uncaptureSprite;

    public Image _spLabel; // 이름 써지는 스프라이트 
    public Text _lblName; // 이름 

    public GameObject _jail; // 감옥 
    public Image _spFace; // 얼굴

    public void OffPortrait() {
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 데이터 세팅 
    /// </summary>
    /// <param name="data"></param>
    public void OnPortrait(BossDataRow data) {

        this.gameObject.SetActive(true);

        _jail.SetActive(false);
        _spLabel.sprite = _uncaptureSprite;

        // 얼굴과 이름 얻기 
        _spFace.sprite = PIER.main.GetBossPortraitSprite(data._portrait);
        _lblName.text = data._name;

        // 잡혔는지 체크 
        if(data._level < PIER.CurrentLevel) {
            // 잡혔다!
            _jail.SetActive(true);
            _spLabel.sprite = _captureSprite;
        }
    }
}
