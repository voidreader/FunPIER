using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine;

public class SubscribeView : MonoBehaviour
{
    public static SubscribeView main = null;

    private void Awake() {
        main = this;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.X)) {
            CloseView();
        }
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
