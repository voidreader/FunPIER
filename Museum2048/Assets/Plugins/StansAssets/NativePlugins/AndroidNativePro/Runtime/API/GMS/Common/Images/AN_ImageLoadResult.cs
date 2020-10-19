using System;
using UnityEngine;
using SA.Foundation.Templates;
using StansAssets.Foundation.Extensions;

namespace SA.Android.GMS.Common.Images
{
    /// <summary>
    /// Object reflects an image load result via <see cref="AN_ImageManager"/>
    /// </summary>
    [Serializable]
    public class AN_ImageLoadResult : SA_Result
    {
        Texture2D m_image = null;
        [SerializeField]
        string m_imageBase64 = null;

        public AN_ImageLoadResult(SA_Error error)
            : base(error) { }

        public Texture2D Image
        {
            get
            {
                if (m_image == null)
                {
                    if (string.IsNullOrEmpty(m_imageBase64)) return null;

                    m_image = new Texture2D(1, 1);
                    m_image.LoadFromBase64(m_imageBase64);
                }

                return m_image;
            }
        }
    }
}
