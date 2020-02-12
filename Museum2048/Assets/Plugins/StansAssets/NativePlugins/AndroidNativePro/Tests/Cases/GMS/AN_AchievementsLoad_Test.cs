using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Foundation.Tests;
using SA.Android.App;
using SA.Android.GMS.Games;

namespace SA.Android.Tests.GMS.Achievements
{
    public class AN_AchievementsLoad_Test : SA_BaseTest
    {

        public override void Test() {

            var client = AN_Games.GetAchievementsClient();
            client.Load(false, (result) => {

                if (result.IsSucceeded) {
                    AN_Logger.Log("Load Achievements Succeeded, count: " + result.Achievements.Count);
                    foreach (var achievement in result.Achievements) {
                        AN_Logger.Log("------------------------------------------------");
                        AN_Logger.Log("achievement.AchievementId: " + achievement.AchievementId);
                        AN_Logger.Log("achievement.Description: " + achievement.Description);
                        AN_Logger.Log("achievement.Name: " + achievement.Name);
                        AN_Logger.Log("achievement.UnlockedImageUri: " + achievement.UnlockedImageUri);
                        AN_Logger.Log("achievement.CurrentSteps: " + achievement.CurrentSteps);
                        AN_Logger.Log("achievement.TotalSteps: " + achievement.TotalSteps);
                        AN_Logger.Log("achievement.Type: " + achievement.Type);
                        AN_Logger.Log("achievement.Sate: " + achievement.State);
                    }
                    AN_Logger.Log("------------------------------------------------");
                    SetResult(SA_TestResult.OK);
                } else {
                    SetAPIResult(result);
                }
            });
 
        }
    }
}