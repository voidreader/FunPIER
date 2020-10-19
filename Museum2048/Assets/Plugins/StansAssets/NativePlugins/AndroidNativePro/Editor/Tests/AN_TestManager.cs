using SA.Android.Vending.BillingClient;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SA.Android.Editor
{
    static class AN_TestManager
    {
        static readonly string GAMES_IDS = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<resources> " +
            "<string name=\"app_id\" translatable=\"false\" > 721571874513</string>" +
            "<string name=\"package_name\" translatable=\"false\">com.stansassets.androidnative.pro</string>" +
            "</resources>";

        public static void ApplyExampleConfig()
        {
            Debug.Log("AN_TestManager::ApplyExampleConfig");
            PlayerSettings.productName = "Android Native";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.stansassets.androidnative.pro");
            PlayerSettings.Android.keystorePass = "89Regila";
            PlayerSettings.Android.keyaliasName = "android native pro";
            PlayerSettings.Android.keyaliasPass = "89Regila";

            var settings = AN_Settings.Instance;

            settings.LogLevel.Info = true;
            settings.LogLevel.Warning = true;
            settings.LogLevel.Error = true;

            settings.WTFLogging = true;

            //App APIs - Media Player
            settings.MediaPlayer = true;

            //Making environment for Vending Test
            settings.Vending = true;

            //Licensing
            settings.Licensing = true;
            settings.RSAPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAonqY2kxgUKeAioN2tnMB2jtS1tBVwm0RHvsrFkDewHfzMGyBZvHsg9UN47H1MO6omXtNvsVuOnACV02MWIY16w7TPnttYTY7e2pULARafq7GwPuh9F7gLDdGluIoi/dJGjhaCTzvY6TpslI/FegJ/tDXVsNZh7urAxO1pWP4vrs412lANAjN8O6KF2dxF0VSThejyjzhyL0QWVtXtB6mJ9Ulsw16+0ndY4/Y4gL0BYSiJ4Qa+y7Ron6IXEGOnimixvGWasQQSKZHtEOLrh593ssp4a9PKMLQHWP7Pu2AYDmzhfR/ZkR1ZupKattjsviPnz5fTpsZ3oggSK+7IDBWQwIDAQAB";

            //InApps
            settings.GooglePlayBilling = true;
            settings.InAppProducts.Clear();

            settings.AddInAppProduct("androidnative.test.product.1", AN_BillingClient.SkuType.inapp);
            settings.AddInAppProduct("androidnative.product.test.2", AN_BillingClient.SkuType.inapp);
            settings.AddInAppProduct("android.test.purchased", AN_BillingClient.SkuType.inapp, true);

            //Application
            settings.LocalNotifications = true;
            settings.SkipPermissionsDialog = true;

            //Contacts
            settings.Contacts = true;

            //Social
            settings.Social = true;

            //CameraAndGallery
            settings.CameraAndGallery = true;

            //Google Play
            settings.GooglePlay = true;
            settings.GooglePlayGamesAPI = true;
            AN_GoolgePlayRersources.OverrideGamesIds(GAMES_IDS);
        }

        public static void OpenTestScene()
        {
            EditorSceneManager.OpenScene(AN_Settings.ANDROID_TEST_SCENE_PATH, OpenSceneMode.Single);
        }

        public static void MakeTestBuild()
        {
            var playerOptions = new BuildPlayerOptions();
            playerOptions.target = BuildTarget.Android;
            playerOptions.scenes = new[] { AN_Settings.ANDROID_TEST_SCENE_PATH };
            playerOptions.locationPathName = "android_native_test.apk";

            playerOptions.options = BuildOptions.Development | BuildOptions.AutoRunPlayer;

            BuildPipeline.BuildPlayer(playerOptions);
        }
    }
}
