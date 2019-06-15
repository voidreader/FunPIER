using UnityEngine;
using System.Collections;

public class TAB_PLAYCOUNT : TAB_BaseServer
{

    public override void init()
    {
        Sort = rpgames.game.RequestMapList.Sort.PlayCount;

        base.init();
    }
}
