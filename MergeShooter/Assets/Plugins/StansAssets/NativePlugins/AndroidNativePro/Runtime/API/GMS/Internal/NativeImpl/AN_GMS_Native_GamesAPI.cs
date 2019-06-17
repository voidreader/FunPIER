using System;
using System.Collections;
using System.Collections.Generic;
using SA.Foundation.Templates;
using UnityEngine;

using SA.Android.GMS.Games;
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


        //--------------------------------------
        // AN_Player
        //--------------------------------------

        const string AN_Player = JAVA_PACKAGE  + "AN_Player";


        public string Player_GetPlayerId(AN_Player player) {
            return AN_Java.Bridge.CallStatic<string>(AN_Player, "GetPlayerId", player.HashCode);
        }

        public string Player_GetDisplayName(AN_Player player) {
            return AN_Java.Bridge.CallStatic<string>(AN_Player, "GetDisplayName", player.HashCode);
        }

        public string Player_GetTitle(AN_Player player) {
            return AN_Java.Bridge.CallStatic<string>(AN_Player, "GetTitle", player.HashCode);
        }

        public bool Player_HasIconImage(AN_Player player) {
            return AN_Java.Bridge.CallStatic<bool>(AN_Player, "HasIconImage", player.HashCode);
        }

        public bool Player_HasHiResImage(AN_Player player) {
            return AN_Java.Bridge.CallStatic<bool>(AN_Player, "HasHiResImage", player.HashCode);
        }

        public string Player_GetHiResImageUri(AN_Player player) {
            return AN_Java.Bridge.CallStatic<string>(AN_Player, "GetHiResImageUri", player.HashCode);
        }

        public string Player_GetIconImageUri(AN_Player player) {
            return AN_Java.Bridge.CallStatic<string>(AN_Player, "GetIconImageUri", player.HashCode);
        }

    }
}