using System;
using SA.Android.GMS.Games;
using SA.Android.Utilities;
using StansAssets.Foundation.Async;

namespace SA.Android.GMS.Internal
{
    class AN_GMS_Editor_PlayersAPI : AN_iGMS_PlayersAPI
    {
        public void GetCurrentPlayer(AN_PlayersClient client, Action<AN_SerializedObjectResult<AN_Player>> callback)
        {
             CoroutineUtility.WaitForSeconds(1, () =>
            {
                var result = new AN_SerializedObjectResult<AN_Player>(new AN_Player());
                callback.Invoke(result);
            });
        }
    }
}
