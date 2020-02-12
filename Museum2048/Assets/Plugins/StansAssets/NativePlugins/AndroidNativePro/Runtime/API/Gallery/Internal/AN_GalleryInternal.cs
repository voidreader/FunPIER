using System;
using UnityEngine;
using SA.Android.Utilities;
using SA.Foundation.Async;
using SA.Foundation.Templates;

namespace SA.Android.Gallery.Internal
{
    internal static class AN_GalleryInternal 
    {
        private static string ANDROID_CLASS = "com.stansassets.gallery.AN_Gallery";
        private static string AN_MEDIASTORE_CLASS = "com.stansassets.gallery.AN_MediaStore";
        
        public static void StartNativeImageCaptureIntent(Action<AN_GalleryPickResult> callback) 
        {            
            AN_Java.Bridge.CallStaticWithCallback(AN_MEDIASTORE_CLASS, "TakePicture", callback);
        }

        public static void PickImageFromGallery(int maxSize, AN_GalleryChooseType type, bool allowMultiSelect, Action<AN_GalleryPickResult> callback) 
        {
            if (Application.isEditor) 
            {
                SA_Coroutine.WaitForSeconds(1, () => 
                {
                    var error = new SA_Error(100, "Gallery does not available on current device");
                    callback.Invoke(new AN_GalleryPickResult(error));
                });
                return;
            }

            var chooserType = (int)type;
            AN_Java.Bridge.CallStaticWithCallback(ANDROID_CLASS, "PickImageFromGallery", callback, maxSize, chooserType, allowMultiSelect);
        }

        public static void SaveImageToGallery(Texture2D image, string name, string appDirectory, AN_GalleryFormat format, Action<AN_GallerySaveResult> callback) 
        {
            var base64 = image.ToBase64String();
            var saveFormat = (int)format;

            if (Application.isEditor) 
            {
                SA_Coroutine.WaitForSeconds(1, () => 
                {
                    var error = new SA_Error(100, "Gallery does not avaliable on current device");
                    callback.Invoke(new AN_GallerySaveResult(error));
                });
                return;
            }
            
            AN_Java.Bridge.CallStaticWithCallback(ANDROID_CLASS, "SaveImageToGallery", callback, base64, appDirectory, name, saveFormat);
        }

        public static void DeleteChooserTmpDirectory() 
        {
            AN_Java.Bridge.CallStatic(ANDROID_CLASS, "DeleteChooserTmpDirectory");
        }
    }
}