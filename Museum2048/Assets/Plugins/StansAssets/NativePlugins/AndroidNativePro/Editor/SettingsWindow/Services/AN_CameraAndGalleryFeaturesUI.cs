using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Foundation.Editor;

namespace SA.Android
{

    public class AN_CameraAndGalleryFeaturesUI : AN_ServiceSettingsUI
    {

        public override void OnAwake() {
            base.OnAwake();

            AddFeatureUrl("Getting Started", "https://unionassets.com/android-native-pro/getting-started-696");
            AddFeatureUrl("Save to Gallery", "https://unionassets.com/android-native-pro/save-to-gallery-697");
            AddFeatureUrl("Get an Image", "https://unionassets.com/android-native-pro/get-image-or-video-698");
            AddFeatureUrl("Get a Video", "https://unionassets.com/android-native-pro/get-image-or-video-698");
            AddFeatureUrl("Capture an Image ", "https://unionassets.com/android-native-pro/capture-image-from-camera-699#camera-image-capture");
            AddFeatureUrl("Capture a Video ", "https://unionassets.com/android-native-pro/capture-image-from-camera-699#camera-video-capture");

        }

        public override string Title {
            get {
                return "Camera & Gallery";
            }
        }

        public override string Description {
            get {
                return "A simplifayed way to get images or video from camera or gallery.";
            }
        }


        protected override Texture2D Icon {
            get {
                return AN_Skin.GetIcon("android_gallery.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return AN_Preprocessor.GetResolver<AN_CameraAndGalleryResolver>();
            }
        }

        protected override void OnServiceUI() {
            // throw new System.NotImplementedException();
        }
    }
}