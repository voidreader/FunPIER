using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KySceneUnlock : KyScene {
    private KyStateManager mState = new KyStateManager();	//	ステート管理
    private GuiButton mButtonYes = null;    //	YESボタン
    private GuiButton mButtonNo = null;     //	NOボタン

    public GameObject gObj = null;
    public GameObject confirmObj = null;


    private string input_str = "";

    public string NextScene = null;	// 終了後に移行するシーン

    // Use this for initialization
    void Start () {
        KyUtil.AlignTop(KyUtil.FindChild(gameObject, "navi"), 0);

        KyUtil.SetText(gameObject, "navi/label", KyText.GetText(20045));

        KyUtil.AlignBottom(KyUtil.FindChild(gameObject, "message"), 45);
        KyUtil.SetText(gameObject, "message", "");

        KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Return, KySoftKeys.Label.None);

        mState.ChangeState(StateMain);
    }
	
	// Update is called once per frame
	void Update () {
        if (mState != null) {
            mState.Execute();
        }
    }


    void OnGUI() {
        /*
        if ((Application.platform != RuntimePlatform.IPhonePlayer) && (Application.platform != RuntimePlatform.Android)) {
            // PC上でのテスト用
            float ofsx = (100);
            float ofsy = (20);
            Rect rect = new Rect(ofsx, ofsy, 230, 60);

            input_str = GUI.TextField(rect, input_str);
        }
        */
    }


    private int StatePinInputYes(object sender) {
        if (mState.Sequence == 2) {
            mState.Sequence = 3;
        }
        else {
            mState.Sequence = 92;
        }
        return 0;
    }
    private int StatePinInputNo(object sender) {
        mState.Sequence = 99;
        return 0;
    }


    private int StateMain() {
        if (mState.Sequence == 0) {
            if (gObj == null) {
                gObj = confirmObj;
                gObj.transform.parent = transform;


                KyUtil.SetText(gObj, "message", KyText.GetText(23004));
                KyUtil.SetText(gObj, "btnYes", KyText.GetText(22000)); // はい
                KyUtil.SetText(gObj, "btnNo", KyText.GetText(22001)); //いいえ

                GuiButton ButtonYes = KyUtil.GetComponentInChild<GuiButton>(gObj, "btnYes");
                GuiButton ButtonNo = KyUtil.GetComponentInChild<GuiButton>(gObj, "btnNo");
                
                ButtonYes.GuiEnabled = false;
                ButtonNo.GuiEnabled = false;

                /*
                ButtonYes.ButtonSelected += StateTweetYes;
                ButtonNo.ButtonSelected += StateTweetNo;
                ButtonOth.ButtonSelected += StateTweetOther;
                */

                ScreenFader.Main.FadeIn();

                mState.Sequence++;
            }
            else {
                mState.ChangeState(StateLeave);
                mState.Sequence = 0;
            }
        } // end of sequence 0
        else {
            if (mState.Sequence == 1) {

                if (!ScreenFader.Main.FadeRunning) {

                    GuiButton ButtonYes = KyUtil.GetComponentInChild<GuiButton>(gObj, "btnYes");
                    GuiButton ButtonNo = KyUtil.GetComponentInChild<GuiButton>(gObj, "btnNo");
                    

                    ButtonYes.GuiEnabled = true;
                    ButtonNo.GuiEnabled = true;

                    ButtonYes.ButtonSelected += StatePinInputYes;
                    ButtonNo.ButtonSelected += StatePinInputNo;


                    mState.Sequence++;
                }
            }
            else if (mState.Sequence == 99) {
                GuiButton ButtonYes = KyUtil.GetComponentInChild<GuiButton>(gObj, "btnYes");
                GuiButton ButtonNo = KyUtil.GetComponentInChild<GuiButton>(gObj, "btnNo");
                ButtonYes.GuiEnabled = false;
                ButtonNo.GuiEnabled = false;
                mState.ChangeState(StateLeave);
                mState.Sequence = 0;
            }

        }


        return 0;
    }


    private int StateLeave() {
        if (mState.Sequence == 0) {
            // とりあえずタイトルにでも戻す
            ScreenFader.Main.FadeOut();
            mState.Sequence++;
        }
        else
        if (mState.Sequence == 1) {
            if (!ScreenFader.Main.FadeRunning) {
                if (NextScene == null) {
                    ChangeScene("KySceneTitle");
                }
                else {
                    ChangeScene(NextScene);
                }
                mState.Sequence++;
            }
        }
        return 0;
    }

}
