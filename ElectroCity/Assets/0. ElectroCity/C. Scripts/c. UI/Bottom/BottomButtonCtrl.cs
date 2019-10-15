using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BottomButtonCtrl : MonoBehaviour
{

    public static List<BottomButtonCtrl> ListBotButtons = new List<BottomButtonCtrl>();


    public Image _button;
    public GameObject _text;
    public GameObject _icon;

    private void Start() {
        if (!ListBotButtons.Contains(this))
            ListBotButtons.Add(this);

        SetActive(false);
    }

    public void SetActive(bool flag) {

        // 버튼 활성화시 
        if(flag) {
            _button.sprite = Stock.main.SpriteBottomButtonActive;
            _button.SetNativeSize();

            this.transform.DOLocalMoveY(8, 0.4f);


            // 하나가 활성화 되면 나머지는 다 비활성 처리 
            for(int i=0; i<ListBotButtons.Count;i++) {
                if (ListBotButtons[i] == this)
                    continue;

                ListBotButtons[i].SetActive(false);
            }


            _text.GetComponent<Outline>().enabled = true;
            _text.GetComponent<Text>().color = Color.white;

            _text.transform.DOLocalMoveY(-18, 0.4f);
            _icon.transform.DOLocalMoveY(15, 0.4f);
        }
        else { // 비 활성화
            _button.sprite = Stock.main.SpriteBottomButtonInactive;
            _button.SetNativeSize();

            this.transform.DOLocalMoveY(0, 0.4f);


            _text.GetComponent<Outline>().enabled = false;
            _text.GetComponent<Text>().color = Stock.main.ColorBottomButtonInactiveText;

            _text.transform.DOLocalMoveY(-16, 0.4f);
            _icon.transform.DOLocalMoveY(14, 0.4f);
        }

    }
    
}
