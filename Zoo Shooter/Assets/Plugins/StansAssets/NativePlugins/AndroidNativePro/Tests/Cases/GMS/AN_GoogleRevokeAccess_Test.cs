using UnityEngine;
using System;
using SA.Android.Utilities;
using SA.Foundation.Tests;

using SA.Android.App;
using SA.Android.App.View;
using SA.Android.GMS.Games;


namespace SA.Android.Tests.GMS
{
    public class AN_GoogleRevokeAccess_Test : AN_GoogleSignIn_Test
    {

        public override void Test() {
            SignInClient.RevokeAccess(() => {

                //Now we need to make sure we can't Sing in siletly
                SilentSignIn((result) => {
                    if(result.IsSucceeded) {
                        SetResult(SA_TestResult.WithError("User was able to do Silent SignIn after RevokeAccess"));
                    } else {
                        //InteractiveSignIn should work
                        InteractiveSignIn((InteractiveSignInResult) => {
                            SetAPIResult(InteractiveSignInResult);

                            if(InteractiveSignInResult.IsSucceeded) {
                                var gamesClient = AN_Games.GetGamesClient();
                                gamesClient.SetViewForPopups(AN_MainActivity.Instance);

                                //optionally
                                gamesClient.SetGravityForPopups(AN_Gravity.TOP | AN_Gravity.CENTER_HORIZONTAL);
                            }
                        });
                    }
                });
            });
        }

    }
}