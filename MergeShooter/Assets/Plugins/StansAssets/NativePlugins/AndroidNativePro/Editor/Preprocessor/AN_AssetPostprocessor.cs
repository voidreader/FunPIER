using UnityEngine;
using UnityEditor;


namespace SA.Android
{
    public class AN_AssetPostprocessor : AssetPostprocessor
    {



        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            foreach (string assetPath in importedAssets) {


                //games-ids.xml was created or modified;
                if (assetPath.Equals(AN_Settings.ANDROID_GAMES_IDS_FILE_PATH)) {
                    AN_GoolgePlayRersources.LoadLocalGamesIds();
                }
                AN_FirebaseDefinesResolver.ProcessAssetImport(assetPath);
                
            }


            foreach (string assetPath in deletedAssets) {

                //games-ids.xml was deleted;
                if (assetPath.Equals(AN_Settings.ANDROID_GAMES_IDS_FILE_PATH)) {
                    AN_GoolgePlayRersources.DropGamesIds();
                }

                AN_FirebaseDefinesResolver.ProcessAssetDelete(assetPath);

            }
        }
    }
}