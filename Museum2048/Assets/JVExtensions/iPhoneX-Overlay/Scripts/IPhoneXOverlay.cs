using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class IPhoneXOverlay : MonoBehaviour {

	[Header("Display")]
	public	bool	landscapeOverlay	=	true;
	public	bool	portraitOvelay		=	false;

	[Header("Components")]
	public	bool	showFrame		=	true;
	public	bool	showSafeArea	=	true;
	public	bool	showDangerArea	=	true;

	[Header("Colors")]
	public	Color	frameColor		=	Color.grey;
	public	Color	safeColor		=	Color.green;
	public	Color	dangerColor		=	Color.red;

	void Start () {
		if (Application.isPlaying) {
			DontDestroyOnLoad(gameObject);
		}
	}

	void Update () {
		
	}

}