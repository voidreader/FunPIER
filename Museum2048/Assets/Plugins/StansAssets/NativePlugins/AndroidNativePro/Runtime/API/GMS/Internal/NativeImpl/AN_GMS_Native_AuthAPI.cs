using System;
using System.Collections;
using System.Collections.Generic;
using SA.Foundation.Templates;
using UnityEngine;

using SA.Android.GMS.Auth;
using SA.Android.GMS.Common;
using SA.Android.Utilities;

namespace SA.Android.GMS.Internal
{

    internal class AN_GMS_Native_AuthAPI : AN_iGMS_AuthAPI
    {

        //--------------------------------------
        // AN_GoogleApiAvailability
        //--------------------------------------
        
        const string AN_GoogleApiAvailability = "com.stansassets.gms.common.AN_GoogleApiAvailability";

        public int IsGooglePlayServicesAvailable() {
            return AN_Java.Bridge.CallStatic<int>(AN_GoogleApiAvailability, "IsGooglePlayServicesAvailable");
        }

        public void MakeGooglePlayServicesAvailable(Action<SA_Result> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_GoogleApiAvailability, "MakeGooglePlayServicesAvailable", callback);
        }


        //--------------------------------------
        // AN_GoogleSignInAccount
        //--------------------------------------

        const string AN_GoogleSignInAccount = "com.stansassets.gms.auth.api.signin.AN_GoogleSignInAccount";

        public string GoogleSignInAccount_GetId(AN_GoogleSignInAccount account) {
            return AN_Java.Bridge.CallStatic<string>(AN_GoogleSignInAccount, "GetId", account.HashCode);
        }

        public string GoogleSignInAccount_GetDisplayName(AN_GoogleSignInAccount account) {
            return AN_Java.Bridge.CallStatic<string>(AN_GoogleSignInAccount, "GetDisplayName", account.HashCode);
        }

        public string GoogleSignInAccount_GetEmail(AN_GoogleSignInAccount account) {
            return AN_Java.Bridge.CallStatic<string>(AN_GoogleSignInAccount, "GetEmail", account.HashCode);
        }

        public string GoogleSignInAccount_GetGivenName(AN_GoogleSignInAccount account) {
            return AN_Java.Bridge.CallStatic<string>(AN_GoogleSignInAccount, "GetGivenName", account.HashCode);
        }

        public string GoogleSignInAccount_GetPhotoUrl(AN_GoogleSignInAccount account) {
            return AN_Java.Bridge.CallStatic<string>(AN_GoogleSignInAccount, "GetPhotoUrl", account.HashCode);
        }

        public string GoogleSignInAccount_GetServerAuthCode(AN_GoogleSignInAccount account) {
            return AN_Java.Bridge.CallStatic<string>(AN_GoogleSignInAccount, "GetServerAuthCode", account.HashCode);
        }

        public string GoogleSignInAccount_GetIdToken(AN_GoogleSignInAccount account) {
            return AN_Java.Bridge.CallStatic<string>(AN_GoogleSignInAccount, "GetIdToken", account.HashCode);
        }

        //--------------------------------------
        // AN_GoogleSignIn
        //--------------------------------------

        const string AN_GoogleSignIn = "com.stansassets.gms.auth.api.signin.AN_GoogleSignIn";

        public AN_GoogleSignInClient GoogleSignIn_GetClient(AN_GoogleSignInOptions gso) {
            string json  = AN_Java.Bridge.CallStatic<string>(AN_GoogleSignIn, "GetClient", gso.HashCode);
            return JsonUtility.FromJson<AN_GoogleSignInClient>(json);
        }


        public AN_GoogleSignInAccount GoogleSignIn_GetLastSignedInAccount() {

            string json = AN_Java.Bridge.CallStatic<string>(AN_GoogleSignIn, "GetLastSignedInAccount");
            if(json != null) {
                return JsonUtility.FromJson<AN_GoogleSignInAccount>(json);
            } else {
                return null;
            }  
        }


        //--------------------------------------
        // AN_GoogleSignInClient
        //--------------------------------------

        const string AN_GoogleSignInClient = "com.stansassets.gms.auth.api.signin.AN_GoogleSignInClient";


        public void GoogleSignInClient_SignIn(AN_GoogleSignInClient client, Action<AN_GoogleSignInResult> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_GoogleSignInClient, "SignIn", callback, client.HashCode);
        }

        public void GoogleSignInClient_SilentSignIn(AN_GoogleSignInClient client, Action<AN_GoogleSignInResult> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_GoogleSignInClient, "SilentSignIn", callback, client.HashCode);
        }

        public void GoogleSignInClient_SignOut(AN_GoogleSignInClient client, Action<SA_Result> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_GoogleSignInClient, "SignOut", callback, client.HashCode);
        }

        public void GoogleSignInClient_RevokeAccess(AN_GoogleSignInClient client, Action<SA_Result> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_GoogleSignInClient, "RevokeAccess", callback, client.HashCode);
        }

        //--------------------------------------
        // AN_GoogleSignInOptionsBuilder
        //--------------------------------------

        const string AN_GoogleSignInOptionsBuilder = "com.stansassets.gms.auth.api.signin.AN_GoogleSignInOptionsBuilder";

        public int GoogleSignInOptions_Builder_Create(int id) {
            return  AN_Java.Bridge.CallStatic<int>(AN_GoogleSignInOptionsBuilder, "Create", id);
        }

        public AN_GoogleSignInOptions GoogleSignInOptions_Builder_Build(AN_GoogleSignInOptions.Builder builder) {

            string json = AN_Java.Bridge.CallStatic<string>(AN_GoogleSignInOptionsBuilder, "Build", builder.HashCode);
            return JsonUtility.FromJson<AN_GoogleSignInOptions>(json);

        }

        public void GoogleSignInOptions_Builder_RequestId(AN_GoogleSignInOptions.Builder builder) {
            AN_Java.Bridge.CallStatic(AN_GoogleSignInOptionsBuilder, "RequestId", builder.HashCode);
        }

        public void GoogleSignInOptions_Builder_RequestEmail(AN_GoogleSignInOptions.Builder builder) {
            AN_Java.Bridge.CallStatic(AN_GoogleSignInOptionsBuilder, "RequestEmail", builder.HashCode);
        }

        public void GoogleSignInOptions_Builder_RequestProfile(AN_GoogleSignInOptions.Builder builder) {
            AN_Java.Bridge.CallStatic(AN_GoogleSignInOptionsBuilder, "RequestProfile", builder.HashCode);
        }


        public void GoogleSignInOptions_Builder_RequestServerAuthCode(AN_GoogleSignInOptions.Builder builder, string serverClientId, bool forceCodeForRefreshToken) {
            AN_Java.Bridge.CallStatic(AN_GoogleSignInOptionsBuilder, "RequestServerAuthCode", builder.HashCode, serverClientId, forceCodeForRefreshToken);
        }

        
        public void GoogleSignInAccount_Builder_RequestIdToken(AN_GoogleSignInOptions.Builder builder, string serverClientId) {
            AN_Java.Bridge.CallStatic(AN_GoogleSignInOptionsBuilder, "RequestIdToken", builder.HashCode, serverClientId);
        }


        public void GoogleSignInAccount_Builder_RequestScope(AN_GoogleSignInOptions.Builder builder, AN_Scope scope) {
            AN_Java.Bridge.CallStatic(AN_GoogleSignInOptionsBuilder, "RequestScope", builder.HashCode, scope);
        }

    }
}