using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;


namespace SA.Android.Gallery
{

    /// <summary>
    /// Gallery images pick result
    /// </summary>
    [Serializable]
    public class AN_GalleryPickResult : SA_Result
    {

        [SerializeField] List<AN_Media> m_media = new List<AN_Media>();

        public AN_GalleryPickResult(SA_Error error) : base(error) { } 


        /// <summary>
        /// Picked images
        /// </summary>
        public List<AN_Media> Media {
            get {
                return m_media;
            }
        }
    }
}