using System;
using System.Collections.Generic;
using UnityEngine;



namespace SA.Android.Contacts
{

    /// <summary>
    /// Representation of the <see cref="AN_ContactInfo"/> postal address.
    /// </summary>
    [Serializable]
    public class AN_ContactPostalAddress 
    {
#pragma warning disable 649
        [SerializeField] string m_poBox;
        [SerializeField] string m_street;
        [SerializeField] string m_city;
        [SerializeField] string m_state;
        [SerializeField] string m_postalCode;
        [SerializeField] string m_country;
        [SerializeField] string m_type;
#pragma warning restore 649

        /// <summary>
        /// postal box
        /// </summary>
        public string PoBox {
            get {
                return m_poBox;
            }
        }

        /// <summary>
        /// Postal Address Street
        /// </summary>
        public string Street {
            get {
                return m_street;
            }

        }


        /// <summary>
        /// Postal Address City
        /// </summary>
        public string City {
            get {
                return m_city;
            }
        }

        /// <summary>
        /// Postal Address State
        /// </summary>
        public string State {
            get {
                return m_state;
            }
        }

        /// <summary>
        /// Postal Address Postal Code
        /// </summary>
        public string PostalCode {
            get {
                return m_postalCode;
            }

        }

        /// <summary>
        /// Postal Address Country
        /// </summary>
        public string Country {
            get {
                return m_country;
            }
        }

        /// <summary>
        /// Postal Address Type
        /// </summary>
        public string Type {
            get {
                return m_type;
            }

        }
    }
}