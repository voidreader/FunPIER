using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;
using SA.Android.Gallery;


namespace SA.Android.Camera
{
    /// <summary>
    /// Reult of capturing photo or video from camera
    /// </summary>
    [Serializable]
    public class AN_CameraCaptureResult : SA_Result
    {
        private AN_Media m_media = null;
        public AN_CameraCaptureResult(AN_Media media):base() {
            m_media = media;
        }
        public AN_CameraCaptureResult(SA_Error error):base(error) { }


        /// <summary>
        /// Captured media
        /// </summary>
        public AN_Media Media {
            get {
                return m_media;
            }

        }
    }
}