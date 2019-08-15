using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Android.GMS.Games;
using SA.Android.GMS.Common;

namespace SA.Android.GMS.Internal
{
    internal interface AN_iGMS_LeaderboardsAPI
    {


        //--------------------------------------
        // AN_LeaderboardsClient
        //--------------------------------------

        void GetAllLeaderboardsIntent(AN_LeaderboardsClient client, Action<AN_IntentResult> callback);
        void GetLeaderboardIntent(AN_LeaderboardsClient client, string leaderboardId, int timeSpan, int collection, Action<AN_IntentResult> callback);
        void LoadCurrentPlayerLeaderboardScore(AN_LeaderboardsClient client, string leaderboardId, int span, int leaderboardCollection, Action<AN_LinkedObjectResult<AN_LeaderboardScore>> callback);
        void LoadLeaderboardMetadata(AN_LeaderboardsClient client, bool forceReload, Action<AN_LeaderboardsLoadResult> callback);
        void LoadLeaderboardMetadata(AN_LeaderboardsClient client, string leaderboardId, bool forceReload, Action<AN_LeaderboardLoadResult> callback);

        void LoadMoreScores(AN_LeaderboardsClient client, AN_LeaderboardScoreBuffer buffer, int maxResults, int pageDirection, Action<AN_LinkedObjectResult<AN_LeaderboardScores>> callback);
        void LoadPlayerCenteredScores(AN_LeaderboardsClient client, string leaderboardId, int span, int leaderboardCollection, int maxResults, bool forceReload, Action<AN_LinkedObjectResult<AN_LeaderboardScores>> callback);
        void LoadTopScores(AN_LeaderboardsClient client, string leaderboardId, int span, int leaderboardCollection, int maxResults, bool forceReload, Action<AN_LinkedObjectResult<AN_LeaderboardScores>> callback);

        void SubmitScore(AN_LeaderboardsClient client, string leaderboardId, long score, string scoreTag);
        void SubmitScoreImmediate(AN_LeaderboardsClient client, string leaderboardId, long score, string scoreTag, Action<AN_LinkedObjectResult<AN_ScoreSubmissionData>> callback);


        //--------------------------------------
        // AN_LeaderboardScore
        //--------------------------------------

        string LeaderboardScore_GetDisplayRank(AN_LeaderboardScore score);
        AN_Player LeaderboardScore_GetScoreHolder(AN_LeaderboardScore score);
        string LeaderboardScore_GetDisplayScore(AN_LeaderboardScore score);
        long LeaderboardScore_GetRawScore(AN_LeaderboardScore score);
        long LeaderboardScore_GetRank(AN_LeaderboardScore score);
        string LeaderboardScore_GetScoreHolderDisplayName(AN_LeaderboardScore score);
        string LeaderboardScore_GetScoreHolderIconImageUri(AN_LeaderboardScore score);
        string LeaderboardScore_GetScoreHolderHiResImageUri(AN_LeaderboardScore score);

        string LeaderboardScore_GetScoreTag(AN_LeaderboardScore score);
        long LeaderboardScore_GetTimestampMillis(AN_LeaderboardScore score);



        //--------------------------------------
        // AN_LeaderboardScoreBuffer
        //--------------------------------------

        AN_LeaderboardScoreBuffer.ScoresList LeaderboardScoreBuffer_GetScores(AN_LeaderboardScoreBuffer buffer);


        //--------------------------------------
        // AN_LeaderboardScores
        //--------------------------------------

        AN_Leaderboard LeaderboardScores_GetLeaderboard(AN_LeaderboardScores scores);
        AN_LeaderboardScoreBuffer LeaderboardScores_GetScores(AN_LeaderboardScores scores);



        //--------------------------------------
        // AN_ScoreSubmissionData
        //--------------------------------------


        string ScoreSubmissionData_GetLeaderboardId(AN_ScoreSubmissionData data);
        string ScoreSubmissionData_GetPlayerId(AN_ScoreSubmissionData data);
        AN_ScoreSubmissionData.Result ScoreSubmissionData_GetScoreResult(AN_ScoreSubmissionData data, int timeSpan);



        //--------------------------------------
        // AN_ScoreSubmissionDataResult
        //--------------------------------------

        string ScoreSubmissionDataResult_GetFormattedScore(AN_ScoreSubmissionData.Result data);
        bool ScoreSubmissionDataResult_GetNewBest(AN_ScoreSubmissionData.Result data);
        long ScoreSubmissionDataResult_GetRawScore(AN_ScoreSubmissionData.Result data);
        string ScoreSubmissionDataResult_GetScoreTag(AN_ScoreSubmissionData.Result data);

    }
}
