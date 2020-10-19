using System.Collections.Generic;
using StansAssets.Foundation.Editor;
using UnityEditor;
using UnityEditor.PackageManager;

namespace SA.Android
{
    public static class AN_Packages
    {
        public static readonly string FirebaseAnalyticsPackage = "com.google.firebase.analytics";
        public static readonly string FirebaseMessagingPackage = "com.google.firebase.messaging";
        
        const string k_FirebaseSdkVersion = "6.15.2";
        const string k_Edm4UName = "com.google.external-dependency-manager";
        const string k_Edm4UVersion = "1.2.157";

        const string k_Edm4UNameOldAssetPath = "Assets/PlayServicesResolver";
        const string k_Edm4UNameNewAssetPath = "Assets/ExternalDependencyManager";

        static ScopeRegistry GoogleScopeRegistry =>
            new ScopeRegistry("Game Package Registry by Google",
                "https://unityregistry-pa.googleapis.com",
                new HashSet<string>
                {
                    "com.google"
                });

        public static void InstallEdm4U()
        {
            if (AssetDatabase.IsValidFolder(k_Edm4UNameOldAssetPath) || AssetDatabase.IsValidFolder(k_Edm4UNameNewAssetPath))
            {
                // Looks like user has it installed, let's remove the package to avoid a conflict
                Client.Remove(k_Edm4UName);
                return;
            }

            AddGooglePackage(k_Edm4UName, k_Edm4UVersion);
        }

        public static void InstallFirebasePackage(string packageName)
        {
            AddGooglePackage(packageName, k_FirebaseSdkVersion);
        }

        public static bool IsMessagingSdkInstalled 
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
        
        public static bool IsAnalyticsSdkInstalled 
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
        
        static void AddGooglePackage(string packageName, string packageVersion)
        {
            AddPackage(GoogleScopeRegistry, packageName, packageVersion);
        }

        static void AddPackage(ScopeRegistry scopeRegistry, string packageName, string packageVersion)
        {
            var manifest = new StansAssets.Foundation.Editor.Manifest();
            manifest.Fetch();
            
            var manifestUpdated = false;
            if (!manifest.TryGetScopeRegistry(scopeRegistry.Url, out  _))
            {
                manifest.SetScopeRegistry(scopeRegistry.Url, scopeRegistry);
                manifestUpdated = true;
            }
            
            if (!manifest.IsDependencyExists(packageName))
            {
                manifest.SetDependency(packageName, packageVersion);
                manifestUpdated = true;
            }

            if (manifestUpdated)
                manifest.ApplyChanges();
        }
    }
}
