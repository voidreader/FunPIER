using UnityEngine;
using System.Collections;

[RequireComponent( typeof( BlockBase ) )]
public class object_02_push : MonoBehaviour {

	public BlockBase Base { get; private set; }
	BoxCollider2D m_Collider;
	public float terminalVelocity = -15f;

	private float length = 15;
	private float lookAhead;
	private int layerMask;
	private Vector2 extent;
	public Vector2 WorldPosition
	{
		get
		{
			return WorldExtent - ( Vector2 )( transform.rotation * new Vector2(0,-1) * length );
		}
	}

	public Vector2 WorldExtent
	{
		get
		{
			return transform.position + transform.rotation * extent;
		}
	}

	public Vector2 Velocity
	{
		get;
		protected set;
	}

	void Awake()
	{
		Base = GetComponent<BlockBase>();
		m_Collider = GetComponentInChildren<BoxCollider2D>( true );
	}

	public void AddVelocity( float x, float y )
	{
		Velocity = new Vector2( Velocity.x + x, Velocity.y + y );
		if ( Velocity.y < terminalVelocity )
			Velocity = new Vector2( Velocity.x, terminalVelocity );
	}

	void Update()
	{
		/*
		if ( GameManager.Instance.IsPlaying )
		{
			bool down = true;
			RaycastHit2D[] hit2D = GetRaycastHits();
			for ( int index = 0; index < hit2D.Length; ++index )
			{
				if ( hit2D[index].transform.gameObject.layer == 12 && !hit2D[index].transform.gameObject.Equals( gameObject ) )
					down = false;
			}
			if ( down )
			{
				AddVelocity( 0, PlatformerPro.TimeManager.FrameTime * Physics2D.gravity.y );
				Translate( 0, Velocity.y * PlatformerPro.TimeManager.FrameTime, true );
			}
		}
		/**/
	}

	public RaycastHit2D[] GetRaycastHits()
	{
		//Physics2D.RaycastAll()
		return Physics2D.RaycastAll( transform.position, transform.rotation * new Vector2( 0, -1 ), 0.70F );
		//return Physics2D.RaycastAll( WorldPosition, transform.rotation * new Vector2(0,-1), length + lookAhead, layerMask );
	}

	public void Translate( float x, float y, bool applyTransformsInWorldSpace )
	{
		if ( applyTransformsInWorldSpace )
		{
			//if ( gravity.IsGravityFlipped )
			//	y = -y;
			transform.Translate( x, y, 0, Space.World );
		}
	}
}
