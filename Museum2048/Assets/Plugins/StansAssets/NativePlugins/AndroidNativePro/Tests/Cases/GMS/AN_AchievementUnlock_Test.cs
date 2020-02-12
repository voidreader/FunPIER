
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Foundation.Tests;
using SA.Android.App;
using SA.Android.GMS.Games;

namespace SA.Android.Tests.GMS.Achievements
{
    public class AN_AchievementUnlock_Test : SA_BaseTest
    {
        private AN_Achievement m_achievement;

        public override void Test() {

            var client = AN_Games.GetAchievementsClient();
            client.Load(false, (result) => {
                if (result.IsSucceeded) {
                    foreach (var achievement in result.Achievements) {
                        if (achievement.Type == AN_Achievement.AchievementType.STANDARD && achievement.State == AN_Achievement.AchievementState.REVEALED) {
                             m_achievement = achievement;
                            break;
                        }
                    }

                    if (m_achievement != null) {
                        TestOperation();
                    } else {
                        SetResult(SA_TestResult.WithError("Wasn't able to find suitable Achievement to Unlock"));
                    }
                } else {
                    SetAPIResult(result);
                }
            });
        }




        private void TestOperation() {
            Debug.Log("UnlockImmediate: " + m_achievement.Name);
            var client = AN_Games.GetAchievementsClient();
            client.UnlockImmediate(m_achievement.AchievementId, (result) => {
                SetAPIResult(result);
            });
        }

    }
}