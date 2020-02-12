using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Foundation.Editor;

namespace SA.Android
{

    public class AN_ContactsFeaturesUI : AN_ServiceSettingsUI
    {

        public override void OnAwake() {
            base.OnAwake();

            AddFeatureUrl("Getting Started", "https://unionassets.com/android-native-pro/getting-started-700");
            AddFeatureUrl("Retrieving Contacts", "https://unionassets.com/android-native-pro/retrieving-phone-contacts-701");

       }

        public override string Title {
            get {
                return "Contacts";
            }
        }


        public override string Description {
            get {
                return "Powerful and flexible Android component that manages the device's central repository of data about people.";
            }
        }

        protected override Texture2D Icon {
            get {
                return AN_Skin.GetIcon("android_contacts.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return AN_Preprocessor.GetResolver<AN_ContactsResolver>();
            }
        }

        protected override void OnServiceUI() {
            // throw new System.NotImplementedException();
        }
    }
}