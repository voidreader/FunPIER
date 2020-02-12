
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Foundation.Tests;
using SA.Android.App;
using SA.Android.GMS.Games;

namespace SA.Android.Tests.GMS.Leaderboards
{
    public class AN_LeaderboardsSubmit_Test : AN_LeaderboarSingleBase_Test
    {


        protected override void TestLeaderboard(AN_Leaderboard leaderboard, AN_LeaderboardsClient client) {
            foreach (var variant in leaderboard.Variants) {
                if (variant.Collection == AN_Leaderboard.Collection.Public && variant.TimeSpan == AN_Leaderboard.TimeSpan.AllTime) {
                    long score = variant.RawPlayerScore;
                    score++;

                    client.SubmitScoreImmediate(leaderboard.LeaderboardId, score, "Test_tag", (submitResult) => {
                        if (submitResult.IsSucceeded) {
                            var scoreSubmissionData = submitResult.Data;
                            AN_Logger.Log("SubmitScoreImmediate completed");
                            AN_Logger.Log("scoreSubmissionData.PlayerId: " + scoreSubmissionData.PlayerId);
                            AN_Logger.Log("scoreSubmissionData.LeaderboardId: " + scoreSubmissionData.LeaderboardId);

                            foreach (AN_Leaderboard.TimeSpan span in (AN_Leaderboard.TimeSpan[])System.Enum.GetValues(typeof(AN_Leaderboard.TimeSpan))) {
                                var scoreSubmissionResult = scoreSubmissionData.GetScoreResult(span);
                                AN_Logger.Log("scoreSubmissionData.FormattedScore: " + scoreSubmissionResult.FormattedScore);
                                AN_Logger.Log("scoreSubmissionData.NewBest: " + scoreSubmissionResult.NewBest);
                                AN_Logger.Log("scoreSubmissionData.RawScore: " + scoreSubmissionResult.RawScore);
                                AN_Logger.Log("scoreSubmissionData.ScoreTag: " + scoreSubmissionResult.ScoreTag);
                            }
                        }

                        SetAPIResult(submitResult);

                    });
                    return;
                }

            }

        }

    }
}