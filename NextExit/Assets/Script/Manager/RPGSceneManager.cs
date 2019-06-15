using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RPGSceneManager : RPGSingleton<RPGSceneManager>
{

    //public eSceneIndex StartScene = eSceneIndex.UIMain;

	// Use this for initialization
	void Start () {
        //pushScene<UIMain>();
		
        //UIDownload.show();
	}
	/*
	// Update is called once per frame
	void Update () {
	
	}
    */
    /*
    public enum ePopupIndex
    {
        MessagePrint,
        MessageBox,
        MessageInput,
    }
    */
    /*
    public enum eSceneIndex
    {
        UIEditorStage,        // 스테이지 편집 UI
        UIMain,               // 메인 UI
        UICustomMain,         // 커스텀 메인 UI
    }
    */

    [Header("게임 씬")]
    public RPGLayer GameScene;
    [Header("UI 씬")]
    public RPGLayer UIScene;
    [Header("Popup 씬")]
    public RPGLayer PopupScene;
    [Header("Loading 씬")]
    public RPGLayer LoadingScene;


    public RPGLayer PlayLayer { get; private set; }
    public RPGLayer CustomLayer { get; private set; }

    public GameBtnControl GameButtonControl { get; set; }
    public CustomBtnControl CustomButtonControl { get; set; }

    void Awake()
    {
	}

	public void InitScene()
	{
		base.Init();
		PlayLayer = GameScene.getTransform().Find("PlayLayer").GetComponent<RPGLayer>();
        CustomLayer = GameScene.getTransform().Find("CustomLayer").GetComponent<RPGLayer>();

        GameButtonControl = PlayLayer.getTransform().Find("GameBtnControl").GetComponent<GameBtnControl>();
        CustomButtonControl = CustomLayer.getTransform().Find("CustomBtnControl").GetComponent<CustomBtnControl>();
        //CustomButtonControl = CustomLayer.GetComponentInChildren<CustomBtnControl>();

        //GameManager.Instance.player = PlayLayer.getTransform().FindChild("Player").GetComponent<Player>();
        GameManager.Instance.TextTime = PlayLayer.getTransform().Find("TEXT/Content/text_Time").GetComponent<RPGTextMesh>();
        GameManager.Instance.TextStage = PlayLayer.getTransform().Find("TEXT/Content/text_Stage").GetComponent<RPGTextMesh>();
        GameManager.Instance.TextDeath = PlayLayer.getTransform().Find("TEXT/Content/text_Death").GetComponent<RPGTextMesh>();
        //GameManager.Instance.Effect_Die = PlayLayer.getTransform().FindChild("Effect/Effect_Die").GetComponent<tk2dSpriteAnimator>();
        GameManager.Instance.EffectLayer = PlayLayer.getTransform().Find("Effect").GetComponent<RPGLayer>();

        GameManager.Instance.BtnLeft = GameButtonControl.transform.Find("BtnLeft");
        GameManager.Instance.BtnRight = GameButtonControl.transform.Find("BtnRight");
        GameManager.Instance.BtnJump = GameButtonControl.transform.Find("BtnJump");

        UIScene.removeAllChild();
        PopupScene.removeAllChild();
        LoadingScene.gameObject.SetActive(false);
    }
	

    #region Popup
    /*
    string getPopupName(ePopupIndex index)
    {
        switch (index)
        {
            case ePopupIndex.MessagePrint:
            case ePopupIndex.MessageBox:
            case ePopupIndex.MessageInput: 
                return "Prefabs/ETC/"+index.ToString();
            default:
                return "Prefabs/Popup/" + index.ToString();
        }
    }

    public RPGLayer pushPopup(ePopupIndex index)
    {
        return pushPopup(index, null);
    }

    public RPGLayer pushPopup(ePopupIndex index, Dictionary<string, object> dic)
    {
        RPGLayer layer = PopupScene.addChild(getPopupName(index)).GetComponent<RPGLayer>();
        if (dic != null)
            layer.init(dic);
        return layer;
    }
    */
    public T pushPopup<T>(string prefabPath)
    {
        return pushPopup<T>(prefabPath, null);
    }

    public T pushPopup<T>(string prefabPath, Dictionary<string, object> dic)
    {
        RPGLayer layer = PopupScene.addChild(prefabPath).GetComponent<RPGLayer>();
		//if( PopupScene.transform.childCount =
		layer.transform.localPosition = new Vector3( 0f, 0f,  -( (PopupScene.transform.childCount - 1) * 100f ) ) / 20f;
        if (dic != null)
            layer.init(dic);
        return (T)(object)layer;
    }
    #endregion

    #region UI

    /*
    string getSceneName<T>()
    {
        
        switch (index)
        {
            case eSceneIndex.UIEditorStage: return "Prefabs/UI/UIEditorStage";
            case eSceneIndex.UIMain: return "Prefabs/UI/UIMain";
            
        }
        return "";
        
    }
    */
    public T pushScene<T>(string prefabPath)
    {
        return pushScene<T>(prefabPath, null);
    }

    public T pushScene<T>(string prefabPath, Dictionary<string, object> dic)
    {
        UIScene.removeAllChild();

        RPGLayer layer = UIScene.addChild(prefabPath).GetComponent<RPGLayer>();
        if (dic != null)
            layer.init(dic);
        return (T)(object)layer; 
    }
    #endregion

	void Update()
	{
		//	exit
		if ( Input.GetKeyDown( KeyCode.Escape ) )
		{
			CheckBack();
		}
	}
	/// <summary>
	/// 핸드폰 에서 백키 눌렀을시 상황별 설정. 
	/// </summary>
	private void CheckBack()
	{
		int popupCount = RPGSceneManager.Instance.PopupScene.transform.childCount;
		if ( popupCount > 0 )
		{
			Transform tranChid = RPGSceneManager.Instance.PopupScene.transform.GetChild( 0 );
			int popupIndex = 0;
			Vector3 pos = tranChid.localPosition;

			for ( int index = 1; index < popupCount; ++index )
			{
				tranChid = RPGSceneManager.Instance.PopupScene.transform.GetChild( index );
				if ( pos.z > tranChid.transform.localPosition.z )
				{
					pos = tranChid.transform.localPosition;
					popupIndex = index;

				}
			}
			RPGLayer popupLayer = RPGSceneManager.Instance.PopupScene.transform.GetChild( popupIndex ).gameObject.GetComponent<RPGLayer>();
			popupLayer.OnPopupClose();
			if( popupLayer == null )
			{
				Debug.Log( "popupLayer" );
			}
		}
		else
		{
			int sceneCount = RPGSceneManager.Instance.UIScene.transform.childCount;
			Debug.Log( "sceneCount : " + sceneCount );
			if( sceneCount == 0 )
			{
				PopupPause.show();
			}
			else
			{
				PopupGameExit.show();
			}
			/*
			Transform tranChid = RPGSceneManager.Instance.UIScene.transform.GetChild( 0 );
			if ( tranChid.gameObject.GetComponent<UI_Achieve>() != null )
			{
				tranChid.gameObject.GetComponent<UI_Achieve>().OnHomeClick();
			}
			else if ( tranChid.gameObject.GetComponent<UI_Collection>() != null )
			{
				tranChid.gameObject.GetComponent<UI_Collection>().OnMainClick();
			}
			else if ( tranChid.gameObject.GetComponent<UI_StageSelect>() != null )
			{
				tranChid.gameObject.GetComponent<UI_StageSelect>().OnMainClick();
			}
			else if ( tranChid.gameObject.GetComponent<UI_Game>() != null )
			{
				tranChid.gameObject.GetComponent<UI_Game>().OnPauseClick();
			}
			else if ( tranChid.gameObject.GetComponent<UI_Main>() != null )
			{
				Popup_GameExit.show();
			}
			*/
		}
	}
}
