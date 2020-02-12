
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Foundation.Tests;
using SA.Android.App;
using SA.Android.GMS.Games;
using SA.Android.GMS.Common;

namespace SA.Android.Tests.GMS.Leaderboards
{
    public abstract class AN_LeaderboarSingleBase_Test : SA_BaseTest
    {

        protected int PAGE_ITEMS_COUNT = 5;

        public override void Test() {
            var client = AN_Games.GetLeaderboardsClient();
            client.LoadLeaderboardMetadata(false, (result) => {
                if (result.IsSucceeded) {
                    if (result.Leaderboards.Count == 0) {
                        SetResult(SA_TestResult.WithError("Result is Succeeded, but Leaderboards list is empty"));
                        return;
                    }

                    TestLeaderboard(result.Leaderboards[0], client);
                } else {
                    SetAPIResult(result);
                }

            });
        }

        protected void PrintScoresResultData(AN_LinkedObjectResult<AN_LeaderboardScores> result) {

            var scores = result.Data;
            var buffer = scores.Scores;

            AN_Logger.Log("scores.Leaderboard.DisplayName: " + scores.Leaderboard.DisplayName);
            AN_Logger.Log("Loaded scores Count: " + buffer.Scores.Count);

            foreach (var score in buffer.Scores) {
                PrintScoreInfo(score);
            }
        }

        protected void PrintScoreInfo(AN_LeaderboardScore score) {
            AN_Logger.Log("score.DisplayRank: " + score.DisplayRank);
            AN_Logger.Log("score.DisplayScore: " + score.DisplayScore);
            AN_Logger.Log("score.Rank: " + score.Rank);
            AN_Logger.Log("score.RawScore: " + score.RawScore);
            AN_Logger.Log("score.ScoreHolder: " + score.ScoreHolder);
            AN_Logger.Log("score.ScoreHolderDisplayName: " + score.ScoreHolderDisplayName);
            AN_Logger.Log("score.ScoreHolderIconImageUri: " + score.ScoreHolderIconImageUri);
            AN_Logger.Log("score.ScoreHolderHiResImageUri: " + score.ScoreHolderHiResImageUri);
            AN_Logger.Log("score.ScoreTag: " + score.ScoreTag);
            AN_Logger.Log("score.TimestampMillis: " + score.TimestampMillis);
            AN_Logger.Log("------------------------------------------------");
        }

        protected abstract void TestLeaderboard(AN_Leaderboard leaderboard, AN_LeaderboardsClient client);

       
    }
}