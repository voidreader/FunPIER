using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class SimpleMessageBox : MonoBehaviour
{
    public static SimpleMessageBox _main;
    public static bool isShowing = false;
    public Text textMessage;


    public static SimpleMessageBox main {
        get {
            if (_main == null) {
                _main = Instantiate(Resources.Load<SimpleMessageBox>("Prefabs/SimpleMessageBox"));
            }

            return _main;
        }
    }

    private void Awake() {
        _main = this;
        this.gameObject.SetActive(false);
    }


    public static void SetNoAdsMessage() {
        if (isShowing)
            return;


        main.textMessage.text = "There is no ad inventory. Please try later";
        main.TweenBox();
    }

    public static void SetNetworkMessage() {

        if (isShowing)
            return;


        main.textMessage.text = "You must be connected to the internet";
        main.TweenBox();


    }

    public void TweenBox() {
        isShowing = true;

        this.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        textMessage.color = new Color(1, 1, 1, 0);

        this.gameObject.SetActive(true);
        this.transform.DOKill();
        textMessage.DOKill();


        this.transform.localPosition = new Vector3(0, -80, 0);
        this.transform.DOLocalMoveY(0, 0.5f);
        this.GetComponent<Image>().DOColor(new Color(0, 0, 0, 0.8f), 0.5f).OnComplete(TweenStep2);
        textMessage.DOColor(new Color(1, 1, 1, 1), 0.5f);
    }

    void TweenStep2() {
        this.GetComponent<Image>().DOColor(new Color(0, 0, 0, 0), 0.5f).OnComplete(OnCompleteTween).SetDelay(1.5f);
        textMessage.DOColor(new Color(1, 1, 1, 0), 0.5f).SetDelay(1.5f); 
    }

    void OnCompleteTween() {
        this.gameObject.SetActive(false);
        isShowing = false;
    }

}
