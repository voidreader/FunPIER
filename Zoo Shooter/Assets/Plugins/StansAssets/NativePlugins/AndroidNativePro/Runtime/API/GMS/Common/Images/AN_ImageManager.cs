using System;
using SA.Android.GMS.Internal;

namespace SA.Android.GMS.Common.Images
{
    /// <summary>
    /// This class is used to load images from the network and handles local caching for you.
    /// https://developers.google.com/android/reference/com/google/android/gms/common/images/ImageManager
    /// </summary>
    public class AN_ImageManager 
    {

        /// <summary>
        /// Load an image to display from a URL. 
        /// Note that this does not support arbitrary URIs - the URI must be something 
        /// that was retrieved from another call to Google Play services.
        /// 
        /// Note that you should hold a reference to the listener provided until the callback is complete.
        /// For this reason, the use of anonymous implementations is discouraged.
        /// 
        /// The result is delivered to the given callback on the main thread.
        /// </summary>
        /// <param name="url">URL to load the image data from.</param>
        /// <param name="callback">The callback that is called when the load is complete.</param>
        public void LoadImage(string url, Action<AN_ImageLoadResult> callback) {
            AN_GMS_Lib.Common.LoadImage(url, callback);
        }
    }
}