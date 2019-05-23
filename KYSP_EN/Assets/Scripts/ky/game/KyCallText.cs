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

        switch (textId)
        {
            case 1100:
            case 1110:
            case 1111:
            case 1103:
                pos = new Vector3(70, 174,0);
                break;
            case 1104:
            case 1108:
                pos = new Vector3(50, 174, 0);
                break;
            case 1105:
                pos = new Vector3(115, 174, 0);
                break;

            case 1102:
                pos = new Vector3(175, 182, 0);
                break;

            case 1101:
            case 1107:
            case 1210:
                pos = new Vector3(103, 174, 0);
                break;

            case 1205:
            case 1207:
            case 1211:
                pos = new Vector3(103, 174, 0);
                break;

            case 1106:
            case 1206:
                pos = new Vector3(121, 174, -1f);
                break;


            
            case 1109:
            case 1200:
                pos = new Vector3(85, 174, -1f);
                break;
            case 1201:
            case 1203:
            case 1204:
                pos = new Vector3(60, 174, 0);
                break;
            case 1202:
                RandomTextSprite.CharacterSize = new Vector2(20, 20);
                pos = new Vector3(161, 165, 0);
                break;
            case 1208:
                RandomTextSprite.CharacterSize = new Vector2(20, 20);
                pos = new Vector3(140, 165, 0);
                break;
            case 1209:
                RandomTextSprite.CharacterSize = new Vector2(20, 20);
                pos = new Vector3(151, 165, 0);
                break;

        }
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
