using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.GMS.Games;

namespace SA.Android.GMS.Internal
{
    internal interface AN_iGMS_GamesAPI
    {

        AN_GamesClient GetGamesClient();
        AN_PlayersClient GetPlayersClient();
        AN_AchievementsClient GetAchievementsClient();
        AN_LeaderboardsClient GetLeaderboardsClient();
        AN_SnapshotsClient GetSnapshotsClient();

        string Player_GetPlayerId(AN_Player player);
        string Player_GetDisplayName(AN_Player player);
        string Player_GetTitle(AN_Player player);
        bool Player_HasIconImage(AN_Player player);
        bool Player_HasHiResImage(AN_Player player);
        string Player_GetHiResImageUri(AN_Player player);
        string Player_GetIconImageUri(AN_Player player);
    }
}

