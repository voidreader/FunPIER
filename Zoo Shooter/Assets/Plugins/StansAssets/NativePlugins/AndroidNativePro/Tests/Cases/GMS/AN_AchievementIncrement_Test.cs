
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Foundation.Tests;
using SA.Android.App;
using SA.Android.GMS.Games;

namespace SA.Android.Tests.GMS.Achievements
{
    public class AN_AchievementIncrement_Test : SA_BaseTest
    {
        private AN_Achievement m_achievement;

        public override void Test() {

            var client = AN_Games.GetAchievementsClient();
            client.Load(false, (result) => {
                if (result.IsSucceeded) {
                    foreach (var achievement in result.Achievements) {
                        if(achievement.Type == AN_Achievement.AchievementType.INCREMENTAL) {
                            if(achievement.TotalSteps > achievement.CurrentSteps) {
                                m_achievement = achievement;
                            }
                        }
                    }

                    if (m_achievement != null) {
                        TestIncrement();
                    } else {
                        SetResult(SA_TestResult.WithError("Wasn't able to find suitable Achievement to Incremental"));
                    }
                } else {
                    SetAPIResult(result);
                }
            });
        }


        private void TestIncrement() {
            var client = AN_Games.GetAchievementsClient();
            Debug.Log("Incrementing: " + m_achievement.Name);
            client.IncrementImmediate(m_achievement.AchievementId, 1, (result) => {
                SetAPIResult(result);
            });
        }
    }
}