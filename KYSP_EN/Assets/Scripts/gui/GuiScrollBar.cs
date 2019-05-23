using UnityEngine;
using System.Collections;

public class GuiScrollBar : GuiComponent {

	public enum ButtonState {
		None = 0,
		Up,
		Down,
		Selected,
		Disabled,
	}

	#region MonoBehaviour Methods

	//	初期化処理を記述します。
	void Start () {
	
	}
	
	//	フレーム毎の処理を記述します。
	void Update () {
		if (!GuiEnabled) { return; }
		UpdateInput();
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

	private void UpdateInput() {
		if (State == ButtonState.Up) {
			if (Input.GetMouseButtonDown(0)) {
				if (HitTest(Camera.main.ScreenToWorldPoint(Input.mousePosition))) {
					State = ButtonState.Down;
				}
			}
		} else if (State == ButtonState.Down) {
			if (Input.GetMouseButton(0)) {
				Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				float posx = Vertical ? pos.y - transform.localPosition.y : pos.x - transform.localPosition.x;
				SetPositionOnly(posx);
				SetValueOnly(Mathf.InverseLerp(PositionMin, PositionMax, mPosition));
				SetPageOnly((int)(mValue * PageCount));
			} else {
				State = ButtonState.Up;
			}
		}
	}

	public override bool HitTest(Vector3 pos) {
		if (HitRect.width != 0 || HitRect.height != 0) {
			Rect rect = HitRect;
			rect.x += transform.position.x;
			rect.y += transform.position.y;
			return rect.Contains(pos);
		} else {
			return false;
		}
	}

	private void SetPositionOnly(float pos) {
		float min = Mathf.Min(PositionMin, PositionMax);
		float max = Mathf.Max(PositionMin, PositionMax);
		pos = Mathf.Clamp(pos, min, max);
		mPosition = pos;

		Vector3 thumb = ScrollThumb.transform.localPosition;
		if (Vertical) {
			thumb.y = pos;
		} else {
			thumb.x = pos;
		}
		ScrollThumb.transform.localPosition = thumb;
	}

	private void SetValueOnly(float value) {
		value = Mathf.Clamp01(value);
		mValue = value;
		/*mPage = Mathf.Clamp((int)(mValue * PageCount), 0, PageCount - 1);
		if (mPage != mPageOld) {
			mPageOld = mPage;
			OnPageChanged();
		}*/
	}

	private void SetPageOnly(int page) {
		page = Mathf.Clamp(page, 0, PageCount - 1);
		mPage = page;
		if (mPage != mPageOld) {
			mPageOld = mPage;
			OnPageChanged();
			//DebugUtil.Log("page changed : " + mPage);
		}
	}

	protected virtual void OnPageChanged() {
		if (PageChanged != null) {
			PageChanged(this);
		}
	}

	#endregion

	#region Properties

	/// <summary>
	///	スクロール値を取得・設定する。
	/// </summary>
	public float Value {
		get { return mValue; }
		//set { mValue = Mathf.Clamp(value, ValueMin, ValueMax); }
		set {
			SetValueOnly(value);
			SetPageOnly((int)(mValue * PageCount));
			SetPositionOnly(Mathf.Lerp(PositionMin, PositionMax, mValue));
		}
	}

	/// <summary>
	///	スクロール値を0から1の間の値で取得・設定する。
	/// </summary>
	/*public float Value01 {
		get { return Mathf.InverseLerp(ValueMin, ValueMax, mValue); }
		set {
			mValue = Mathf.Lerp(ValueMin, ValueMax, value);
		}
	}*/

	public int Page {
		get { return mPage; }
		set {
			SetPageOnly(value);
			SetValueOnly(Mathf.InverseLerp(0, PageCount, mPage));
			SetPositionOnly(Mathf.Lerp(PositionMin, PositionMax, mValue));
		}
	}

	#endregion

	#region Fields

	public GameObject ScrollThumb = null;	//	スクロールサム(つかむ部分)
	public bool Vertical = false;	//	水平ならfalse, 垂直ならtrue
	//public float ValueMin = 0.0f;	//	スクロール値の最小値
	//public float ValueMax = 1.0f;	//	スクロール値の最大値
	public float PositionMin = 0.0f;	//	スクロール値が最小のときのスクロールサムの位置
	public float PositionMax = 100.0f;	//	スクロール値が最大のときのスクロールサムの位置
	public Rect HitRect;	//	タッチヒット判定
	public ButtonState State = ButtonState.Up;
	public int PageCount = 10;	//	ページ数
	public event System.Func<object, int> PageChanged;

	private float mValue;	//	スクロール値
	private float mPosition;
	private int mPage;	//	現在のページ
	private int mPageOld;	//	前フレームのページ

	#endregion
}
