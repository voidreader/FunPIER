using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;

using SA.Android.GMS.Common;

namespace SA.Android.GMS.Auth
{
    /// <summary>
    /// Google sign in flow result
    /// </summary>

    [Serializable]
    public class AN_GoogleSignInResult : AN_LinkedObjectResult<AN_GoogleSignInAccount>
    {


        public AN_GoogleSignInResult(AN_GoogleSignInAccount account) :base() {
            m_linkedObject = account;
        }

        public AN_GoogleSignInResult(SA_Error error) : base(error) { }

        /// <summary>
        /// Basic account information of the signed in Google user
        /// </summary>
        public AN_GoogleSignInAccount Account {
            get {
                return m_linkedObject;
            }
        }

        public AN_CommonStatusCodes StatusCode {
            get {
                if (Error != null) {
                    return (AN_CommonStatusCodes) Error.Code;
                } else {
                    return AN_CommonStatusCodes.SUCCESS;
                }
            }
        }
    }
}