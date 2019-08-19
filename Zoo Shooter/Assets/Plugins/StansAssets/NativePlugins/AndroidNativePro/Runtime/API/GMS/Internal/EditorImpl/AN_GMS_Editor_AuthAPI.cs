using System;
using UnityEngine;

using SA.Foundation.Templates;
using SA.Foundation.Async;



using SA.Android.GMS.Auth;
using SA.Android.GMS.Common;
using SA.Android.Utilities;


namespace SA.Android.GMS.Internal
{

    internal class AN_GMS_Editor_AuthAPI : AN_iGMS_AuthAPI
    {

        //--------------------------------------
        // AN_GoogleApiAvailability
        //--------------------------------------
        
        const string AN_GoogleApiAvailability = "com.stansassets.gms.common.AN_GoogleApiAvailability";

        public int IsGooglePlayServicesAvailable() {
            return AN_ConnectionResult.SUCCESS;
        }

        public void MakeGooglePlayServicesAvailable(Action<SA_Result> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                callback.Invoke(new SA_Result());
            });
        }


        //--------------------------------------
        // AN_GoogleSignInAccount
        //--------------------------------------

        

        public string GoogleSignInAccount_GetId(AN_GoogleSignInAccount account) {
            return "editor_id";
        }

        public string GoogleSignInAccount_GetDisplayName(AN_GoogleSignInAccount account) {
            return "Account Display Name Editor";
        }

        public string GoogleSignInAccount_GetEmail(AN_GoogleSignInAccount account) {
            return "Account Email Editor";
        }

        public string GoogleSignInAccount_GetGivenName(AN_GoogleSignInAccount account) {
            return "Account Given Name Editor";
        }

        public string GoogleSignInAccount_GetPhotoUrl(AN_GoogleSignInAccount account) {
            return string.Empty;
        }

        public string GoogleSignInAccount_GetServerAuthCode(AN_GoogleSignInAccount account) {
            return "Account Server Auth Code Editor";
        }


        public string GoogleSignInAccount_GetIdToken(AN_GoogleSignInAccount account) {
            return "Id Token Editor";
        }

        //--------------------------------------
        // AN_GoogleSignIn
        //--------------------------------------

        static bool m_alreadySignedIn = false;
        static bool m_canDoSilentSignedIn = false;

        public AN_GoogleSignInClient GoogleSignIn_GetClient(AN_GoogleSignInOptions gso) {
            return new AN_GoogleSignInClient();
        }


        public AN_GoogleSignInAccount GoogleSignIn_GetLastSignedInAccount() {
            if (m_alreadySignedIn) {
                return new AN_GoogleSignInAccount();
            } else {
                return null;
            }
        }


        //--------------------------------------
        // AN_GoogleSignInClient
        //--------------------------------------

 
        public void GoogleSignInClient_SignIn(AN_GoogleSignInClient client, Action<AN_GoogleSignInResult> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                m_alreadySignedIn = true;
                m_canDoSilentSignedIn = true;
                callback.Invoke(new AN_GoogleSignInResult(new AN_GoogleSignInAccount()));
            });
        }

        public void GoogleSignInClient_SilentSignIn(AN_GoogleSignInClient client, Action<AN_GoogleSignInResult> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                if(m_canDoSilentSignedIn) {
                    m_alreadySignedIn = true;
                    callback.Invoke(new AN_GoogleSignInResult(new AN_GoogleSignInAccount()));
                } else {
                    var error = new SA_Error((int) AN_CommonStatusCodes.SIGN_IN_REQUIRED, "SIGN_IN_REQUIRED");
                    callback.Invoke(new AN_GoogleSignInResult(error));
                }
            });
        }

        public void GoogleSignInClient_SignOut(AN_GoogleSignInClient client, Action<SA_Result> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                m_alreadySignedIn = false;
                callback.Invoke(new SA_Result());
            });
        }

        public void GoogleSignInClient_RevokeAccess(AN_GoogleSignInClient client, Action<SA_Result> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                m_alreadySignedIn = false;
                m_canDoSilentSignedIn = false;
                callback.Invoke(new SA_Result());
            });
        }

        //--------------------------------------
        // AN_GoogleSignInOptionsBuilder
        //--------------------------------------

        public int GoogleSignInOptions_Builder_Create(int id) {
            return 0;
        }

        public AN_GoogleSignInOptions GoogleSignInOptions_Builder_Build(AN_GoogleSignInOptions.Builder builder) {
            return new AN_GoogleSignInOptions();
        }

        public void GoogleSignInOptions_Builder_RequestId(AN_GoogleSignInOptions.Builder builder) {
          
        }

        public void GoogleSignInOptions_Builder_RequestEmail(AN_GoogleSignInOptions.Builder builder) {
           
        }

        public void GoogleSignInOptions_Builder_RequestProfile(AN_GoogleSignInOptions.Builder builder) {
           
        }


        public void GoogleSignInOptions_Builder_RequestServerAuthCode(AN_GoogleSignInOptions.Builder builder, string serverClientId, bool forceCodeForRefreshToken) {
           
        }

        
        public void GoogleSignInAccount_Builder_RequestIdToken(AN_GoogleSignInOptions.Builder builder, string serverClientId) {
           
        }


        public void GoogleSignInAccount_Builder_RequestScope(AN_GoogleSignInOptions.Builder builder, AN_Scope scope) {
          
        }

    }
}