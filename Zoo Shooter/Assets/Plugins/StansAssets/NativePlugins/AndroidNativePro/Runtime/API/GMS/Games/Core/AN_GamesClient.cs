using System;
using System.Collections.Generic;
using UnityEngine;

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
        public void SetViewForPopups(AN_Activity activity) {
             AN_Java.Bridge.CallStatic(AN_GamesClientJAVA, "SetViewForPopups", HashCode, activity);
        }


        /// <summary>
        /// asynchronously sets the part of the screen at which games service pop-ups 
        /// (for example, "welcome back" or "achievement unlocked" popups) will be displayed using gravity.
        /// 
        /// Default value is <see cref="App.View.AN_Gravity.TOP"/> | <see cref="App.View.AN_Gravity.CENTER_HORIZONTAL"/>
        /// </summary>
        /// <param name="gravity">The gravity which controls the placement of games service pop-ups.</param>
        public void SetGravityForPopups(int gravity) {
            AN_Java.Bridge.CallStatic(AN_GamesClientJAVA, "SetGravityForPopups", HashCode, gravity);
        }




    }
}