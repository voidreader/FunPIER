using UnityEngine;
using System.Collections;

public class GuiComponent : MonoBehaviour {

	protected virtual bool OnMouseInput() {
		return false;	
	}

	protected bool DispatchMouseInput() {
		//foreach (Transform child in transform) {
		//	child.GetComponent<GuiComponent>
		//
		return false;
	}

	public virtual bool HitTest(Vector3 pos) {
		return false;
	}

	public bool GuiEnabled = true;
}
