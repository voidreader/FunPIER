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

        switch (textId)
        {

            case 1003:
                pos = new Vector3(273, 130, -1f);
                break;

            case 1024:
                pos = new Vector3(322, 130, -1f);
                break;




            case 1004:
            case 1015:
            case 1016:
            case 1017:
            case 1021:
            case 1022:
            case 1025:
            case 1026:
            case 1030:
            case 1031:
            case 1032:
            case 1034:
            case 1037:
                pos = new Vector3(135, 130, -1f);
                break;

            case 1001:
            case 1005:
            case 1006:
            case 1008:
            case 1011:
            case 1014:
            case 1018:
            case 1020:
            
            case 1027:
            case 1028:
            case 1035:
            case 1036:
                pos = new Vector3(122, 130, -1f);
                break;

            case 1007:
            case 1009:
            case 1012:
            case 1013:
            case 1019:
                pos = new Vector3(85, 130, -1f);
                break;
            case 1010:
                pos = new Vector3(100, 130, -1f);
                break;
            case 1002:
                pos = new Vector3(190, 130, -1f);
                break;
            case 1023:
            case 1033:
                pos = new Vector3(149, 130, -1f);
                break;
            case 1029:
                pos = new Vector3(174, 130, -1f);
                break;
        }
        return pos;
    }

	public int RandomTextIdMin = 1001;
	public int RandomTextIdMax = 1037;
	public int SpeekCount = 5;
	public SpriteTextCustom RandomTextSprite = null;

	private int[] mTextIds = null;
	private int mTextIndex = 0;
}
