using UnityEngine;
using System;
using SA.Android.Utilities;
using SA.Foundation.Tests;

using SA.Android.GMS.Auth;
using SA.Android.GMS.Drive;

namespace SA.Android.Tests.GMS
{
    public class AN_GoogleSignIn_Test : SA_BaseTest
    {

        public override void Test() {

            if(AN_GoogleSignIn.GetLastSignedInAccount() != null) {
                SignInClient.SignOut(() => {
                    SignIn();
                });
            } else {
                SignIn();
            }
           
        }


        private void SignIn() {
            SilentSignIn((silentSignInResult) => {
                if(silentSignInResult.IsSucceeded) {
                    SetResult(SA_TestResult.OK);
                } else {
                    InteractiveSignIn((InteractiveSignInResult) => {
                        SetAPIResult(InteractiveSignInResult);
                    });
                }
            });
        }


        protected void SilentSignIn(Action<AN_GoogleSignInResult> result) {

            AN_Logger.Log("Let's try Silent SignIn first");
            SignInClient.SilentSignIn((signInResult) => {
                if (signInResult.IsSucceeded) {
                    PrintInfo(signInResult.Account);
                }
                result.Invoke(signInResult);
            });
        }


        protected void InteractiveSignIn(Action<AN_GoogleSignInResult> result) {

            AN_Logger.Log("Starting the Interactive Sign in flow");
            SignInClient.SignIn((signInResult) => {
                AN_Logger.Log("Sign In StatusCode: " + signInResult.StatusCode);
                if (signInResult.IsSucceeded) {
                    PrintInfo(signInResult.Account);
                }
                result.Invoke(signInResult);
            });

        }


        protected AN_GoogleSignInClient SignInClient {
            get {
                AN_GoogleSignInOptions.Builder builder = new AN_GoogleSignInOptions.Builder(AN_GoogleSignInOptions.DEFAULT_GAMES_SIGN_IN);
                builder.RequestId();
                builder.RequestEmail();
                builder.RequestProfile();


                // Stan win work
                //string serverId = "721571874513-8nptrjeg9oru616rno0124r4tr79vms8.apps.googleusercontent.com";

                // Anroid Native pro keystore
               // string serverId = "721571874513-n2kcsfkn9gfb4g758sfauap7g2gti8bg.apps.googleusercontent.com"; 

                builder.RequestScope(AN_Drive.SCOPE_APPFOLDER);
               // builder.RequestServerAuthCode(serverId, false);
                AN_GoogleSignInOptions gso =  builder.Build();
                return AN_GoogleSignIn.GetClient(gso); 
            }
        }


        


        private void PrintInfo(AN_GoogleSignInAccount account) {
            AN_Logger.Log("account.GetDisplayName: " + account.GetId());
            AN_Logger.Log("account.GetDisplayName: " + account.GetDisplayName());
            AN_Logger.Log("account.GetGivenName: " + account.GetGivenName());
            AN_Logger.Log("account.GetEmail: " + account.GetEmail());
            AN_Logger.Log("account.GetServerAuthCode: " + account.GetServerAuthCode());
            SetResult(SA_TestResult.OK);
        }
    }
}