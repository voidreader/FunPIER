using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Foundation.Editor;

namespace SA.Android.Editor
{
    class AN_ContactsFeaturesUI : AN_ServiceSettingsUI
    {
        public override void OnAwake()
        {
            base.OnAwake();

            AddFeatureUrl("Getting Started", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Getting-Started-(Contacts)");
            AddFeatureUrl("Retrieving Contacts", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Retrieving-Phone-Contacts");
        }

        public override string Title => "Contacts";

        public override string Description => "Powerful and flexible Android component that manages the device's central repository of data about people.";

        protected override Texture2D Icon => AN_Skin.GetIcon("android_contacts.png");

        public override SA_iAPIResolver Resolver => AN_Preprocessor.GetResolver<AN_ContactsResolver>();

        protected override void OnServiceUI()
        {
            // throw new System.NotImplementedException();
        }
    }
}
