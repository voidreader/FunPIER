using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayItem : MonoBehaviour {

    public UILabel _lblStep;
    public int _step;
    public Theme _theme;
    public int _index;
    public UISprite _sprite;
    public bool _isOpen = false;

    /// <summary>
    /// Step은 1부터 들어와야한다.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="step"></param>
    /// <param name="index"></param>
    public void SetDisplayItem(Theme t, int step, int index, bool isOpen) {

        _theme = t;
        _step = step;
        _index = index;
        _isOpen = isOpen;

        float x = 0;
        int row = 0;

        if (index % 3 == 0)
            x = -180;
        else if (index % 3 == 1)
            x = 0;
        else
            x = 180;


        row = index / 3;

        _lblStep.text = PierSystem.GetIDByStep(step).ToString();

        switch(_theme) {
            case Theme.Car:
                _sprite.atlas = PierSystem.main._themeAtlas[0];
                break;

            case Theme.Wine:
                _sprite.atlas = PierSystem.main._themeAtlas[1];
                break;

            case Theme.Viking:
                _sprite.atlas = PierSystem.main._themeAtlas[2];
                break;
                
        }

        if (isOpen)
            _sprite.spriteName = step.ToString();
        else
            _sprite.spriteName = "0";

        // 위치 잡기 
        this.transform.localPosition = new Vector3(x, 450 - (row * 300), 0);
        this.gameObject.SetActive(true);
           

    }


    
}
