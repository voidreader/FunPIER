using UnityEngine;
using System.Collections;

public class TAB_SCORE : TAB_BaseServer
{

    public override void init()
    {
        Sort = rpgames.game.RequestMapList.Sort.StarScore;

        base.init();
    }
}
