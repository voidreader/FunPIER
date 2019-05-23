using UnityEngine;
using System.Collections;

public class KyTennisUfo : MonoBehaviour {

	public	float[]	plist = {0.0f,90.0f,270.0f,180.0f,12.0f,170.0f,120.0f,320.0f};
	public	float[]	slist = {1.0f, 0.8f,  0.4f,  1.2f, 0.3f,  1.1f,  0.8f,  1.3f};
	public	float	leng = 40.0f;

	private	int		mod=0;
	private int		ptr=0;
	private	float	wait=0;
	private	Vector3 orgpos = Vector3.zero;
	private	Vector3 nowpos = Vector3.zero;
	private Vector3 movvec = Vector3.zero;

	// Use this for initialization
	void Start () {
		orgpos = transform.position;
		Vector3 pos = orgpos;
		pos.x += 140;
		transform.position = pos;
	}
	
	// Update is called once per frame
	void Update () {
		switch(mod){
			case 0:
				{
					Vector3 pos = orgpos;
					pos.x += (leng * Mathf.Cos( plist[ptr] * Mathf.Deg2Rad ));
					pos.y += (leng * Mathf.Sin( plist[ptr] * Mathf.Deg2Rad ));
					pos -= transform.position;
					movvec = pos;
					wait = slist[ptr];
				}
				mod ++;
				break;
			case 1:
				break;
		}
		nowpos = transform.position;
		nowpos.x += (movvec.x * (Time.deltaTime / slist[ptr]));
		nowpos.y += (movvec.y * (Time.deltaTime / slist[ptr]));
		transform.position = nowpos;
		wait -= Time.deltaTime;
		if(wait<=0.0f){
			ptr++;
			mod=0;
			if(ptr>=plist.Length){
				ptr =0;
			}
		}
	}
}
