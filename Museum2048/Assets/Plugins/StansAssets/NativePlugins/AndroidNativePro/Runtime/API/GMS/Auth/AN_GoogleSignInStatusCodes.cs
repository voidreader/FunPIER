using SA.Android.GMS.Common;

namespace SA.Android.GMS.Auth
{
    /// <summary>
    /// Google Sign In specific status codes
    /// </summary>
    public enum AN_GoogleSignInStatusCodes
    {
        /// <summary>
        /// The sign in was cancelled by the user. i.e.
        /// user cancelled some of the sign in resolutions, e.g. account picking or OAuth consent.
        /// </summary>
        SIGN_IN_CANCELLED = 12501,
        
        /// <summary>
        /// A sign in process is currently in progress and the current one cannot continue.
        /// e.g. the user clicks the SignInButton multiple times and more than one sign in intent was launched.
        /// </summary>
        SIGN_IN_CURRENTLY_IN_PROGRESS = 12502,
        
        /// <summary>
        /// The sign in attempt didn't succeed with the current account.
        ///
        /// Unlike <see cref="AN_CommonStatusCodes.SIGN_IN_REQUIRED"/>. when seeing this error code,
        /// there is nothing user can do to recover from the sign in failure.
        /// Switching to another account may or may not help. Check adb log to see details if any.
        /// </summary>
        SIGN_IN_FAILED = 12500
    }
}