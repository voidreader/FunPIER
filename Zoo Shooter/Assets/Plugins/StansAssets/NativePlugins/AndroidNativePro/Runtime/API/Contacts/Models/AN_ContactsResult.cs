using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;

namespace SA.Android.Contacts
{
    /// <summary>
    /// Contacts retrive result
    /// </summary>
    [Serializable]
    public class AN_ContactsResult : SA_Result
    {

        [SerializeField] List<AN_ContactInfo> m_contacts = new List<AN_ContactInfo>();


        public AN_ContactsResult() : base() { }
        public AN_ContactsResult(SA_Error error):base(error) {  }



        /// <summary>
        /// The list of loaded contacts.
        /// </summary>
        public List<AN_ContactInfo> Contacts {
            get {
                return m_contacts;
            }
        }
    }
}