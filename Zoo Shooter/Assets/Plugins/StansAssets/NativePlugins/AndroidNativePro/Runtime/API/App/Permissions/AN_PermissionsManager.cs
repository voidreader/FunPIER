using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Android.Manifest;
using SA.Android.Content.Pm;
using SA.Android.OS;

namespace SA.Android.App {
    
    public static class AN_PermissionsManager 
    {
        private static string ANDROID_CLASS = "com.stansassets.android.app.permissions.AN_PermissionsManager";

        //--------------------------------------
        // Public Methods
        //--------------------------------------

        /// <summary>
        /// Determine whether you have been granted a particular permission.
        /// </summary>
        /// <param name="permission">The name of the permission being checked.</param>
        public static AN_PackageManager.PermissionState CheckSelfPermission(AMM_ManifestPermission permission) 
        {

            if(Application.isEditor || AN_Build.VERSION.SDK_INT < AN_Build.VERSION_CODES.M) {
                return AN_PackageManager.PermissionState.Granted;
            }

            var val =  AN_Java.Bridge.CallStatic<int>(ANDROID_CLASS, "CheckSelfPermission", permission.GetFullName());
            return (AN_PackageManager.PermissionState)val;
        }



        /// <summary>
        /// Gets whether you should show UI with rationale for requesting a permission. 
        /// You should do this only if you do not have the permission and the context 
        /// in which the permission is requested does not clearly communicate to the user what would be the benefit from granting this permission.
        /// 
        /// For example, if you write a camera app, 
        /// requesting the camera permission would be expected by the user and no rationale for why it is requested is needed. 
        /// If however, the app needs location for tagging photos then a non-tech savvy user may wonder how location is related to taking photos. 
        /// In this case you may choose to show UI with rationale of requesting this permission.
        /// </summary>
        /// <param name="permission">A permission your app wants to request.</param>
        public static bool ShouldShowRequestPermissionRationale(AMM_ManifestPermission permission) 
        {
            if (Application.isEditor) 
            {
                return true;
            }
            
            if(AN_Build.VERSION.SDK_INT < AN_Build.VERSION_CODES.M) 
            {
                return false;
            }

            return AN_Java.Bridge.CallStatic<bool>(ANDROID_CLASS, "ShouldShowRequestPermissionRationale", permission.GetFullName());
        }


        /// <summary>
        /// Requests permissions to be granted to this application. 
        /// These permissions must be requested in your manifest, they should not be granted to your app, 
        /// and they should have protection level #PROTECTION_DANGEROUS dangerous, regardless whether they are declared by the platform or a third-party app.
        /// 
        /// Requests permissions to be granted to this application. 
        /// These permissions must be requested in your manifest, they should not be granted to your app, 
        /// and they should have protection level #PROTECTION_DANGEROUS dangerous, 
        /// regardless whether they are declared by the platform or a third-party app.
        /// 
        /// If your app does not have the requested permissions the user will be presented with UI for accepting them. 
        /// After the user has accepted or rejected the requested permissions you will receive a callback reporting whether the permissions were granted or not. 
        ///
        /// Note that requesting a permission does not guarantee it will be granted and your app should be able to run without having this permission.
        /// </summary>
        /// <param name="permission">The requested permission. Must me non-null and not empty.</param>
        /// <param name="callback">Results of permission requests will be delivered vai this callback </param>
        public static void RequestPermission(AMM_ManifestPermission permission, Action<AN_PermissionsRequestResult> callback) 
        {
            RequestPermissions(new[] { permission }, callback);
        }


        /// <summary>
        /// Requests permissions to be granted to this application. 
        /// These permissions must be requested in your manifest, they should not be granted to your app, 
        /// and they should have protection level #PROTECTION_DANGEROUS dangerous, regardless whether they are declared by the platform or a third-party app.
        /// 
        /// Requests permissions to be granted to this application. 
        /// These permissions must be requested in your manifest, they should not be granted to your app, 
        /// and they should have protection level #PROTECTION_DANGEROUS dangerous, 
        /// regardless whether they are declared by the platform or a third-party app.
        /// 
        /// If your app does not have the requested permissions the user will be presented with UI for accepting them. 
        /// After the user has accepted or rejected the requested permissions you will receive a callback reporting whether the permissions were granted or not. 
        ///
        /// Note that requesting a permission does not guarantee it will be granted and your app should be able to run without having this permission.
        /// </summary>
        /// <param name="permissions">The requested permissions. Must me non-null and not empty.</param>
        /// <param name="callback">Results of permission requests will be delivered vai this callback </param>
        public static void RequestPermissions(AMM_ManifestPermission[] permissions, Action<AN_PermissionsRequestResult> callback) 
        {
            if (Application.isEditor) 
            {
                var result = new AN_PermissionsRequestResult();
                foreach (AMM_ManifestPermission perm in permissions) 
                {
                    var response = new AN_PermissionsRequestResponce(perm, AN_PackageManager.PermissionState.Granted);
                    result.AddResponce(response);
                }

                callback.Invoke(result);
                return;
            }

            var request = new AN_PermissionsRequest(permissions);
            AN_Java.Bridge.CallStaticWithCallback(ANDROID_CLASS,"RequestPermissions", callback, JsonUtility.ToJson(request));
        }

        //--------------------------------------
        // Private Inner Classes
        //--------------------------------------

        [Serializable]
        private class AN_PermissionsRequest
        {
            [SerializeField] List<string> m_permissions;

            public AN_PermissionsRequest(IEnumerable<AMM_ManifestPermission> permissions) 
            {
                m_permissions = new List<string>();
                foreach (var perm in permissions) 
                {
                    m_permissions.Add(perm.GetFullName());
                }
            }
        }


    }
}