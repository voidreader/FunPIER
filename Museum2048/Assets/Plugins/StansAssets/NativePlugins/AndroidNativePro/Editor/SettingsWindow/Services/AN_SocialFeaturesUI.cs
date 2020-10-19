using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Foundation.Editor;

namespace SA.Android.Editor
{
    class AN_SocialFeaturesUI : AN_ServiceSettingsUI
    {
        public override void OnAwake()
        {
            base.OnAwake();

            AddFeatureUrl("Facebook", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Facebook");
            AddFeatureUrl("Twitter", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Twitter");
            AddFeatureUrl("Instagram", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Instagram");
            AddFeatureUrl("WhatsApp", "https://github.com/StansAssets/com.stansassets.android-native/wiki/WhatsApp");
            AddFeatureUrl("E-mail", "https://github.com/StansAssets/com.stansassets.android-native/wiki/E-mail");
            AddFeatureUrl("Default Sharing Dialog", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Native-Sharing");
        }

        public override string Title => "Social";

        public override string Description => "Implementing an effective and user friendly share actions in your app.";

        protected override Texture2D Icon => AN_Skin.GetIcon("android_social.png");

        public override SA_iAPIResolver Resolver => AN_Preprocessor.GetResolver<AN_SocialResolver>();

        protected override void OnServiceUI()
        {
            // throw new System.NotImplementedException();
        }
    }
}
