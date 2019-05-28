using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GuiScrollList : GuiComponent {

	public enum TouchState {
		None = 0,
		Touching,
		Moving,
		Snapping,
	}

	#region MonoBehaviour Methods

	void Awake() {
		ButtonGroup.ButtonSelected += HandleButtonSelected;
	}

	void Start () {
	}
	
	void Update () {
		if (!GuiEnabled) { return; }
		UpdateInput();
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.blue;
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

	#region GuiComponent Methods

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

	#endregion

	#region Methods

	public void SetGuiEnabled(bool enabled) {
		GuiEnabled = enabled;
		ButtonGroup.SetGuiEnabled(enabled);
	}

	protected void UpdateInput() {
		if (!Scrollable) { return; }

		if (mTouchState == TouchState.None) {
			if (Input.GetMouseButtonDown(0)) {
				Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				if (HitTest(pos)) {
					mStartPosition = pos;
					mEndPosition = pos;
					mTouchState = TouchState.Touching;
				}
			}
		}

        else if (mTouchState == TouchState.Touching) {
			if (Input.GetMouseButton(0)) {
				mEndPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				mDeltaPos = mEndPosition.y - mStartPosition.y;
				if (Mathf.Abs(mDeltaPos) > 10) {
					foreach (GuiButton button in ButtonGroup.Buttons) {
						button.ChangeState(GuiButton.ButtonState.Up);
					}
					mStartPosition = mEndPosition;
					mTouchState = TouchState.Moving;
				}
			} else {
				mTouchState = TouchState.None;
			}
		}
       
        else if (mTouchState == TouchState.Moving)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mDeltaPos = touchPos.y - mStartPosition.y;
                if (Mathf.Abs(mDeltaPos) > 0)
                {
                     OnScroll(mDeltaPos);
                    mStartPosition = touchPos; // 마지막 위치를 시작 위치로 바꿔준다.
                }
            }
            else
            {
                if (SnapEnabled)
                {
                    mTouchState = TouchState.Snapping;
                    float y = ButtonGroup.transform.localPosition.y - GridOffset; //가장 위에 있을때는 0이다.
                    int ry = (int)Mathf.Round(y / GridLength); //가령 가장 상단일때는 0 / 96이 된다.  결국 0 
                    float speed = mDeltaPos / Time.deltaTime;
                    if (speed > 480)
                    {
                        mGripPosition = Mathf.Min(ry - 4, GetGripPosition(mScrollMax));
                        mSnapSpeed = 1200;
                    }
                    else if (speed < -480)
                    {
                        mGripPosition = Mathf.Max(ry + 4, GetGripPosition(mScrollMin));
                        mSnapSpeed = 1200;
                    }
                    else
                    {
                        mGripPosition = ry;
                        mSnapSpeed = 240;
                    }
                    mGridEnd = mGripPosition * GridLength + GridOffset;
                }
                else
                {
                    mTouchState = TouchState.None;
                }
            }
        }

        else if (mTouchState == TouchState.Snapping) {
			float listPos = ButtonGroup.transform.localPosition.y; //312
			float diff = mGridEnd - listPos; //0 - 312 = -312 가 나온다.
            float delta = Mathf.Min(Mathf.Abs(diff), mSnapSpeed * Time.deltaTime); // -312, 1200*Time.deltaTime; 흠.. 그래서 312?
            OnScroll(Mathf.Sign(diff) * delta);
			listPos = ButtonGroup.transform.localPosition.y;
            
            if (listPos < mScrollMin || mScrollMax < listPos) {
				listPos = Mathf.Clamp(listPos, mScrollMin, mScrollMax);
				mGripPosition = (int)Mathf.Round((listPos - GridOffset) / GridLength);
				mGridEnd = mGripPosition * GridLength + GridOffset;
            }
            else if (delta <= Mathf.Epsilon) {
				mTouchState = TouchState.None;
			}
            
        }

        

    }

    
    protected void OnScroll(float scrollLength) {
		Vector3 pos = ButtonGroup.transform.localPosition;
        pos.y += scrollLength;
       
        pos.y = Mathf.Clamp(pos.y, mScrollMin, mScrollMax);
        
        ButtonGroup.transform.localPosition = pos;
        
    }

    public void SetGridPosition(int index) {
		mGripPosition = index;
		Vector3 pos = ButtonGroup.transform.localPosition;
        //pos.x = index * GridLength + GridOffset; // -1 * 96 + 0 = -96
        pos.x = index * StandardHight + GridOffset;
        ButtonGroup.transform.localPosition = pos;
	}

	public void CreateList() {
		int count = mDataSet.Count;

		ButtonGroup.RemoveAllButtons();
		foreach (Transform child in ButtonGroup.transform) {
			if (child.gameObject.name.StartsWith("_")) {
				Destroy(child.gameObject);
			}
		}
		for (int i = 0; i <= count; ++i) {
            //           196 - 79 * count;
            float y = StandardHight - (OffsetHight * i) -5; // x가 79씩이면 된다.

            if (i < count) {
				
				GameObject goItem = (GameObject)GameObject.Instantiate(ListItemPrefab);
				goItem.name = "_item" + i;
				GuiButton item = goItem.GetComponent<GuiButton>();
				if (item == null) { throw new System.Exception("the prefab don't have GuiListItem."); }
				item.ButtonIndex = i;
				goItem.transform.parent = ButtonGroup.transform;
                goItem.transform.localPosition = new Vector3(0, y, 0); //new Vector3(x, 0, 0); //79씩
               

                ButtonGroup.AddButton(item);
				item.UserData = mDataSet[i];
			}
			
			GameObject goSep = (GameObject)GameObject.Instantiate(SeparatorPrefab);
			goSep.name = "_sep" + i;
			goSep.transform.parent = ButtonGroup.transform;
            goSep.transform.localPosition = new Vector3(192, y + SeparatorDisplacement, 1); // 232 // 153 //
                //new Vector3(-145, x + SeparatorDisplacement, 1); //new Vector3(x + SeparatorDisplacement, 0, 1);
          
		}
        mScrollMin = HitRect.yMax - MarginHead;
        mScrollMax = HitRect.yMin + OffsetHight * count - MarginTail;

        //Vector3 pos = ButtonGroup.transform.localPosition;
        //pos.x = mScrollMin;
        //ButtonGroup.transform.localPosition = pos;
    }

	public void UpdateList() {
		int index = 0;
		foreach (GuiButton button in ButtonGroup.Buttons) {
			button.UserData = mDataSet[index];
			index++;
		}
		Vector3 pos = ButtonGroup.transform.localPosition;
		pos.x = mScrollMin;
		ButtonGroup.transform.localPosition = pos;
	}

	protected void ResetAllButton() {
		foreach (GuiButton button in ButtonGroup.Buttons) {
			button.ChangeState(GuiButton.ButtonState.Up);
		}
	}

	public void UpdateDataSet() {
		if (mDataSet != null) {
			
		}
	}

	protected virtual void OnButtonSelected() {
		SetGuiEnabled(false);
		if (ButtonSelected != null) {
			ButtonSelected(this);
		}
	}

	private int HandleButtonSelected(object sender) {
		OnButtonSelected();
		return 0;
	}

	private int GetGripPosition(float pos) {
		return (int)Mathf.Round((pos - GridOffset) / GridLength);
	}

	#endregion

	#region Properties

	public ArrayList DataSet {
		get { return mDataSet; }
		set {
			mDataSet = value;
			UpdateDataSet();
		}
	}

	public GuiButton SelectedButton {
		get { return ButtonGroup.SelectedButton; }
	}

	public int SelectedButtonIndex {
		get { return ButtonGroup.SelectedButtonIndex; }
	}

	#endregion

	#region Fields

	public GameObject ListItemPrefab = null;	//	깏긚긣귺귽긡?궻긵깒긪긳
	public GameObject SeparatorPrefab = null;	//	귺귽긡?듩긜긬깒??궻긵깒긪긳
	public GuiButtonGroup ButtonGroup = null;

	public Rect HitRect;
    public float ListItemStride = -48; //-48;
    public float SeparatorDisplacement = 26; // 36→26 ;
    public float StandardHight = 196;
	public float OffsetHight = 69; // OffsetWidth 79 - 69
    public float MarginHead = 0.0f;
	public float MarginTail = 0.0f; //1128
	public float GridLength = 96;
	public float GridOffset = 0;
	public bool Scrollable = true;
	public bool SnapEnabled = true;
	public event System.Func<object, int> ButtonSelected;

	private ArrayList mDataSet = null;
	private float mScrollMin = 0;
	private float mScrollMax = 0;
    
    //////////////////////////////////////////
    private float mPosition;
    public float PositionMin = 0.0f;    //	スクロール値が最小のときのスクロールサムの位置
    public float PositionMax = 100.0f;
   
    //////////////////////////////////

    protected TouchState mTouchState = TouchState.None;
	protected Vector3 mStartPosition = new Vector3();
	protected Vector3 mEndPosition = new Vector3();
	protected float mGridEnd = 0;
	protected float mSnapSpeed = 240;
	protected int mGripPosition = 0;
	protected float mDeltaPos = 0;
    Vector3 movePos= new Vector3();
	#endregion
}

