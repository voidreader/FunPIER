using UnityEngine;
using System.Collections.Generic;
using SA.Android.Utilities;
using SA.Android.Vending.Licensing;
using SA.Foundation.Tests;
using SA.Foundation.Async;

using SA.Android.App;
using SA.Android.Content;
using SA.Android.Content.Pm;


namespace SA.Android.Tests.Application
{
    public class AN_ProgressDialog_Test : SA_BaseTest
    {

        public override void Test() {

            AN_Preloader.LockScreen("Lock Test");
         
            SA_Coroutine.WaitForSeconds(3f, () => {
                AN_Preloader.UnlockScreen();
            });

        }
    }
}