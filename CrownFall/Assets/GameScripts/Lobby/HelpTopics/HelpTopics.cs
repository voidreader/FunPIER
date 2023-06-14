using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class HelpTopics : MonoBehaviour {
	/////////////////////////////////////////
	//---------------------------------------
	public Sprite _texture = null;

	public string m_szSubject;
	public string m_szDescript;


	/////////////////////////////////////////
	//---------------------------------------
	public Sprite GetTexture () {
		return _texture;
	}
}
