using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Android.Contacts
{
    /// <summary>
    /// Representation of the <see cref="AN_ContactInfo"/> organization
    /// </summary>
    [Serializable]
    public class AN_ContactOrganization 
    {
        [SerializeField] string m_name = null;
        [SerializeField] string m_title = null;


        /// <summary>
        /// Organization Name
        /// </summary>
        public string Name {
            get {
                return m_name;
            }
        }

        /// <summary>
        /// Organization Title
        /// </summary>
        public string Title {
            get {
                return m_title;
            }
        }
    }
}