using UnityEngine;
using System.Collections;

public class GameBtnControl : MonoBehaviour {

    //Transform BtnLeft;
    //Transform BtnRight;
    //Transform BtnJump;

    void Awake()
    {
        //Transform t = transform;
        //BtnLeft = t.FindChild("BtnLeft");
        //BtnRight = t.FindChild("BtnRight");
        //BtnJump = t.FindChild("BtnJump");
    }

	// Use this for initialization
	void Start () {
        //GameManager.Instance.BtnLeft = BtnLeft;
        //GameManager.Instance.BtnRight = BtnRight;
        //GameManager.Instance.BtnJump = BtnJump;
    }
	
    void OnTest()
    {
        MessagePrint.show("Test Message!");
    }

    void OnBtnPause()
    {
		RPGSoundManager.Instance.PlayUISound( 3 );
        //GameManager.Instance.exitInGame();
        //UIMain.show();
        PopupPause.show();
    }


}
