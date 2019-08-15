using UnityEngine;
using System;
using SA.Android.Utilities;
using SA.Foundation.Tests;

using SA.Android.GMS.Auth;
using SA.Android.GMS.Drive;

namespace SA.Android.Tests.GMS
{
    public class AN_GoogleSignOut_Test  : AN_GoogleSignIn_Test
    {

        public override void Test() {
            SignInClient.SignOut(() => {

                //Now we need to make sure we can Sing in siletly
                SilentSignIn((result) => {
                    SetAPIResult(result);
                });
            });
        }

    }
}