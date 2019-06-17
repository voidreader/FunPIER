using System;
using System.Collections;
using System.Collections.Generic;
using SA.Foundation.Templates;
using UnityEngine;

using SA.Android.GMS.Common;
using SA.Android.GMS.Games;
using SA.Android.Utilities;

using SA.Foundation.Async;

namespace SA.Android.GMS.Internal
{

    internal class AN_GMS_Editor_PlayersAPI : AN_iGMS_PlayersAPI
    {

       


        public void GetCurrentPlayer(AN_PlayersClient client, Action<AN_LinkedObjectResult<AN_Player>> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                var result = new AN_LinkedObjectResult<AN_Player>(new AN_Player());
                callback.Invoke(result);
            });
        }


    }
}