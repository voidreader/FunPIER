using UnityEngine;
using System.Collections;

public class Collider_CallBack : MonoBehaviour
{
	public GameObject obj;

	void OnTriggerEnter2D( Collider2D other )
	{
		//Debug.Log( "OnTriggerEnter2D = " + other.tag );
		obj.SendMessage( "OnTriggerEnter2D", other );
	}
	
	void OnCollisionEnter2D( Collision2D _collision )
	{
		//Debug.Log( "_collision : " + _collision.gameObject.tag );

		obj.SendMessage( "OnCollisionEnter2D", _collision );
	}
}
