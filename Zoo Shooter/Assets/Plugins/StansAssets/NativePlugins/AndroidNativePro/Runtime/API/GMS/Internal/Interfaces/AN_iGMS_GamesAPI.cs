using SA.Android.GMS.Games;
using SA.Android.GMS.Games.Multiplayer;

namespace SA.Android.GMS.Internal
{
    internal interface AN_iGMS_GamesAPI
    {

        AN_GamesClient GetGamesClient();
        AN_PlayersClient GetPlayersClient();
        AN_AchievementsClient GetAchievementsClient();
        AN_LeaderboardsClient GetLeaderboardsClient();
        AN_SnapshotsClient GetSnapshotsClient();
        AN_RealTimeMultiplayerClient GetRealTimeMultiplayerClient();
    }
}

