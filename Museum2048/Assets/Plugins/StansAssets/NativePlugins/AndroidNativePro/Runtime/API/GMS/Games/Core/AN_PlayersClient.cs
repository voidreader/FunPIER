using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.GMS.Common;
using SA.Android.GMS.Games;

using SA.Android.Utilities;
using SA.Android.GMS.Internal;

namespace SA.Android.GMS.Games
{
    /// <summary>
    /// A client to interact with Players.
    /// </summary>
    [Serializable]
    public class AN_PlayersClient : AN_LinkedObject
    {


        /// <summary>
        /// Asynchronously loads the current signed-in <see cref="AN_Player"/> , if available.
        /// 
        /// Required Scopes: SCOPE_GAMES_LITE
        /// </summary>
        /// <param name="callback">player load callback.</param>
        public void GetCurrentPlayer(Action<AN_SerializedObjectResult<AN_Player>> callback) {
            AN_GMS_Lib.Players.GetCurrentPlayer(this, callback);
        }

    }
}