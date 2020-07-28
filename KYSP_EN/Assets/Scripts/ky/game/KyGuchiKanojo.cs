using UnityEngine;
using System.Collections;

/// <summary>
/// 問題「ぐち」の彼女のセリフ用スクリプト。
/// </summary>
public class KyGuchiKanojo : KyScriptObject {

	protected override void Start () {
		base.Start();
		mScriptDriven = false;

		int[] textIdPool = new int[RandomTextIdMax - RandomTextIdMin + 1];
		for (int i = 0; i < textIdPool.Length; ++i) {
			textIdPool[i] = RandomTextIdMin + i;
		}

		mTextIds = new int[SpeekCount];
		for (int i = 0; i < SpeekCount; ++i) {
			int index = Random.Range(0, textIdPool.Length - i - 1);
			int textId = textIdPool[index];
			textIdPool[index] = textIdPool[textIdPool.Length - i - 1];
			textIdPool[textIdPool.Length - i - 1] = textId;
			mTextIds[i] = textId;
		}
	}

	public void OnUpdateText() {
		if (mTextIndex >= SpeekCount) {
			return;
		}
		int textId = mTextIds[mTextIndex];
        
		RandomTextSprite.Text = KyText.GetText(textId);
        RandomTextSprite.transform.localPosition = SetPosition(textId);
        RandomTextSprite.UpdateAll();
		mTextIndex++;
		if (mTextIndex >= SpeekCount) {
			CommandManager.SetVariable("finish", 1);
		}
	}

    Vector3 SetPosition(int textId)
    {
        Vector3 pos = new Vector3();
        int xpos = 0;

        switch (textId)
        {
            case 1012:
            case 1016:
            case 1028:
            case 1033:
                xpos = 145;
                break;


            case 1019:
                xpos = 165;
                break;


            
            
            case 1030:
            case 1031:
            case 1032:
            case 1034:
            
            case 1036:
            case 1037:


            case 1004:
            
            
            
            case 1026:
            case 1001:
            
            case 1014:
            case 1020:
            case 1007:
            case 1009:
            
            case 1013:
            
            case 1010:
            case 1002:
            
                xpos = 200;
                break;


            case 1018:
                xpos = 225;
                break;

            case 1008:
            case 1025:
            
            case 1021:
                xpos = 210;
                break;

            case 1015:
            case 1035:
            case 1017:
            case 1029:
                xpos = 255;
                break;

            case 1005:
            case 1011:
                xpos = 280;
                break;

            case 1003:
            
            case 1027:
            case 1006:
            case 1022:
            case 1023:
                xpos = 273;
                break;



            case 1024:
                xpos = 322;
                break;
        }

        pos = new Vector3(xpos, 130, -1);
        return pos;
    }

    

	public int RandomTextIdMin = 1001;
	public int RandomTextIdMax = 1037;
	public int SpeekCount = 5;
	public SpriteTextCustom RandomTextSprite = null;

	private int[] mTextIds = null;
	private int mTextIndex = 0;
}
