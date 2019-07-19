using UnityEngine;
using System;
using SA.Foundation.Tests;
using SA.Foundation.Utility;

using SA.Android.App;


namespace SA.Android.Tests.Application
{
    public class AN_NotificationManager_Test : SA_BaseTest
    {

        public int variant = 1;
        private AN_NotificationRequest m_sendedRequest;

        public override void Test() {


            AN_NotificationManager.CancelAll();

            AN_NotificationManager.Cancel(1);
            AN_NotificationManager.Unschedule(1);
            

            AN_NotificationManager.OnNotificationReceived.AddListener((request) => {

                bool requestValid = ValidateRequest(request);
                if (requestValid) {
                    variant++;
                    TestNotificationWithVariantId(variant);
                } else {
                    string msg = "Received request does not match sent request   received: " +
                    JsonUtility.ToJson(request) + " sent: " + JsonUtility.ToJson(m_sendedRequest);
                    SetResult(SA_TestResult.WithError(msg));
                }
            });

            TestNotificationWithVariantId(variant);
            
        }



        private void TestNotificationWithVariantId(int variantId) {


            var builder = new AN_NotificationCompat.Builder();
          

            //should be created automatically
            builder.SetChanelId("test_chanel");
            var icon = SA_IconManager.GetIcon(Color.cyan, 32, 32);
            builder.SetLargeIcon(icon);


            var trigger = new AN_AlarmNotificationTrigger();
            trigger.SetDate(TimeSpan.FromSeconds(1));

            int id = SA_IdFactory.NextId;


            switch (variantId) {
                case 1:
                    builder.SetContentText("Default");
                    builder.SetContentTitle("SetDefaults Test");
                    builder.SetDefaults(AN_Notification.DEFAULT_LIGHTS | AN_Notification.DEFAULT_SOUND);
                    break;

                case 2:
                   
                    builder.SetContentText("BigTextStyle");
                    builder.SetContentTitle("BigTextStyle Title");

                    var bigTextStyle = new AN_NotificationCompat.BigTextStyle();
                    bigTextStyle.BigText("This is test big text style");
                    builder.SetStyle(bigTextStyle);
                    builder.SetDefaults(AN_Notification.DEFAULT_ALL);
                    break;

                case 3:
                    
                    builder.SetContentText("BigPictureStyle");
                    builder.SetContentTitle("BigPictureStyle title");

                    var bigPictureStyle = new AN_NotificationCompat.BigPictureStyle();
                    bigPictureStyle.BigPicture(SA_IconManager.GetIcon(Color.red, 32, 32));
                    bigPictureStyle.BigLargeIcon(SA_IconManager.GetIcon(Color.green, 32, 32));
                    builder.SetStyle(bigPictureStyle);
                    builder.SetDefaults(AN_Notification.DEFAULT_ALL);
                    break;

            }


            if(variantId == 4) {
                SetResult(SA_TestResult.OK);
            } else {
                m_sendedRequest = new AN_NotificationRequest(id, builder, trigger);
                AN_NotificationManager.Schedule(m_sendedRequest);
            }
          

        }


        private bool ValidateRequest(AN_NotificationRequest request) {

            //Id
            if(!m_sendedRequest.Identifier.Equals(request.Identifier)) {
                return false;
            }


            //Trigger
            if (!m_sendedRequest.Trigger.Seconds.Equals(request.Trigger.Seconds)) {
                return false;
            }

            if (!m_sendedRequest.Trigger.Repeating.Equals(request.Trigger.Repeating)) {
                return false;
            }


            //Content
            if (!m_sendedRequest.Content.Text.Equals(request.Content.Text)) {
                return false;
            }

            if (!m_sendedRequest.Content.Title.Equals(request.Content.Title)) {
                return false;
            }


            if (!m_sendedRequest.Content.SoundName.Equals(request.Content.SoundName)) {
                return false;
            }


            return true;
           
        }


    }
}