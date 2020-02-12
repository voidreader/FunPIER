using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
#if AN_FIREBASE_ANALYTICS && (UNITY_IOS || UNITY_ANDROID)
using Fire = Firebase.Analytics;
#endif

namespace SA.Android.Firebase.Analytics
{
    public static class AN_FirebaseAnalytics
    {

        /// <summary>
        /// Logs an app event.
        /// </summary>
        /// <param name="eventName">event name.</param>
        public static void LogEvent(string eventName) {
#if AN_FIREBASE_ANALYTICS && (UNITY_IOS || UNITY_ANDROID)
            Debug.Log("LogEvent sent");
            Fire.FirebaseAnalytics.LogEvent(eventName);
#endif
        }


        /// <summary>
        /// Logs an app event.
        /// </summary>
        /// <param name="eventName">event name.</param>
        /// <param name="data">event data.</param>
        public static void LogEvent(string eventName, IDictionary<string, object> data) {
#if AN_FIREBASE_ANALYTICS && (UNITY_IOS || UNITY_ANDROID)
            Debug.Log("LogEvent with data sent");
            List<Fire.Parameter> parameters = new List<Fire.Parameter>();
            foreach (var pair in data) {
                string key = pair.Key;
                object value = pair.Value;
                Fire.Parameter p = null;

                if (value is double || value is float) {
                    p = new Fire.Parameter(key, System.Convert.ToDouble(value));
                }

                if (value is short || value is int || value is long) {
                    p = new Fire.Parameter(key, System.Convert.ToInt64(value));
                }

                if(p == null) {
                    p = new Fire.Parameter(key, System.Convert.ToString(value));
                }

                parameters.Add(p);
            }
            

            Fire.FirebaseAnalytics.LogEvent(eventName, parameters.ToArray());
#endif
        }

        /// <summary>
        /// Tracking Monetization (optional).
        /// </summary>
        /// <param name="productId">The id of the purchased item.</param>
        /// <param name="amount">The price of the item.</param>
        /// <param name="currency">
        ///  Abbreviation of the currency used for the transaction. For example “USD” (United
        ///  States Dollars). See http:en.wikipedia.orgwikiISO_4217 for a standardized list
        ///  of currency abbreviations.
        /// </param>
        public static void Transaction(string productId, float amount, string currency) {
#if AN_FIREBASE_ANALYTICS && (UNITY_IOS || UNITY_ANDROID)

            Debug.Log("Monetization with data sent");
            List<Fire.Parameter> parameters = new List<Fire.Parameter>();
            parameters.Add(new Fire.Parameter(Fire.FirebaseAnalytics.ParameterCurrency, currency));
            parameters.Add(new Fire.Parameter(Fire.FirebaseAnalytics.ParameterItemId, productId));
            parameters.Add(new Fire.Parameter(Fire.FirebaseAnalytics.ParameterPrice, amount));
            parameters.Add(new Fire.Parameter(Fire.FirebaseAnalytics.ParameterQuantity, 1));

            Fire.FirebaseAnalytics.LogEvent(Fire.FirebaseAnalytics.EventEcommercePurchase, parameters.ToArray());
#endif
        }

        /// <summary>
        /// Sets the duration of inactivity that terminates the current session. 
        /// The default value is (30 minutes).
        /// </summary>
        /// <param name="timeSpan">time span</param>
        public static void SetSessionTimeoutDuration(TimeSpan timeSpan) {
#if AN_FIREBASE_ANALYTICS && (UNITY_IOS || UNITY_ANDROID)
            Fire.FirebaseAnalytics.SetSessionTimeoutDuration(timeSpan);
#endif
        }

        /// <summary>
        /// Sets the user ID property.
        /// </summary>
        /// <param name="userId">user id.</param>
        public static void SetUserId(string userId) {
#if AN_FIREBASE_ANALYTICS && (UNITY_IOS || UNITY_ANDROID)
            Fire.FirebaseAnalytics.SetUserId(userId);
#endif
        }

        /// <summary>
        /// Sets a user property to a given value.
        /// </summary>
        /// <param name="name">property name.</param>
        /// <param name="property">property value.</param>
        public static void SetUserProperty(string name, string property) {
#if AN_FIREBASE_ANALYTICS && (UNITY_IOS || UNITY_ANDROID)
            Fire.FirebaseAnalytics.SetUserProperty(name, property);
#endif
        }

    }
}