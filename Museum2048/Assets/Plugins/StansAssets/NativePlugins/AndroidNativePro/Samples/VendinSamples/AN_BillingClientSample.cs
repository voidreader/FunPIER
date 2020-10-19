using System;
using System.Collections.Generic;
using SA.Android.App;
using SA.Android.Utilities;
using SA.Android.Vending.BillingClient;
using SA.Foundation.Templates;
using UnityEngine;

namespace SA.Android.Samples
{
    public class AN_BillingClientSample : AN_iPurchasesUpdatedListener, AN_iBillingClientStateListener
    {
        bool m_IsConnected = false;
        AN_BillingClient m_BillingClient;

        public event Action OnStoreStateUpdated;

        //--------------------------------------
        // Public Methods
        //--------------------------------------

        public void Connect()
        {
            using (var builder = AN_BillingClient.NewBuilder())
            {
                builder.SetListener(this);
                builder.EnablePendingPurchases();
                builder.SetChildDirected(AN_BillingClient.ChildDirected.Unspecified);
                builder.SetUnderAgeOfConsent(AN_BillingClient.UnderAgeOfConsent.Unspecified);

                m_BillingClient = builder.Build();
                m_BillingClient.StartConnection(this);
            }
        }

        //--------------------------------------
        // Get / Set
        //--------------------------------------

        public bool IsConnected => m_IsConnected;

        public AN_BillingClient Client => m_BillingClient;

        //--------------------------------------
        //  AN_iPurchasesUpdatedListener
        //--------------------------------------

        public void onPurchasesUpdated(SA_iResult billingResult, List<AN_Purchase> purchases)
        {
            if (billingResult.IsSucceeded)
            {
                foreach (var purchase in purchases)
                {
                    if (purchase.PurchaseState == AN_Purchase.State.Purchased)
                    {
                        //Reward user for the item purchase
                    }

                    PrintPurchaseInfo(purchase);
                }

                //We will subscribed on this even in order to update UI accordingly
                OnStoreStateUpdated.Invoke();
            }
            else
            {
                ShowErrorMessage(billingResult.Error);
            }
        }

        //--------------------------------------
        //  AN_iBillingClientStateListener
        //--------------------------------------

        public void OnBillingSetupFinished(SA_iResult billingResult)
        {
            if (billingResult.IsSucceeded)
            {
                m_IsConnected = true;
                Debug.Log("Service Connected");
                return;
            }

            ShowErrorMessage(billingResult.Error);
        }

        public void OnBillingServiceDisconnected()
        {
            m_IsConnected = false;
            Debug.Log("Service Disconnected");
        }

        //--------------------------------------
        // Private Methods
        //--------------------------------------

        public static void ShowErrorMessage(SA_iError error)
        {
            var billingResponseCode = (AN_BillingClient.BillingResponseCode)error.Code;
            var message = new AN_AlertDialog(AN_DialogTheme.Material);
            message.Title = "Error";
            message.Message = error.FullMessage + billingResponseCode;

            message.SetPositiveButton("Okay", () => { });
            message.Show();
        }

        void PrintPurchaseInfo(AN_Purchase purchase)
        {
            Debug.Log("purchase.Sku: " + purchase.Sku);
            Debug.Log("purchase.Type: " + purchase.Type);
            Debug.Log("purchase.PurchaseToken: " + purchase.PurchaseToken);
            Debug.Log("purchase.IsAcknowledged: " + purchase.IsAcknowledged);
            Debug.Log("purchase.IsAutoRenewing: " + purchase.IsAutoRenewing);
            Debug.Log("purchase.Signature: " + purchase.Signature);
            Debug.Log("purchase.OrderId: " + purchase.OrderId);
            Debug.Log("purchase.PackageName: " + purchase.PackageName);
            Debug.Log("purchase.PurchaseState: " + purchase.PurchaseState);
            Debug.Log("purchase.DeveloperPayload: " + purchase.DeveloperPayload);
        }
    }
}
