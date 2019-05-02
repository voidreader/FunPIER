using System;
using System.Collections;
using System.Collections.Generic;
using SA.Foundation.Templates;
using UnityEngine;

using SA.Android.GMS.Games;
using SA.Android.Utilities;

namespace SA.Android.GMS.Internal
{

    internal class AN_GMS_Editor_GamesAPI : AN_iGMS_GamesAPI
    {



        //--------------------------------------
        // AN_Games
        //--------------------------------------


        public AN_GamesClient GetGamesClient() { 
            return new AN_GamesClient();
        }

        public AN_PlayersClient GetPlayersClient() {
            return new AN_PlayersClient();
        }

        public AN_AchievementsClient GetAchievementsClient() {
            return new AN_AchievementsClient();
        }


        public AN_LeaderboardsClient GetLeaderboardsClient() {
            return new AN_LeaderboardsClient();
        }

        public AN_SnapshotsClient GetSnapshotsClient() {
            return new AN_SnapshotsClient();
        }


        //--------------------------------------
        // AN_Player
        //--------------------------------------


        public string Player_GetPlayerId(AN_Player player) {
            return "editor_id";
        }

        public string Player_GetDisplayName(AN_Player player) {
            return "Player Display Name Editor";
        }

        public string Player_GetTitle(AN_Player player) {
            return "Player Title Editor";
        }

        public bool Player_HasIconImage(AN_Player player) {
            return false;
        }

        public bool Player_HasHiResImage(AN_Player player) {
            return false;
        }

        public string Player_GetHiResImageUri(AN_Player player) {
            return string.Empty;
        }

        public string Player_GetIconImageUri(AN_Player player) {
            return string.Empty;
        }

    }
}