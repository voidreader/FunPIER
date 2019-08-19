using UnityEngine;
using SA.Foundation.Tests;
using SA.Foundation.Async;

using SA.Android.Camera;
using SA.Android.Utilities;
using SA.Foundation.Utility;

namespace SA.Android.Tests.Gallery
{
    public class AN_GetImageFromCamera_Test : SA_BaseTest
    {

        public override bool RequireUserInteraction { get { return true; } }

        public override void Test() {

            int maxSize = 1024;
            AN_Camera.CaptureImage(maxSize, (result) => {

                if (result.IsSucceeded) {
                    Texture2D image = result.Media.Thumbnail;
                    AN_Logger.Log("Captured image Color: " + image.GetPixel(10, 10));
                    AN_Logger.Log("Captured image Color: " + image.GetPixel(50, 50));
                    AN_Logger.Log("m_texture.width: " + image.width + " height: " + image.height);
                }
                SetAPIResult(result);
            });

          
        }
    }
}