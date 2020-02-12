using SA.Android.GMS.Internal;

namespace SA.Android.GMS.Auth
{
    /// <summary>
    /// Entry point for the Google Sign In API.
    /// </summary>
    public static class AN_GoogleSignIn
    {
        /// <summary>
        /// Create a new instance of <see cref="AN_GoogleSignInOptions"/>
        /// </summary>
        /// <param name="options">
        /// A <see cref="AN_GoogleSignInOptions"/> used to configure the GoogleSignInClient. 
        /// It is recommended to build out a DEFAULT_SIGN_IN.</param>
        public static AN_GoogleSignInClient GetClient(AN_GoogleSignInOptions options) 
        {
            return AN_GMS_Lib.Auth.GoogleSignIn_GetClient(options);
        }

        /// <summary>AN_GoogleSignInClient
        /// Gets the last account that the user signed in with
        /// </summary>
        /// <returns>
        /// <see cref="AN_GoogleSignInAccount"/> from last known successful sign-in. 
        /// If user has never signed in before or has signed out / revoked access, null is returned.
        /// </returns>
        public static AN_GoogleSignInAccount GetLastSignedInAccount() 
        {
            return AN_GMS_Lib.Auth.GoogleSignIn_GetLastSignedInAccount();
        }
    }
}