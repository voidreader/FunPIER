using System;

namespace SA.Android.App
{
    /// <summary>
    /// Proxy activity that is displayed on top of the application activity.
    /// Use it of you need to obtain and activity result.
    /// </summary>
    [Serializable]
    public class AN_ProxyActivity : AN_Activity
    {
        //--------------------------------------
        // Initialization
        //--------------------------------------

        public AN_ProxyActivity()
        {
            m_classId = AN_ActivityId.Proxy;
            m_instanceId = GetHashCode().ToString();
        }
    }
}
