using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Android.GMS.Internal;

namespace SA.Android.GMS.Auth
{
    /// <summary>
    /// Class that holds the basic account information of the signed in Google user.
    /// </summary>
    [System.Serializable]
    public class AN_GoogleSignInAccount : AN_LinkedObject
    {

        /// <summary>
        /// Returns the unique ID for the Google account if you built your configuration starting from DEFAULT_SIGN_IN or with RequestId() configured; 
        /// null otherwise.
        /// 
        /// This is the preferred unique key to use for a user record.
        /// 
        /// Important: Do not use this returned Google ID to communicate the currently signed in user to your backend server. 
        /// Instead, send an ID token (requestIdToken(String)), which can be securely validated on the server; 
        /// or send a server auth code (requestServerAuthCode(String)) which can be in turn exchanged for id token.
        /// </summary>
        /// <returns>The identifier.</returns>
        public string GetId() {
            return AN_GMS_Lib.Auth.GoogleSignInAccount_GetId(this);
        }


        /// <summary>
        /// Returns the display name of the signed in user if you built your configuration starting from 
        /// <see cref="AN_GoogleSignInOptions.DEFAULT_SIGN_IN"/> or with <see cref="AN_GoogleSignInOptions.Builder.RequestProfile"/> configured; 
        /// null otherwise. 
        /// 
        /// Not guaranteed to be present for all users, even when configured.
        /// </summary>
        /// <returns>The display name.</returns>
        public string GetDisplayName() {
            return AN_GMS_Lib.Auth.GoogleSignInAccount_GetDisplayName(this);
        }

        /// <summary>
        /// Returns the given name of the signed in user if you built your configuration starting from 
        /// <see cref="AN_GoogleSignInOptions.DEFAULT_SIGN_IN"/> or with <see cref="AN_GoogleSignInOptions.Builder.RequestProfile"/> configured; 
        /// null otherwise. 
        /// 
        /// Not guaranteed to be present for all users, even when configured.
        /// </summary>
        /// <returns>The display name.</returns>
        public string GetGivenName() {
            return AN_GMS_Lib.Auth.GoogleSignInAccount_GetGivenName(this);
        }

        /// <summary>
        /// Returns the email address of the signed in user if <see cref="AN_GoogleSignInOptions.Builder.RequestEmail"/>() was configured; 
        /// null otherwise.
        /// Applications should not key users by email address since a Google account's email address can change. 
        /// Use <see cref="GetId"/> as a key instead.
        /// </summary>
        /// <returns>The email.</returns>
        public string GetEmail() {
            return AN_GMS_Lib.Auth.GoogleSignInAccount_GetEmail(this);
        }

        /// <summary>
        /// Returns the photo url of the signed in user if you built your configuration starting from 
        /// <see cref="AN_GoogleSignInOptions.DEFAULT_SIGN_IN"/> or with <see cref="AN_GoogleSignInOptions.Builder.RequestProfile"/> configured; 
        /// null otherwise. 
        /// 
        /// Not guaranteed to be present for all users, even when configured.
        /// </summary>
        public string GetPhotoUrl() {
            return AN_GMS_Lib.Auth.GoogleSignInAccount_GetPhotoUrl(this);
        }


        /// <summary>
        /// Returns a one-time server auth code to send to your web server which can be exchanged for access token 
        /// and sometimes refresh token 
        /// if  <see cref="AN_GoogleSignInOptions.Builder.RequestServerAuthCode(string, bool)"/> is configured. 
        /// null otherwise.
        /// </summary>
        public string GetServerAuthCode() {
            return AN_GMS_Lib.Auth.GoogleSignInAccount_GetServerAuthCode(this);
        }

        /// <summary>
        /// Returns an ID token that you can send to your server 
        /// and sometimes refresh token 
        /// if  <see cref="AN_GoogleSignInOptions.Builder.RequestIdToken(string)"/> was configured; null otherwise.
        /// ID token is a JSON Web Token signed by Google that can be used to identify a user to a backend.
        /// </summary>
        public string GetIdToken() {
            return AN_GMS_Lib.Auth.GoogleSignInAccount_GetIdToken(this);
        }

    }
}