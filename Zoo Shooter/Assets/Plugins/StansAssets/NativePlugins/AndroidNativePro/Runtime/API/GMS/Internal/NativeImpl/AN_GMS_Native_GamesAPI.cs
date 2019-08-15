using System;
using System.Collections;
using System.Collections.Generic;
using SA.Foundation.Templates;
using UnityEngine;

using SA.Android.GMS.Games;
using SA.Android.GMS.Games.Multiplayer;
using SA.Android.Utilities;

namespace SA.Android.GMS.Internal
{

    internal class AN_GMS_Native_GamesAPI : AN_iGMS_GamesAPI
    {
        const string JAVA_PACKAGE = "com.stansassets.gms.games.";


        //--------------------------------------
        // AN_Games
        //--------------------------------------

        const string AN_Games = JAVA_PACKAGE + "AN_Games";

        public AN_GamesClient GetGamesClient() {
            var json = AN_Java.Bridge.CallStatic<string>(AN_Games, "GetGamesClient");
            return JsonUtility.FromJson<AN_GamesClient>(json);
        }

        public AN_PlayersClient GetPlayersClient() {
            var json = AN_Java.Bridge.CallStatic<string>(AN_Games, "GetPlayersClient");
            return JsonUtility.FromJson<AN_PlayersClient>(json);
        }

        public AN_AchievementsClient GetAchievementsClient() {
            var json = AN_Java.Bridge.CallStatic<string>(AN_Games, "GetAchievementsClient");
            return JsonUtility.FromJson<AN_AchievementsClient>(json);
        }


        public AN_LeaderboardsClient GetLeaderboardsClient() {
            var json = AN_Java.Bridge.CallStatic<string>(AN_Games, "GetLeaderboardsClient");
            return JsonUtility.FromJson<AN_LeaderboardsClient>(json);
        }

        public AN_SnapshotsClient GetSnapshotsClient() {
            var json = AN_Java.Bridge.CallStatic<string>(AN_Games, "GetSnapshotsClient");
            return JsonUtility.FromJson<AN_SnapshotsClient>(json);
        }

        public AN_RealTimeMultiplayerClient GetRealTimeMultiplayerClient()
        {
            var json = AN_Java.Bridge.CallStatic<string>(AN_Games, "GetRealTimeMultiplayerClient");
            return JsonUtility.FromJson<AN_RealTimeMultiplayerClient>(json);
        }
    }
}