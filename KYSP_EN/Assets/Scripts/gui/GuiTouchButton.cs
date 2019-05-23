using UnityEngine;
using System.Collections;

public class GuiTouchButton : GuiButton {

	#region MonoBehaviour Methods

	void Start () {
	
	}

	#endregion

	#region Methods

	protected virtual void UpdateInput() {
		if (State == ButtonState.Disabled) { return; }
		if (State == ButtonState.Up) {
			if (Input.GetMouseButton(0)) {
				if (HitTest(Camera.main.ScreenToWorldPoint(Input.mousePosition))) {
					mNextState = ButtonState.Selected;
					mStateChanged = true;
				}
			}
		}
	}

	#endregion

	#region Fields

	#endregion
}
