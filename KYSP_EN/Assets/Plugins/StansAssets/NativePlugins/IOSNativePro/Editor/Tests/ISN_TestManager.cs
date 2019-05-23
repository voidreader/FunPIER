﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

using SA.iOS.XCode;
using SA.iOS.StoreKit;

namespace SA.iOS.Utilities {

    public static class ISN_TestManager
    {

        public const string SMALL_PACK = "buying_10000";
        public const string NC_PACK = "mm_subscription";


        public static void ApplyExampleConfig() {

            Debug.Log("ISN_TestManager::ApplyExampleConfig");
            PlayerSettings.iOS.applicationDisplayName = "IOS Native";
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.iOS.appleDeveloperTeamID = "P42C7H5LKK";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.iosnative");


            var settings = ISN_Settings.Instance;

            //Contacts
            settings.Contacts = true;


            //In-Apps
            ISN_Settings.Instance.InAppProducts.Clear();

            var p = new ISN_SKProduct();
            p.ProductIdentifier = "your.product.id1.here";

            var p2 = new ISN_SKProduct();
            p2.ProductIdentifier = "your.product.id2.here";

            ISN_Settings.Instance.InAppProducts.Add(p);
            ISN_Settings.Instance.InAppProducts.Add(p2);



            //GameKit
            ISD_API.Capability.GameCenter.Enabled = true;


            //Replay Kit
            settings.ReplayKit = true;

            //User Notifications
            settings.UserNotifications = true;
            ISD_API.Capability.PushNotifications.Enabled = true;


            //Makign environment for Vending Test
            ISD_API.Capability.InAppPurchase.Enabled = true;

            //social 
            settings.Social = true;


        }

        public static void OpenTestScene() {
            EditorSceneManager.OpenScene(ISN_Settings.TEST_SCENE_PATH, OpenSceneMode.Single);
        }


        public static void MakeTestBuild() {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.target = BuildTarget.iOS;
            playerOptions.scenes = new string[] { ISN_Settings.TEST_SCENE_PATH };
            playerOptions.locationPathName = "ios_native_test";


            playerOptions.options = BuildOptions.Development | BuildOptions.AutoRunPlayer;

            BuildPipeline.BuildPlayer(playerOptions);
        }

    }
}



