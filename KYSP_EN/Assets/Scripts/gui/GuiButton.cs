using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GuiButton : GuiComponent {

	public enum ButtonState {
		None = 0,
		Up,
		Down,
		Selected,
		Disabled,
	}

	public enum ButtonType {
		Push = 0,
		Touch,
	}

	#region MonoBehaviour Methods

	protected void Awake() {
		mSelfSprite = GetComponent<Sprite>();
	}

	protected virtual void Start () {
		Refresh();
	}
	
	protected virtual void Update () {
		if (!GuiEnabled) { return; }
		mStateChanged = false;
		UpdateInput();
		if (mNextState != ButtonState.None) {
			State = mNextState;
			mNextState = ButtonState.None;
			mStateChanged = true;
			OnStateChanged();
			if (State == ButtonState.Selected) {
				OnButtonSelected();
			}
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		if (HitRect.width != 0 && HitRect.height != 0) {
			Vector3 pos = transform.position;
			pos.x = transform.position.x + HitRect.xMin + HitRect.width / 2;
			pos.y = transform.position.y + HitRect.yMin + HitRect.height / 2;
			Gizmos.DrawWireCube(
				pos,
				new Vector3(HitRect.width, HitRect.height, 1));
		}
	}

	#endregion

	#region Methods

	protected virtual void UpdateInput() {

		if (State == ButtonState.Disabled) { return; }

		if (Type == ButtonType.Push) {

            

			if (State == ButtonState.Up) {
				if (Input.GetMouseButtonDown(0)) {
					if (HitTest(Camera.main.ScreenToWorldPoint(Input.mousePosition))) {
						mNextState = ButtonState.Down;
						mStateChanged = true;
					}
				}
			} else if (State == ButtonState.Down) {
				if (!Input.GetMouseButton(0)) {
					mNextState = ButtonState.Selected;
					mStateChanged = true;
				} else if (!HitTest(Camera.main.ScreenToWorldPoint(Input.mousePosition))) {
					mNextState = ButtonState.Up;
					mStateChanged = true;
				}
			}
		} else if (Type == ButtonType.Touch) {

            

            if (State == ButtonState.Up || State == ButtonState.Selected) {
				if (Input.GetMouseButton(0)) {
					if (HitTest(Camera.main.ScreenToWorldPoint(Input.mousePosition))) {
						mNextState = ButtonState.Down;
						mStateChanged = true;
					}
				}
			} else if (State == ButtonState.Down) {
				if (!Input.GetMouseButton(0) ||
					!HitTest(Camera.main.ScreenToWorldPoint(Input.mousePosition))) {
					mNextState = ButtonState.Selected;
					mStateChanged = true;
				}
			}
		}
	}

	public virtual void Refresh() {
	}

	/*public void SetButtonState(ButtonState state) {
		if (State != state) {
			State = state;
		}
	}*/

	public override bool HitTest(Vector3 pos) {

        if (EventSystem.current.IsPointerOverGameObject()) {
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

	public void ChangeState(ButtonState state) {
		mNextState = state;
	}

	protected virtual void OnStateChanged() {
		//DebugUtil.Log("state:" + State);
		Refresh();
		if (StateChanged != null) {
			StateChanged(this);
		}
	}

	protected virtual void OnButtonSelected() {
		if (ButtonSelected != null) {
			ButtonSelected(this);
		}
	}

	protected virtual void OnUserDataChanged() {
		if (UserDataChanged != null) {
			UserDataChanged(this);
		}
	}

	#endregion

	#region Properties

	public object UserData {
		get { return mUserData; }
		set {
			if (mUserData != value) {
				mUserData = value;
				OnUserDataChanged();
			}
		}
	}

	#endregion

	#region Fields

	public bool Selected = false;
	public ButtonState State = ButtonState.Up;
	public ButtonType Type = ButtonType.Push;
	public Rect HitRect;
	public int ButtonIndex = 0;
	public event System.Func<object, int> StateChanged;
	public event System.Func<object, int> ButtonSelected;
	public event System.Func<object, int> UserDataChanged;

	protected object mUserData = null;
	protected bool mStateChanged = false;
	protected bool mSelectChanged = false;
	protected Sprite mSelfSprite = null;
	protected ButtonState mNextState = ButtonState.None;

	#endregion
}
