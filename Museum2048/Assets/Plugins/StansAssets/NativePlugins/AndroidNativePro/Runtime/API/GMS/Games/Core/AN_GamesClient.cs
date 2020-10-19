using System;
using SA.Android.App;
using SA.Android.Utilities;

namespace SA.Android.GMS.Games
{
    /// <summary>
    /// A client to interact with games basic functionality.
    /// </summary>
    [Serializable]
    public class AN_GamesClient : AN_LinkedObject
    {
        const string AN_GamesClientJAVA = "com.stansassets.gms.games.AN_GamesClient";

        /// <summary>
        /// Asynchronously sets the Activity root view to use as a content view for popups
        /// </summary>
        public void SetViewForPopups(AN_Activity activity)
        {
            AN_Java.Bridge.CallStatic(AN_GamesClientJAVA, "SetViewForPopups", HashCode, activity);
        }

        /// <summary>
        /// asynchronously sets the part of the screen at which games service pop-ups
        /// (for example, "welcome back" or "achievement unlocked" popups) will be displayed using gravity.
        ///
        /// Default value is <see cref="App.View.AN_Gravity.TOP"/> | <see cref="App.View.AN_Gravity.CENTER_HORIZONTAL"/>
        /// </summary>
        /// <param name="gravity">The gravity which controls the placement of games service pop-ups.</param>
        public void SetGravityForPopups(int gravity)
        {
            AN_Java.Bridge.CallStatic(AN_GamesClientJAVA, "SetGravityForPopups", HashCode, gravity);
        }

        /// <summary>
        /// Asynchronously loads an Intent to show the Settings screen
        /// that allows the user to configure Games-related features for the current game.
        /// Note that this must be invoked with <see cref="App.AN_Activity.StartActivityForResult(Content.AN_Intent, Action{App.AN_ActivityResult})"/>
        /// so that the identity of the calling package can be established.
        ///
        /// Required Scopes: `SCOPE_GAMES_LITE`
        /// </summary>
        /// <param name="callback">Task completion callback.</param>
        public void GetSettingsIntent(Action<AN_IntentResult> callback)
        {
            AN_Java.Bridge.CallStaticWithCallback(AN_GamesClientJAVA, "GetSettingsIntent", callback, HashCode);
        }

    }
}
