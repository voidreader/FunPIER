using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TAB_UPLOAD : TAB_BaseServer
{

    public override void init()
    {
        SearchOwner = NENetworkManager.Instance.UserID;
        Sort = rpgames.game.RequestMapList.Sort.CreateDate;

        base.init();
    }

}
