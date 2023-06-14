using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////////////////
//---------------------------------------
public sealed class UIPopup_Tutorial : HT.UIPopup
{
    //---------------------------------------
    [Header("TUTORIAL OBJECT INFO")]
    [SerializeField]
    private CanvasGroup _rootCanvas = null;
    [SerializeField]
    private GameObject _tutorial_Keyboard = null;
    [SerializeField]
    private GameObject _tutorial_Joystick = null;
    [SerializeField]
    private GameObject _tutorial_Mobile = null;
	[SerializeField]
	private GameObject[] _tutorial_Game = null;

	[SerializeField]
    private Button _button = null;

    //---------------------------------------
    private static bool _tutorialOpened = false;
    public static bool TutorialOpened { get{ return _tutorialOpened; } }

    //---------------------------------------
    public void InitTutorial(bool bShowGameTutorial, Action onTutorialEnd)
	{
        StartCoroutine(Tutorial_Internal(bShowGameTutorial, onTutorialEnd));
    }

    //---------------------------------------
    private IEnumerator Tutorial_Internal(bool bShowGameTutorial, Action onTutorialEnd)
    {
        _tutorialOpened = true;
        BattleFramework._Instance.GamePaused = true;
        HT.TimeUtils.SetTimeScale(0.0f, GameDefine.nTimeScaleLayer_Tutorial);

        if (HT.HTAfxPref.IsMobilePlatform)
        {
            _tutorial_Mobile.SetActive(true);
            _tutorial_Keyboard.SetActive(false);
            _tutorial_Joystick.SetActive(false);

		}
        else
        {
            if (HT.HTInputManager.Instance.JoystickConnected)
            {
                _tutorial_Joystick.SetActive(true);
                _tutorial_Keyboard.SetActive(false);
                _tutorial_Mobile.SetActive(false);
			}
            else
            {
                _tutorial_Keyboard.SetActive(true);
                _tutorial_Joystick.SetActive(false);
                _tutorial_Mobile.SetActive(false);
			}
		}

		for (int nInd = 0; nInd < _tutorial_Game.Length; ++nInd)
			_tutorial_Game[nInd].SetActive(false);

		//-----
		float fTime = 2.0f;
        while(fTime > 0.0f)
        {
            fTime -= HT.TimeUtils.RealTime;
            _rootCanvas.alpha = 1.0f - (fTime * 0.5f);

            yield return new WaitForEndOfFrame();
        }

        _rootCanvas.alpha = 1.0f;

        //-----
        bool bWaitForButton = true;
        _button.onClick.AddListener(() => { bWaitForButton = false; });

        while (bWaitForButton)
            yield return new WaitForEndOfFrame();
		
        //-----
        fTime = 1.0f;
        while(fTime > 0.0f)
        {
            fTime -= HT.TimeUtils.RealTime;
            _rootCanvas.alpha = fTime;

            yield return new WaitForEndOfFrame();
        }

		_rootCanvas.alpha = 0.0f;

		//-----
		if (bShowGameTutorial)
		{
			_tutorial_Keyboard.SetActive(false);
			_tutorial_Joystick.SetActive(false);
			_tutorial_Mobile.SetActive(false);

			for(int nInd = 0; nInd < _tutorial_Game.Length; ++nInd)
			{
				for (int nReset = 0; nReset < _tutorial_Game.Length; ++nReset)
					_tutorial_Game[nReset].SetActive(false);

				_tutorial_Game[nInd].SetActive(true);

				//-----
				fTime = 2.0f;
				while (fTime > 0.0f)
				{
					fTime -= HT.TimeUtils.RealTime;
					_rootCanvas.alpha = 1.0f - (fTime * 0.5f);

					yield return new WaitForEndOfFrame();
				}

				//-----
				bWaitForButton = true;
				while (bWaitForButton)
					yield return new WaitForEndOfFrame();

				//-----
				fTime = 1.0f;
				while (fTime > 0.0f)
				{
					fTime -= HT.TimeUtils.RealTime;
					_rootCanvas.alpha = fTime;

					yield return new WaitForEndOfFrame();
				}

				//-----
				_rootCanvas.alpha = 0.0f;
			}
		}

		//-----
		_button.onClick.RemoveAllListeners();

		HT.Utils.SafeInvoke(onTutorialEnd);

        _tutorialOpened = false;
        BattleFramework._Instance.GamePaused = false;
        HT.TimeUtils.SetTimeScale(1.0f, GameDefine.nTimeScaleLayer_Tutorial);

        ClosePopup();
    }

    //---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------