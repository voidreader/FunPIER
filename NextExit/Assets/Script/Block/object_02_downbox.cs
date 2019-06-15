using UnityEngine;
using System.Collections;

[RequireComponent( typeof( BlockBase ) )]
public class object_02_downbox : MonoBehaviour
{

	public BlockBase Base
	{
		get;
		private set;
	}

	[Header( "발사체 Transform" )]
	public Transform BulletTrans;
	[Header( "오브첵트가 떨어지는 속도" )]
	public float MoveSpeed = 10f;// 최소 속도 10, 최대 속도 50.
	
	[Header( "오브첵트가 떨어지는 최도 속도" )]
	public float MoveSpeedMax = 50f;

	[Header( "오브첵트가 제자리로 가능 속도" )]
	public float UpMoveSpeed = 30f;

	BoxCollider2D m_Collider;

	public GameObject objUpDownbox = null;

	/// <summary>
	/// 및에 object_02_downbox 오브젝트 일경우.
	/// </summary>
	private object_02_downbox downtrans = null;

	public bool collisionGround = true;
	public bool collisionBullet = true;

	bool m_IsOverlap = false;

	void Awake()
	{
		Base = GetComponent<BlockBase>();
		m_Collider = GetComponentInChildren<BoxCollider2D>( true );

		Base.DelegateReset = () =>
		{
			if ( m_IsOverlap )
			{
				m_IsOverlap = false;
				StopCoroutine( "cOverlap" );
				StopCoroutine( "C_ObjectUp" );
				//BulletTrans.gameObject.SetActive( true );
			}
		};
	}


	void Update()
	{
		if ( GameManager.Instance.IsPlaying )
		{
			if ( !m_IsOverlap && ( m_Collider.OverlapPoint( GameManager.Instance.player.groundCheck_Down_Left.transform.position ) ||
					m_Collider.OverlapPoint( GameManager.Instance.player.groundCheck_Down_Right.transform.position ) ) )
			{
				Vector3 overlapPointLeft = new Vector3( GameManager.Instance.player.groundCheck_Down_Left.transform.position.x, BulletTrans.position.y );
				Vector3 overlapPointRight = new Vector3( GameManager.Instance.player.groundCheck_Down_Right.transform.position.x, BulletTrans.position.y );
				if ( m_Collider.OverlapPoint( overlapPointLeft ) ||
					m_Collider.OverlapPoint( overlapPointRight ) )
				{
					m_IsOverlap = true;
					//Debug.Log("Overlap");
					StartCoroutine( "cOverlap" );
					StopCoroutine( "C_ObjectUp" );
				}
			}
			else if( m_IsOverlap && !( m_Collider.OverlapPoint( GameManager.Instance.player.groundCheck_Down_Left.transform.position ) ||
					m_Collider.OverlapPoint( GameManager.Instance.player.groundCheck_Down_Right.transform.position ) ) )
			{
				m_IsOverlap = false;
				StopCoroutine( "cOverlap" );
				StartCoroutine( "C_ObjectUp" );
				MoveSpeed = 10f;
			}
		}
	}

	IEnumerator cOverlap()
	{
		float timer = 0f;
		float time = .05f;

		while ( true )
		{
			BulletTrans.localPosition -= new Vector3( 0, MoveSpeed / GameConfig.PixelsPerMeter * Time.deltaTime, 0 );
			if ( downtrans != null )
				downtrans.BulletTrans.localPosition -= new Vector3( 0, MoveSpeed / GameConfig.PixelsPerMeter * Time.deltaTime, 0 );
			if ( MoveSpeed < MoveSpeedMax )
			{
				timer += Time.deltaTime;
				if ( timer > time )
				{
					timer = 0f;
					MoveSpeed += 1f;
				}
			}
			yield return null;
		}
	}

	IEnumerator C_ObjectUp()
	{
		while ( true )
		{
			BulletTrans.localPosition -= new Vector3( 0, -( UpMoveSpeed / GameConfig.PixelsPerMeter * Time.deltaTime ), 0 );
			if ( downtrans != null )
			{
				downtrans.BulletTrans.transform.localPosition -= new Vector3( 0, -( UpMoveSpeed / GameConfig.PixelsPerMeter * Time.deltaTime ), 0 );
				float Distance_trans = Vector3.Distance( downtrans.BulletTrans.transform.localPosition, Vector3.zero );
				if ( Distance_trans < 0.1F )
				{
					downtrans.BulletTrans.localPosition = Vector3.zero;
					downtrans.objUpDownbox = null;
					downtrans = null;
				}
			}
			float Distance = Vector3.Distance( BulletTrans.localPosition, Vector3.zero );
			if ( Distance < 0.1F )
			{
				BulletTrans.localPosition = Vector3.zero;
				break;
			}
			yield return null;
		}
	}

	public void StopcOverlap()
	{
		StopCoroutine( "cOverlap" );
	}

	void OnCollisionEnter2D( Collision2D _collision )
	{
		if ( !GameManager.Instance.IsPlaying )
			return;
		//Debug.Log( "_collision : " + _collision.gameObject.tag );
		if ( collisionGround )
		{
			if ( _collision.gameObject.layer == 12 )
			{
				bool down = true;
				RaycastHit2D[] hit2D = GetRaycastHits();
				for ( int index = 0; index < hit2D.Length; ++index )
				{
					if ( hit2D[index].transform.gameObject.layer == 12 && !hit2D[index].transform.gameObject.Equals( BulletTrans.gameObject ) )
					{
						if ( hit2D[index].transform.parent.tag.Equals( "ground_downbox" ) )
						{
							downtrans = hit2D[index].transform.parent.gameObject.GetComponent<object_02_downbox>();
							downtrans.objUpDownbox = gameObject;
						}
						else
						{
							down = false;
						}
					}
				}

				if ( !down )
				{
					if ( objUpDownbox == null )
						StopCoroutine( "cOverlap" );
					else
						objUpDownbox.SendMessage( "StopcOverlap" );
				}
			}
		}
	}

	public RaycastHit2D[] GetRaycastHits()
	{
		return Physics2D.RaycastAll( BulletTrans.position, BulletTrans.rotation * new Vector2( 0, -1 ), 0.70F );
	}

	void OnTriggerEnter2D( Collider2D other )
	{
		if ( !GameManager.Instance.IsPlaying )
			return;
		//Debug.Log( "OnTriggerEnter2D = " + other.tag );
		if ( collisionBullet )
		{
			if ( other.tag.Equals( "Bullet" ) )
			{
				if ( objUpDownbox == null )
					StopCoroutine( "cOverlap" );
				else
					objUpDownbox.SendMessage( "StopcOverlap" );
			}
		}
	}
}
