using UnityEngine;
using System.Collections;

public class KyTaifuRoll : KyScriptObject {

	public	float	MoveTime = 4.0f;
	public	float	OutTime = 1.0f;
	public	float	WaitTime = 4.0f;
	public	float	RollSpeed = 8.0f;
	public	float	AddRadius = 10.0f;
	public	float	MaxRadius = 50.0f;
	public	float	SubRadius = 30.0f;
	
	public	float	lsx =0.0f;
	public	float	lsy =0.0f;

	private bool	init_update = true;
	private	Vector3	StartPosition = Vector3.zero;
	private bool	rolling = false;
	private int		rollmod = 0;
	private	float	rolllng = 0;
	private	float	rollang = 0;
	private float	WaitRest;


	// Use this for initialization
	protected override void Start () {
		base.Start();

		Vector3	pos = Vector3.zero;
		pos.y = -280.0f;
		transform.position = pos;
		WaitRest = WaitTime;
		init_update = true;
	}

	protected override void UpdateCore () {
		if(init_update){
			init_update = false;
			lsx = transform.localScale.x;
			lsy = transform.localScale.x;
		}	
		
		KySpriteAnimation sprite = this.GetComponent<KySpriteAnimation>();
		switch(sprite.AnimationIndex){
			case 0:{
					float movy = (280/MoveTime)*Time.deltaTime;
					Vector3	pos = transform.position;
					pos.y += movy;
					if(pos.y>0.0f){
						pos.y=0.0f;
						sprite.AnimationIndex = 2;
					}
					transform.position = pos;
					if (!CommandManager.PreviewMode) {
						if ((pos.y < -120) && (WaitRest > 0.0f) && (Input.GetMouseButton(0))) {
							sprite.AnimationIndex = 1;
						}
					}
				}break;
			case 1:{
					WaitRest-=Time.deltaTime;
					
					lsx +=(30.0f * Time.deltaTime);
					lsy +=(30.0f * Time.deltaTime);
					if( lsx > 220.0f ){
						CommandManager.SetVariable("magon", 1);
					}
					if (!CommandManager.PreviewMode) {
						if ((WaitRest < 0.0f) || (!Input.GetMouseButton(0))) {
							sprite.AnimationIndex = 0;
						}
					}
				}break;
			case 2:{
					if(!rolling){
						StartPosition = transform.position;
						rolling = true;
						rollmod = 0;
					}else{
						rollang-=(RollSpeed*Time.deltaTime);
						switch(rollmod){
							case 0:
								rolllng+=(AddRadius*Time.deltaTime);
								if(rolllng>MaxRadius){
									rolllng = MaxRadius;
									rollmod = 1;
								}
								break;
							case 1:
								rolllng-=(SubRadius*Time.deltaTime);
								if(rolllng<0){
									rolllng = 0;
									rollmod = 2;
									sprite.AnimationIndex = 3;
								}
								break;
						}
					}
					
					Vector3 pos = StartPosition;
				
					pos.x += (rolllng * Mathf.Cos( rollang ));
					pos.y += (rolllng * Mathf.Sin( rollang ));
					transform.position = pos;
				}break;
			case 3:{
					float movy = (280/OutTime)*Time.deltaTime;
					Vector3	pos = transform.position;
					pos.y += movy;
					pos.x += (movy/2.0f);
					if(pos.y>360.0f){
						pos.y=400.0f;
						pos.x=400.0f;
						sprite.AnimationIndex = 4;
					}
					transform.position = pos;				
				}break;
			case 4:
				break;
		}
		Vector3 scl = Vector3.one ;
		scl.x = lsx;
		scl.y = lsy;
		transform.localScale = scl ;
	}
}
