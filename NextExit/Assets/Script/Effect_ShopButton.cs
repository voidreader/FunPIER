using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_ShopButton : MonoBehaviour
{
	public List<Vector3> posList = new List<Vector3>();

	public float speed = 0.1f;

	private float timer = 0f;
	private float distance = 0;

	private int index_1 = 0;
	private int index_2 = 1;

	public Transform tranDummy_1;
	public Transform tranDummy_2;

	bool start = false;

	void Start ()
	{
		float distance = Vector2.Distance( posList[index_1] / 20f, posList[index_2] / 20f );
		start = true;
	}
	
	void Update ()
	{
		if ( start )
		{
			timer += Time.deltaTime * ( speed / distance );
			tranDummy_1.localPosition = Vector3.Lerp( posList[index_1], posList[index_2], timer ) / 20f;
			tranDummy_2.localPosition = Vector3.Lerp( posList[index_1], posList[index_2], timer ) / 20f;
			if ( timer >= 1f )
			{
				timer = 0f;
				index_1 = index_1 + 1 >= 4 ? 0 : index_1 + 1;
				index_2 = index_2 + 1 >= 4 ? 0 : index_2 + 1;
				distance = Vector2.Distance( posList[index_1] / 20f, posList[index_2] / 20f );
			}
		}
	}
}
