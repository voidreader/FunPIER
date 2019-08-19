using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Foundation.Tests;
using SA.Android.App;
using SA.Android.GMS.Games;

namespace SA.Android.Tests.GMS.Leaderboards
{
    public class AN_LeaderboardsLoadTopScores_Test : AN_LeaderboarSingleBase_Test
    {

        protected override void TestLeaderboard(AN_Leaderboard leaderboard, AN_LeaderboardsClient client) {
            client.LoadTopScores(leaderboard.LeaderboardId, PAGE_ITEMS_COUNT, (result) => {
                if (result.IsSucceeded) {
                    PrintScoresResultData(result);
                    LoadMore(client, result.Data.Scores);
                } else {
                    SetAPIResult(result);
                }
            });
        }
        

        private void LoadMore(AN_LeaderboardsClient client, AN_LeaderboardScoreBuffer buffer) {

     
            client.LoadMoreScores(buffer, PAGE_ITEMS_COUNT, AN_PageDirection.Next, (result) => {
                if (result.IsSucceeded) {
                    PrintScoresResultData(result);
                }

                SetAPIResult(result);
            });
        }




    }
}