using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Foundation.Editor;

namespace SA.Android.Editor
{
    class AN_CameraAndGalleryFeaturesUI : AN_ServiceSettingsUI
    {
        public override void OnAwake()
        {
            base.OnAwake();

            AddFeatureUrl("Getting Started", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Getting-Started-(Camera-&-Gallery)");
            AddFeatureUrl("Save to Gallery", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Save-to-Gallery");
            AddFeatureUrl("Get an Image", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Get-Image-or-Video");
            AddFeatureUrl("Get a Video", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Get-Image-or-Video");
            AddFeatureUrl("Capture an Image ", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Camera-API#camera-image-capture");
            AddFeatureUrl("Capture a Video ", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Camera-API#camera-video-capture");
        }

        public override string Title => "Camera & Gallery";

        public override string Description => "A simplifayed way to get images or video from camera or gallery.";

        protected override Texture2D Icon => AN_Skin.GetIcon("android_gallery.png");

        public override SA_iAPIResolver Resolver => AN_Preprocessor.GetResolver<AN_CameraAndGalleryResolver>();

        protected override void OnServiceUI()
        {
            // throw new System.NotImplementedException();
        }
    }
}
