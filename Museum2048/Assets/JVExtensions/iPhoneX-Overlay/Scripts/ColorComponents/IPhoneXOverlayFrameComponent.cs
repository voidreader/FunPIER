using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]

public class IPhoneXOverlayFrameComponent : MonoBehaviour {

	public 	IPhoneXOverlay 	overlayManager;
	public	Image			imageDisplay;

	void Update () {
		imageDisplay.color		=	overlayManager.frameColor;
		imageDisplay.enabled 	= 	overlayManager.showFrame;
	}

}