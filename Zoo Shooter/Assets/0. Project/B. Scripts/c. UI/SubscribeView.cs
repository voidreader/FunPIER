using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine;

public class SubscribeView : MonoBehaviour
{
    public static SubscribeView main = null;

    public Text textTerm, textTrail;

    private void Awake() {
        main = this;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.X)) {
            CloseView();
        }
    }

    public void OnView() {
        textTrail.text = string.Format("3 DAYS FREE TRIAL,\nTHEN {0} PER WEEK", IAPControl.main.GetSubscriptionProductPrice());
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
            + "24 hours before the period ends.", IAPControl.main.GetSubscriptionProductPrice());
    }

    public void PurchaseSubscription() {
        Debug.Log(">> PurchaseSubscription <<");
        IAPControl.main.Purchase("hm_weekly_subs");
    }


    public void CloseView() {
        GameEventMessage.SendEvent("CloseEvent");
        UIViewManager.main.ConfirmSpecialistMessage();
    }
    
}
