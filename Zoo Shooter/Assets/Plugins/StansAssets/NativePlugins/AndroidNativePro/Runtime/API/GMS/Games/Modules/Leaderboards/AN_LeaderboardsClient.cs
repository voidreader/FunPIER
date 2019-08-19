using System;
using SA.Android.Utilities;
using SA.Android.GMS.Internal;
using SA.Android.GMS.Common;

namespace SA.Android.GMS.Games
{
    /// <summary>
    /// A client to interact with leaderboards functionality.
    /// https://developers.google.com/android/reference/com/google/android/gms/games/LeaderboardsClient
    /// </summary>
    [Serializable]
    public class AN_LeaderboardsClient : AN_LinkedObject
    {
        /// <summary>
        /// Asynchronously loads an Intent to show the list of leaderboards for a game. 
        /// Note that this must be invoked with <see cref="App.AN_Activity.StartActivityForResult(Content.AN_Intent, Action{App.AN_ActivityResult})"/> 
        /// so that the identity of the calling package can be established.
        /// 
        /// Required Scopes: SCOPE_GAMES_LITE
        /// </summary>
        /// <param name="callback"></param>
        public void GetAllLeaderboardsIntent(Action<AN_IntentResult> callback) 
        {
            AN_GMS_Lib.Leaderboards.GetAllLeaderboardsIntent(this, callback);

        }

        /// <summary>
        /// Asynchronously loads an Intent to show a leaderboard for a game specified by a leaderboardId. 
        /// Note that the Intent returned from the Task must be invoked with <see cref="App.AN_Activity.StartActivityForResult(Content.AN_Intent, Action{App.AN_ActivityResult})"/>, 
        /// so that the identity of the calling package can be established.
        /// </summary>
        /// <param name="leaderboardId">The ID of the leaderboard to view.</param>
        /// <param name="callback">Request async callback</param>
        public void GetLeaderboardIntent(string leaderboardId, Action<AN_IntentResult> callback) {
            GetLeaderboardIntent(leaderboardId, AN_Leaderboard.TimeSpan.AllTime, AN_Leaderboard.Collection.Public, callback);
        }

        /// <summary>
        /// Asynchronously loads an Intent to show a leaderboard for a game specified by a leaderboardId. 
        /// Note that the Intent returned from the Task must be invoked with <see cref="App.AN_Activity.StartActivityForResult(Content.AN_Intent, Action{App.AN_ActivityResult})"/>, 
        /// so that the identity of the calling package can be established.
        /// </summary>
        /// <param name="leaderboardId">The ID of the leaderboard to view.</param>
        /// <param name="span">Time span to retrieve data for. </param>
        /// <param name="callback">Request async callback</param>
        public void GetLeaderboardIntent(string leaderboardId, AN_Leaderboard.TimeSpan span, Action<AN_IntentResult> callback) {
            GetLeaderboardIntent(leaderboardId, span, AN_Leaderboard.Collection.Public, callback);
        }

        /// <summary>
        /// Asynchronously loads an Intent to show a leaderboard for a game specified by a leaderboardId. 
        /// Note that the Intent returned from the Task must be invoked with <see cref="App.AN_Activity.StartActivityForResult(Content.AN_Intent, Action{App.AN_ActivityResult})"/>, 
        /// so that the identity of the calling package can be established.
        /// </summary>
        /// <param name="leaderboardId">The ID of the leaderboard to view.</param>
        /// <param name="span">Time span to retrieve data for. </param>
        /// <param name="leaderboardCollection">The collection to show by default. </param>
        /// <param name="callback">Request async callback</param>
        public void GetLeaderboardIntent(string leaderboardId, AN_Leaderboard.TimeSpan span, AN_Leaderboard.Collection leaderboardCollection, Action<AN_IntentResult> callback) {
            AN_GMS_Lib.Leaderboards.GetLeaderboardIntent(this, leaderboardId, (int)span, (int)leaderboardCollection, callback);
        }



        /// <summary>
        /// Asynchronously loads <see cref="AN_LeaderboardScore"/> result 
        /// that represents the signed-in player's score for the leaderboard specified by leaderboardId. 
        /// 
        /// Request uses <see cref="AN_Leaderboard.TimeSpan.AllTime"/> & <see cref="AN_Leaderboard.Collection.Public"/>
        /// </summary>
        /// <param name="leaderboardId">The ID of the leaderboard to view.</param>
        /// <param name="callback">Request async callback</param>
        public void LoadCurrentPlayerLeaderboardScore(string leaderboardId, Action<AN_LinkedObjectResult<AN_LeaderboardScore>> callback) {
            LoadCurrentPlayerLeaderboardScore(leaderboardId, AN_Leaderboard.TimeSpan.AllTime, AN_Leaderboard.Collection.Public, callback);
        }


        /// <summary>
        /// Asynchronously loads <see cref="AN_LeaderboardScore"/> result 
        /// that represents the signed-in player's score for the leaderboard specified by leaderboardId. 
        /// </summary>
        /// <param name="leaderboardId">The ID of the leaderboard to view.</param>
        /// <param name="span">Time span to retrieve data for. </param>
        /// <param name="leaderboardCollection">The collection to show by default. </param>
        /// <param name="callback">Request async callback</param>
        public void LoadCurrentPlayerLeaderboardScore(string leaderboardId, AN_Leaderboard.TimeSpan span, AN_Leaderboard.Collection leaderboardCollection, Action<AN_LinkedObjectResult<AN_LeaderboardScore>> callback) {
            AN_GMS_Lib.Leaderboards.LoadCurrentPlayerLeaderboardScore(this, leaderboardId, (int)span, (int)leaderboardCollection, callback);
        }

        /// <summary>
        /// Asynchronously loads <see cref="AN_LeaderboardsLoadResult"/> that represents a list of leaderboards metadata for this game.
        /// </summary>
        /// <param name="forceReload">
        /// If <c>true</c>, this call will clear any locally cached data and attempt to fetch the latest data from the server. 
        /// This would commonly be used for something like a user-initiated refresh. 
        /// Normally, this should be set to <c>false</c> to gain advantages of data caching.
        /// </param>
        /// <param name="callback">Request async callback</param>
        public void LoadLeaderboardMetadata(bool forceReload, Action<AN_LeaderboardsLoadResult> callback) {
            AN_GMS_Lib.Leaderboards.LoadLeaderboardMetadata(this, forceReload, callback);
        }

        /// <summary>
        /// Asynchronously loads an <see cref="AN_LeaderboardLoadResult"/> specified by <see cref="leaderboardId"/>.
        /// </summary>
        /// <param name="leaderboardId">ID of the leaderboard to load metadata for.</param>
        /// <param name="forceReload">
        /// If <c>true</c>, this call will clear any locally cached data and attempt to fetch the latest data from the server. 
        /// This would commonly be used for something like a user-initiated refresh. 
        /// Normally, this should be set to <c>false</c> to gain advantages of data caching.
        /// </param>
        /// <param name="callback">Request async callback</param>
        public void LoadLeaderboardMetadata(string leaderboardId, bool forceReload, Action<AN_LeaderboardLoadResult> callback) {
            AN_GMS_Lib.Leaderboards.LoadLeaderboardMetadata(this, leaderboardId, forceReload, callback);
        }


        /// <summary>
        /// Asynchronously loads an <see cref="AN_LeaderboardScores"/> that represents an additional page of score data 
        /// for the given score buffer. A new score buffer will be delivered that replaces the given buffer.
        /// </summary>
        /// <param name="buffer">
        /// The existing buffer that will be expanded. 
        /// The buffer is allowed to be closed prior to being passed in to this method.</param>
        /// <param name="maxResults">
        /// The maximum number of scores to fetch per page.
        /// Must be between 1 and 25. Note that the number of scores returned here may be greater than this value, 
        /// depending on how much data is cached on the device.
        /// </param>
        /// <param name="pageDirection">The direction to expand the buffer.</param>
        /// <param name="callback">Request async callback</param>
        public void LoadMoreScores(AN_LeaderboardScoreBuffer buffer, int maxResults, AN_PageDirection pageDirection, Action<AN_LinkedObjectResult<AN_LeaderboardScores>> callback) {
            AN_GMS_Lib.Leaderboards.LoadMoreScores(this, buffer, maxResults, (int) pageDirection, callback);
        }


        /// <summary>
        /// Asynchronously loads an <see cref="AN_LeaderboardScores"/> that represents the player-centered page of scores 
        /// for the leaderboard specified by <see cref="leaderboardId"/>. 
        /// If the player does not have a score on this leaderboard, this call will return the top page instead.
        /// 
        /// Request uses <see cref="AN_Leaderboard.TimeSpan.AllTime"/> & <see cref="AN_Leaderboard.Collection.Public"/>
        /// </summary>
        /// <param name="leaderboardCollection">The leaderboard collection to retrieve scores for.</param>
        /// <param name="maxResults">The maximum number of scores to fetch per page. Must be between 1 and 25.</param>
        /// <param name="callback">Request async callback</param>
        public void LoadPlayerCenteredScores(string leaderboardId, int maxResults, Action<AN_LinkedObjectResult<AN_LeaderboardScores>> callback) {
            LoadPlayerCenteredScores(leaderboardId, AN_Leaderboard.TimeSpan.AllTime, AN_Leaderboard.Collection.Public, maxResults, false, callback);
        }


        /// <summary>
        /// Asynchronously loads an <see cref="AN_LeaderboardScores"/> that represents the player-centered page of scores 
        /// for the leaderboard specified by specified by <see cref="leaderboardId"/>. 
        /// If the player does not have a score on this leaderboard, this call will return the top page instead.
        /// </summary>
        /// <param name="leaderboardId">ID of the leaderboard.</param>
        /// <param name="span">Time span to retrieve data for.</param>
        /// <param name="leaderboardCollection">The leaderboard collection to retrieve scores for.</param>
        /// <param name="maxResults">The maximum number of scores to fetch per page. Must be between 1 and 25.</param>
        /// <param name="forceReload">
        /// If <c>true</c>, this call will clear any locally cached data and attempt to fetch the latest data from the server. 
        /// This would commonly be used for something like a user-initiated refresh. 
        /// Normally, this should be set to <c>false</c> to gain advantages of data caching.
        /// </param>
        /// <param name="callback">Request async callback</param>
        public void LoadPlayerCenteredScores(string leaderboardId, AN_Leaderboard.TimeSpan span, AN_Leaderboard.Collection leaderboardCollection, int maxResults, bool forceReload, Action<AN_LinkedObjectResult<AN_LeaderboardScores>> callback) {
            AN_GMS_Lib.Leaderboards.LoadPlayerCenteredScores(this, leaderboardId, (int) span, (int)leaderboardCollection, maxResults, forceReload, callback);
        }



        /// <summary>
        /// Asynchronously loads an <see cref="AN_LeaderboardScores"/> that represents represents the top page of scores
        /// for a given leaderboard specified by specified by <see cref="leaderboardId"/>. 
        /// If the player does not have a score on this leaderboard, this call will return the top page instead.
        /// 
        /// Request uses <see cref="AN_Leaderboard.TimeSpan.AllTime"/> & <see cref="AN_Leaderboard.Collection.Public"/>
        /// </summary>
        /// <param name="leaderboardId">ID of the leaderboard.</param>
        /// <param name="maxResults">The maximum number of scores to fetch per page. Must be between 1 and 25.</param>
        /// <param name="callback">Request async callback</param>
        public void LoadTopScores(string leaderboardId, int maxResults, Action<AN_LinkedObjectResult<AN_LeaderboardScores>> callback) {
            LoadTopScores(leaderboardId, AN_Leaderboard.TimeSpan.AllTime, AN_Leaderboard.Collection.Public, maxResults, false, callback);
        }



        /// <summary>
        /// Asynchronously loads an <see cref="AN_LeaderboardScores"/> that represents represents the top page of scores
        /// for a given leaderboard specified by specified by <see cref="leaderboardId"/>. 
        /// If the player does not have a score on this leaderboard, this call will return the top page instead.
        /// </summary>
        /// <param name="leaderboardId">ID of the leaderboard.</param>
        /// <param name="span">Time span to retrieve data for.</param>
        /// <param name="leaderboardCollection">The leaderboard collection to retrieve scores for.</param>
        /// <param name="maxResults">The maximum number of scores to fetch per page. Must be between 1 and 25.</param>
        /// <param name="forceReload">
        /// If <c>true</c>, this call will clear any locally cached data and attempt to fetch the latest data from the server. 
        /// This would commonly be used for something like a user-initiated refresh. 
        /// Normally, this should be set to <c>false</c> to gain advantages of data caching.
        /// </param>
        /// <param name="callback">Request async callback</param>
        public void LoadTopScores(string leaderboardId, AN_Leaderboard.TimeSpan span, AN_Leaderboard.Collection leaderboardCollection, int maxResults, bool forceReload, Action<AN_LinkedObjectResult<AN_LeaderboardScores>> callback) {
            AN_GMS_Lib.Leaderboards.LoadTopScores(this, leaderboardId, (int) span, (int)leaderboardCollection, maxResults, forceReload, callback);
        }


        /// <summary>
        /// Submit a score to a leaderboard for the currently signed-in player. 
        /// The score is ignored if it is worse (as defined by the leaderboard configuration) than a previously submitted score 
        /// for the same player.
        ///
        /// This form of the API is a fire-and-forget form.
        /// Use this if you do not need to be notified of the results of submitting the score, 
        /// though note that the update may not be sent to the server until the next sync.
        ///
        /// The meaning of the score value depends on the formatting of the leaderboard established in the developer console.
        /// Leaderboards support the following score formats:
        /// Fixed-point: score represents a raw value, 
        /// and will be formatted based on the number of decimal places configured.
        /// A score of 1000 would be formatted as 1000, 100.0, or 10.00 for 0, 1, or 2 decimal places.
        /// 
        /// Time: score represents an elapsed time in milliseconds.
        /// The value will be formatted as an appropriate time value.
        /// 
        /// Currency: score represents a value in micro units. 
        /// For example, in USD, a score of 100 would display as $0.0001, while a score of 1000000 would display as $1.00
        /// </summary>
        /// <param name="leaderboardId">The leaderboard to submit the score to.</param>
        /// <param name="score">The raw score value.</param>
        /// <param name="scoreTag">
        /// Optional metadata about this score. 
        /// The value may contain no more than 64 URI-safe characters as defined by section 2.3 of RFC 3986.
        /// </param>
        public void SubmitScore(string leaderboardId, long score, string scoreTag = "") {
            AN_GMS_Lib.Leaderboards.SubmitScore(this, leaderboardId, score, scoreTag);
        }


        /// <summary>
        /// Asynchronously submits the score to the leaderboard for the currently signed-in player.
        /// The score is ignored if it is worse(as defined by the leaderboard configuration) 
        /// than a previously submitted score or the same player.
        /// 
        /// This form of the API will attempt to submit the score to the server immediately within the callback, 
        /// returning a <see cref="AN_ScoreSubmissionData"/> on success with information about the submission.
        ///
        /// The meaning of the score value depends on the formatting of the leaderboard established in the developer console.
        /// Leaderboards support the following score formats:
        /// Fixed-point: score represents a raw value, 
        /// and will be formatted based on the number of decimal places configured.
        /// A score of 1000 would be formatted as 1000, 100.0, or 10.00 for 0, 1, or 2 decimal places.
        /// 
        /// Time: score represents an elapsed time in milliseconds.
        /// The value will be formatted as an appropriate time value.
        /// 
        /// Currency: score represents a value in micro units. 
        /// For example, in USD, a score of 100 would display as $0.0001, while a score of 1000000 would display as $1.00
        /// </summary>
        /// <param name="leaderboardId">The leaderboard to submit the score to.</param>
        /// <param name="score">The raw score value.</param>
        /// <param name="scoreTag">
        /// Optional metadata about this score. 
        /// The value may contain no more than 64 URI-safe characters as defined by section 2.3 of RFC 3986.
        /// </param>
        /// <param name="callback">Request async callback</param>
        public void SubmitScoreImmediate(string leaderboardId, long score, string scoreTag, Action<AN_LinkedObjectResult<AN_ScoreSubmissionData>> callback) {
            AN_GMS_Lib.Leaderboards.SubmitScoreImmediate(this, leaderboardId, score, scoreTag, callback);
        }



    }
}