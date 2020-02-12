
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;

namespace SA.Android
{
    public class AN_ServicesTab : SA_ServicesTab
    {
        protected override void OnCreateServices() {
            RegisterService(CreateInstance<AN_AppFeaturesUI>());
            RegisterService(CreateInstance<AN_VendingFeaturesUI>());

            RegisterService(CreateInstance<AN_GooglePlayFeaturesUI>());
            RegisterService(CreateInstance<AN_SocialFeaturesUI>());
            RegisterService(CreateInstance<AN_CameraAndGalleryFeaturesUI>());
            RegisterService(CreateInstance<AN_LocalNotificationsFeaturesUI>());
            RegisterService(CreateInstance<AN_ContactsFeaturesUI>());
            RegisterService(CreateInstance<AN_FirebaseFeaturesUI>());
        }
    }
}