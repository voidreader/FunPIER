using UnityEngine;
using System.Collections;

public class KyTennisBall : MonoBehaviour {

	public	float	grav = 0.06f;
	public	float	sped = 10.0f;
	private	float	down = 0.0f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		KySpriteAnimation sprite = this.GetComponent<KySpriteAnimation>();
		switch(sprite.AnimationIndex){
			case 0:
				break;
			case 1:{
				Vector3	pos = transform.position;
				if(pos.x < 280.0f){
					pos.x += (sped*Time.deltaTime*60.0f);
					pos.y -= (down*Time.deltaTime*60.0f);
					transform.position = pos;
					down += grav;
				}else{
					down = 0.0f;
				}
			}break;
			case 2:{
				Vector3	pos = transform.position;
				if(pos.x > -280.0f){
					pos.x -= (sped*Time.deltaTime*60.0f);
					pos.y -= (down*Time.deltaTime*60.0f);
					transform.position = pos;
					down += grav;
				}else{
					down = 0.0f;
				}
			}break;
		}				
	}
}
