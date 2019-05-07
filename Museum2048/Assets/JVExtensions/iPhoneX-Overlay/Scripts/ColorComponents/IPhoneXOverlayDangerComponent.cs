using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]

public class IPhoneXOverlayDangerComponent : MonoBehaviour {

	public 	IPhoneXOverlay 	overlayManager;
	public	Image			imageDisplay;

	void Update () {
		imageDisplay.color		=	overlayManager.dangerColor;
		imageDisplay.enabled 	= 	overlayManager.showDangerArea;
	}

}