using System;
using SA.Android.Utilities;
using SA.Foundation.Templates;
using UnityEngine;

namespace SA.Android.Vending.BillingClient
{
    /// <inheritdoc />
    ///  <summary>
    ///  Main interface for communication between the library and user application code.
    ///
    ///  It provides convenience methods for in-app billing.
    ///  You can create one instance of this class for your application and use it to process in-app billing operations.
    ///  It provides synchronous (blocking) and asynchronous (non-blocking) methods for many common in-app billing operations.
    ///
    ///  All methods are supposed to be called from the Ui thread
    ///  and all the asynchronous callbacks will be returned on the Ui thread as well.
    ///  </summary>
    public class AN_BillingClient : AN_LinkedObject
    {
        public enum FeatureType
        {
            /// <summary>
            /// Purchase/query for in-app items on VR.
            /// </summary>
            inAppItemsOnVr,

            /// <summary>
            /// Launch a price change confirmation flow.
            /// </summary>
            priceChangeConfirmation,

            /// <summary>
            /// Purchase/query for subscriptions.
            /// </summary>
            subscriptions,

            /// <summary>
            /// Purchase/query for subscriptions on VR.
            /// </summary>
            subscriptionsOnVr,

            /// <summary>
            /// Subscriptions update/replace.
            /// </summary>
            subscriptionsUpdate
        }

        /// <summary>
        /// Supported SKU types.
        /// </summary>
        public enum SkuType
        {
            /// <summary>
            /// A type of SKU for in-app products.
            /// </summary>
            inapp,

            /// <summary>
            /// A type of SKU for subscriptions.
            /// </summary>
            subs
        }

        /// <summary>
        /// Developers are able to specify whether you would like your app to be treated as child-directed for
        /// purposes of the Children’s Online Privacy Protection Act (COPPA) -
        /// <see cref="http://business.ftc.gov/privacy-and-security/childrens-privacy"/>.
        /// </summary>
        public enum ChildDirected
        {
            /// <summary>
            /// App has not specified whether its ad requests should be treated as child directed or not.
            /// </summary>
            Unspecified = 0,

            /// <summary>
            /// App indicates its ad requests should be treated as child-directed.
            /// </summary>
            ChildDirected = 1,

            /// <summary>
            /// App indicates its ad requests should NOT be treated as child-directed.
            /// </summary>
            NotChildDirected = 2
        }

        /// <summary>
        /// Developers are able to specify whether to mark your ad requests to receive treatment
        /// for users in the European Economic Area (EEA) under the age of consent.
        /// </summary>
        public enum UnderAgeOfConsent
        {
            /// <summary>
            /// App has not specified how ad requests shall be handled.
            /// </summary>
            Unspecified = 0,

            /// <summary>
            /// App indicates the ad requests shall be handled in a manner suitable for users under the age
            /// of consent.
            /// </summary>
            UnderAgeOfConsent = 1,

            /// <summary>
            /// App indicates the ad requests shall NOT be handled in a manner suitable for users under the
            /// age of consent.
            /// </summary>
            NotUnderAgeOfConsent = 2
        }

        /// <summary>
        /// Possible response codes.
        /// </summary>
        public enum BillingResponseCode
        {
            /// <summary>
            /// The request has reached the maximum timeout before Google Play responds.
            /// </summary>
            ServiceTimeout = -3,

            /// <summary>
            /// Requested feature is not supported by Play Store on the current device.
            /// </summary>
            FeatureNotSupported = -2,

            /// <summary>
            /// Play Store service is not connected now - potentially transient state.
            ///
            /// E.g. Play Store could have been updated in the background while your app was still running.
            /// So feel free to introduce your retry policy for such use case.
            /// It should lead to a call to <see cref="AN_BillingClient.StartConnection"/>
            /// right after or in some time after you received this code.
            /// </summary>
            ServiceDisconnected = -1,

            /// <summary>
            /// Success
            /// </summary>
            Ok = 0,

            /// <summary>
            /// User pressed back or canceled a dialog
            /// </summary>
            UserCanceled = 1,

            /// <summary>
            /// Network connection is down
            /// </summary>
            ServiceUnavailable = 2,

            /// <summary>
            /// Billing API version is not supported for the type requested
            /// </summary>
            BillingUnavailable = 3,

            /// <summary>
            /// Requested product is not available for purchase
            /// </summary>
            ItemUnavailable = 4,

            /// <summary>
            /// Invalid arguments provided to the API.
            /// This error can also indicate that the application was not correctly signed
            /// or properly set up for In-app Billing in Google Play,
            /// or does not have the necessary permissions in its manifest
            /// </summary>
            DeveloperError = 5,

            /// <summary>
            /// Fatal error during the API action
            /// </summary>
            Error = 6,

            /// <summary>
            /// Failure to purchase since item is already owned
            /// </summary>
            ItemAlreadyOwned = 7,

            /// <summary>
            /// Failure to consume since item is not owned
            /// </summary>
            ItemNotOwned = 8,
        }

        /// <inheritdoc />
        /// <summary>
        /// Builder to configure and create a <see cref="T:SA.Android.Vending.BillingClient.AN_BillingClient" /> instance.
        /// </summary>
        [Serializable]
        public class Builder : AN_LinkedObject
        {
            const string k_NativeBillingClientBuilder = "com.stansassets.billing.AN_BillingClientBuilder";

            public AN_BillingClient Build()
            {
                if (!AN_Settings.Instance.Vending)
                    throw new InvalidOperationException("AN_BillingClient can only be build when Vending service is enabled. " +
                        "Please enable it using the plugin editor settings.");

                if (Application.isEditor)
                    return new AN_BillingClient();

                var json = AN_Java.Bridge.CallStatic<string>(k_NativeBillingClientBuilder,
                    "Build",
                    HashCode);
                return JsonUtility.FromJson<AN_BillingClient>(json);
            }

            /// <summary>
            ///  Enables pending purchase support.
            ///
            /// This method is required to be called to acknowledge your application has been updated to
            /// support purchases that are pending. Pending purchases are not automatically enabled since
            /// your application will require updates to ensure entitlement is not granted before payment has
            /// been secured. For more information on how to handle pending transactions see
            /// <see>
            ///     <cref>https://developer.android.com/google/play/billing/billing_library_overview</cref>
            /// </see>
            ///  If this method is not called, BillingClient instance creation fails.
            /// </summary>
            public Builder EnablePendingPurchases()
            {
                AN_Java.Bridge.CallStatic(k_NativeBillingClientBuilder,
                    "EnablePendingPurchases",
                    HashCode);

                return this;
            }

            /// <summary>
            /// Developers are able to specify whether this app is child directed or not to ensure compliance
            /// with US COPPA & EEA age of consent laws.
            ///
            /// This is most relevant for rewarded skus as child directed applications are explicitly not
            /// allowed to collect information that can be used to personalize the rewarded videos to the user.
            /// </summary>
            /// <param name="childDirected">childDirected</param>
            public Builder SetChildDirected(ChildDirected childDirected)
            {
                AN_Java.Bridge.CallStatic(k_NativeBillingClientBuilder,
                    "SetChildDirected",
                    HashCode,
                    (int)childDirected);

                return this;
            }

            /// <summary>
            /// Developers are able to specify whether this app is under age of consent or not to ensure compliance
            /// with US COPPA & EEA age of consent laws.
            /// </summary>
            /// <param name="underAgeOfConsent">underAgeOfConsent</param>
            public Builder SetUnderAgeOfConsent(UnderAgeOfConsent underAgeOfConsent)
            {
                AN_Java.Bridge.CallStatic(k_NativeBillingClientBuilder,
                    "SetUnderAgeOfConsent",
                    HashCode,
                    (int)underAgeOfConsent);

                return this;
            }

            /// <summary>
            /// Specify a valid listener for onPurchasesUpdated event.
            /// </summary>
            /// <param name="listener">Your listener for app initiated and Play Store initiated purchases.</param>
            public Builder SetListener(AN_iPurchasesUpdatedListener listener)
            {
                AN_Java.Bridge.CallStaticWithCallback<AN_PurchasesUpdatedResult>(
                    k_NativeBillingClientBuilder,
                    "SetListener",
                    result =>
                    {
                        listener.onPurchasesUpdated(result, result.Purchases);
                    },
                    HashCode);

                return this;
            }
        }

        const string k_NativeBillingClient = "com.stansassets.billing.AN_BillingClient";

        /// <summary>
        /// Constructs a new <see cref="Builder"/> instance.
        /// </summary>
        /// <returns>Returns a new <see cref="Builder"/> instance.</returns>
        public static Builder NewBuilder()
        {
            if (Application.isEditor) return new Builder();

            var json = AN_Java.Bridge.CallStatic<string>(k_NativeBillingClient, "NewBuilder");
            return JsonUtility.FromJson<Builder>(json);
        }

        /// <summary>
        /// Starts up BillingClient setup process asynchronously.
        /// You will be notified through the <see cref="AN_iBillingClientStateListener"/>
        /// listener when the setup process is complete.
        /// </summary>
        /// <param name="listener">The listener to notify when the setup process is complete.</param>
        public void StartConnection(AN_iBillingClientStateListener listener)
        {
            var javaRequestBuilder = new AN_JavaRequestBuilder(k_NativeBillingClient, "StartConnection");
            javaRequestBuilder.AddArgument(HashCode);
            javaRequestBuilder.AddCallback<SA_Result>(listener.OnBillingSetupFinished);

            javaRequestBuilder.AddCallback<SA_Result>(result =>
            {
                listener.OnBillingServiceDisconnected();
            });

            javaRequestBuilder.Invoke();
        }

        /// <summary>
        /// Initiate the billing flow for an in-app purchase or subscription.
        /// The result will be delivered via the <see cref="AN_iPurchasesUpdatedListener"/> interface.
        /// </summary>
        /// <param name="params"></param>
        public void LaunchBillingFlow(AN_BillingFlowParams @params)
        {
            AN_Java.Bridge.CallStatic(k_NativeBillingClient,
                "LaunchBillingFlow",
                HashCode,
                @params);
        }

        /// <summary>
        /// Consumes a given in-app product.
        /// Consuming can only be done on an item that's owned, and as a result of consumption, the user will no longer own it.
        ///
        /// Consumption is done asynchronously and the listener receives the callback specified upon completion.
        ///
        /// Warning! All purchases require acknowledgement.
        /// Failure to acknowledge a purchase will result in that purchase being refunded.
        /// For one-time products ensure you are using this method which acts as an implicit acknowledgement
        /// or you can explicitly acknowledge the purchase via <see cref="AcknowledgePurchase(AN_AcknowledgePurchaseParams, AN_iAcknowledgePurchaseResponseListener)"/>.
        /// For subscriptions use <see cref="AcknowledgePurchase(AN_AcknowledgePurchaseParams, AN_iAcknowledgePurchaseResponseListener)"/>.
        ///
        /// Please refer to https://developer.android.com/google/play/billing/billing_library_overview#acknowledge for more details.
        /// </summary>
        /// <param name="consumeParams">Params specific to consume purchase.</param>
        /// <param name="listener">
        /// Implement it to get the result of your consume operation returned asynchronously through the callback with
        /// token and BillingClient.BillingResponseCode parameters.
        /// </param>
        public void ConsumeAsync(AN_ConsumeParams consumeParams, AN_iConsumeResponseListener listener)
        {
            AN_Java.Bridge.CallStaticWithCallback<AN_OnConsumeResponseResult>(
                k_NativeBillingClient,
                "ConsumeAsync",
                result =>
                {
                    listener.OnConsumeResponse(result, result.PurchaseToken);
                },
                HashCode,
                consumeParams);
        }

        /// <summary>
        /// Acknowledge in-app purchases.
        /// Developers are required to acknowledge that they have granted entitlement for all in-app purchases for their application.
        ///
        /// Warning! All purchases require acknowledgement.
        /// Failure to acknowledge a purchase will result in that purchase being refunded.
        /// For one-time products ensure you are using <see cref="ConsumeAsync"/> which acts as an implicit acknowledgement
        /// or you can explicitly acknowledge the purchase via this method.
        /// For subscriptions use <see cref="AcknowledgePurchase(AN_AcknowledgePurchaseParams, AN_iAcknowledgePurchaseResponseListener)"/>.
        /// Please refer to https://developer.android.com/google/play/billing/billing_library_overview#acknowledge for more details.
        /// </summary>
        /// <param name="params">Params specific to this acknowledge purchase request.</param>
        /// <param name="listener">Implement it to get the result of the acknowledge operation returned asynchronously through the callback.</param>
        public void AcknowledgePurchase(AN_AcknowledgePurchaseParams @params, AN_iAcknowledgePurchaseResponseListener listener)
        {
            AcknowledgePurchase(@params, listener.onAcknowledgePurchaseResponse);
        }

        /// <summary>
        /// Acknowledge in-app purchases.
        /// Developers are required to acknowledge that they have granted entitlement for all in-app purchases for their application.
        ///
        /// Warning! All purchases require acknowledgement.
        /// Failure to acknowledge a purchase will result in that purchase being refunded.
        /// For one-time products ensure you are using <see cref="ConsumeAsync"/> which acts as an implicit acknowledgement
        /// or you can explicitly acknowledge the purchase via this method.
        /// For subscriptions use <see cref="AcknowledgePurchase(AN_AcknowledgePurchaseParams, AN_iAcknowledgePurchaseResponseListener)"/>.
        /// Please refer to https://developer.android.com/google/play/billing/billing_library_overview#acknowledge for more details.
        /// </summary>
        /// <param name="params">Params specific to this acknowledge purchase request.</param>
        /// <param name="callback">Result of the acknowledge operation returned asynchronously through the callback.</param>
        public void AcknowledgePurchase(AN_AcknowledgePurchaseParams @params, Action<SA_iResult> callback)
        {
            AN_Java.Bridge.CallStaticWithCallback<SA_Result>(
                k_NativeBillingClient,
                "AcknowledgePurchase",
                callback.Invoke,
                HashCode,
                @params);
        }

        /// <summary>
        /// Perform a network query to get SKU details and return the result asynchronously.
        /// </summary>
        /// <param name="params">Params specific to this query request <see cref="AN_SkuDetailsParams"/>.</param>
        /// <param name="listener">
        /// Implement it to get the result of your query operation returned asynchronously through the callback
        /// with the <see cref="BillingResponseCode"/> and the list of SkuDetails.
        /// </param>
        public void QuerySkuDetailsAsync(AN_SkuDetailsParams @params, AN_iSkuDetailsResponseListener listener)
        {
            AN_Java.Bridge.CallStaticWithCallback<AN_SkuDetailsResult>(
                k_NativeBillingClient,
                "QuerySkuDetailsAsync",
                result =>
                {
                    if (result.IsSucceeded)
                        foreach (var nativeDetails in result.SkuDetailsList)
                        {
                            var localSkuDetails = GetSkuDetails(nativeDetails.Sku);
                            if (localSkuDetails != null)
                                OverrideLocalSkuWithNativeData(localSkuDetails, nativeDetails);
                            else
                                AN_Settings.Instance.InAppProducts.Add(nativeDetails);
                        }

                    listener.OnSkuDetailsResponse(result, result.SkuDetailsList);
                },
                HashCode,
                @params);
        }

        internal static void OverrideLocalSkuWithNativeData(AN_SkuDetails local, AN_SkuDetails native)
        {
            //Save custom data
            var icon = local.Icon;
            var isConsumable = local.IsConsumable;

            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(native), local);

            //Restore custom data
            local.Icon = icon;
            local.IsConsumable = isConsumable;
        }

        /// <summary>
        /// Loads a rewarded sku in the background and returns the result asynchronously.
        ///
        /// If the rewarded sku is available, the response will be BILLING_RESULT_OK.
        /// Otherwise the response will be ITEM_UNAVAILABLE.
        /// There is no guarantee that a rewarded sku will always be available.
        /// After a successful response, only then should the offer be given to a user
        /// to obtain a rewarded item and call launchBillingFlow.
        /// </summary>
        /// <param name="params">params specific to this load request <see cref="AN_RewardLoadParams"/></param>
        /// <param name="listener"> Implement it to get the result of the load operation returned asynchronously through the callback.</param>
        public void LoadRewardedSku(AN_RewardLoadParams @params, AN_iRewardResponseListener listener)
        {
            AN_Java.Bridge.CallStaticWithCallback<SA_Result>(
                k_NativeBillingClient,
                "LoadRewardedSku",
                listener.OnSkuDetailsResponse,
                HashCode,
                @params);
        }

        /// <summary>
        /// Get purchases details for all the items bought within your app.
        /// This method uses a cache of Google Play Store app without initiating a network request.
        ///
        /// Note: It's recommended for security purposes to go through purchases verification on your backend
        /// (if you have one) by calling one of the following APIs:
        /// <see>
        ///     <cref>https://developers.google.com/android-publisher/api-ref/purchases/products/get</cref>
        /// </see>
        /// <see>
        ///     <cref>https://developers.google.com/android-publisher/api-ref/purchases/subscriptions/get</cref>
        /// </see>
        /// </summary>
        /// <param name="skuType">The type of SKU, either "inapp" or "subs" as in <see cref="SkuType"/> </param>
        /// <returns>The <see cref="AN_Purchase.PurchasesResult"/> containing the list of purchases and the response code.</returns>
        public AN_Purchase.PurchasesResult QueryPurchases(SkuType skuType)
        {
            var resultJSON = AN_Java.Bridge.CallStatic<string>(
                k_NativeBillingClient,
                "QueryPurchases",
                HashCode,
                skuType.ToString());

            var result = JsonUtility.FromJson<AN_Purchase.PurchasesResult>(resultJSON);
            return result;
        }

        /// <summary>
        /// Returns the most recent purchase made by the user for each SKU, even if that purchase is expired, canceled, or consumed.
        /// </summary>
        /// <param name="skuType">The type of SKU, either "inapp" or "subs" as in <see cref="SkuType"/>.</param>
        /// <param name="listener">Implement it to get the result of your query operation returned asynchronously through the callback.</param>
        public void QueryPurchaseHistoryAsync(SkuType skuType, AN_iPurchaseHistoryResponseListener listener)
        {
            AN_Java.Bridge.CallStaticWithCallback<AN_OnPurchaseHistoryResponseResult>(
                k_NativeBillingClient,
                "QueryPurchaseHistoryAsync",
                result =>
                {
                    listener.OnConsumeResponse(result, result.PurchaseHistoryRecordList);
                },
                HashCode,
                skuType.ToString());
        }

        /// <summary>
        /// Close the connection and release all held resources such as service connections.
        /// Call this method once you are done with this BillingClient reference.
        /// </summary>
        public void EndConnection()
        {
            AN_Java.Bridge.CallStatic(k_NativeBillingClient, "EndConnection", HashCode);
            Dispose();
        }

        /// <summary>
        /// Checks if the client is currently connected to the service, so that requests to other methods will succeed.
        ///
        /// Note: It also means that In App items are supported for purchasing,
        /// queries and all other actions.
        /// If you need to check support for SUBSCRIPTIONS or something different,
        /// use <see cref="IsFeatureSupported"/> method.
        /// </summary>
        /// <returns>Returns true if the client is currently connected to the service, false otherwise.</returns>
        public bool IsReady()
        {
            return AN_Java.Bridge.CallStatic<bool>(k_NativeBillingClient, "IsReady", HashCode);
        }

        /// <summary>
        /// Check if specified feature or capability is supported by the Play Store.
        /// </summary>
        /// <param name="feature">One of <see cref="FeatureType"/> constants.</param>
        /// <returns><see cref="BillingResponseCode.Ok"/> if feature is supported and corresponding error code otherwise.</returns>
        public BillingResponseCode IsFeatureSupported(FeatureType feature)
        {
            var result = AN_Java.Bridge.CallStatic<int>(
                k_NativeBillingClient,
                "IsReady",
                HashCode,
                feature.ToString());

            return (BillingResponseCode)result;
        }

        /// <summary>
        /// Retries saved product details by id.
        /// </summary>
        /// <param name="sku">Product sku.</param>
        /// <returns>Returns saved product sku or null if products wasn't found.</returns>
        internal static AN_SkuDetails GetSkuDetails(string sku)
        {
            foreach (var product in AN_Settings.Instance.InAppProducts)
                if (product.Sku.Equals(sku))
                    return product;
            return null;
        }
    }
}
