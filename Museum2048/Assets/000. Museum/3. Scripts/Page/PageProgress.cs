using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageProgress : UILayer {

    public UIPanel _scrollview;
    public List<DisplayItem> _listItems;
    public Theme _theme;

    
    
    

    public int MaxStep;
    public int CurrentStep; 

    public void SetTheme(Theme t) {
        _theme = t;
    }

    public override UILayer Init(UILayerEnum type, Transform parent, Action pOpen, Action pClose) {

        _scrollview.gameObject.SetActive(false);

        for(int i=0; i<_listItems.Count; i++) {
            _listItems[i].gameObject.SetActive(false);
        }

        return base.Init(type, parent, SetProgressList, pClose);
    }

    /// <summary>
    /// 
    /// </summary>
    void SetProgressList() {
        _scrollview.gameObject.SetActive(true);


        // Theme 별로.. 
        MaxStep = PierSystem.main.GetMaxProgress(_theme);
        CurrentStep = PierSystem.main.GetCurrentProgress(_theme);

        for(int i=1; i<=MaxStep;i++) {

            if (i <= CurrentStep)
                _listItems[i].SetDisplayItem(_theme, i, i-1, true);
            else
                _listItems[i].SetDisplayItem(_theme, i, i-1, false);
        }

    }
}
