using UnityEngine;
using System.Collections;

public class TAB_POPULARITY : TAB_BaseServer
{

    public override void init()
    {
        Sort = rpgames.game.RequestMapList.Sort.Popularity;

        base.init();
    }
}
