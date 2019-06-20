using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockManager : RPGSingleton<BlockManager> {

	/*
	void Start()
	{
		JSONObject json = new JSONObject();
		for ( int index = 0; index < 5; ++index )
			json.Add( "stage" + ( index + 1 ).ToString() );
		string customData = json.print();
		Debug.Log( "customData : " + customData );
		// 데이터 압축.
		string compressData = Zipper.ZipString( customData );
		// 데이터 암호화.
		string cryptData = RPGAesCrypt.Encrypt( compressData );
		Debug.Log( "cryptData : " + cryptData );
	}
	/**/
    /// <summary>
    /// 커스텀 블럭 리스트를 데이터로 변환합니다.
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string ListToMapData(List<BlockBase> list)
    {
        JSONObject json = new JSONObject();
        json.AddField("version", GameConfig.MapDataVersion);
        JSONObject blockJson = new JSONObject(JSONObject.Type.ARRAY);
        for (int i = 0; i < list.Count; i++)
        {
            BlockBase block = list[i];
            blockJson.Add(block.convertDataToJson());
        }
        json.AddField("block", blockJson);
        string customData = json.print();
        // 데이터 압축.
        string compressData = Zipper.ZipString(customData);
        // 데이터 암호화.
        string cryptData = RPGAesCrypt.Encrypt(compressData);

        return cryptData;
    }

    /// <summary>
    /// 커스텀 맵 데이터를 리스트로 변환합니다.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ArrayList MapDataToList(string cryptData)
    {
        // 데이터 복호화.
        string compressData = RPGAesCrypt.Decrypt(cryptData);
        // 데이터 압축해제.
        string customData = Zipper.UnzipString(compressData);
        // json 변환.
        JSONObject json = new JSONObject(customData);
		//Debug.Log( "json : " + json.print() );
        // dictionary 변환.
        Dictionary<string, object> data = json.ToDictionary();
        // 버전정보.
        string version = data["version"].ToString();
        //Debug.Log("mapData version = " + version);
        // array로 변환.
        ArrayList blockList = data["block"] as ArrayList;
        return blockList;
    }

    public static ArrayList MapDateToListDecrypted(string map) {
        JSONObject json = new JSONObject(map);
        //Debug.Log( "json : " + json.print() );
        // dictionary 변환.
        Dictionary<string, object> data = json.ToDictionary();
        // 버전정보.
        string version = data["version"].ToString();
        //Debug.Log("mapData version = " + version);
        // array로 변환.
        ArrayList blockList = data["block"] as ArrayList;
        return blockList;
    }

    /*
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    */

    [Header("인게임 블록이 생성되는 레이어.")]
    public RPGLayer GameBlockLayer;
    [Header("커스텀 블록이 생성되는 레이어.")]
    public RPGLayer CustomBlockLayer;

    public string SelectBlockID { get; set; }

    Dictionary<string, BlockBase> m_BlockList = new Dictionary<string, BlockBase>();

    public Dictionary<string, BlockBase> BlockList { get { return m_BlockList; } }

    /// <summary>
    /// 커스텀맵 제작중의 블럭 리스트.
    /// </summary>
    List<BlockBase> m_CustomBlockList = new List<BlockBase>();
    /// <summary>
    /// 게임 플레이중의 블럭 리스트.
    /// </summary>
    List<BlockBase> m_GameBlockList = new List<BlockBase>();

    /// <summary>
    /// 해당 위치에 블럭이 존재하는지 체크.
    /// </summary>
    BlockBase[,] m_IsHaveBlock = new BlockBase[BlockBase.BlockCountX + 1, BlockBase.BlockCountY + 1];

    /// <summary>
    /// 입구 오브젝트.
    /// </summary>
    public object_01_entry Object_Entry { get; private set; }
    /// <summary>
    /// 출구 오브젝트.
    /// </summary>
    public object_01_exit Object_Exit { get; private set; }

    public override void Init()
    {
        base.Init();

        SelectBlockID = "";
        BlockBase[] Blocks = GetComponentsInChildren<BlockBase>(true);
        for (int i=0; i<Blocks.Length; i++)
        {
            BlockBase block = Blocks[i];
			m_BlockList[block.name] = block;

            block.gameObject.SetActive(false);
        }

    }

    public void clearAll()
    {
        clearGameBlock();
        clearCustomBlock();
    }

    public void clearGameBlock()
    {
        GameBlockLayer.removeAllChild();
        Object_Entry = null;
        Object_Exit = null;
        m_GameBlockList.Clear();
    }

    public void clearCustomBlock()
    {
        CustomBlockLayer.removeAllChild();
        System.Array.Clear(m_IsHaveBlock, 0, m_IsHaveBlock.Length);
        m_CustomBlockList.Clear();
    }

    /// <summary>
    /// 모든 블럭을 리셋합니다.
    /// </summary>
    public void resetAllBlock()
    {
        for (int i = 0; i < m_GameBlockList.Count; i++)
        {
            BlockBase block = m_GameBlockList[i];
            block.Setting(true);
        }
    }

    public BlockBase getBlock(string name)
    {
        if (!m_BlockList.ContainsKey(name))
        {
            Debug.LogError("not found block id = " + name);
            return null;
        }
        return m_BlockList[name];
    }

    /// <summary>
    /// 입구를 찾습니다.
    /// </summary>
    /// <returns></returns>
    public object_01_entry getEntry()
    {
        if (Object_Entry == null)
            Object_Entry = GameBlockLayer.transform.GetComponentInChildren<object_01_entry>();
        return Object_Entry;
    }

    /// <summary>
    /// 출구를 찾습니다.
    /// </summary>
    /// <returns></returns>
    public object_01_exit getExit()
    {
        if (Object_Exit == null)
            Object_Exit = GameBlockLayer.transform.GetComponentInChildren<object_01_exit>();
        return Object_Exit;
    }

    /// <summary>
    /// 중복되는 블럭이 있는지 체크합니다.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public bool overlapCheck(BlockBase block, int x, int y, BlockBase.eBlockRotateWay way)
    {
        for (int xx = 0; xx < block.BlockData.Width; xx++)
        {
            for (int yy = 0; yy < block.BlockData.Height; yy++)
            {
                int _x = x + (xx * block.BlockData.SignX(way));
                int _y = y + (yy * block.BlockData.SignY(way));
                if (_x > BlockBase.BlockCountX || _y > BlockBase.BlockCountY)
                {
                    // 블럭을 놓을 수 없는 위치 입니다.
                    MessagePrint.show(DefineMessage.getMsg(10002));
                    return false;
                }
                if (m_IsHaveBlock[_x, _y] != null)
                {
                    // 해당 위치에 이미 배치된 블럭이 있습니다.
                    MessagePrint.show(DefineMessage.getMsg(10001));
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 해당 타입의 블록이 존재하는지 확인.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool checkHaveBlockType(BlockBase.eBlockType type)
    {
        for (int i=0; i<m_CustomBlockList.Count; i++)
        {
            if (m_CustomBlockList[i].BlockData.BlockType == type)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 커스텀 모드에서 블럭을 생성합니다.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void createBlock(string id, int x, int y)
    {
		//Debug.Log( "createBlock " + "id : " + id + "x : " + x + "y : " + y );
        BlockBase block = getBlock(id);
        
        BlockBase baseBlock = null;
        BlockBase.eBlockRotateWay way = BlockBase.eBlockRotateWay.Up;
        // 벽에 붙어야 하는 오브젝트는 벽이 있는지 체크를 먼저 한다. 4방향 체크.
        if (block.BlockData.IsNearBlock)
        {
            if (!checkNearBlock(x, y, ref way, ref baseBlock))
            {
                //근접한 블록이 있어야 합니다.
                MessagePrint.show(DefineMessage.getMsg(10007));
                return;
            }
        }
        if (!block.BlockData.IsAutoRotate)
            way = BlockBase.eBlockRotateWay.Up;

        if (overlapCheck(block, x, y, way))
        {
            // 입구가 이미 생성되었는지 체크.
            if (block.BlockData.BlockType == BlockBase.eBlockType.Entry)
            {
                if (checkHaveBlockType(block.BlockData.BlockType))
                {
                    //입구는 한개만 배치할 수 있습니다.
                    MessagePrint.show(DefineMessage.getMsg(10003));
                    return;
                }
            }
            // 출구가 이미 생성되었는지 체크.
            else if (block.BlockData.BlockType == BlockBase.eBlockType.Exit)
            {
                if (checkHaveBlockType(block.BlockData.BlockType))
                {
                    //출구는 한개만 배치할 수 있습니다.
                    MessagePrint.show(DefineMessage.getMsg(10004));
                    return;
                }
            }

            // 블록 생성.
            GameObject objBlock = CustomBlockLayer.addChild(block.gameObject);
            objBlock.SetActive(true);
            BlockBase copyBlock = objBlock.GetComponent<BlockBase>();
            copyBlock.SetPosition(x, y, way);
            if (baseBlock != null)
                baseBlock.LinkBlock.Add(copyBlock);

            m_CustomBlockList.Add(copyBlock);

            // 해당위치를 블럭이 있음 상태로 변경.
            for (int xx = 0; xx < block.BlockData.Width; xx++)
            {
                for (int yy = 0; yy < block.BlockData.Height; yy++)
                {
                    int _x = x + (xx * block.BlockData.SignX(way));
                    int _y = y + (yy * block.BlockData.SignY(way));

                    m_IsHaveBlock[_x, _y] = copyBlock;
                }
            }
        }
    }

    /// <summary>
    /// 해당 위치의 블록을 찾습니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public BlockBase findBlock(int x, int y)
    {
        if (x > BlockBase.BlockCountX || y > BlockBase.BlockCountY)
            return null;
        return m_IsHaveBlock[x, y];
    }

    /// <summary>
    /// 가까운 블록을 찾고 각도값과 링크된 블록을 가져온다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="rotate"></param>
    /// <param name="baseBlock"></param>
    /// <returns></returns>
    bool checkNearBlock(int x, int y, ref BlockBase.eBlockRotateWay way, ref BlockBase baseBlock)
    {
        // 아래.
        if (checkNearBlock(x, y - 1, ref baseBlock))
        {
            way = BlockBase.eBlockRotateWay.Up;
            return true;
        }
        // 위.
        else if (checkNearBlock(x, y + 1, ref baseBlock))
        {
            way = BlockBase.eBlockRotateWay.Down;
            return true;
        }
        // 오른쪽.
        else if (checkNearBlock(x + 1, y, ref baseBlock))
        {
            way = BlockBase.eBlockRotateWay.Left;
            return true;
        }
        // 왼쪽.
        else if (checkNearBlock(x - 1, y, ref baseBlock))
        {
            way = BlockBase.eBlockRotateWay.Right;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 해당 위치에 블록이 있는지 체크.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool checkNearBlock(int x, int y, ref BlockBase baseBlock)
    {
        if (y == -1) // 아래 테투리 벽 체크
            return true;
        if (y == BlockBase.BlockCountY + 1) // 위 테두리 벽 체크.
            return true;
        if (x == -1) // 왼쪽 테두리 벽 체크.
            return true;
        if (x == BlockBase.BlockCountX + 1) // 오른쪽 테두리 벽 체크.
            return true;

        BlockBase b = findBlock(x, y);
        if (b != null && b.BlockData.BlockType == BlockBase.eBlockType.Block)
        {
            baseBlock = b;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 생성된 블록을 제거합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void deleteBlock(int x, int y)
    {
        if (m_IsHaveBlock[x, y] == null)
            return;
        deleteBlock(findBlock(x, y));
    }

    public void deleteBlock(BlockBase block)
    {
        if (block == null)
            return;

        for (int xx = 0; xx < block.BlockData.Width; xx++)
        {
            for (int yy = 0; yy < block.BlockData.Height; yy++)
            {
                int _x = block.BlockData.X + (xx * block.BlockData.SignX());
                int _y = block.BlockData.Y + (yy * block.BlockData.SignY());

                if (_x <= BlockBase.BlockCountX && _y <= BlockBase.BlockCountY)
                    m_IsHaveBlock[_x, _y] = null;
            }
        }
        m_CustomBlockList.Remove(block);
        CustomBlockLayer.removeChild(block.gameObject);
        for (int i = 0; i < block.LinkBlock.Count; i++)
            deleteBlock(block.LinkBlock[i]);
    }

    /// <summary>
    /// 맵 데이터 저장 팝업 띄우고 맵 저장.
    /// </summary>
    public void saveCustom()
    {
        if (!checkHaveBlockType(BlockBase.eBlockType.Entry))
        {
            //입구를 배치해야 합니다.
            MessagePrint.show(DefineMessage.getMsg(10005));
            return;
        }
        if (!checkHaveBlockType(BlockBase.eBlockType.Exit))
        {
            //출구를 배치해야 합니다.
            MessagePrint.show(DefineMessage.getMsg(10006));
            return;
        }

        if (GameManager.Instance.IsAdminMode)
        {
            // 관리자 모드에서는 스테이지를 등록합니다.
            MessageInput input = MessageInput.show();
            //스테이지 이름을 입력하세요
            input.setMessage(DefineMessage.getMsg(20001));
            input.addYesButton((m) =>
            {
                if (m.Text.Length == 0)
                {
                    //스테이지 이름은 반드시 입력해야 합니다.
                    MessagePrint.show(DefineMessage.getMsg(20002));
                    return;
                }
                m.Close();
                RPGDefine.writeStringToFile(ListToMapData(m_CustomBlockList), GameConfig._SaveFileFullPath + m.Text + GameConfig._Extention);
            });
            input.addNoButton();
            input.addCloseButton();
        }
        else
        {
            // 관리자 모드가 아닌경우에는 나의 인벤토리에 추가합니다.
            if (DataSaveManager.Instance.ModifyIndex == 0)
            {
                MessageInput input = MessageInput.show();
                //스테이지 이름을 입력하세요
                input.setMessage(DefineMessage.getMsg(20001));
                input.addYesButton((m) =>
                {
                    if (m.Text.Length == 0)
                    {
                        //스테이지 이름은 반드시 입력해야 합니다.
                        MessagePrint.show(DefineMessage.getMsg(20002));
                        return;
                    }
                    m.Close();
                    DataSaveManager.Instance.Save(ListToMapData(m_CustomBlockList), m.Text);
                    UICustomMyPage.show(0, 2);
                });
                input.addNoButton();
                input.addCloseButton();
            }
            else
            {
                MessageBox box = MessageBox.show();
                //저장 하시겠습니까?
                box.setMessage(DefineMessage.getMsg(10008));
                box.addYesButton((m) =>
                {
                    m.Close();
                    DataSaveManager.Instance.Modify(ListToMapData(m_CustomBlockList));
                    UICustomMyPage.show(0, 2);
                });
                box.addNoButton();
                box.addCloseButton();
            }
        }
    }

    /// <summary>
    /// 커스텀 맵을 로드하는 팝업을 띄우고 로드합니다.
    /// </summary>
    public void loadCustom()
    {
        MessageInput input = MessageInput.show();
        //스테이지 이름을 입력하세요
        input.setMessage(DefineMessage.getMsg(20001));
        input.addYesButton((m) =>
        {
            if (m.Text.Length == 0)
            {
                //스테이지 이름은 반드시 입력해야 합니다.
                MessagePrint.show(DefineMessage.getMsg(20002));
                return;
            }

            string cryptData = RPGDefine.readStringFromFile(GameConfig._SaveFileFullPath + m.Text + GameConfig._Extention);
            if (cryptData.Length == 0)
            {
                //존재하지 않는 스테이지 입니다.
                MessagePrint.show(DefineMessage.getMsg(20003));
                return;
            }

            m.Close();
            loadCustom(cryptData);
            GameManager.Instance.startInGameReady(GameManager.ePlayMode.Custom);
        });
        input.addNoButton();
        input.addCloseButton();
    }

    /// <summary>
    /// 암호화된 커스텀 데이터를 가지고 플레이 맵을 생성합니다.
    /// </summary>
    /// <param name="cryptData"></param>
    public void loadCustom(string cryptData)
    {
        ArrayList blockList = MapDataToList(cryptData);
        loadCustom(blockList);
    }

    /// <summary>
    /// 블럭리스트를 가지고 플레이 맵을 생성합니다.
    /// </summary>
    /// <param name="blockList"></param>
    public void loadCustom(ArrayList blockList)
    {
        clearGameBlock();

        for (int i = 0; i < blockList.Count; i++)
        {
            Dictionary<string, object> dic = blockList[i] as Dictionary<string, object>;
            string id = ParsingDictionary.ToString(dic, "id");
            int x = ParsingDictionary.ToInt(dic, "x");
            int y = ParsingDictionary.ToInt(dic, "y");
            BlockBase.eBlockRotateWay way = (BlockBase.eBlockRotateWay)ParsingDictionary.ToInt(dic, "way");

            BlockBase block = getBlock(id);

            // 블록 생성.
            GameObject objBlock = GameBlockLayer.addChild(block.gameObject);
            objBlock.SetActive(true);
            BlockBase copyBlock = objBlock.GetComponent<BlockBase>();
            copyBlock.SetPosition(x, y, way);
            m_GameBlockList.Add(copyBlock);
        }
    }


	/// <summary>
	/// 블럭리스트를 가지고 플레이 맵을 생성합니다.
	/// </summary>
	/// <param name="blockList"></param>
	public void loadCustom( ArrayList blockList, RPGLayer _layer )
	{
		_layer.removeAllChild();
		for ( int i = 0; i < blockList.Count; i++ )
		{
			Dictionary<string, object> dic = blockList[i] as Dictionary<string, object>;
			string id = ParsingDictionary.ToString( dic, "id" );
			int x = ParsingDictionary.ToInt( dic, "x" );
			int y = ParsingDictionary.ToInt( dic, "y" );
			BlockBase.eBlockRotateWay way = ( BlockBase.eBlockRotateWay )ParsingDictionary.ToInt( dic, "way" );

			BlockBase block = getBlock( id );
			// 블록 생성.
			GameObject objBlock = _layer.addChild( block.gameObject );
			objBlock.SetActive( true );
			BlockBase copyBlock = objBlock.GetComponent<BlockBase>();
			copyBlock.SetPosition( x, y, way );
		}
	}
}
