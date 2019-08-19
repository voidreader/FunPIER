using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Tests;
using SA.Android.App;
using SA.Android.Content;


namespace SA.Android.Tests.Application
{
    public class AN_Settings_Test : SA_BaseTest
    {

        public override void Test() {


            var intent = new AN_Intent(Provider.AN_Settings.ACTION_SETTINGS);

            AN_MainActivity.Instance.StartActivity(intent);
            SetResult(SA_TestResult.OK);
            /*
            var m_proxyActivity = new AN_ProxyActivity();
            m_proxyActivity.StartActivityForResult(intent, (result) => {
                m_proxyActivity.Finish();

               
            });*/
        }


    }
}



