using System;
using System.Collections;
using System.Collections.Generic;
using SA.Foundation.Templates;
using UnityEngine;

using SA.Android.GMS.Common;
using SA.Android.GMS.Games;
using SA.Android.Utilities;

namespace SA.Android.GMS.Internal
{

    internal class AN_GMS_Native_PlayersAPI : AN_iGMS_PlayersAPI
    {

        const string AN_PlayersClient = "com.stansassets.gms.games.AN_PlayersClient";


        public void GetCurrentPlayer(AN_PlayersClient client, Action<AN_SerializedObjectResult<AN_Player>> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_PlayersClient, "GetCurrentPlayer", callback, client.HashCode);
        }


    }
}