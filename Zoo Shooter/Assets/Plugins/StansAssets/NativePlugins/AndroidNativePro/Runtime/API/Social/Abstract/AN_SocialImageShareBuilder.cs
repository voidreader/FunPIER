using UnityEngine;

namespace SA.Android.Social
{

    public abstract class AN_SocialImageShareBuilders : AN_SocialShareBuilder
    {
        public virtual void AddImage(Texture2D image) {
            m_images.Add(image);
        }


    }
}