using System;
using UnityEngine;

using SA.Android.Gallery.Internal;

namespace SA.Android.Gallery
{
    public static class AN_Gallery 
    {
        /// <summary>
        /// Saves images to the device gallery.
        /// </summary>
        /// <param name="image">Image that needs to be saved</param>
        /// <param name="name">Image name. If image with the same name already exists, it will be replaced</param>
        /// <param name="callback">Operation callback.</param>
        public static void SaveImageToGallery(Texture2D image, string name,  Action<AN_GallerySaveResult> callback) {

            var appDirectory = Application.productName.Replace(" ", String.Empty);
            SaveImageToGallery(image, name, appDirectory, AN_GalleryFormat.PNG, callback);
        }


        /// <summary>
        /// Saves images to the device gallery.
        /// </summary>
        /// <param name="image">Image that needs to be saved</param>
        /// <param name="name">Image name. If image with the same name already exists, it will be replaced</param>
        /// <param name="appDirectory">app directory name</param>
        /// <param name="format">save format</param>
        /// <param name="callback">Operation callback.</param>
        public static void SaveImageToGallery(Texture2D image, string name, string appDirectory, AN_GalleryFormat format, Action<AN_GallerySaveResult> callback) {

            AN_GalleryInternal.SaveImageToGallery(image, name, appDirectory, format, callback);
        }
    }
}