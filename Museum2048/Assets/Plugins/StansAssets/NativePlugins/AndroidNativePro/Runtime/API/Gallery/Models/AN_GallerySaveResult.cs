using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;


namespace SA.Android.Gallery
{

    /// <summary>
    /// Gallery save result model.
    /// </summary>
    [Serializable]
    public class AN_GallerySaveResult : SA_Result
    {
        [SerializeField] string m_path = null;


        public AN_GallerySaveResult(SA_Error error) : base(error) { }

        /// <summary>
        /// Image saved path
        /// </summary>
        public string Path {
            get {
                return m_path;
            }
        }
    }
}