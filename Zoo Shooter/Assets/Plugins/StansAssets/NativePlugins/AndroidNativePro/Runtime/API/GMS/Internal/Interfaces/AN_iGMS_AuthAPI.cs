using System;
using System.Collections.Generic;
using UnityEngine;


using SA.Android.GMS.Auth;
using SA.Android.GMS.Common;

using SA.Foundation.Templates;


namespace SA.Android.GMS.Internal
{
    internal interface AN_iGMS_AuthAPI
    {

        //--------------------------------------
        // AN_GoogleApiAvailability
        //--------------------------------------

        int IsGooglePlayServicesAvailable();
        void MakeGooglePlayServicesAvailable(Action<SA_Result> callback);


        //--------------------------------------
        // AN_GoogleSignInAccount
        //--------------------------------------

        string GoogleSignInAccount_GetId(AN_GoogleSignInAccount account);
        string GoogleSignInAccount_GetDisplayName(AN_GoogleSignInAccount account);
        string GoogleSignInAccount_GetGivenName(AN_GoogleSignInAccount account);
        string GoogleSignInAccount_GetEmail(AN_GoogleSignInAccount account);
        string GoogleSignInAccount_GetPhotoUrl(AN_GoogleSignInAccount account);
        string GoogleSignInAccount_GetServerAuthCode(AN_GoogleSignInAccount account);
        string GoogleSignInAccount_GetIdToken(AN_GoogleSignInAccount account);


        //--------------------------------------
        // AN_GoogleSignIn
        //--------------------------------------

        AN_GoogleSignInClient GoogleSignIn_GetClient(AN_GoogleSignInOptions gso);
        AN_GoogleSignInAccount GoogleSignIn_GetLastSignedInAccount();


        //--------------------------------------
        // AN_GoogleSignInClient
        //--------------------------------------

        void GoogleSignInClient_SignIn(AN_GoogleSignInClient client, Action<AN_GoogleSignInResult> callback);
        void GoogleSignInClient_SilentSignIn(AN_GoogleSignInClient client, Action<AN_GoogleSignInResult> callback);
        void GoogleSignInClient_SignOut(AN_GoogleSignInClient client, Action<SA_Result> callback);
        void GoogleSignInClient_RevokeAccess(AN_GoogleSignInClient client, Action<SA_Result> callback);



        //--------------------------------------
        // AN_GoogleSignInOptionsBuilder
        //--------------------------------------

        int GoogleSignInOptions_Builder_Create(int id);
        AN_GoogleSignInOptions GoogleSignInOptions_Builder_Build(AN_GoogleSignInOptions.Builder builder);
        void GoogleSignInOptions_Builder_RequestId(AN_GoogleSignInOptions.Builder builder);
        void GoogleSignInOptions_Builder_RequestEmail(AN_GoogleSignInOptions.Builder builder);
        void GoogleSignInOptions_Builder_RequestProfile(AN_GoogleSignInOptions.Builder builder);
        void GoogleSignInOptions_Builder_RequestServerAuthCode(AN_GoogleSignInOptions.Builder builder, string serverClientId, bool forceCodeForRefreshToken);
        void GoogleSignInAccount_Builder_RequestIdToken(AN_GoogleSignInOptions.Builder builder, string serverClientId);
        void GoogleSignInAccount_Builder_RequestScope(AN_GoogleSignInOptions.Builder builder, AN_Scope scope);
    }
}