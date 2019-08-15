using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SA.Android.Utilities;
using SA.Android.Firebase.Messaging;

public class FB_MessagingExample : MonoBehaviour {

#pragma warning disable 649
    [SerializeField] Button m_initializeButton;
    [SerializeField] Text m_tokenText;
    [SerializeField] Text m_messageText;
#pragma warning restore 649
    
    void Start () {
        m_initializeButton.onClick.AddListener(() => {
            AN_FirebaseMessaging.Initialize((token) => {
                m_tokenText.text = token;
                AN_Logger.Log("FB Token: " + m_tokenText.text);
            });
        });
                
        AN_FirebaseMessaging.OnFbMessageReceived.AddListener((message) => {
            AN_Logger.Log("msg.CollapseKey " + message.CollapseKey);
            AN_Logger.Log("msg.Error " + message.Error);
            AN_Logger.Log("msg.ErrorDescription " + message.ErrorDescription);
            AN_Logger.Log("msg.MessageId " + message.MessageId);
            AN_Logger.Log("msg.MessageType " + message.MessageType);
            AN_Logger.Log("msg.NotificationOpened " + message.NotificationOpened);
            AN_Logger.Log("msg.Priority " + message.Priority);
            AN_Logger.Log("msg.RawData " + message.RawData);
            AN_Logger.Log("msg.TO " + message.To);
            
            foreach (KeyValuePair<string, string> data in message.Data) {
                AN_Logger.Log("data key: " + data.Key + " data value: " + data.Value);
            }
            
            AN_Logger.Log("msg.link " + message.Link);
            AN_Logger.Log("msg.TimeToLive " + message.TimeToLive);
            
            if (message.Notification == null)
                return;
            
            AN_Logger.Log("msg.Notification.Title " + message.Notification.Title);
            AN_Logger.Log("msg.TitleLocalizationKey " + message.Notification.TitleLocalizationKey);
            AN_Logger.Log("msg.Body " + message.Notification.Body);

            m_messageText.text = message.Notification.Body;
        });
    }
}
