using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]

public class IPhoneXOverlayPortraitComponent : MonoBehaviour {

	public 	IPhoneXOverlay 	overlayManager;
	public 	Canvas		 	canvas;

	void Update () {
		canvas.enabled = overlayManager.portraitOvelay;
	}

}
