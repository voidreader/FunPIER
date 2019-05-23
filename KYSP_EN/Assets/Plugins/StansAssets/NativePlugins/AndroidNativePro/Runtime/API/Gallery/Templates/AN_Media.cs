using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Android.Gallery
{

    /// <summary>
    /// Picked image from gallery representation
    /// </summary>
    [Serializable]
    public class AN_Media 
    {

        [SerializeField] string m_base64String = null;
        [SerializeField] string m_path = null;
        [SerializeField] AN_MediaType m_type = AN_MediaType.Image;

        private Texture2D m_texture;


        /// <summary>
        /// The image path
        /// </summary>
        /// <value>The path.</value>
        public string Path {
            get {
                return m_path;
            }
        }

        /// <summary>
        /// Picked Image
        /// </summary>
        public Texture2D Thumbnail {
            get {
                if (m_texture == null) {
                    m_texture = new Texture2D(1, 1);
                    m_texture.LoadImageFromBase64(Base64String);
                }
                return m_texture;
            }

        }

        public AN_MediaType Type {
            get {
                return m_type;
            }
        }

        public string Base64String {
            get {
                return m_base64String;
            }

        }
    }
}