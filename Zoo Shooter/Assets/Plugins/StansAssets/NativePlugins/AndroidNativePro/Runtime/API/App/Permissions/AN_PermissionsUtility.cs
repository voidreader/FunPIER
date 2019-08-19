using System;
using System.Collections.Generic;
using UnityEngine;
using SA.Android.Manifest;
using SA.Android.Utilities;
using SA.Android.Content.Pm;

namespace SA.Android.App
{
    /// <summary>
    /// Collection of permissions based shortcut actions
    /// </summary>
    public static class AN_PermissionsUtility
    {
        /// <summary>
        /// Tries to resolve specified permission.
        /// </summary>
        /// <param name="permission">Android permission.</param>
        /// <param name="callback">Flow callback with resolution result.</param>
        public static void TryToResolvePermission(AMM_ManifestPermission permission, Action<bool> callback) {
            TryToResolvePermission(new[] { permission }, callback);
        }
        
        /// <summary>
        /// Tries to resolve specified permission.
        /// </summary>
        /// <param name="permissions">Android permission array.</param>
        /// <param name="callback">Flow callback with resolution result.</param>
        public static void TryToResolvePermission(AMM_ManifestPermission[] permissions, Action<bool> callback) {

            #if !UNITY_2018_3_OR_NEWER
            //Permissions already been asked on app launch
            if(!AN_Settings.Instance.SkipPermissionsDialog) {
                callback.Invoke(true);
            }
            #endif

            var nonGrantedPermissions = new List<AMM_ManifestPermission>();
            foreach(var permission in permissions) {
                var result = AN_PermissionsManager.CheckSelfPermission(permission);
                if(result != AN_PackageManager.PermissionState.Granted) {
                    nonGrantedPermissions.Add(permission);
                }
            }

            if(nonGrantedPermissions.Count == 0) {
                callback.Invoke(true);
                return;
            }

            AN_PermissionsManager.RequestPermissions(nonGrantedPermissions.ToArray(), (permissionRequestResult) => {

                var granted = true;
                foreach (var grantResults in permissionRequestResult.GrantResults) {
                    if(grantResults.GrantResult != AN_PackageManager.PermissionState.Granted) {
                        AN_Logger.Log("Permission Request Result failed. " + JsonUtility.ToJson(permissions));
                        granted = false;
                        break;
                    }
                }
               
                callback.Invoke(granted);
            });
        }
    }
}