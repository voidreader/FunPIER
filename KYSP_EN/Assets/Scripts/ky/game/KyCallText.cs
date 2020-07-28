using UnityEngine;
using System.Collections;

/// <summary>
/// 問題「コ?ルアンドレス?ンス」の??カルのセリフ用スクリプト。
/// </summary>
public class KyCallText : KyScriptObject {

	enum AnswerType {
		None = 0,
		Yes,
		No
	}

	#region MonoBehaviour Methods

	protected override void Start() {
		base.Start();
		mScriptDriven = false;
		int textCount = RandomTextIdMax - RandomTextIdMin + 1;
		int scriptCount = textCount / SpeakCount;
        int scriptIndex = Random.Range(0, scriptCount);
		mTextIndex = scriptIndex * SpeakCount + RandomTextIdMin;
	}

	#endregion

	#region Methods

	public void OnUpdateText() {
		string message = KyText.GetText(mTextIndex);
		char answer = message[message.Length - 1];
		message = message.Substring(0, message.Length - 1);
		mAnswer =
			answer == 'E' ? AnswerType.None :
			answer == 'Y' ? AnswerType.Yes :
			answer == 'N' ? AnswerType.No : AnswerType.None;
		DebugUtil.Log("answer is :" + answer);

		RandomTextSprite.Text = message;
        RandomTextSprite.transform.localPosition = SetPosition(mTextIndex);
        
        RandomTextSprite.UpdateAll();
	}

	public void OnJudgeAndNext() {
		AnswerType yourAnswer = 
			(int)CommandManager.GetVariable("yeah") == 1 ? AnswerType.Yes : AnswerType.No;
		if (yourAnswer == AnswerType.Yes) {
			mYearCount++;
		}
		if (mAnswer != AnswerType.None) {
			mQuest++;
			if (yourAnswer == mAnswer) {
				mScore++;
			} else {
				mScore--;
			}
		}
		DebugUtil.Log("score rate:" + (float)mScore / mQuest);
		CommandManager.SetVariable("score", (float)mScore/mQuest);
		CommandManager.SetVariable("yeah", 0);
		CommandManager.SetVariable("yeahCount", mYearCount);
		mTextIndex++;
	}

    Vector3 SetPosition(int textId)
    {
        Vector3 pos = new Vector3();
        int xpos = 0;
        RandomTextSprite.CharacterSize = new Vector2(20, 20);

        switch (textId)
        {


            


            case 1100:
            case 1101:
            case 1104:
            case 1105:
            case 1108:
            case 1109:
                xpos = 70;
                break;

            case 1102:
            case 1208:
            case 1209:
                xpos = 165;
                break;

            default:
                xpos = 100;
                break;

        }

        pos = new Vector3(xpos, 165, 0);

        return pos;
    }

    #endregion

    #region Fields

    public int RandomTextIdMin = 1100;
	public int RandomTextIdMax = 1111;
	public int SpeakCount = 3;
	public SpriteTextCustom RandomTextSprite = null;

	private int mTextIndex = 0;
	private int mScore = 0;
	private int mQuest = 0;
	private int mYearCount = 0;
	private AnswerType mAnswer = AnswerType.None;

	#endregion
}
