using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GuiButtonGroup : GuiComponent {

	#region Inner Classes

	public enum TouchState {
		None = 0,
		Touching,
		TouchMoving,
	}

	#endregion

	#region MonoBehaviour Methods

	protected void Awake() {
		foreach (GuiButton button in InitialButtons) {
			AddButton(button);
		}
		InitialButtons = null;
	}

	#endregion

	#region Methods

	public void SetGuiEnabled(bool enabled/*, bool children*/) {
		GuiEnabled = enabled;
		//if (children) {
			foreach (GuiButton button in mButtons) {
				button.GuiEnabled = enabled;
			}
		//}
	}

	public void ResetAllButtons() {
		foreach (GuiButton button in mButtons) {
			button.State = GuiButton.ButtonState.Up;
			button.Refresh();
			//button.SetButtonState(GuiButton.ButtonState.Up);
		}
		mSelectedButton = null;
		mSelectedButtonIndex = -1;
	}

	public override bool HitTest(Vector3 pos) {


        if (Application.isEditor) {
            if (EventSystem.current.IsPointerOverGameObject()) {
                return false;
            }
        }
        else {
            if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(0))
                return false;
        }

        if (HitRect.width != 0 || HitRect.height != 0) {
			Rect rect = HitRect;
			rect.x += transform.position.x;
			rect.y += transform.position.y;
			return rect.Contains(pos);
		} else {
			return false;
		}
	}

	public void AddButton(GuiButton button) {
		button.StateChanged += ChildStateChanged;
		mButtons.Add(button);
	}

	public void RemoveButton(GuiButton button) {
		button.StateChanged -= ChildStateChanged;
		mButtons.Remove(button);
	}

	public void RemoveAllButtons() {
		foreach (GuiButton button in mButtons) {
			button.StateChanged -= ChildStateChanged;
		}
		mButtons.Clear();
	}

	public GuiButton GetButton(int index) {
		foreach (GuiButton button in mButtons) {
			if (button.ButtonIndex == index) {
				return button;
			}
		}
		return null;
	}

	public void SelectButton(int index) {
		foreach (GuiButton button in mButtons) {
			if (button.ButtonIndex == index) {
				SelectButton(button);
			}
		}
	}

	public void SelectButton(GuiButton button) {
		button.State = GuiButton.ButtonState.Selected;
		button.Refresh();
		SelectButtonInternal(button);
	}

	private int ChildStateChanged(object sender) {
		GuiButton button = (GuiButton)sender;
		if (button.State == GuiButton.ButtonState.Selected) {
			SelectButtonInternal(button);
		}
		OnButtonStateChanged(button);
		return 0;
	}

	private void SelectButtonInternal(GuiButton button) {
		if (mSelectedButton != button && mSelectedButton != null) {
			mSelectedButton.ChangeState(GuiButton.ButtonState.Up);
		}
		mSelectedButton = button;
		mSelectedButtonIndex = mSelectedButton.ButtonIndex;
		OnButtonSelected();
	}

	protected virtual void OnButtonSelected() {
		if (ButtonSelected != null) {
			ButtonSelected(this);
		}
	}

	protected virtual void OnButtonStateChanged(GuiButton button) {
		if (ButtonStateChanged != null) {
			ButtonStateChanged(this, button);
		}
	}

	#endregion

	#region Properties

	public List<GuiButton> Buttons {
		get { return mButtons; }
	}

	public GuiButton SelectedButton {
		get { return mSelectedButton; }
	}

	public int SelectedButtonIndex {
		get {
			return mSelectedButton != null ? mSelectedButton.ButtonIndex : -1;
		}
		set {
			if (mSelectedButton != null && mSelectedButton.ButtonIndex != value) {
				mSelectedButton.ChangeState(GuiButton.ButtonState.Up);
			}
			foreach (GuiButton button in mButtons) {
				if (button.ButtonIndex == value) {
					button.ChangeState(GuiButton.ButtonState.Selected);
					//mSelectedButton = button;
					break;
				}
			}
		}
	}

	#endregion

	#region Fields

	public GuiButton[] InitialButtons;
	public Rect HitRect;
	public event System.Func<object, int> ButtonSelected;
	public event System.Func<object, object, int> ButtonStateChanged;
	protected List<GuiButton> mButtons = new List<GuiButton>();
	protected GuiButton mSelectedButton = null;
	protected int mSelectedButtonIndex = -1;
	protected TouchState mTouchState = TouchState.None;
	protected Vector3 mTouchPosition = new Vector3();
	protected Vector3 mMovePosition = new Vector3();

	#endregion
}
