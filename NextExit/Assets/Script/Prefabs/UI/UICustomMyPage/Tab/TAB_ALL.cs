using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TAB_ALL : TAB_BaseServer
{
    public override void init()
    {
        Sort = rpgames.game.RequestMapList.Sort.CreateDate;

        base.init();
    }

}
