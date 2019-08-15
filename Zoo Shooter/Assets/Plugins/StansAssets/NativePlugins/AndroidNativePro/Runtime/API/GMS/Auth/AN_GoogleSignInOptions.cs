using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Android.GMS.Common;
using SA.Android.GMS.Internal;

namespace SA.Android.GMS.Auth
{

    /// <summary>
    /// GoogleSignInOptions is options used to configure the GOOGLE_SIGN_IN_API.
    /// </summary>
    [System.Serializable]
    public class AN_GoogleSignInOptions : AN_LinkedObject
    {


        /// <summary>
        /// Default and recommended configuration for Games Sign In.
        /// Can be used as parametr when createing  to <see cref="AN_GoogleSignInOptions.Builder"/>
        /// </summary>
        public static int DEFAULT_GAMES_SIGN_IN = 100;


        /// <summary>
        /// Default configuration for Google Sign In. 
        /// You can get a stable user ID and basic profile info back via <see cref="AN_GoogleSignInAccount.GetId"/> 
        /// after you trigger sign in from either <see cref="AN_GoogleSignInClient.SignIn"/> 
        /// If you require more information for the sign in result, please build a configuration via new 
        /// 
        /// Can be used as parametr when createing  to <see cref="AN_GoogleSignInOptions.Builder"/>
        /// </summary>
        public static int DEFAULT_SIGN_IN = 101;


        /// <summary>
        /// Builder for <see cref="AN_GoogleSignInOptions"/>.
        /// </summary>
        [System.Serializable]
        public class Builder : AN_LinkedObject {


            /// <summary>
            /// Default Builder for <see cref="AN_GoogleSignInOptions"/> which starts with clean configuration.
            /// 
            /// </summary>
            /// <param name="singInConf">The predefined Builder configuration, use <see cref="DEFAULT_GAMES_SIGN_IN"/> or <see cref="DEFAULT_SIGN_IN"/> </param>
            public Builder(int singInConf = 0) {
                m_HashCode = AN_GMS_Lib.Auth.GoogleSignInOptions_Builder_Create(singInConf);
            }

            /// <summary>
            /// Builds the <see cref="AN_GoogleSignInOptions"/> object.
            /// </summary>
            public AN_GoogleSignInOptions Build() {
                return AN_GMS_Lib.Auth.GoogleSignInOptions_Builder_Build(this);
            }

            /// <summary>
            /// Specifies that user ID is requested by your application.
            /// </summary>
            public void RequestId() {
                AN_GMS_Lib.Auth.GoogleSignInOptions_Builder_RequestId(this);
            }

            /// <summary>
            /// Specifies that email info is requested by your application.
            /// </summary>
            public void RequestEmail() {
                AN_GMS_Lib.Auth.GoogleSignInOptions_Builder_RequestEmail(this);
            }

            /// <summary>
            /// Specifies that user's profile info is requested by your application
            /// </summary>
            public void RequestProfile() {
                AN_GMS_Lib.Auth.GoogleSignInOptions_Builder_RequestProfile(this);
            }



            /// <summary>
            /// Specifies that an ID token for authenticated users is requested. 
            /// Requesting an ID token requires that the server client ID be specified.
            /// </summary>
            /// <param name="serverClientId">The client ID of the server that will verify the integrity of the token.</param>
            public void RequestIdToken(string serverClientId) {
                AN_GMS_Lib.Auth.GoogleSignInAccount_Builder_RequestIdToken(this, serverClientId);
            }

            /// <summary>
            /// Specifies that offline access is requested. 
            /// Requesting offline access requires that the server client ID be specified.
            /// 
            /// You don't need to use <see cref="RequestIdToken(string)"/> when you use this option.
            /// When your server exchanges the code for tokens, an ID token will be returned together 
            /// (as long as you either use <see cref="RequestEmail"/> or <see cref="RequestProfile"/> along with this configuration).
            /// </summary>
            /// <param name="serverClientId">The client ID of the server that will need the auth code.</param>
            /// <param name="forceCodeForRefreshToken">
            /// If true, the granted code can be exchanged for an access token and a refresh token. 
            /// The first time you retrieve a code, a refresh_token will be granted automatically. 
            /// Subsequent requests will require additional user consent. Use false by default; 
            /// only use true if your server has suffered some failure and lost the user's refresh token.
            /// </param>
            public void RequestServerAuthCode(string serverClientId, bool forceCodeForRefreshToken) {
                AN_GMS_Lib.Auth.GoogleSignInOptions_Builder_RequestServerAuthCode(this, serverClientId, forceCodeForRefreshToken);
            }

            /// <summary>
            /// Specifies OAuth 2.0 scopes your application requests. See <see cref="AN_Scopes"/> for more information.
            /// </summary>
            /// <param name="scope">An OAuth 2.0 scope requested by your app.</param>
            public void RequestScope(AN_Scope scope) {
                AN_GMS_Lib.Auth.GoogleSignInAccount_Builder_RequestScope(this, scope);
            }
            


        }

    }
}

