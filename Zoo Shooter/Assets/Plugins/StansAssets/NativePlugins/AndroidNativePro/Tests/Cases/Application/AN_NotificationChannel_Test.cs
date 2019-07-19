using UnityEngine;
using System;
using System.Collections.Generic;
using SA.Android.Utilities;
using SA.Android.Vending.Licensing;
using SA.Foundation.Tests;
using SA.Foundation.Async;
using SA.Android.OS;
using SA.Android.App;


namespace SA.Android.Tests.Application
{
    public class AN_NotificationChannel_Test : SA_BaseTest
    {

        public override void Test() {

            if (AN_Build.VERSION.SDK_INT >= AN_Build.VERSION_CODES.O) {

                string channelId = "my_channel_id";
                string name = "My Channel Name";
                string description = "My Channel Description";
                var importance = AN_NotificationManager.Importance.DEFAULT;


                AN_NotificationChannel channel = new AN_NotificationChannel(channelId, name, importance);
                channel.Description = description;

                // Register the channel with the system; you can't change the importance
                // or other notification behaviors after this
                AN_NotificationManager.CreateNotificationChannel(channel);


                //Now let's Read notification channel settings and make sure we have our chnnael registred
                List<AN_NotificationChannel> channels;
                channels = AN_NotificationManager.GetNotificationChannels();
                bool found = false;
                foreach (var stsChannel in channels) {
                    PrintChannelInfo(stsChannel);
                    if (stsChannel.Id.Equals(channelId)) {
                        found = true;
                        break;
                    }
                }

                if(!found) {
                    SetResult(SA_TestResult.WithError("The Notification Channel wasn't registred in the system"));
                    return;
                }


                channel = AN_NotificationManager.GetNotificationChannel(channelId);
                if(channel == null) {
                    SetResult(SA_TestResult.WithError("The Notification Channel wasn't registred in the system"));
                    return;
                } else {
                    PrintChannelInfo(channel);
                }


                AN_NotificationManager.DeleteNotificationChannel(channelId);


                channels = AN_NotificationManager.GetNotificationChannels();
                found = false;
                foreach (var stsCahnnel in channels) {
                    PrintChannelInfo(stsCahnnel);
                    if (stsCahnnel.Id.Equals(channelId)) {
                        found = true;
                        break;
                    }
                }

                if(found) {
                    SetResult(SA_TestResult.WithError("The Notification Channel wasn't deleted from the system"));
                    return;
                }


                SetResult(SA_TestResult.OK);
            } else {
                SetResult(SA_TestResult.WithError("Notification Channels can only be tested on android 8.0 oreo (api 26) or higher"));
            }  
        }

        private void PrintChannelInfo(AN_NotificationChannel channel) {
            AN_Logger.Log("channel.Id: " + channel.Id);
            AN_Logger.Log("channel.Name: " + channel.Name);
            AN_Logger.Log("channel.Description: " + channel.Description);
            AN_Logger.Log("channel.Importance: " + channel.Importance);
        }


    }
}