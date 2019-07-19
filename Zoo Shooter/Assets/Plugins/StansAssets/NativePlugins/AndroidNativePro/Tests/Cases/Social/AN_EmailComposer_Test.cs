using UnityEngine;
using SA.Foundation.Tests;
using SA.Foundation.Async;

using SA.Android.Social;
using SA.Foundation.Utility;


namespace SA.Android.Tests.Social
{
    public class AN_EmailComposer_Test : SA_BaseTest
    {

        public override bool RequireUserInteraction { get { return true; } }

        public override void Test() {

            SA_ScreenUtil.TakeScreenshot(256, (screenshot) => {
                var composer = new AN_EmailComposer();

                composer.SetText("Hello world");
                composer.SetSubject("Testing the emails sharing example");

                composer.AddRecipient("ceo@stansassets.com");
                composer.AddRecipient("support@stansassets.com");

                composer.AddImage(screenshot);
                composer.AddImage(screenshot);

                composer.Share(() => {
                    Debug.Log("Sharing flow is finished, User has retured to the app");
                    SetResult(SA_TestResult.OK);
                });
            });

        }

    
    }
}