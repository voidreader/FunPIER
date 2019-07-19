using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Foundation.Tests;
using SA.Android.App;
using SA.Android.GMS.Games;

namespace SA.Android.Tests.GMS.Achievements
{
    public class AN_AchievementsUI_Test : SA_BaseTest
    {

        public override bool RequireUserInteraction { get { return true; } }

        public override void Test() {
            var client = AN_Games.GetAchievementsClient();
            client.GetAchievementsIntent((result) => {
                if (result.IsSucceeded) {
                    var intent = result.Intent;
                    AN_ProxyActivity proxy = new AN_ProxyActivity();
                    proxy.StartActivityForResult(intent, (intentResult) => {
                        proxy.Finish();
                        SetResult(SA_TestResult.OK);
                    });

                } else {
                    SetAPIResult(result);
                }
            });

        }
    }
}