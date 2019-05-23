using UnityEngine;
using System.Collections;

public class KyMailKati : KyScriptObject {
	
	public GameObject KatiPrefab = null;
	public	int		katiMax = 3;
	private	GameObject[]	katikati;
	private int		katiCount = 0;
	private int		touchS = 0;

	public float[]	katiX = {-80,100,0,0};
	public float[]	katiY = {0,40,80,-80};

	// Use this for initialization
	protected override void Start () {
		base.Start();
		touchS = 3;
		katikati = new GameObject[ katiMax ];
		katiCount = 0;
		katikati[0] = (GameObject)GameObject.Instantiate(KatiPrefab);
		katikati[0].transform.parent = transform;	
		katikati[0].transform.position = transform.position;
	}
	
	// Update is called once per frame
	protected override void UpdateCore () {
		if(CommandManager.GetVariable("kati") > touchS){
			touchS +=3 ;
			katiCount ++;
			if(katiCount<katiMax){
				katikati[katiCount] = (GameObject)GameObject.Instantiate(KatiPrefab);
				katikati[katiCount].transform.parent = transform;
				Vector3	pos = transform.position;
				pos.x += katiX[katiCount];
				pos.y += katiY[katiCount];
				katikati[katiCount].transform.position = pos;
				
			}
		}
	}
}
