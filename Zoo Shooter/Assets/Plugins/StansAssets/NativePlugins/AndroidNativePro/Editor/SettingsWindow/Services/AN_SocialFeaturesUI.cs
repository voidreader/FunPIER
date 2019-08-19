using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Editor;

namespace SA.Android
{

    public class AN_SocialFeaturesUI : AN_ServiceSettingsUI
    {
        public override void OnAwake() {
            base.OnAwake();

            AddFeatureUrl("Facebook", "https://unionassets.com/android-native-pro/facebook-690");
            AddFeatureUrl("Twitter", "https://unionassets.com/android-native-pro/twitter-691");
            AddFeatureUrl("Instagram", "https://unionassets.com/android-native-pro/instagram-692");
            AddFeatureUrl("WhatsApp", "https://unionassets.com/android-native-pro/whatsapp-693");
            AddFeatureUrl("E-mail", "https://unionassets.com/android-native-pro/e-mail-694");
            AddFeatureUrl("Default Sharing Dialog", "https://unionassets.com/android-native-pro/default-sharing-dialog-695");

     }

        public override string Title {
            get {
                return "Social";
            }
        }


        public override string Description {
            get {
                return "Implementing an effective and user friendly share actions in your app.";
            }
        }

        protected override Texture2D Icon {
            get {
                return AN_Skin.GetIcon("android_social.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return AN_Preprocessor.GetResolver<AN_SocialResolver>();
            }
        }

        protected override void OnServiceUI() {
            // throw new System.NotImplementedException();
        }
    }
}