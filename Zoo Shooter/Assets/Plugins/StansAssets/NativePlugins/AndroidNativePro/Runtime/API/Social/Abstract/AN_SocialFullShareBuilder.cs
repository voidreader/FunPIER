using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SA.Android.Social
{

    public abstract class AN_SocialFullShareBuilder : AN_SocialImageShareBuilders
    {


        public void SetText(string text) {
            m_text = text;
        }

    }
}