using System;
using System.Collections.Generic;
using StansAssets.Foundation.Extensions;
using UnityEngine;

namespace SA.Android.Gallery
{
    /// <summary>
    /// Picked image from gallery representation
    /// </summary>
    [Serializable]
    public class AN_Media
    {
        [SerializeField]
        string m_base64String = null;
        [SerializeField]
        string m_path = null;
        [SerializeField]
        AN_MediaType m_type = AN_MediaType.Image;

        Texture2D m_texture;

        /// <summary>
        /// The image path
        /// </summary>
        /// <value>The path.</value>
        public string Path => m_path;

        /// <summary>
        /// Picked Image
        /// </summary>
        public Texture2D Thumbnail
        {
            get
            {
                if (string.IsNullOrEmpty(m_base64String))
                    return null;

                if (m_texture == null)
                {
                    m_texture = new Texture2D(1, 1);
                    m_texture.LoadFromBase64(m_base64String);
                }

                return m_texture;
            }
        }

        /// <summary>
        /// Gets image raw bytes
        /// </summary>
        public byte[] RawBytes
        {
            get
            {
                if (string.IsNullOrEmpty(m_base64String)) return null;
                return Convert.FromBase64String(m_base64String);
            }
        }

        public AN_MediaType Type => m_type;
    }
}
