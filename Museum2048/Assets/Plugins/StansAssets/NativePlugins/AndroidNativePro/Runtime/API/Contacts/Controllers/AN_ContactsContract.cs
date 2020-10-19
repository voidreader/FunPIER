using System;
using UnityEngine;
using SA.Android.App;
using SA.Android.Manifest;
using SA.Android.Utilities;
using SA.Foundation.Templates;
using StansAssets.Foundation.Async;

namespace SA.Android.Contacts
{
    /// <summary>
    /// The contract between the contacts provider and applications.
    /// Contains definitions for the supported URIs and columns.
    /// </summary>
    public static class AN_ContactsContract
    {
        static readonly string ANDROID_CLASS = "com.stansassets.core.features.contacts.AN_ContactsContract";

        /// <summary>
        /// Method will retrieve all contacts async from user phone address book.
        /// </summary>
        /// <param name="callback">address book contacts list will be delivered via this callback</param>
        /// <param name="checkForPermission">
        /// Will automatically check for READ_CONTACTS permissions.
        /// Please note That AndroidX lib is required to preform the permission check.
        /// </param>
        public static void RetrieveAllAsync(Action<AN_ContactsResult> callback, bool checkForPermission = true)
        {
            if (HandleEditorBehaviour(callback))
                return;

            if (checkForPermission)
                TryToResolvePermission(callback, () =>
                {
                    RetrieveNativeAsync(0, GetContactsCountNative(), callback);
                });
            else
                RetrieveNativeAsync(0, GetContactsCountNative(), callback);
        }

        /// <summary>
        /// Method will retrieve contacts async from user phone address book.
        /// </summary>
        /// <param name="index">index of offset</param>
        /// <param name="count">count of contacts to load</param>
        /// <param name="callback">address book contacts list will be delivered via this callback</param>
        /// <param name="checkForPermission">
        /// Will automatically check for READ_CONTACTS permissions.
        /// Please note That AndroidX lib is required to preform the permission check.
        /// </param>
        public static void RetrieveAsync(int index, int count, Action<AN_ContactsResult> callback, bool checkForPermission = true)
        {
            if (HandleEditorBehaviour(callback))
                return;

            if (checkForPermission)
                TryToResolvePermission(callback, () =>
                {
                    RetrieveNativeAsync(index, count, callback);
                });
            else
                RetrieveNativeAsync(index, count, callback);
        }

        /// <summary>
        /// Method will retrieve all contacts from user phone address book.
        /// </summary>
        public static AN_ContactsResult RetrieveAll()
        {
            if (Application.isEditor)
            {
                var result = new AN_ContactsResult();
                return result;
            }

            return RetrieveNative(0, GetContactsCountNative());
        }

        /// <summary>
        /// Method will retrieve contacts from user phone address book.
        /// </summary>
        /// <param name="index">index of offset.</param>
        /// <param name="count">count of contacts to load.</param>
        public static AN_ContactsResult Retrieve(int index, int count)
        {
            if (Application.isEditor)
            {
                var result = new AN_ContactsResult();
                return result;
            }

            return RetrieveNative(index, count);
        }

        /// <summary>
        /// Returns amount of records inside user phone book.
        /// </summary>
        /// <returns>Amount of records inside user phone book.</returns>
        public static int GetContactsCount()
        {
            if (Application.isEditor)
                return 0;

            return GetContactsCountNative();
        }

        static bool HandleEditorBehaviour(Action<AN_ContactsResult> callback)
        {
            if (Application.isEditor)
            {
                 CoroutineUtility.WaitForSeconds(1, () =>
                {
                    var result = new AN_ContactsResult();
                    callback.Invoke(result);
                });

                return true;
            }

            return false;
        }

        static void TryToResolvePermission(Action<AN_ContactsResult> callback, Action onPermissionGranted)
        {
            AN_PermissionsUtility.TryToResolvePermission(AMM_ManifestPermission.READ_CONTACTS, granted =>
            {
                if (granted)
                {
                    onPermissionGranted.Invoke();
                }
                else
                {
                    var error = new SA_Error(1, "READ_CONTACTS permission is missing");
                    var contactsResult = new AN_ContactsResult(error);
                    callback.Invoke(contactsResult);
                }
            });
        }

        static void RetrieveNativeAsync(int index, int count, Action<AN_ContactsResult> callback)
        {
            AN_Java.Bridge.CallStaticWithCallback(ANDROID_CLASS, "RetrieveAsync", callback, index, count);
        }

        static AN_ContactsResult RetrieveNative(int index, int count)
        {
            return AN_Java.Bridge.CallStatic<AN_ContactsResult>(ANDROID_CLASS, "Retrieve", index, count);
        }

        static int GetContactsCountNative()
        {
            return AN_Java.Bridge.CallStatic<int>(ANDROID_CLASS, "GetContactsCount");
        }
    }
}
