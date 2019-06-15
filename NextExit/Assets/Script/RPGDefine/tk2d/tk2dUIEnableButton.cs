using UnityEngine;
using System.Collections;

[AddComponentMenu("2D Toolkit/UI/tk2dUIEnableButton")]
public class tk2dUIEnableButton : tk2dUIBaseItemControl
{

    /// <summary>
    /// State that will be active if up and deactivated if down
    /// </summary>
    public GameObject enableStateGO;

    /// <summary>
    /// State that will be active if down and deactivated if up
    /// </summary>
    public GameObject disbleStateGO;

    public tk2dSprite colorChangeSprite;

    public Color enableStateColor = new Color(255, 255, 255, 255);
    public Color disableStateColor = new Color(110, 110, 110, 255);

    void OnEnable()
    {
        SetState();
    }

    void OnDisable()
    {
        SetState();
    }

    private void SetState()
    {
        if (enableStateGO != null && disbleStateGO != null)
        {
            ChangeGameObjectActiveStateWithNullCheck(enableStateGO, !enabled);
            ChangeGameObjectActiveStateWithNullCheck(disbleStateGO, enabled);
        }
        else if (colorChangeSprite != null)
        {
            colorChangeSprite.color = enabled ? enableStateColor : disableStateColor;
        }
    }
}
