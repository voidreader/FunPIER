using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]

public class IPhoneXOverlaySafeComponent : MonoBehaviour {

	public 	IPhoneXOverlay 	overlayManager;
	public	Image			imageDisplay;

	void Update () {
		imageDisplay.color		=	overlayManager.safeColor;
		imageDisplay.enabled 	= 	overlayManager.showSafeArea;
	}

}