using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlatformerPro;

public class PlatformerInputManager : RPGSingleton<PlatformerInputManager>
{
    /// <summary>
    /// A "digital" version of the horizontal axis in which the only valid values are -1 for LEFT, 
    /// 0 for NONE, and 1 for RIGHT.
    /// </summary>
    public int HorizontalAxisDigital { get; private set; }

    ButtonState _JumpButton = ButtonState.NONE;
    public ButtonState JumpButton 
    { 
        get 
        {
            if (_JumpButton == ButtonState.DOWN)
            {
                _JumpButton = ButtonState.HELD;
                return ButtonState.DOWN;
            }
            return _JumpButton;
        } 
        private set
        {
            _JumpButton = value;
        }
    }

    public override void Init()
    {
        base.Init();
        reset();
    }

    public void reset()
    {
        HorizontalAxisDigital = 0;
        JumpButton = ButtonState.NONE;
    }

    public void setKeyDown(KeyCode code)
    {
        switch (code)
        {
            case KeyCode.LeftArrow: HorizontalAxisDigital = -1; break;
            case KeyCode.RightArrow: HorizontalAxisDigital = 1; break;
            case KeyCode.UpArrow: JumpButton = ButtonState.DOWN; /*StartCoroutine("cJumpButton");*/ break;
            //default: HorizontalAxisDigital = 0; break;
        }
    }

    public void setKeyUp(KeyCode code)
    {
        switch (code)
        {
            case KeyCode.LeftArrow: if (HorizontalAxisDigital == -1) { HorizontalAxisDigital = 0; } break;
            case KeyCode.RightArrow: if (HorizontalAxisDigital == 1) { HorizontalAxisDigital = 0; } break;
            case KeyCode.UpArrow: JumpButton = ButtonState.UP; break;
        }
    }

    public void setKey(KeyCode code)
    {
        if (code == KeyCode.UpArrow)
            JumpButton = ButtonState.HELD; 
    }

    IEnumerator cJumpButton()
    {
        JumpButton = ButtonState.DOWN;
        yield return new WaitForEndOfFrame();
        JumpButton = ButtonState.HELD;
    }

}

