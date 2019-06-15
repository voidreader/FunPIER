using UnityEngine;
using System.Collections;

[RequireComponent( typeof( BlockBase ) )]
public class object_02_jump : MonoBehaviour
{
	public BlockBase Base { get; private set; }
	BoxCollider2D m_Collider;

	[Header( "점프하는 높이" )]
	public int jumpHeight;

	void Awake()
	{
		Base = GetComponent<BlockBase>();
		m_Collider = GetComponentInChildren<BoxCollider2D>( true );
	}

	void Update()
	{
		if ( GameManager.Instance.IsPlaying )
		{
			if ( GameManager.Instance.player.groundCheck_Down_Left != null )
			{
				if ( m_Collider.OverlapPoint( GameManager.Instance.player.groundCheck_Down_Left.transform.position ) ||
					m_Collider.OverlapPoint( GameManager.Instance.player.groundCheck_Down_Right.transform.position ) )
				{
					GameManager.Instance.player.myCharacter.JumpBlock( jumpHeight );
				}
			}
		}
	}
}
