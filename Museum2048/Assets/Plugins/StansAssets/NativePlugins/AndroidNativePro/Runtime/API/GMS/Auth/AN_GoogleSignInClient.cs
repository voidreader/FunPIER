using System;
using SA.Android.Utilities;
using SA.Android.GMS.Internal;

namespace SA.Android.GMS.Auth
{
    /// <summary>
    /// A client for interacting with the Google Sign In API.
    /// </summary>
    [Serializable]
    public class AN_GoogleSignInClient : AN_LinkedObject
    {
        /// <summary>
        /// Start the Google Sign In flow
        /// </summary>
        /// <param name="callback">Sign In flow callback.</param>
        public void SignIn(Action<AN_GoogleSignInResult> callback) 
        {
            AN_GMS_Lib.Auth.GoogleSignInClient_SignIn(this, callback);
        }
        
        /// <summary>
        /// Returns the <see cref="AN_GoogleSignInAccount"/> information for the user who is signed in to this app. 
        /// If no user is signed in, try to sign the user in without displaying any user interface.
        /// </summary>
        /// <param name="callback">Sign In flow callback.</param>
        public void SilentSignIn(Action<AN_GoogleSignInResult> callback) 
        {
            AN_GMS_Lib.Auth.GoogleSignInClient_SilentSignIn(this, callback);
        }

        /// <summary>
        /// Signs out the current signed-in user if any. 
        /// It also clears the account previously selected by the user and a future sign in attempt will require the user pick an account again.
        /// </summary>
        /// <param name="callback">Sign out flow callback.</param>
        public void SignOut(Action callback) 
        {
            AN_GMS_Lib.Auth.GoogleSignInClient_SignOut(this, result => 
            {
                callback.Invoke();
            });
        }

        /// <summary>
        /// Revokes access given to the current application. 
        /// Future sign-in attempts will require the user to re-consent to all requested scopes. 
        /// Applications are required to provide users that are signed in with Google the ability to disconnect their Google account from the app. 
        /// If the user deletes their account, you must delete the information that your app obtained from the Google APIs.
        /// </summary>
        /// <param name="callback">Sign out flow callback.</param>
        public void RevokeAccess(Action callback) 
        {
            AN_GMS_Lib.Auth.GoogleSignInClient_RevokeAccess(this, result => 
            {
                callback.Invoke();
            });
        }
    }
}