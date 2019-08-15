using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Content;

namespace SA.Android.App
{
    [Serializable]
    public class AN_ProxyActivity : AN_Activity
    {

        //--------------------------------------
        // Initialization
        //--------------------------------------

        public AN_ProxyActivity() {
            m_classId = AN_ActivityId.Proxy;
            m_instanceId = GetHashCode().ToString();
        }


    }
}