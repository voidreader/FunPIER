using UnityEngine;
using System.Collections;

public class TAB_DIFFICULT : TAB_BaseServer
{

    public override void init()
    {
        Sort = rpgames.game.RequestMapList.Sort.Difficult;

        base.init();
    }
}
