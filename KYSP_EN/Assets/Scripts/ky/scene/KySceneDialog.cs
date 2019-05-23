using UnityEngine;
using System.Collections;

/// <summary>
/// ダイアログシーン。
/// </summary>
public class KySceneDialog : KyScene {

	#region Inner Classes

	public enum Result {
		None = -1,
		No = 0,
		Yes = 1
	}

	#endregion

	#region MonoBehaviour Methods

	public void Start () {
		//	テキストセットアップ
		KyUtil.SetText(gameObject, "dialogBox/btnYes", KyText.GetText(22000));
		KyUtil.SetText(gameObject, "dialogBox/btnNo", KyText.GetText(22001));
		KyUtil.SetText(gameObject, "dialogBox/message", Message);
		//	ダイアログコンポーネントセットアップ
		mButtonYes = KyUtil.GetComponentInChild<GuiButton>(gameObject, "dialogBox/btnYes");
		mButtonNo = KyUtil.GetComponentInChild<GuiButton>(gameObject, "dialogBox/btnNo");
		mDialogBox = KyUtil.FindChild(gameObject, "dialogBox");
		mButtonYes.GuiEnabled = false;
		mButtonNo.GuiEnabled = false;
		mButtonYes.ButtonSelected += YesButtonSelected;
		mButtonNo.ButtonSelected += NoButtonSelected;
		mDialogBox.transform.localPosition = mStartPosition;
		//	初期ステート
		mState.ChangeState(StateEnter);
	}

	public override void Update() {
		if (mState != null) {
			mState.Execute();
		}
	}

	#endregion

	#region State Methods

	/// <summary>
	/// ダイアログ表示処理ステート。
	/// </summary>
	private int StateEnter() {
		if (mState.Sequence == 0) {
			mTweener = mDialogBox.AddComponent<KyTweener>();
			mTweener.TweenPosition(mStartPosition, mEndPosition);
			mTweener.Duration = 0.5f;
			mTweener.ScaleFactor = 0.01f;
			mTweener.IgnoreZ = true;
			mTweener.AutoDestroy = true;
			KyTweener tweener = KyUtil.FindChild(gameObject, "dark").AddComponent<KyTweener>();
			tweener.TweenColor(new Color(0, 0, 0, 0), new Color(0, 0, 0, 0.5f));
			tweener.Duration = 0.5f;
			tweener.AutoDestroy = true;
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (mTweener == null) {
				mState.ChangeState(StateMain);
			}
		}
		return 0;
	}

	/// <summary>
	/// ダイアログ非表示処理ステート。
	/// </summary>
	private int StateLeave() {
		if (mState.Sequence == 0) {
			mButtonYes.GuiEnabled = false;
			mButtonNo.GuiEnabled = false;
			mTweener = mDialogBox.AddComponent<KyTweener>();
			mTweener.TweenPosition(mDialogBox.transform.position, mStartPosition);
			mTweener.Duration = 0.5f;
			mTweener.ScaleFactor = 0.01f;
			mTweener.IgnoreZ = true;
			mTweener.AutoDestroy = true;
			KyTweener tweener = KyUtil.FindChild(gameObject, "dark").AddComponent<KyTweener>();
			tweener.TweenColor(new Color(0, 0, 0, 0.5f), new Color(0, 0, 0, 0));
			tweener.Duration = 0.5f;
			tweener.AutoDestroy = true;
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (mTweener == null) {
				PopScene(false);
			}
		}
		return 0;
	}

	/// <summary>
	/// ユーザ入力待ちメインステート。
	/// </summary>
	private int StateMain() {
		if (mState.Sequence == 0) {
			mButtonYes.GuiEnabled = true;
			mButtonNo.GuiEnabled = true;
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
		}
		return 0;
	}

	#endregion

	#region Methods

	private int YesButtonSelected(object sender) {
		mDialogResult = Result.Yes;
		mState.ChangeState(StateLeave);
		return 0;
	}

	private int NoButtonSelected(object sender) {
		mDialogResult = Result.No;
		mState.ChangeState(StateLeave);
		return 0;
	}

	#endregion

	#region Properties

	public Result DialogResult {
		get { return mDialogResult; }
	}

	#endregion

	#region Fields

	public string Message = "";
	private KyStateManager mState = new KyStateManager();	//	ステート管理
	private GuiButton mButtonYes = null;	//	YESボタン
	private GuiButton mButtonNo = null;		//	NOボタン
	private GameObject mDialogBox = null;	//	ダイアログボックス
	private KyTweener mTweener;	//	アニメーショントゥイーン
	private Result mDialogResult;		//	ダイアログリザルト
	private static readonly Vector3 mStartPosition = new Vector3(0, -640, -50);
	private static readonly Vector3 mEndPosition = new Vector3(0, 0, -50);

	#endregion
}
