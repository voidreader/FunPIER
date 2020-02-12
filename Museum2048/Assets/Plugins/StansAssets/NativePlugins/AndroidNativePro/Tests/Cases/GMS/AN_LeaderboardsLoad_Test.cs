
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Foundation.Tests;
using SA.Android.App;
using SA.Android.GMS.Games;

namespace SA.Android.Tests.GMS.Leaderboards
{
    public class AN_LeaderboardsLoad_Test : SA_BaseTest
    {

        public override void Test() {
            var leaderboards = AN_Games.GetLeaderboardsClient();
            leaderboards.LoadLeaderboardMetadata(false, (result) => {
                if (result.IsSucceeded) {
                    if(result.Leaderboards.Count == 0) {
                        SetResult(SA_TestResult.WithError("Request returned with Succeeded, but Leaderboards list is empty"));
                        return;
                    }

                    AN_Logger.Log("Load Leaderboards Metadata Succeeded, count: " + result.Leaderboards.Count);
                    PrintLeaderboardsInfo(result.Leaderboards);
                }

                SetAPIResult(result);
            });
        }

        private void PrintLeaderboardsInfo(List<AN_Leaderboard> leaderboards) {
            foreach (var leaderboard in leaderboards) {
                AN_Logger.Log("------------------------------------------------");
                AN_Logger.Log("leaderboard.LeaderboardId: " + leaderboard.LeaderboardId);
                AN_Logger.Log("leaderboard.Description: " + leaderboard.DisplayName);
                AN_Logger.Log("leaderboard.Name: " + leaderboard.IconImageUri);
                AN_Logger.Log("leaderboard.UnlockedImageUri: " + leaderboard.LeaderboardScoreOrder);

                AN_Logger.Log("leaderboard.Variants.Count: " + leaderboard.Variants.Count);

                foreach (var variant in leaderboard.Variants) {
                    AN_Logger.Log("***************************");
                    AN_Logger.Log("variant.Collection: " + variant.Collection);
                    AN_Logger.Log("variant.DisplayPlayerRank: " + variant.DisplayPlayerRank);
                    AN_Logger.Log("variant.DisplayPlayerScore: " + variant.DisplayPlayerScore);
                    AN_Logger.Log("variant.NumScores: " + variant.NumScores);
                    AN_Logger.Log("variant.PlayerRank: " + variant.PlayerRank);
                    AN_Logger.Log("variant.PlayerScoreTag: " + variant.PlayerScoreTag);
                    AN_Logger.Log("variant.RawPlayerScore: " + variant.RawPlayerScore);
                    AN_Logger.Log("variant.TimeSpan: " + variant.TimeSpan);
                    AN_Logger.Log("variant.HasPlayerInfo: " + variant.HasPlayerInfo);

                }
            }
            AN_Logger.Log("------------------------------------------------");
        }
    }
}