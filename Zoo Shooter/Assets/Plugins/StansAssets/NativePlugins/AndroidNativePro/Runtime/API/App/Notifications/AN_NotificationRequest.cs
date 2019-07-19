using System;
using UnityEngine;

namespace SA.Android.App
{
    [Serializable]
    public class AN_NotificationRequest
    {

        [SerializeField] int m_identifier;
        [SerializeField] AN_NotificationCompat.Builder m_content;
        [SerializeField] AN_AlarmNotificationTrigger m_trigger;
       

        /// <summary>
        /// Creates and returns a local notification request object.
        /// </summary>
        /// <param name="identifier">An identifier for the request; this parameter must not be <c>null</c>. 
        /// You can use this identifier to cancel the request if it is still pending.</param>
        /// <param name="content">
        /// The content of the notification. This parameter must not be <c>null</c>. 
        /// </param>
        /// <param name="trigger">
        /// The condition that causes the notification to be delivered. Specify <c>null</c> to deliver the notification right away.
        /// </param>
        public AN_NotificationRequest(int identifier, AN_NotificationCompat.Builder content, AN_AlarmNotificationTrigger trigger) {

            m_identifier = identifier;

            m_content = content;
            m_trigger = trigger;
        }


        /// <summary>
        /// The unique identifier for this notification request.
        /// </summary>
        public int Identifier {
            get {
                return m_identifier;
            }
        }


        /// <summary>
        /// The content associated with the notification.
        /// 
        /// Use this property to access the contents of the notification. 
        /// The content object contains the badge information, sound to be played, 
        /// or alert text to be displayed to the user, in addition to the notification’s thread identifier.
        /// </summary>
        public AN_NotificationCompat.Builder Content {
            get {
                return m_content;
            }
        }


        /// <summary>
        /// The conditions that trigger the delivery of the notification.
        /// 
        /// For notifications that have already been delivered, use this property 
        /// to determine what caused the delivery to occur.
        /// </summary>
        public AN_AlarmNotificationTrigger Trigger {
            get {
                return m_trigger;
            }
        }
    }
}