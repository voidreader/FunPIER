using UnityEditor;
using UnityEngine;

using System.Collections.Generic;

using SA.Android;

using SA.Foundation.Editor;
using SA.Foundation.Utility;
using SA.Foundation.UtilitiesEditor;

namespace SA.Android {

    public class AN_FirebaseDefinesResolver
    {

        private const string AN_FIREBASE_MESSAGING_DEFINE = "AN_FIREBASE_MESSAGING";
        private const string AN_FIREBASE_ANALYTICS_DEFINE = "AN_FIREBASE_ANALYTICS";

        private const string FIREBASE_MESSAGING_LIB_NAME = "Firebase.Messaging.dll";
        private const string FIREBASE_ANALYTICS_LIB_NAME = "Firebase.Analytics.dll";

        //--------------------------------------
        //  Public Methods
        //--------------------------------------

        public static void ProcessAssets() 
        {
            List<string> projectLibs = SA_AssetDatabase.FindAssetsWithExtentions("Assets", ".dll");
            foreach (var lib in projectLibs) 
            {
                ProcessAssetImport(lib);
            }
        }

        public static void ProcessAssetImport(string assetPath) 
        {
            bool detected = IsPathEqualsSDKName(assetPath, FIREBASE_MESSAGING_LIB_NAME);
            if (detected) 
            {
                UpdateMessagingLibState(true);
            }

            detected = IsPathEqualsSDKName(assetPath, FIREBASE_ANALYTICS_LIB_NAME);
            if (detected) 
            {
                UpdateAnalyticsLibState(true);
            }

        }

        public static void ProcessAssetDelete(string assetPath) 
        {
            bool detected = IsPathEqualsSDKName(assetPath, FIREBASE_MESSAGING_LIB_NAME);
            if (detected) 
            {
                UpdateMessagingLibState(false);
            }

            detected = IsPathEqualsSDKName(assetPath, FIREBASE_ANALYTICS_LIB_NAME);
            if (detected) 
            {
                UpdateAnalyticsLibState(false);
            }
        }

        //--------------------------------------
        //  Get / Set
        //--------------------------------------


        public static bool IsMessagingSDKInstalled 
        {
            get 
            {
#if AN_FIREBASE_MESSAGING
                return true;
#else
                return false;
#endif
            }
        }


        public static bool IsAnalyticsSDKInstalled 
        {
            get 
            {
#if AN_FIREBASE_ANALYTICS
                return true;
#else
                return false;
#endif
            }
        }

        //--------------------------------------
        //  Private Methods
        //--------------------------------------


        private static bool IsPathEqualsSDKName(string assetPath, string SDKName) 
        {
            string fileName = SA_PathUtil.GetFileName(assetPath);
            if (fileName.Equals(SDKName)) 
            {
                return true;
            } 
            else 
            {
                return false;
            }

        }

        private static void UpdateMessagingLibState(bool enabled) 
        {
            if (enabled) 
            {
                if (!SA_EditorDefines.HasCompileDefine(AN_FIREBASE_MESSAGING_DEFINE)) 
                {
                    SA_EditorDefines.AddCompileDefine(AN_FIREBASE_MESSAGING_DEFINE);
                }

            } 
            else 
            {
                if (SA_EditorDefines.HasCompileDefine(AN_FIREBASE_MESSAGING_DEFINE)) 
                {
                    SA_EditorDefines.RemoveCompileDefine(AN_FIREBASE_MESSAGING_DEFINE);
                }
            }
        }


        private static void UpdateAnalyticsLibState(bool enabled) 
        {
            if (enabled) 
            {
                if (!SA_EditorDefines.HasCompileDefine(AN_FIREBASE_ANALYTICS_DEFINE)) 
                {
                    SA_EditorDefines.AddCompileDefine(AN_FIREBASE_ANALYTICS_DEFINE);
                }

            } 
            else 
            {
                if (SA_EditorDefines.HasCompileDefine(AN_FIREBASE_ANALYTICS_DEFINE)) 
                {
                    SA_EditorDefines.RemoveCompileDefine(AN_FIREBASE_ANALYTICS_DEFINE);
                }
            }
        }

    }

    
}
