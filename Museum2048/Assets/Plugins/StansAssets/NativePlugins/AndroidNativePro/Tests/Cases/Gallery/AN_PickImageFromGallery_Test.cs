using UnityEngine;
using SA.Foundation.Tests;
using SA.Foundation.Async;

using SA.Android.Gallery;
using SA.Android.Utilities;
using SA.Foundation.Utility;

namespace SA.Android.Tests.Gallery
{
    public class AN_PickImageFromGallery_Test : SA_BaseTest
    {

        public override bool RequireUserInteraction {get { return true;}}

        public override void Test() {

            AN_MediaPicker picker = new AN_MediaPicker(AN_MediaType.Video);
            picker.AllowMultiSelect = true;
            picker.MaxSize = 256;

            picker.Show((result) => {
                if (result.IsSucceeded) {
                    foreach (var an_image in result.Media) {
                        AN_Logger.Log("ImagePath: " + an_image.Path);
                        AN_Logger.Log("Captured image Color: " + an_image.Thumbnail.GetPixel(10, 10));
                        AN_Logger.Log("Captured image Color: " + an_image.Thumbnail.GetPixel(50, 50));
                        AN_Logger.Log("m_texture.width: " + an_image.Thumbnail.width + " height: " + an_image.Thumbnail.height);
                    }
                }

                SetAPIResult(result);
            });

        }
    }
}