using System;

using SA.Android.App;
using SA.Android.Manifest;
using SA.Android.Gallery.Internal;


namespace SA.Android.Camera
{

    /// <summary>
    /// Entry point for the camera API.
    /// </summary>
    public static class AN_Camera {

      
        /// <summary>
        /// Capture an image from the device camera.
        /// </summary>
        /// <param name="callback">Operation result callback.</param>
        public static void CaptureImage(Action<AN_CameraCaptureResult> callback) {
            CaptureImage(512, callback);
        }

        /// <summary>
        /// Capture an image from the device camera.
        /// </summary>
        /// <param name="maxSize">Max captured media thumbnail size that will be transferred to Unity</param>
        /// <param name="callback">Operation result callback.</param>
        public static void CaptureImage(int maxSize, Action<AN_CameraCaptureResult> callback) {
            AN_PermissionsUtility.TryToResolvePermission(
            new [] { AMM_ManifestPermission.READ_EXTERNAL_STORAGE, AMM_ManifestPermission.WRITE_EXTERNAL_STORAGE }, 
                (granted) => {

                    AN_GalleryInternal.PickImageFromGallery(maxSize, AN_GalleryChooseType.CAPTURE_PICTURE, false, (result) => {
                    AN_CameraCaptureResult captureResult;

                    if (result.IsFailed) {
                        captureResult = new AN_CameraCaptureResult(result.Error);
                    } else {
                        captureResult = new AN_CameraCaptureResult(result.Media[0]);
                    }
                    callback.Invoke(captureResult);
                });

            });


        }


        /// <summary>
        /// Capture a video from the device camera.
        /// </summary>
        /// <param name="callback">Operation result callback.</param>
        public static void CaptureVideo(Action<AN_CameraCaptureResult> callback) {
            CaptureVideo(512, callback);
        }



        /// <summary>
        /// Capture a video from the device camera.
        /// </summary>
        /// <param name="maxSize">Max captured media thumbnail size that will be transferred to Unity</param>
        /// <param name="callback">Operation result callback.</param>
        public static void CaptureVideo(int maxSize, Action<AN_CameraCaptureResult> callback) {


            AN_PermissionsUtility.TryToResolvePermission(
           new [] { AMM_ManifestPermission.READ_EXTERNAL_STORAGE, AMM_ManifestPermission.WRITE_EXTERNAL_STORAGE },
               (granted) => {

                   AN_GalleryInternal.PickImageFromGallery(maxSize, AN_GalleryChooseType.CAPTURE_VIDEO, false, (result) => {
                       AN_CameraCaptureResult captureResult;

                       if (result.IsFailed) {
                           captureResult = new AN_CameraCaptureResult(result.Error);
                       } else {
                           captureResult = new AN_CameraCaptureResult(result.Media[0]);
                       }
                       callback.Invoke(captureResult);
                   });
            });
        }



        /// <summary>
        /// All media that was captured using device camera on the device storage.
        /// Since Unity part will only get media thumbnails as texture2D, you might want to get original captured media
        /// using the <see cref="Gallery.AN_Media.Path"/>. 
        /// Once you've complete operations with media, you may removed all captured media files using this method.
        /// </summary>
        public static void DeleteCapturedMedia() {
            AN_GalleryInternal.DeleteChooserTmpDirectory();
        }


    }
}