using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Android.App
{

    [Serializable]
    public class AN_PermissionsRequestResult 
    {

        [SerializeField] List<AN_PermissionsRequestResponce> m_grantResults = new List<AN_PermissionsRequestResponce>();
      

        public void AddResponce(AN_PermissionsRequestResponce responce) {
            m_grantResults.Add(responce);
        }


        public List<AN_PermissionsRequestResponce> GrantResults {
            get {
                return m_grantResults;
            }
        }

    }
}