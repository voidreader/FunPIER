using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlatformerPro;

public class Player : MonoBehaviour {

    public Character myCharacter { get; private set; }

    /// <summary>
    /// 바닥 체크용 transform
    /// </summary>
    public Transform groundCheck_Down_Left { get; private set; }
    /// <summary>
    /// 바닥 체크용 transform
    /// </summary>
    public Transform groundCheck_Down_Right { get; private set; }
    /// <summary>
    /// 머리 체크용 transform
    /// </summary>
    public Transform groundCheck_Up_Left { get; private set; }
    /// <summary>
    /// 머리 체크용 transform
    /// </summary>
    public Transform groundCheck_Up_Right { get; private set; }

	//kjh 케릭터의 앞뒷면 체크용. 

	/// <summary>
    /// 뒷면 체크용 transform
    /// </summary>
    public Transform groundCheck_Middle_Left { get; private set; }
    /// <summary>
    /// 앞면 체크용 transform
    /// </summary>
    public Transform groundCheck_Middle_Right { get; private set; }

	public BoxCollider2D playerBoxCollider;

    /// <summary>
    /// 애니메이션용.
    /// </summary>
	public tk2dSpriteAnimator Animated;// { get; private set; }

    Dictionary<string, GameObject> m_EffectList = new Dictionary<string, GameObject>();

    bool m_IsFlip = false;
    public bool IsFlip {
        set
        {
            if (m_IsFlip != value)
            {
                m_IsFlip = value;
                transform.localScale = new Vector3(m_IsFlip ? -1 : 1, transform.localScale.y, transform.localScale.z);
            }
        }
        get
        {
            return m_IsFlip;
        }
    }

	// Use this for initialization
	public void init () {
        myCharacter = GetComponent<Character>();
        //GameManager.Instance.player = this;
        //Animated = transform.FindChild("Animated").GetComponent<tk2dSpriteAnimator>();

        groundCheck_Down_Left = transform.Find("groundCheck_Down/Left");
        groundCheck_Down_Right = transform.Find("groundCheck_Down/Right");
        groundCheck_Up_Left = transform.Find("groundCheck_Up/Left");
        groundCheck_Up_Right = transform.Find("groundCheck_Up/Right");

		groundCheck_Middle_Left = transform.Find( "groundCheck_Middle/Left" );
		groundCheck_Middle_Right = transform.Find( "groundCheck_Middle/Right" );

        Transform Effects = transform.Find("Effects");
        for (int i=0; i<Effects.childCount; i++)
        {
            Transform effect = Effects.GetChild(i);
            m_EffectList[effect.name] = effect.gameObject;
            effect.gameObject.SetActive(false);
        }

        myCharacter.ChangeAnimationState += AnimationStateChanged;
	}

	public void LoadChar( string _charId )
	{
		if( Animated != null )
		{
			GameObject.Destroy( Animated.gameObject );
			//Animated = null;
		}
		GameObject obj = GameObject.Instantiate( Resources.Load( "Prefabs/Character/" + _charId ) ) as GameObject;
		obj.transform.parent = transform;
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = new Vector3( 0f, 0f, -1f / 20f );
		Animated = obj.GetComponent<tk2dSpriteAnimator>();
		PlatformerInputManager.Instance.reset();
	}

    void OnDestroy()
    {
        if (myCharacter != null) myCharacter.ChangeAnimationState -= AnimationStateChanged;
    }

	// Update is called once per frame
	void Update () {

        if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow) || UnityEngine.Input.GetKeyDown(KeyCode.D))
            KeyDown(KeyCode.RightArrow);
        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow) || UnityEngine.Input.GetKeyDown(KeyCode.A))
            KeyDown(KeyCode.LeftArrow);
        if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) || UnityEngine.Input.GetKeyDown(KeyCode.W))
            KeyDown(KeyCode.UpArrow);

        if (UnityEngine.Input.GetKeyUp(KeyCode.RightArrow) || UnityEngine.Input.GetKeyUp(KeyCode.D))
            KeyUp(KeyCode.RightArrow);
        if (UnityEngine.Input.GetKeyUp(KeyCode.LeftArrow) || UnityEngine.Input.GetKeyUp(KeyCode.A))
            KeyUp(KeyCode.LeftArrow);
        if (UnityEngine.Input.GetKeyUp(KeyCode.UpArrow) || UnityEngine.Input.GetKeyUp(KeyCode.W))
            KeyUp(KeyCode.UpArrow);

        IsFlip = myCharacter.LastFacedDirection == 1 ? false : true;

        /*
        //케릭터 모션.
        if (m_Rigidbody.velocity.y != 0.0f && !grounded)
        {
            if (!Animated.IsPlaying("jump"))
                Animated.Play("jump");
        }
        else if (m_Rigidbody.velocity.x != 0.0f)
        {
            if (!Animated.IsPlaying("run"))
                Animated.PlayFromFrame("run", 0);
            //Animated.ClipFps = 20;
        }
        else
        {
            if (Animated.Playing)
            {
                Animated.PlayFromFrame("run", 0);
                Animated.Stop();
            }
        }
        */

        // TEst
        if(UnityEngine.Input.GetKeyDown(KeyCode.N)) {
            StartCoroutine(NextFrame(() => { GameManager.Instance.ShowGameClear(); }, 0));
        }
    }

    public void KeyDown(KeyCode code)
    {
        Debug.Log("KeyDown = " + code);
		Debug.Log( "GameManager.Instance.GameMode : " + GameManager.Instance.GameMode );
        if (GameManager.Instance.GameMode == GameManager.eGameMode.InGameReady)
            GameManager.Instance.startInGame();
        if (GameManager.Instance.GameMode != GameManager.eGameMode.InGame)
            return;

        PlatformerInputManager.Instance.setKeyDown(code);

        /*
        switch (code)
        {
            case KeyCode.RightArrow: MovePos = Vector2.right; IsFlip = false; break;
            case KeyCode.LeftArrow: MovePos = Vector2.left; IsFlip = true; break;
            case KeyCode.UpArrow: IsBtnJump = true; IsBtnJumping = true; break;
        }
        */
    }

    public void KeyUp(KeyCode code)
    {
        if (GameManager.Instance.GameMode != GameManager.eGameMode.InGame)
            return;

        PlatformerInputManager.Instance.setKeyUp(code);

        //Debug.Log("KeyUp = " + code);
        /*
        if (code == KeyCode.UpArrow)
        {
            IsBtnJump = false;
            IsBtnJumping = false;
        }
        if ((code == KeyCode.RightArrow && MovePos == Vector2.right) ||
            (code == KeyCode.LeftArrow && MovePos == Vector2.left))
            MovePos = Vector2.zero;
        */
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.GameMode == GameManager.eGameMode.InGameReady)
            return;
    }

    void ShowEffect(string effectName, int _soundIndex, float showTime = 1.0f)
    {
		RPGSoundManager.Instance.PlayEffectSound( _soundIndex );
        StartCoroutine(cShowEffect(m_EffectList[effectName], showTime));
    }

    IEnumerator cShowEffect(GameObject effectObj, float showTime)
    {
        GameObject obj = GameManager.Instance.EffectLayer.addChild(effectObj);
        obj.transform.position = transform.position;

//        effectObj.SetActive(false);
        obj.SetActive(true);
		GameObject.Destroy( obj, showTime );

        yield return new WaitForSeconds(showTime);
        //GameManager.Instance.EffectLayer.removeChild(obj);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("OnTriggerEnter2D = " + other.tag);
        if (other.CompareTag("Exit"))
        {
			//Debug.Log( "other" );
            //GameManager.Instance.NextStage();
            StartCoroutine(NextFrame(() => { GameManager.Instance.ShowGameClear(); }, 0 ));
        }
        else if (other.CompareTag("Bullet"))
        {
			if ( GameManager.Instance.is_Test )
				return;
            //GameManager.Instance.ReStart();
            BlockBase parentBlock = other.gameObject.GetComponentInParent<BlockBase>();
            switch (parentBlock.BlockData.BlockEffect)
            {
				case BlockBase.eBlockEffect.Bomb:
				{
					ShowEffect( "NE_EF03", 3, 0.3f );
					StartCoroutine(NextFrame(() => { GameManager.Instance.ReStart(true); }, 0f));
				}
				break;
				case BlockBase.eBlockEffect.Electric:
				{
					ShowEffect( "NE_EF04", 4, 1f );
					StartCoroutine(NextFrame(() => { GameManager.Instance.ReStart(true); }, 1f));
				}
				break;
				case BlockBase.eBlockEffect.Fire:
				{
					//ShowEffect( "NE_EF05", 5, 0.6f );
					ShowEffect( "NE_EF05", 5, 1f );
					StartCoroutine(NextFrame(() => { GameManager.Instance.ReStart(true); }, 1f ));
				}
				break;
				default : StartCoroutine(NextFrame(() => { GameManager.Instance.ReStart(true); }, 0f));break;
            }
        }
    }
    
    IEnumerator NextFrame(System.Action action, float _time )
    {
		if ( _time == 0 )
			yield return new WaitForEndOfFrame();
		else
		{
			playerBoxCollider.enabled = false;
			Animated.gameObject.SetActive( false );
			yield return new WaitForSeconds( _time );
			playerBoxCollider.enabled = true;
			Animated.gameObject.SetActive( true );
		}//GameManager.Instance.NextStage();
        action();
    }

    public void stop()
    {
        /*
        MovePos = Vector2.zero;
        IsBtnJump = false;
        IsJumpUp = false;
        IsBtnJumping = false;
        JumpStep = 0;
        if (m_Rigidbody != null)
            m_Rigidbody.velocity = new Vector2(0, 0);
        */
        PlatformerInputManager.Instance.reset();
        //myCharacter.Respawn();

		//kjh:: 
		myCharacter.C_ActionMoveBlockStop();
    }

    void AnimationStateChanged(object sender, AnimationEventArgs args)
    {
        Debug.Log("AnimationStateChanged = " + args.State);

        switch (args.State)
        {
            case PlatformerPro.AnimationState.IDLE:
                if (Animated.Playing)
                {
                    //Animated.PlayFromFrame("run", 0);
                    //Animated.Stop();
                    Animated.Play("idle");
                }
                break;
            case PlatformerPro.AnimationState.JUMP:
            case PlatformerPro.AnimationState.AIRBORNE:
            case PlatformerPro.AnimationState.FALL:
                if (!Animated.IsPlaying("jump"))
                    Animated.Play("jump");
                break;
            case PlatformerPro.AnimationState.WALK:
                if (!Animated.IsPlaying("run"))
                    Animated.Play("run");
                break;
        }
    }
}
