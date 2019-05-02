using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageOption : UILayer
{
    // Start is called before the first frame update
    void Start()  {
        
    }

    public override UILayer Init(UILayerEnum type, Transform parent, Action pOpen, Action pClose) {
        return base.Init(type, parent, pOpen, pClose);
    }


}
