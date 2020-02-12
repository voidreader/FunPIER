using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Foundation.Tests;
using SA.Android.App;
using SA.Android.GMS.Games;

namespace SA.Android.Tests.GMS.Leaderboards
{
    public class AN_LeaderboardsLoadCurrentPlayerLeaderboardScore_Test : AN_LeaderboarSingleBase_Test
    {
        protected override void TestLeaderboard(AN_Leaderboard leaderboard, AN_LeaderboardsClient client) {
            client.LoadCurrentPlayerLeaderboardScore(leaderboard.LeaderboardId, (result) => {
                if (result.IsSucceeded) {
                    PrintScoreInfo(result.Data);
                }
                SetAPIResult(result);
            });
        }
    }
}