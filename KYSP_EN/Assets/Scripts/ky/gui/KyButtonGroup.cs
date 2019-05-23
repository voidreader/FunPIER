using UnityEngine;
using System.Collections;

public class KyButtonGroup : KyButton {

	public void Awake() {
		mChildButtons = GetButtonsInChildren();
	}

	public new void Start() {
		base.Start();
		//mChildButtons = GetButtonsInChildren();
	}

	public new void Update() {
		base.Update();
		
		if (SoftTouchChildren) {
			if ((EventFlag & EventType.ButtonSelected) != 0) {
				foreach (KyButton button in mChildButtons) {
					button.SoftTouch = true;
				}
			} else
			if ((EventFlag & EventType.ButtonNotSelected) != 0) {
				foreach (KyButton button in mChildButtons) {
					button.SoftTouch = false;
				}
			}
		}
	}

	public virtual void OnChildButtonSelected(KyButton child) {
		if (mSelectedButton != child) {
			if (mSelectedButton != null && !MultiSelection) {
				mSelectedButton.Selected = false;
			}
			mSelectedButton = child;
		}
		//Selected = true;
	}

	public KyButton GetChildButton(int index) {
		foreach (KyButton button in mChildButtons) {
			if (button.Index == index) {
				return button;
			}
		}
		return null;
	}

	public KyButton[] ChildButtons {
		get { return mChildButtons; }
	}

	public KyButton SelectedButton {
		get { return mSelectedButton; }
	}

	public int SelectedButtonIndex {
		get { 
			return mSelectedButton != null ? mSelectedButton.Index : -1; 
		}
		set {
			if (MultiSelection) {
				foreach (KyButton button in mChildButtons) {
					if (button.Index == value) {
						button.Selected = true;
						mSelectedButton = button;
						return;
					}
				}
			} else {
				mSelectedButton = null;
				foreach (KyButton button in mChildButtons) {
					if (button.Index == value) {
						button.Selected = true;
						mSelectedButton = button;
					} else {
						button.Selected = false;
					}
				}
			}
		}
	}

	public bool MultiSelection = false;
	public bool SoftTouchChildren = false;

	protected KyButton mSelectedButton = null;
	protected KyButton[] mChildButtons = null;
}
