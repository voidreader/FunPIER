using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Android.Manifest;
using SA.Android.Content.Pm;

using System;

public class AndroidPermissionCheckerCtrl : MonoBehaviour {

    public static event Action<bool> OnCompleteGrant = delegate { };

    public void Open() {
        this.gameObject.SetActive(true);
        Debug.Log("AndroidPermissionCheckerCtrl Open");
    }

    public void RequestPermission() {

        /*
        Debug.Log("Start Request Permission");

        List<AMM_ManifestPermission> permissions = new List<AMM_ManifestPermission>();
        bool requestResult = true;

        permissions.Add(AMM_ManifestPermission.READ_EXTERNAL_STORAGE);
        permissions.Add(AMM_ManifestPermission.WRITE_EXTERNAL_STORAGE);
        permissions.Add(AMM_ManifestPermission.READ_PHONE_STATE);

        AN_ActivityCompat.RequestPermissions(permissions.ToArray(), (result) => {
            foreach (var responce in result.GrantResults) {
                // Debug.Log("RequestPermissions:");
                Debug.Log(responce.Permission.ToString() + " / " + responce.GrantResult.ToString());
                if(responce.GrantResult == AN_PackageManager.PermissionState.Denied) { // 하나라도 denied 되면 false 로 전달
                    requestResult = false;
                    break;
                }

            }


            OnCompleteGrant(requestResult);
            OnCompleteGrant = delegate { };

            this.gameObject.SetActive(false);
        });
        */
    }
}
