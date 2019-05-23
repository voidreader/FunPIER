using UnityEngine;
using System.Collections;

public class FpsDisp : MonoBehaviour {

	public	float	update_int = 1.0f;
	private	float	accum = 0.0f;
	private	int		frames = 0;
	private	float	timeleft;

	// Use this for initialization
	void Start () {
		if( !GetComponent<GUIText>() ){
			enabled = false;
			print( "guiText Nothing!" );
			return ;
		}
		timeleft = update_int;
	}
	
	// Update is called once per frame
	void Update () {
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		++frames;
		if( timeleft<=0.0f ){
			int fps = (int)(accum/frames);
			GetComponent<GUIText>().text = "" + fps;
			timeleft = update_int;
			accum = 0.0f;
			frames = 0;
		}	
	}
}
