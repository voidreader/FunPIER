using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomBtnControl : RPGLayer {
    /*
	// Use this for initialization
	void Start () {
	
	}
	*/
    /*
	// Update is called once per frame
	void Update () {

        RPGTouch touch = RPGTouchManager.Instance.Touches[0];

        if (touch.IsEnable && touch.phase == TouchPhase.Ended)
        {

        }
        
	}
    */

    RPGScroll m_Scroll;
    int m_SelectIndex = -1;
    //string m_SelectItemID = "";
    //List<Dictionary<string, object>> m_list = new List<Dictionary<string, object>>();

	//kjh:: ui 변경/
  //public tk2dUIToggleButton ToggleCustomMode { get; private set; }

	/// <summary>
	/// kjh::
	/// 지도 배치 버튼
	/// </summary>
	public tk2dUIItem buttonSet { get; private set; }
	/// <summary>
	/// kjh::
	/// 오브젝트 삭제 버튼. 
	/// </summary>
	public tk2dUIItem buttonDel { get; private set; }

	public GameObject objMenu;

    public override void init()
    {
        Debug.Log("EditorBtnControl init");

        base.init();
		//kjh:: ui 변경/
		//ToggleCustomMode = getTransform().FindChild("BtnMode").GetComponent<tk2dUIToggleButton>();
		buttonSet = getTransform().Find( "BtnEditor" ).GetComponent<tk2dUIItem>();
		buttonDel = getTransform().Find( "BtnDelete" ).GetComponent<tk2dUIItem>();
		objMenu = getTransform().Find( "objMenu" ).gameObject;
		objMenu.gameObject.SetActive( false );
		if(  GameManager.Instance.CustomMode == GameManager.eCustomMode.Editor )
		{
			buttonSet.enabled = false;
			buttonDel.enabled = true;
		}
		else
		{
			buttonSet.enabled = true;
			buttonDel.enabled = false;
		}

        m_Scroll = getTransform().Find("Scroll/Controller").GetComponent<RPGScroll>();

        /*
        foreach (KeyValuePair<string, BlockBase> pair in BlockManager.Instance.BlockList)
            m_list.Add(new Dictionary<string, object>() { { "selected", false }, { "id", pair.Key } });
        */
        /*
        m_list.Add(new Dictionary<string, object>() { { "selected", false }, { "id", "object_01_entry" } });
        m_list.Add(new Dictionary<string, object>() { { "selected", false }, { "id", "object_01_exit" } });
        m_list.Add(new Dictionary<string, object>() { { "selected", false }, { "id", "object_02_brick01" } });
        m_list.Add(new Dictionary<string, object>() { { "selected", false }, { "id", "object_02_brick02" } });
        m_list.Add(new Dictionary<string, object>() { { "selected", false }, { "id", "object_02_box" } });
        m_list.Add(new Dictionary<string, object>() { { "selected", false }, { "id", "object_03_thorm" } });
        
        m_list.Add(new Dictionary<string, object>() { { "selected", false }, { "id", "object_03_thorm" } });
        m_list.Add(new Dictionary<string, object>() { { "selected", false }, { "id", "object_03_thorm" } });
        m_list.Add(new Dictionary<string, object>() { { "selected", false }, { "id", "object_03_thorm" } });
        m_list.Add(new Dictionary<string, object>() { { "selected", false }, { "id", "object_03_thorm" } });
        m_list.Add(new Dictionary<string, object>() { { "selected", false }, { "id", "object_03_thorm" } });
        m_list.Add(new Dictionary<string, object>() { { "selected", false }, { "id", "object_03_thorm" } });
        m_list.Add(new Dictionary<string, object>() { { "selected", false }, { "id", "object_03_thorm" } });
        */

        List<object> scroll_list = new List<object>();
        foreach (KeyValuePair<string, BlockBase> pair in BlockManager.Instance.BlockList)
        {
			Debug.Log( "pair : " + pair.Key );
            CustomScrollItem.ItemData item = new CustomScrollItem.ItemData(pair.Key, false);
            scroll_list.Add(item);
        }

        m_Scroll.Init(scroll_list);

    }

    protected CustomScrollItem.ItemData getData(int index)
    {
        return m_Scroll.m_allItems[index] as CustomScrollItem.ItemData;
    }

    protected CustomScrollItem.ItemData getData()
    {
        return getData(m_SelectIndex);
    }

    void refreshSelectedItem(int index, bool selected)
    {
        CustomScrollItem.ItemData data = getData(index);
        data.IsSelected = selected;
        m_Scroll.setData(index, data);
        if (selected)
            BlockManager.Instance.SelectBlockID = data.ItemID;
    }

    void OnBtnItem(tk2dUIItem item)
    {
        //Debug.Log("item name = " + item.name);
        CustomScrollItem icon = item.transform.parent.GetComponent<CustomScrollItem>();

        if (m_SelectIndex == icon.m_Index)
            return;
        if (m_SelectIndex != -1)
            refreshSelectedItem(m_SelectIndex, false);
        m_SelectIndex = icon.m_Index;
        refreshSelectedItem(m_SelectIndex, true);        
    }

    void OnToggleCustomMode(tk2dUIToggleButton btn)
    {
        if (btn.IsOn)
            GameManager.Instance.CustomMode = GameManager.eCustomMode.Editor;
        else
			GameManager.Instance.CustomMode = GameManager.eCustomMode.Delete;
    }

    void OnBtnSave()
    {
        BlockManager.Instance.saveCustom();
    }

    void OnBtnPause()
    {
        GameManager.Instance.exitInGame();
        UIMain.show();
    }

	public void OnSetClick()
	{
		GameManager.Instance.CustomMode = GameManager.eCustomMode.Editor;
		buttonSet.enabled = false;
		buttonDel.enabled = true;
	}

	public void OnDelClick()
	{
		GameManager.Instance.CustomMode = GameManager.eCustomMode.Delete;
		buttonSet.enabled = true;
		buttonDel.enabled = false;
	}

	private bool onMenu = false;
	public void OnMenuClick()
	{
		onMenu = !onMenu;
		objMenu.gameObject.SetActive( onMenu );
	}
}
