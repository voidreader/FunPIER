using System;
using UnityEngine;


using SA.Android.App;
using SA.Android.Manifest;
using SA.Android.Utilities;
using SA.Foundation.Async;
using SA.Foundation.Templates;


namespace SA.Android.Contacts
{

    public static class AN_ContactsContract {
        private static string ANDROID_CLASS  = "com.stansassets.core.features.contacts.AN_ContactsContract";

        /// <summary>
        /// Method will retrive all contacts from user phone address book
        /// </summary>
        /// <param name="callback">address book contacts list will be delivered via this callback</param>
        /// <param name="checkForPermission">
        /// Will automatically check for READ_CONTACTS permissions. 
        /// Please note That SupportV4 lib is required to prefrom the permission check
        /// </param>
        public static void Retrieve(Action<AN_ContactsResult> callback, bool checkForPermission = true) {

            if(Application.isEditor) {
                SA_Coroutine.WaitForSeconds(1, () => {
                    var result = new AN_ContactsResult();
                    callback.Invoke(result);
                });
                return;
            }

            AN_PermissionsUtility.TryToResolvePermission(AMM_ManifestPermission.READ_CONTACTS, (granted) => {
                if(granted) {
                    RetrieveNative(callback);
                } else {
                    var error = new SA_Error(1, "READ_CONTACTS permission is missing");
                    var contactsResult = new AN_ContactsResult(error);
                    callback.Invoke(contactsResult);
                }
            });

        }

        private static void RetrieveNative(Action<AN_ContactsResult> callback) {
            AN_Java.Bridge.CallStaticWithCallback(ANDROID_CLASS, "Retrieve", callback);
        }
    }
}