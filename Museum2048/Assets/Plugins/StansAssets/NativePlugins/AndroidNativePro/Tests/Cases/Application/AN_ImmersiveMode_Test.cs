using UnityEngine;
using System;
using SA.Android.Utilities;
using SA.Android.Vending.Licensing;
using SA.Foundation.Tests;
using SA.Foundation.Async;

using SA.Android.App.View;


namespace SA.Android.Tests.Application
{
    public class AN_ImmersiveMode_Test : SA_BaseTest
    {

        public override void Test() {

            AN_ImmersiveMode.Enable();
            SetResult(SA_TestResult.OK);
        }


    }
}