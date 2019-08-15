using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Android.App
{
    [Serializable]
    public class AN_MainActivity : AN_Activity
    {
       
        private static AN_MainActivity s_instance = null;

        //--------------------------------------
        // Initialization
        //--------------------------------------


        /// <summary>
        /// Returns a singleton class instance
        /// </summary>
        public static AN_MainActivity Instance {
            get {
                if (s_instance == null) {
                    s_instance = new AN_MainActivity();
                    s_instance.m_classId = AN_ActivityId.Main;
                }
                return s_instance;
            }
        }


        
    }
}