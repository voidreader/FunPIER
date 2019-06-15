using UnityEngine;
using System.Collections;

[RequireComponent( typeof( BlockBase ) )]
public class object_02_spring : MonoBehaviour
{
	public BlockBase Base { get; private set; }
	BoxCollider2D m_Collider;

	void Awake()
	{
		Base = GetComponent<BlockBase>();
		m_Collider = GetComponentInChildren<BoxCollider2D>( true );
	}

	void Update()
	{
		if ( GameManager.Instance.IsPlaying )
		{
			if ( GameManager.Instance.player.groundCheck_Middle_Right != null )
			{
				if ( m_Collider.OverlapPoint( GameManager.Instance.player.groundCheck_Middle_Right.transform.position ) )
				{
					GameManager.Instance.player.myCharacter.ActionMoveBlock();
				}
			}
		}
	}
}
