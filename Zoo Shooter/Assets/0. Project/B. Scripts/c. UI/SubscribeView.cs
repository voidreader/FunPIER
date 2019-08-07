using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscribeView : MonoBehaviour
{
    public void PurchaseSubscription() {
        Debug.Log(">> PurchaseSubscription <<");
        IAPControl.main.Purchase("hm_weekly_subs");
    }
}
