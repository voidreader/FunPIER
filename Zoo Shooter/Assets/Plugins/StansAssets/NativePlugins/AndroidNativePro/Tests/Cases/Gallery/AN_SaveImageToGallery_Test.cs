using UnityEngine;
using SA.Foundation.Tests;
using SA.Foundation.Async;

using SA.Android.Gallery;
using SA.Android.Utilities;
using SA.Foundation.Utility;



namespace SA.Android.Tests.Gallery
{
    public class AN_SaveImageToGallery_Test : SA_BaseTest
    {

        public override void Test() {

            SA_ScreenUtil.TakeScreenshot((screenshot) => {
                AN_Gallery.SaveImageToGallery(screenshot, "Example Scnee", (result) => {
                    if (result.IsSucceeded) {
                        AN_Logger.Log("Screenshot has been saved to:  " + result.Path);
                    }
                    SetAPIResult(result);
                });
            });

        }


    }
}