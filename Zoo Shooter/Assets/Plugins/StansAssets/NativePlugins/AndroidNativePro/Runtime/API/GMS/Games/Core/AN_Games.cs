using SA.Android.GMS.Games.Multiplayer;
using SA.Android.GMS.Internal;

namespace SA.Android.GMS.Games
{
    /// <summary>
    /// Main entry point for the Games APIs. 
    /// This class provides APIs and interfaces to access the Google Play game services functionality.
    /// </summary>
    public static class AN_Games 
    {
        /// <summary>
        /// Returns a new instance of <see cref="AN_PlayersClient"/>
        /// </summary>
        public static AN_PlayersClient GetPlayersClient() 
        {
            return AN_GMS_Lib.Games.GetPlayersClient();
        }


        /// <summary>
        /// Returns a new instance of <see cref="AN_GamesClient"/>
        /// </summary>
        public static AN_GamesClient GetGamesClient() 
        {
            return AN_GMS_Lib.Games.GetGamesClient();
        }

        /// <summary>
        /// Returns a new instance of <see cref="AN_AchievementsClient"/>
        /// </summary>
        public static AN_AchievementsClient GetAchievementsClient() 
        {
            return AN_GMS_Lib.Games.GetAchievementsClient();
        }


        /// <summary>
        /// Returns a new instance of <see cref="AN_LeaderboardsClient"/>
        /// </summary>
        public static AN_LeaderboardsClient GetLeaderboardsClient() 
        {
            return AN_GMS_Lib.Games.GetLeaderboardsClient();
        }


        /// <summary>
        /// Returns a new instance of <see cref="AN_SnapshotsClient"/>
        /// </summary>
        public static AN_SnapshotsClient GetSnapshotsClient() 
        {
            return AN_GMS_Lib.Games.GetSnapshotsClient();
        }
        
        /// <summary>
        /// Returns a new instance of <see cref="AN_RealTimeMultiplayerClient"/>
        /// </summary>
        public static AN_RealTimeMultiplayerClient GetRealTimeMultiplayerClient() 
        {
            return AN_GMS_Lib.Games.GetRealTimeMultiplayerClient();
        }
    }
}