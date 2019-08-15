using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Android.Utilities
{
    public static class AN_Services
    {

        private static Transform s_services = null;

        public static Transform Parent {
            get {
                if (s_services == null) {
                    s_services = new GameObject("Android Native Services").transform;
                    GameObject.DontDestroyOnLoad(s_services);
                }

                return s_services;
            }
        }

    }
}