using System;
using UnityEngine;
using SA.Android.Utilities;
using StansAssets.Foundation.Extensions;

namespace SA.Android.App
{
    /// <summary>
    /// Helper for accessing features in <see cref="AN_Notification"/>
    /// </summary>
    public class AN_NotificationCompat
    {
        const string NATIVE_BUILDER_CLASS_NAME = "com.stansassets.android.app.notifications.AN_NotificationBuilder";

        /// <summary>
        /// If this notification is being shown as a badge, always show as a number.
        /// </summary>
        public const int BADGE_ICON_NONE = 0;

        /// <summary>
        /// If this notification is being shown as a badge, use the icon provided to
        /// <see cref="AN_NotificationCompat.Builder.SetSmallIcon"/> to represent this notification.
        /// </summary>
        public const int BADGE_ICON_SMALL = 1;

        /// <summary>
        /// If this notification is being shown as a badge, use the icon provided to
        /// <see cref="AN_NotificationCompat.Builder.SetLargeIcon"/> to represent this notification.
        /// </summary>
        public const int BADGE_ICON_LARGE = 2;

        /// <summary>
        /// An object that can apply a rich notification style to <see cref="AN_Notification"/>. 
        /// If the platform does not provide rich notification styles, methods in this class have no effect.
        /// </summary>
        public abstract class Style { }

        /// <summary>
        /// Helper class for generating large-format notifications that include a lot of text. 
        /// If the platform does not provide large-format notifications, this method has no effect.The user will always see the normal notification view. 
        /// </summary>
        public class BigTextStyle : Style
        {
            internal string m_text;

            /// <summary>
            /// Provide the longer text to be displayed in the big form of the template in place of the content text.
            /// </summary>
            /// <param name="text"></param>
            public void BigText(string text)
            {
                m_text = text;
            }
        }

        /// <summary>
        /// Helper class for generating large-format notifications that include a large image attachment. 
        /// If the platform does not provide large-format notifications, this method has no effect.The user will always see the normal notification view. 
        /// </summary>
        public class BigPictureStyle : Style
        {
            internal Texture2D m_Picture;
            internal Texture2D m_LargeIcon;

            /// <summary>
            /// Override the large icon when the big notification is shown.
            /// </summary>
            /// <param name="tex">Texture2D</param>
            public void BigLargeIcon(Texture2D tex)
            {
                m_LargeIcon = tex;
            }

            /// <summary>
            /// Provide the <see cref="Texture2D"/> to be used as the payload for the BigPicture notification.
            /// </summary>
            /// <param name="tex">Texture2D</param>
            public void BigPicture(Texture2D tex)
            {
                m_Picture = tex;
            }
        }

        /// <summary>
        /// Builder class for <see cref="AN_Notification"/> objects. 
        /// Allows easier control over all the flags, as well as help constructing the typical notification layouts.
        /// On platform versions that don't offer expanded notifications, methods that depend on expanded notifications have no effect.
        /// For example, action buttons won't appear on platforms prior to Android 4.1. 
        /// Action buttons depend on expanded notifications, which are only available in Android 4.1 and later.
        /// </summary>
        [Serializable]
        public class Builder
        {
#pragma warning disable 414
            [SerializeField]
            string m_Text = string.Empty;
            [SerializeField]
            string m_Title = string.Empty;

            [SerializeField]
            string m_IconName = string.Empty;
            [SerializeField]
            string m_SoundName = string.Empty;
            [SerializeField]
            string m_IconBase64 = string.Empty;

            [SerializeField]
            int m_BadgeNumber = -1;
            [SerializeField]
            int m_BadgeIcon = -1;

            [SerializeField]
            int m_Defaults;
            [SerializeField]
            string m_ChanelId = string.Empty;
            [SerializeField]
            AN_NotificationStyle m_Style;
#pragma warning restore 414

            /// <summary>
            /// Notification body text.
            /// </summary>
            public string Text => m_Text;

            /// <summary>
            /// Notification body title.
            /// </summary>
            public string Title => m_Title;

            /// <summary>
            /// Notification channel id
            /// </summary>
            public string ChanelId => m_ChanelId;

            /// <summary>
            /// Notification sounds name
            /// Note that if objcet was receiveed from nativ part, in case youu have prodvided 
            /// a valid sound before. This filed will have native url to the sound.
            /// </summary>
            public string SoundName => m_SoundName;

            /// <summary>
            /// Notification Defaults. Like <see cref="AN_Notification.DEFAULT_ALL"/>
            /// </summary>
            public int Defaults => m_Defaults;

            /// <summary>
            /// Set the second line of text in the platform notification template.
            /// </summary>
            /// <param name="text">Content Text.</param>
            public void SetContentText(string text)
            {
                m_Text = text;
            }

            /// <summary>
            /// Set the first line of text in the platform notification template.
            /// </summary>
            /// <param name="title">Title.</param>
            public void SetContentTitle(string title)
            {
                m_Title = title;
            }

            /// <summary>
            /// Set the small icon resource, which will be used to represent the notification in the status bar. 
            /// The platform template for the expanded view will draw this icon in the left, 
            /// unless a large icon has also been specified, 
            /// in which case the small icon will be moved to the right-hand side.
            /// 
            /// Do not specify file extension.
            /// Example: myIcon
            /// </summary>
            /// <param name="iconName">A android resource name in the application's package of the drawable to use.</param>
            public void SetSmallIcon(string iconName)
            {
                m_IconName = iconName;
            }

            /// <summary>
            /// Add a large icon to the notification content view. 
            /// In the platform template, this image will be shown on the left of the notification view 
            /// in place of the small icon (which will be placed in a small badge atop the large icon).
            /// </summary>
            /// <param name="icon">Icon as Texture2D</param>
            public void SetLargeIcon(Texture2D icon)
            {
                m_IconBase64 = icon.ToBase64();
            }

            /// <summary>
            /// Set the large number at the right-hand side of the notification.
            /// This is equivalent to setContentInfo,
            /// although it might show the number in a different font size for readability.
            /// </summary>
            /// <param name="number">notification badges number.</param>
            public void SetNumber(int number)
            {
                m_BadgeNumber = number;
            }

            /// <summary>
            /// Sets which icon to display as a badge for this notification.
            /// Must be one of <see cref="AN_NotificationCompat.BADGE_ICON_NONE"/>,
            /// <see cref="AN_NotificationCompat.BADGE_ICON_SMALL"/>, <see cref="AN_NotificationCompat.BADGE_ICON_LARGE"/>.
            /// Note: This value might be ignored, for launchers that don't support badge icons.
            /// </summary>
            /// <param name="icon">icon type.</param>
            public void SetBadgeIconType(int icon)
            {
                m_BadgeIcon = icon;
            }

            /// <summary>
            /// Specifies the channel the notification should be delivered on.
            /// </summary>
            /// <param name="chanelId">Id of the notifications chanel</param>
            public void SetChanelId(string chanelId)
            {
                m_ChanelId = chanelId;
            }

            /// <summary>
            /// Set the sound to play. It will play on the default stream.
            /// On some platforms, a notification that is noisy is more likely to be presented as a heads-up notification.
            /// </summary>
            /// <param name="soundName">sound name</param>
            public void SetSound(string soundName)
            {
                m_SoundName = soundName;
            }

            /// <summary>
            /// Set the default notification options that will be used.
            /// The value should be one or more of the following fields combined with bitwise-or: 
            /// <see cref="AN_Notification.DEFAULT_SOUND"/>, <see cref="AN_Notification.DEFAULT_VIBRATE"/>, <see cref="AN_Notification.DEFAULT_LIGHTS"/> 
            ///
            /// For all default values, use <see cref="AN_Notification.DEFAULT_ALL"/>.
            /// </summary>
            /// <param name="defaults">default value</param>
            public void SetDefaults(int defaults)
            {
                m_Defaults = defaults;
            }

            /// <summary>
            /// Add a rich notification style to be applied at build time. 
            /// If the platform does not provide rich notification styles, this method has no effect.The user will always see the normal notification style.
            /// </summary>
            /// <param name="style">Object responsible for modifying the notification style.</param>
            public void SetStyle(Style style)
            {
                m_Style = new AN_NotificationStyle();
                m_Style.m_type = AN_NotificationStyle.NONE;

                if (style is BigPictureStyle)
                {
                    var bigPictureStyle = (BigPictureStyle)style;
                    m_Style.m_type = AN_NotificationStyle.BIG_PICTURE_STYLE;

                    if (bigPictureStyle.m_Picture != null)
                    {
                        m_Style.m_picture = bigPictureStyle.m_Picture.ToBase64();
                    }

                    if (bigPictureStyle.m_LargeIcon != null)
                    {
                        m_Style.m_largeIcon = bigPictureStyle.m_LargeIcon.ToBase64();
                    }
                }

                if (style is BigTextStyle)
                {
                    var bigTextStyle = (BigTextStyle)style;
                    m_Style.m_type = AN_NotificationStyle.BIG_TEXT_STYLE;
                    m_Style.m_bigText = bigTextStyle.m_text;
                }
            }

            /// <summary>
            /// Combine all of the options that have been set and return a new <see cref="AN_Notification"/> object.
            /// </summary>
            /// <returns>The build.</returns>
            public AN_Notification Build()
            {
                var hash = AN_Java.Bridge.CallStatic<int>(NATIVE_BUILDER_CLASS_NAME, "Build", this);
                var notification = new AN_Notification(hash);
                return notification;
            }
        }

        [Serializable]
        internal class AN_NotificationStyle
        {
            public const int NONE = 0;
            public const int BIG_PICTURE_STYLE = 1;
            public const int BIG_TEXT_STYLE = 2;
            public int m_type = 0;

            //BigPictureStyle 1
            public string m_picture;
            public string m_largeIcon;

            //BigTextStyle 2
            public string m_bigText;
        }
    }
}
