using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine;
using DG.Tweening;

public class SubscribeView : MonoBehaviour
{
    public static SubscribeView main = null;

    public Text textTerm, textTrail;
    public GameObject btnPurchase, textBill, textAlreadySpecial;

    private void Awake() {
        main = this;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.X)) {
            CloseView();
        }
    }

    public void OnView() {
        textTrail.text = string.Format("3 DAYS FREE TRIAL,\nTHEN {0} PER WEEK", IAPControl.GetSubscriptionProductPrice());
        textTerm.text = string.Format("Subscription Terms :\n"
            + "Specialist offers a weekly subscription.\n"
            + "You will have 3 - days FREE trial period.\n"
            + "After this period, you will be charged {0}.\n\n"
            + "Upon purchase of this subscription, you will\n"
            + "immediately get: Full Armor, Special Gun\n"
            + "And 'NO ADS'.This is an Auto - renewable\n"
            + "subscription.The payment is charged to your\n"
            + "account after purchase confirmation.\n"
            + "The subscription is renewed unless you turn it off\n"
            + "24 hours before the period ends.", IAPControl.GetSubscriptionProductPrice());


        btnPurchase.SetActive(false);
        textBill.SetActive(false);
        textAlreadySpecial.SetActive(false);


        // 초기화 되지 않은 경우 
        if(!IAPControl.IsInitialized) {
            textBill.SetActive(true);
            return;
        }

        // 구독중인 경우 
        if(IAPControl.main.IsSubscribe) {
            textAlreadySpecial.SetActive(true);
            return;
        }


        btnPurchase.SetActive(true);
        btnPurchase.transform.DOKill();
        btnPurchase.transform.DOScale(1.05f, 0.4f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);



    }

    public void PurchaseSubscription() {
        Debug.Log(">> PurchaseSubscription <<");
        IAPControl.main.Purchase("hm_weekly_subs");
    }


    public void CloseView() {

        Debug.Log("Called CloseView After Purchase <<<<<< ");

        GameEventMessage.SendEvent("CloseEvent");
        UIViewManager.main.ConfirmSpecialistMessage();
    }
    
}
