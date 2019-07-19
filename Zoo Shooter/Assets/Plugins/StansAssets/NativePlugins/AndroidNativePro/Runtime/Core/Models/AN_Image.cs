using System;
using UnityEngine;

namespace SA.Android.Utilities
{

    [Serializable]
    public class AN_Image
    {

#pragma warning disable 414

        [SerializeField] int m_hasCode = 0;
        [SerializeField] string m_name;
        [SerializeField] string m_imageBase64;

#pragma warning restore 414


        public AN_Image(Texture2D image, string name ="") {
            m_name = name;
            m_hasCode = image.GetHashCode();
            m_imageBase64 = image.ToBase64String();
        }


        public string ToJson() {
            return JsonUtility.ToJson(this);
        }

    }
}