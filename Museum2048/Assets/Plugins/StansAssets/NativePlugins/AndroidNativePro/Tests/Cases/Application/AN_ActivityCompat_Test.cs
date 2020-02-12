using System.Collections.Generic;
using SA.Android.Utilities;
using SA.Foundation.Tests;

using SA.Android.App;
using SA.Android.Manifest;
using SA.Android.Content.Pm;


namespace SA.Android.Tests.Application
{
    public class AN_ActivityCompat_Test : SA_BaseTest
    {

        public override void Test() {


            var state = AN_PermissionsManager.CheckSelfPermission(AMM_ManifestPermission.READ_CONTACTS);
            switch (state) {
                case AN_PackageManager.PermissionState.Granted:
                    AN_Logger.Log("READ_CONTACTS Permission Granted");
                    break;
                case AN_PackageManager.PermissionState.Denied:
                    AN_Logger.Log("READ_CONTACTS Permission Denied");
                    break;
            }

            bool res = AN_PermissionsManager.ShouldShowRequestPermissionRationale(AMM_ManifestPermission.READ_CONTACTS);
            AN_Logger.Log("ShouldShowRequestPermissionRationale: " + res.ToString());



            List<AMM_ManifestPermission> permissions = new List<AMM_ManifestPermission>();

            permissions.Add(AMM_ManifestPermission.READ_CONTACTS);
            permissions.Add(AMM_ManifestPermission.WRITE_CONTACTS);

            AN_PermissionsManager.RequestPermissions(permissions.ToArray(), (result) => {
                foreach (var responce in result.GrantResults) {
                    AN_Logger.Log("RequestPermissions: " + responce.Permission.ToString() + " / " + responce.GrantResult.ToString());
                }

                SetResult(SA_TestResult.OK);
            });

        }

    
    }
}