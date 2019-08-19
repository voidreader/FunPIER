using System;
using System.Collections.Generic;
using UnityEngine;



namespace SA.Android.App.Internal
{
    /// <summary>
    /// This class is for plugin internal use only
    /// </summary>
    public static class AN_AppLib
    {
        private static AN_iAppAPI m_api = null;
        public static AN_iAppAPI API {
            get {
                if (m_api == null) {
                    m_api = new AN_AppNativeAPI();
                }

                return m_api;
            }
        }


        private static AN_iNotificationsAPI m_notificationsApi = null;
        public static AN_iNotificationsAPI Notifications {
            get {
                if (m_notificationsApi == null) {
                    m_notificationsApi = new AN_NativeNotificationsAPI();
                }

                return m_notificationsApi;
            }
        }



        [Serializable]
        public class AN_AlertDialogCloseInfo
        {
            [SerializeField] String m_buttonid = null;

            public string Buttonid {
                get {
                    return m_buttonid;
                }
            }
        }



    }

}
