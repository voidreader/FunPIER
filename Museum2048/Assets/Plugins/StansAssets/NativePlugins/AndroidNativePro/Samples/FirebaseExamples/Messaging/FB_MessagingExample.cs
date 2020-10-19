using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SA.Android.Utilities;
using SA.Android.Firebase.Messaging;

public class FB_MessagingExample : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField]
    Button m_initializeButton;
    [SerializeField]
    Text m_tokenText;
    [SerializeField]
    Text m_messageText;
#pragma warning restore 649

    void Start()
    {
        m_initializeButton.onClick.AddListener(() =>
        {
            AN_FirebaseMessaging.Initialize((token) =>
            {
                m_tokenText.text = token;
                Debug.Log("FB Token: " + m_tokenText.text);
            });
        });

        AN_FirebaseMessaging.OnFbMessageReceived.AddListener((message) =>
        {
            Debug.Log("msg.CollapseKey " + message.CollapseKey);
            Debug.Log("msg.Error " + message.Error);
            Debug.Log("msg.ErrorDescription " + message.ErrorDescription);
            Debug.Log("msg.MessageId " + message.MessageId);
            Debug.Log("msg.MessageType " + message.MessageType);
            Debug.Log("msg.NotificationOpened " + message.NotificationOpened);
            Debug.Log("msg.Priority " + message.Priority);
            Debug.Log("msg.RawData " + message.RawData);
            Debug.Log("msg.TO " + message.To);

            foreach (var data in message.Data) Debug.Log("data key: " + data.Key + " data value: " + data.Value);

            Debug.Log("msg.link " + message.Link);
            Debug.Log("msg.TimeToLive " + message.TimeToLive);

            if (message.Notification == null)
                return;

            Debug.Log("msg.Notification.Title " + message.Notification.Title);
            Debug.Log("msg.TitleLocalizationKey " + message.Notification.TitleLocalizationKey);
            Debug.Log("msg.Body " + message.Notification.Body);

            m_messageText.text = message.Notification.Body;
        });
    }
}
