using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 블럭 시작위치 -540, -208
/// 블럭 끝위치 516, 272
/// 
/// Json Data key
/// "name" = 블럭 이름.
/// "x"
/// "y"
/// "rotate"
/// </summary>
[ExecuteInEditMode]
public class BlockBase : MonoBehaviour {

    public enum eBlockType
    {
        Entry,  // 입구.
        Exit,   // 출구.
        Block,  // 벽돌.
        EffectBlock, // 효과가 있는 벽돌.
        Weapon, // 무기 or 트랩등(플레이어를 공격)
    }

    /// <summary>
    /// 유저가 사망시 발생할 이펙트.
    /// </summary>
    public enum eBlockEffect
    {
        None,
        Bomb,
        Electric,
        Fire,
    }

    public enum eBlockRotateWay
    {
        Up, // 0
        Down, // 180
        Left, // 90
        Right, // 270
    }

    [System.Serializable]
    public class cBlockData
    {
        [Header("블록의 X 좌표. 좌표는 좌측 하단 기준. (0~44)")]
        public int X = 10;
        [Header("블록의 Y 좌표. 좌표는 좌측 하단 기준. (0~20)")]
        public int Y = 10;
        [Header("블록의 넓이")]
        public int Width = 1;
        [Header("블록의 높이")]
        public int Height = 1;
        [Header("블록의 회전 각도 방향")]
        public eBlockRotateWay RotateWay = eBlockRotateWay.Up;
        //public float Rotate = 0.0f;
        [Header("블록이 벽에 근접했을때 자동 회전 여부")]
        public bool IsAutoRotate = false;
        [Header("블록에 붙어 있어야 생성이 가능한 오브젝트 인지 여부")]
        public bool IsNearBlock = false;
        [Header("블록의 종류")]
        public eBlockType BlockType = eBlockType.Block;
        [Header("블록에 맞아서 케릭터 사망시 이펙트 종류")]
        public eBlockEffect BlockEffect = eBlockEffect.None;

        /// <summary>
        /// 방향에 따른 회전값을 가져옵니다.
        /// </summary>
        /// <param name="way"></param>
        /// <returns></returns>
        public float Rotate(eBlockRotateWay way)
        {
            switch(way)
            {
                case eBlockRotateWay.Up:
                    return 0.0f;
                case eBlockRotateWay.Down:
                    return 180.0f;
                case eBlockRotateWay.Left:
                    return 90.0f;
                case eBlockRotateWay.Right:
                    return 270.0f;
            }
            return 0.0f;
        }

        public float Rotate()
        {
            return Rotate(RotateWay);
        }

        /// <summary>
        /// 플레이어와의 회전값을 오브젝트에 맞춰서 변환해서 리턴합니다.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public float DistanceRotate(float angle)
        {
            angle = Rotate(RotateWay) - angle;
            switch (RotateWay)
            {
                case BlockBase.eBlockRotateWay.Up:
                case BlockBase.eBlockRotateWay.Down:
                    angle = angle - 180.0f;
                    break;
            }
            if (angle < 0)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return angle;
        }

        /// <summary>
        /// 발사체가 나가는 각도.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public float BulletRotate(float angle)
        {
            angle = angle - Rotate(RotateWay);
            switch (RotateWay)
            {
                case BlockBase.eBlockRotateWay.Up:
                case BlockBase.eBlockRotateWay.Down:
                    angle = angle - 180.0f;
                    break;
            }
            if (angle < 0)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return angle;
        }

        /// <summary>
        /// 방향에 따른 X축 부호값을 가져옵니다.
        /// </summary>
        /// <param name="way"></param>
        /// <returns></returns>
        public int SignX(eBlockRotateWay way)
        {
            switch (way)
            {
                case eBlockRotateWay.Up:
                case eBlockRotateWay.Right:
                    return 1;
                case eBlockRotateWay.Down:
                case eBlockRotateWay.Left:
                    return -1;
            }
            return 1;
        }

        /// <summary>
        /// 방향에 따른 Y축 부호값을 가져옵니다.
        /// </summary>
        /// <param name="way"></param>
        /// <returns></returns>
        public int SignY(eBlockRotateWay way)
        {
            switch (way)
            {
                case eBlockRotateWay.Up:
                case eBlockRotateWay.Right:
                    return 1;
                case eBlockRotateWay.Down:
                case eBlockRotateWay.Left:
                    return -1;
            }
            return 1;
        }

        public int SignX()
        {
            return SignX(RotateWay);
        }

        public int SignY()
        {
            return SignY(RotateWay);
        }
    }

    /// <summary>
    /// 블럭 배치 X축 시작점.
    /// </summary>
    public const float BlockStartX = -540;
    /// <summary>
    /// 블럭 배치 Y축 시작점.
    /// </summary>
    public const float BlockStartY = -208;
    /// <summary>
    /// 블럭 넓이.
    /// </summary>
    public const float BlockWidth = 24;
    /// <summary>
    /// 블럭 높이.
    /// </summary>
    public const float BlockHeight = 24;

    public const float m_BlockStartX = BlockStartX / GameConfig.PixelsPerMeter;
    public const float m_BlockStartY = BlockStartY / GameConfig.PixelsPerMeter;
    public const float m_BlockWidth = BlockWidth / GameConfig.PixelsPerMeter;
    public const float m_BlockHeight = BlockHeight / GameConfig.PixelsPerMeter;

    /// <summary>
    /// X축으로 생성되는 블럭 개수.
    /// </summary>
    public const int BlockCountX = 44;
    /// <summary>
    /// Y축으로 생성되는 블럭 개수.
    /// </summary>
    public const int BlockCountY = 20;

    //public Transform Image { get; private set; }

    [Header("자동으로 위치가 변경되는 이미지의 Transform")]
    [Tooltip("입력하지 않으면 자동리사이즈가 되지 않음.")]
    public Transform ImageTransform;
    [Header("애니메이션 오브젝트")]
    public tk2dSpriteAnimator Animated;
    [Header("자동으로 리사이즈 될 메인 콜라이더")]
    [Tooltip("입력하지 않으면 자동리사이즈가 되지 않음.")]
    public BoxCollider2D MainCollider;


    [Header("블럭 데이터")]
    public cBlockData BlockData;

    /// <summary>
    /// 자신과 연결된 블록이 있으면 여기에 넣는다.
    /// </summary>
    public List<BlockBase> LinkBlock = new List<BlockBase>();

    public System.Action DelegateReset { get; set; }

    void Awake()
    {
        //Init();        
        Setting();
    }
    /*
	// Use this for initialization
	void Start () {
        Setting();
	}
	*/
	// Update is called once per frame
	void Update () {
        if (!Application.isPlaying)
        {
            //Init();
            Setting();
        }
	}

    /*
    void Init()
    {        
        if (ImageTransform == null)
        {
            ImageTransform = transform.FindChild("Animated");
            Animated = ImageTransform.GetComponent<tk2dSpriteAnimator>();
            if (MainCollider == null)
                MainCollider = ImageTransform.GetComponent<BoxCollider2D>();
        }
    }
    */
    public void Setting(bool isReset=false)
    {
        if (BlockData != null)
        {
            if (isReset && DelegateReset != null)
                DelegateReset();

            SetPosition(BlockData.X, BlockData.Y, BlockData.RotateWay);
            float width = BlockData.Width * m_BlockWidth;
            float height = BlockData.Height * m_BlockHeight;
            if (MainCollider != null)
                MainCollider.size = new Vector2(width, height);
            if (ImageTransform != null)
                ImageTransform.localPosition = new Vector3((width - m_BlockWidth) * 0.5f, (height - m_BlockHeight) * 0.5f);
            
        }
    }

    void SetPosition()
    {
        //SetPosition(BlockData.X, BlockData.Y);
        transform.localPosition = ConvertPositionBlockToScreen(BlockData.X, BlockData.Y);
        transform.localEulerAngles = new Vector3(0, 0, BlockData.Rotate());
    }

    /// <summary>
    /// 해당 좌표의 포지션을 셋팅합니다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetPosition(int x, int y, eBlockRotateWay way)
    {
        BlockData.X = x;
        BlockData.Y = y;
        BlockData.RotateWay = way;
        SetPosition();
    }

    /// <summary>
    /// data를 json데이터로 변환합니다.
    /// </summary>
    /// <returns></returns>
    public JSONObject convertDataToJson()
    {
        JSONObject json = new JSONObject();
        string id = this.name.Replace("(Clone)", "");
        json.AddField("id", id);
        json.AddField("x", BlockData.X);
        json.AddField("y", BlockData.Y);
        //json.AddField("rotate", BlockData.Rotate());
        json.AddField("way", (int)BlockData.RotateWay);
        return json;
    }

    /// <summary>
    /// 블럭 좌표를 화면 좌표로 변환해준다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    static public Vector3 ConvertPositionBlockToScreen(int x, int y)
    {
        return new Vector3(m_BlockStartX + m_BlockWidth * 0.5f + m_BlockWidth * x, m_BlockStartY + m_BlockHeight * 0.5f + m_BlockHeight * y);
    }
    
    /// <summary>
    /// 화면 좌표를 블럭 좌표로 변환해준다.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    static public Vector2 ConvertPositionScreenToBlock(float x, float y)
    {
        if (x < BlockStartX || y < BlockStartY)
            return new Vector2(-1, -1);

        int X = (int)((x - BlockStartX) / BlockWidth);
        int Y = (int)((y - BlockStartY) / BlockHeight);

        if (X > BlockCountX || Y > BlockCountY)
            return new Vector2(-1, -1);

        return new Vector2(X, Y);
    }



}
