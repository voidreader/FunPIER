using UnityEngine; 
using System.Collections;

public class KyDominoMgr : KyScriptObject {

	public GameObject DominoPrefab = null;
	public GameObject OmaePrefab = null;
	public GameObject OmaePrefabR = null;

	public	float			downSpeed = 80.0f;
	public	float			downSpeed2 = 40.0f;
	public	float			dominoStart = 240.0f;
	public	float			dominoSpace = 60.0f;
	public	float			dominoYPos = -90.0f;
	public	int				dominoCount = 9;
	public	int				dominoOmae = 5;
	
	private	GameObject[]	blackDomino;
	private	int[]			dominoMode;
	private	float[]			dominoAng;
	
	private GameObject		testRed;

	// Use this for initialization
	protected override void Start () {
		base.Start();

		blackDomino = new GameObject[dominoCount];
		dominoMode = new int[dominoCount];
		dominoAng = new float[dominoCount];
		for (int i=0;i<dominoCount;i++){
			if(i != dominoOmae){
				blackDomino[i] = (GameObject)GameObject.Instantiate(DominoPrefab);
			}else{
				blackDomino[i] = (GameObject)GameObject.Instantiate(OmaePrefab);
			}
			dominoMode[i] = 0;
			dominoAng[i] = 0.0f;
			blackDomino[i].transform.parent = transform;	
			Vector3	pos = blackDomino[i].transform.position ;
			pos.x = dominoStart - (dominoSpace * i);
			pos.y = dominoYPos;
			blackDomino[i].transform.position = pos ;
		}
		dominoMode[0] = 1;
	}
	
	// Update is called once per frame
	protected override void UpdateCore () {
		for (int i=0;i<dominoCount;i++){
			if(dominoDown(i)){
				if(i<dominoCount-1){
					if(i+1 != dominoOmae){
						if(dominoMode[i+1]==0){
							dominoMode[i+1]=1;
						}
					}
				}
			}
		}
		if(dominoMode[ dominoOmae ]==0){
			if(CommandManager.GetVariable("mov") > 0.0f){
				dominoMode[ dominoOmae ]=1;
				if(dominoMode[ dominoOmae-1 ]<2){
					CommandManager.SetVariable("res",2);
				}else{
					CommandManager.SetVariable("res",1);
				}
			}
			if(dominoMode[ dominoOmae-1 ]==0){
				if(CommandManager.GetVariable("mov") < 0.0f){
					dominoMode[ dominoOmae ]=5;
					dominoMode[ dominoOmae-1 ]=6;
					omaeRevers();
					CommandManager.SetVariable("res",3);
				}
			}
		}
		if(dominoMode[ dominoOmae-1 ]>=1){
			CommandManager.SetVariable("stp",1);
		}
		if(dominoMode[ dominoOmae-1 ]==4){
			if((dominoMode[ dominoOmae ]!=0)&&(dominoMode[ dominoOmae ]<=5)){
				dominoMode[ dominoOmae-1 ]=2;
			}
		}
	}

	private	bool	dominoDown(int i){
		if(Time.deltaTime>=0.1f){
			return false;
		}
		
		bool next = false;
		if(dominoMode[i]==1){
			dominoAng[i] += (downSpeed * Time.deltaTime);
			Vector3 rot = Vector3.zero;
			rot.z = (downSpeed * Time.deltaTime);
			blackDomino[i].transform.Rotate(rot);
			if(dominoAng[i]>=42.0f){
				KyAudioManager.Instance.PlayOneShot("se_domino");
				if(( i+1 < dominoCount)&&(i+1 != dominoOmae)){
					if(dominoMode[i+1]==0){
						dominoMode[i]=2;
						next = true;
					}else{
						dominoMode[i]=4;
					}						
				}else{
					if(dominoMode[ dominoOmae ]==0){
						dominoMode[i]=4;
					}else if(dominoMode[ dominoOmae ]>=5){
						dominoMode[i]=4;
					}else{
						dominoMode[i]=2;
					}
				}
			}
		}else
		if(dominoMode[i]==5){
			dominoAng[i] += (downSpeed * Time.deltaTime);
			Vector3 rot = Vector3.zero;
			rot.z = 0.0f-(downSpeed * Time.deltaTime);
			blackDomino[i].transform.Rotate(rot);
			if(dominoAng[i]>=42.0f){
				KyAudioManager.Instance.PlayOneShot("se_domino");
				dominoMode[i]=6;
				dominoMode[i-1]=6;
			}
		}else
		if(dominoMode[i]==2){
			if(dominoAng[i]<75.0f){
				dominoAng[i] += (downSpeed2 * Time.deltaTime);
				Vector3 rot = Vector3.zero;
				rot.z = (downSpeed2 * Time.deltaTime);
				blackDomino[i].transform.Rotate(rot);
			}else{
				dominoMode[i]=3;
			}
		}
		return next;
	}
	
	private void	omaeRevers(){
		Vector3 vec = blackDomino[dominoOmae].transform.position ;
		Destroy(blackDomino[dominoOmae]);
		blackDomino[dominoOmae] = (GameObject)GameObject.Instantiate(OmaePrefabR);
		blackDomino[dominoOmae].transform.parent = transform;
		vec.x += 16;
		blackDomino[dominoOmae].transform.position = vec;
	}

}
