using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Android.GMS.Common
{
    /// <summary>
    /// Describes an OAuth 2.0 scope to request. 
    /// This has security implications for the user, 
    /// and requesting additional scopes will result in authorization dialogs.
    /// </summary>
    [Serializable]
    public class AN_Scope
    {

#pragma warning disable 414
        [SerializeField] string m_scopeUri;
#pragma warning restore 414



        /// <summary>
        /// Creates a new scope with the given URI.
        /// </summary>
        public AN_Scope(string scopeUri) {
            m_scopeUri = scopeUri;
        }

    }
}