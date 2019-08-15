using System;


using SA.Android.GMS.Common;
using SA.Android.GMS.Games;
using SA.Android.Utilities;


namespace SA.Android.GMS.Internal
{
    internal interface AN_iGMS_PlayersAPI
    {

        //--------------------------------------
        // AN_PlayersClient
        //--------------------------------------

        void GetCurrentPlayer(AN_PlayersClient client, Action<AN_SerializedObjectResult<AN_Player>> callback);
    }
}