using System;
using SA.Android.Utilities;
using SA.Foundation.Templates;

namespace SA.Android.Media
{
    /// <summary>
    /// AN_MediaPlayer class can be used to control playback of audio/video files and streams.
    /// https://developer.android.com/reference/android/media/MediaPlayer
    /// </summary>
    public class AN_MediaPlayer 
    {

        const string AN_MEDIAPLAYER_MODE_CLASS = "com.stansassets.android.media.AN_MediaPlayer";

       
        /// <summary>
        /// Will show native play activity and open video that avaliable by the <see cref="url"/> param.
        /// </summary>
        /// <param name="url">Remove video url.</param>
        public static void ShowRemoteVideo(string url, Action onClose) {
            AN_Java.Bridge.CallStaticWithCallback<SA_Result>(AN_MEDIAPLAYER_MODE_CLASS, "ShowRemoteVideo", (result) => {
                onClose.Invoke();
            }, url);
        }

    }
}


