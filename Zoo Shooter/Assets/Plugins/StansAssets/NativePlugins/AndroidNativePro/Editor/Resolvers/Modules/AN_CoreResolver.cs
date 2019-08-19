using UnityEngine;

using SA.Android.Manifest;

namespace SA.Android
{
    public class AN_CoreResolver : AN_APIResolver
    {
        public override bool IsSettingsEnabled { 
            get { return true; } 
            set {} 
        }

        public override void AppendBuildRequirements(AN_AndroidBuildRequirements buildRequirements) {

            //Always required

            var proxyActivity = new AMM_ActivityTemplate(false, "com.stansassets.core.utility.AN_ProxyActivity");
            proxyActivity.SetValue("android:launchMode", "singleTask");
            proxyActivity.SetValue("android:configChanges", "fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen");
            proxyActivity.SetValue("android:theme", "@android:style/Theme.Translucent.NoTitleBar");

            buildRequirements.AddActivity(proxyActivity);

            var usesSdk = new AMM_PropertyTemplate("uses-sdk");
            usesSdk.SetValue("android:minSdkVersion", "4");
            buildRequirements.AddManifestProperty(usesSdk);



            var permissionsProxyActivity = new AMM_ActivityTemplate(false, "com.stansassets.android.app.permissions.AN_PermissionsProxyActivity");
            permissionsProxyActivity.SetValue("android:launchMode", "singleTask");
            permissionsProxyActivity.SetValue("android:configChanges", "fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen");
            permissionsProxyActivity.SetValue("android:theme", "@android:style/Theme.Translucent.NoTitleBar");
            buildRequirements.AddActivity(permissionsProxyActivity);


            buildRequirements.AddBinaryDependency(AN_BinaryDependency.GSON);
            
    
            

             buildRequirements.AddBinaryDependency(AN_BinaryDependency.AndroidX);
             var provider = new AMM_PropertyTemplate("provider");
             provider.SetValue("android:name",  "androidx.core.content.FileProvider");
             provider.SetValue("android:authorities", Application.identifier + ".fileprovider");
             provider.SetValue("android:exported", "false");
             provider.SetValue("android:grantUriPermissions", "true");
    
             var meta = new AMM_PropertyTemplate("meta-data");
             meta.SetValue("android:name", "android.support.FILE_PROVIDER_PATHS");
             meta.SetValue("android:resource", "@xml/file_paths");
    
             provider.AddProperty(meta);
    
  
             buildRequirements.AddApplicationProperty(provider);

            //Optional
            if (AN_Settings.Instance.SkipPermissionsDialog) {

                //it was removed, and starting from 2018.3 permission dialog will never be asked on start up
                //so no point to use SkipPermissionsDialog meta

#if !UNITY_2018_3_OR_NEWER
                var SkipPermissionsDialog = new AMM_PropertyTemplate("meta-data");
                SkipPermissionsDialog.SetValue("android:name", "unityplayer.SkipPermissionsDialog");
                SkipPermissionsDialog.SetValue("android:value", "true");
                buildRequirements.AddApplicationProperty(SkipPermissionsDialog);
#endif
            }

            if (AN_Settings.Instance.MediaPlayer) {
                var videoPlayerActivity = new AMM_ActivityTemplate(false, "com.stansassets.android.media.AN_VideoPlayerActivity");
                videoPlayerActivity.SetValue("android:launchMode", "singleTask");
                videoPlayerActivity.SetValue("android:configChanges", "orientation|screenSize");
                videoPlayerActivity.SetValue("android:theme", "@android:style/Theme.NoTitleBar.Fullscreen");

                buildRequirements.AddActivity(videoPlayerActivity);
            }

        

        }


    }
}