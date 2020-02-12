using System;
using UnityEngine;
using UnityEngine.UI;

using SA.Android.App;
using SA.Foundation.Utility;

public class AN_NotificationsExample : MonoBehaviour {

    [SerializeField] Button m_simple = null;
    [SerializeField] Button m_withLargeIcon = null;
    [SerializeField] Button m_closeApp = null;


    private void Awake() {

        if(AN_NotificationManager.LastOpenedNotificationRequest != null) {
            Debug.Log("Looks like the app was launched from notifications request: " 
                + JsonUtility.ToJson(AN_NotificationManager.LastOpenedNotificationRequest));
        }

        AN_NotificationManager.OnNotificationClick.AddSafeListener(this, (request) => {
            Debug.Log("request.Identifier: " + request.Identifier);
            Debug.Log("User has opened the local notification request with info: " + JsonUtility.ToJson(request));
        });

        AN_NotificationManager.OnNotificationReceived.AddSafeListener(this, (request) => {
            Debug.Log("request.Identifier: " + request.Identifier);
            Debug.Log("notification request received with info: " + JsonUtility.ToJson(request));
        });


        m_withLargeIcon.onClick.AddListener(() => {

            SA_ScreenUtil.TakeScreenshot(256, (screenshot) => {
                var builder = new AN_NotificationCompat.Builder();
                builder.SetContentText("Text");
                builder.SetContentTitle("Title");

                builder.SetLargeIcon(screenshot);
                builder.SetSmallIcon("custom_icon");
                builder.SetSound("slow_spring_board");

                var trigger = new AN_AlarmNotificationTrigger();
                trigger.SetDate(TimeSpan.FromSeconds(1));

                var id = SA_IdFactory.NextId;
                var request = new AN_NotificationRequest(id, builder, trigger);
                AN_NotificationManager.Schedule(request);
            });
        });


        m_simple.onClick.AddListener(() => {
            var builder = new AN_NotificationCompat.Builder();
            builder.SetContentText("Text 2");
            builder.SetContentTitle("Title 2");

            builder.SetSmallIcon("custom_icon");

            var trigger = new AN_AlarmNotificationTrigger();
            trigger.SetDate(TimeSpan.FromSeconds(5));

            var id = SA_IdFactory.NextId;
            var request = new AN_NotificationRequest(id, builder, trigger);

            AN_NotificationManager.Schedule(request);
        });


        m_closeApp.onClick.AddListener(() => {
            Application.Quit();
        });

    }
}
