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
    public class AN_PackageManager_Test : SA_BaseTest
    {

        public override void Test() {
            

            //Package Info
            string packageName = UnityEngine.Application.identifier;
           
            var pm = AN_MainActivity.Instance.GetPackageManager();
            AN_PackageInfo packageInfo = pm.GetPackageInfo(packageName, 0);
            AN_Logger.Log("packageInfo.VersionName: " + packageInfo.VersionName);
            AN_Logger.Log("packageInfo.PackageName: " + packageInfo.PackageName);
            AN_Logger.Log("packageInfo.SharedUserId: " + packageInfo.SharedUserId);



            //Query Intent Activities TEST

            //Simple intent to get list of the apps that can support the send action
            AN_Intent testIntent = new AN_Intent();
            testIntent.SetAction(AN_Intent.ACTION_SEND);
            testIntent.SetType("text/plain");

            List<AN_ResolveInfo> resolveInfoList = pm.QueryIntentActivities(testIntent, 0);
            foreach (var resolveInfo in resolveInfoList) {
                AN_Logger.Log("resolveInfo.ActivityInfo.Name: " + resolveInfo.ActivityInfo.Name);
                AN_Logger.Log("resolveInfo.ActivityInfo.PackageName: " + resolveInfo.ActivityInfo.PackageName);
            }


            ///Open External App
            AN_Intent startAppIntent = pm.GetLaunchIntentForPackage("com.facebook.katana");
            if (startAppIntent == null) {
                SetResult(SA_TestResult.WithError("App with Id: com.facebook.katana not found on device"));
                return;
            }
            startAppIntent.AddCategory(AN_Intent.CATEGORY_LAUNCHER);

            /*
            AN_ProxyActivity proxy = new AN_ProxyActivity();
            bool started = proxy.StartActivityForResult(startAppIntent, (result) => {
                SetResult(TestResult.OK);
                proxy.Finish();
            });

            if(!started) {
                SetResult(TestResult.GetError("Failed to create activity"));
            }*/

            SetResult(SA_TestResult.OK);
        }
    }
}