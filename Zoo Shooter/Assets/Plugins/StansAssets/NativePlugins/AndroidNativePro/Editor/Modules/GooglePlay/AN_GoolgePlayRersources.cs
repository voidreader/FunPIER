using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Utility;
using SA.Foundation.Editor;
using SA.Foundation.UtilitiesEditor;


namespace SA.Android
{
    public class AN_GoolgePlayRersources 
    {

        private static AN_GamesIds s_gamesIds;

       
        public static AN_GamesIds GamesIds {
            get {
                if(s_gamesIds == null) {
                    if (SA_AssetDatabase.IsFileExists(AN_Settings.ANDROID_GAMES_IDS_FILE_PATH)) {
                        LoadLocalGamesIds();
                    }
                }
                return s_gamesIds;
            }
        }

        public static void OverrideGamesIds(string data) {
            SA_FilesUtil.Write(AN_Settings.ANDROID_GAMES_IDS_FILE_PATH, data);
            SA_AssetDatabase.ImportAsset(AN_Settings.ANDROID_GAMES_IDS_FILE_PATH);
        }


        public static void LoadLocalGamesIds() {
            string rawData = SA_FilesUtil.Read(AN_Settings.ANDROID_GAMES_IDS_FILE_PATH);
            s_gamesIds = new AN_GamesIds(rawData);
        }

        public static void DropGamesIds() {
            s_gamesIds = null;
        }


        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            foreach (string assetPath in importedAssets) {

                //games-ids.xml was created or modified;
                if (assetPath.Equals(AN_Settings.ANDROID_GAMES_IDS_FILE_PATH)) {
                    LoadLocalGamesIds();
                }
            }

           
            foreach (string assetPath in deletedAssets) {

                //games-ids.xml was deleted;
                if (assetPath.Equals(AN_Settings.ANDROID_GAMES_IDS_FILE_PATH)) {
                  
                }
            }

        }

    }
}