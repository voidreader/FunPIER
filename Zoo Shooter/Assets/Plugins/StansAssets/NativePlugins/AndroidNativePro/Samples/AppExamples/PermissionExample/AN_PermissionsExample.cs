using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SA.Android.App;
using SA.Android.Manifest;
using SA.Android.Content.Pm;


public class AN_PermissionsExample : MonoBehaviour {

#pragma warning disable 649
    [SerializeField] Button m_checkSelfPermission;
    [SerializeField] Button m_shouldShowRequestPermissionRationale;
    [SerializeField] Button m_requestPermissions;
#pragma warning restore 649

    public void Awake() {
        m_checkSelfPermission.onClick.AddListener(() => {
            var state = AN_PermissionsManager.CheckSelfPermission(AMM_ManifestPermission.READ_EXTERNAL_STORAGE);
            switch (state) {
                case AN_PackageManager.PermissionState.Granted:
                    Debug.Log("READ_CONTACTS Permission Granted");
                    break;
                case AN_PackageManager.PermissionState.Denied:
                    Debug.Log("READ_CONTACTS Permission Denied");
                    break;
            }
        });

        m_shouldShowRequestPermissionRationale.onClick.AddListener(() => {

            bool result = AN_PermissionsManager.ShouldShowRequestPermissionRationale(AMM_ManifestPermission.READ_EXTERNAL_STORAGE);
            Debug.Log("ShouldShowRequestPermissionRationale: " + result.ToString());

        });

        m_requestPermissions.onClick.AddListener(() => {

            List<AMM_ManifestPermission> permissions = new List<AMM_ManifestPermission>();

            permissions.Add(AMM_ManifestPermission.READ_EXTERNAL_STORAGE);
            permissions.Add(AMM_ManifestPermission.WRITE_EXTERNAL_STORAGE);

            AN_PermissionsManager.RequestPermissions(permissions.ToArray(), (result) => {
                foreach(var responce in result.GrantResults) {
                    Debug.Log("RequestPermissions:");
                    Debug.Log(responce.Permission.ToString() + " / " + responce.GrantResult.ToString());
                }
            });

        });

    }


}
