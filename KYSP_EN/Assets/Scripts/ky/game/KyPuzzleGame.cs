using UnityEngine;
using System.Collections;

/// <summary>
/// 問題「落ち物パズル？」用スクリプト
/// </summary>
public class KyPuzzleGame : KyScriptObject {

	protected override void Start() {
		base.Start();

		mState.ChangeState(StateMain);

		byte[,] field = null;
		if (StageType == 0) {
			field = new byte[,] {
				{1,1,1,1,1,1,1,1,1,3,1,1},
				{1,1,1,1,1,1,1,1,1,0,1,1},
				{1,1,1,1,1,1,1,1,1,0,1,1},
				{1,1,1,1,1,1,1,1,1,0,1,1},
				{0,1,1,1,1,0,0,0,0,0,0,0},
				{0,1,1,1,1,0,0,0,0,0,0,0},
				{0,1,1,1,1,0,0,0,0,0,0,0},
				{0,1,1,1,1,2,0,0,0,0,0,0},
				{0,1,1,1,1,0,0,0,0,0,0,0},
				{0,1,1,1,1,0,0,0,0,0,0,0},
				{0,1,1,1,1,0,0,0,0,0,0,0},
				{0,0,0,1,0,0,0,0,0,0,0,0},
				{0,0,0,4,0,0,0,0,0,0,0,0},
			};
			mBlockPos = new Vector2(6, 14);
			mBlockPlayer = new byte[,] {
				{1}
			};
			mBlockOrigin = new Vector2(0, 0);
		} else if (StageType == 1) {
			field = new byte[,] {
				{1,1,1,1,1,1,1,1,1,4,1,1},
				{1,1,1,1,1,1,1,1,0,0,0,1},
				{0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,2,0,0,0,0,0,0,0,0},
			};
			mBlockPos = new Vector2(6, 14);
			mBlockPlayer = new byte[,] {
				{0,1,0},
				{1,1,1}
			};
			mBlockOrigin = new Vector2(1, 0);
		}
		mBlockField = new byte[StageHeight, StageWidth];
		if (field != null) {
			System.Array.Copy(field, mBlockField, field.Length);
		}
		mBlockPosOld = mBlockPos;
		Player.transform.localPosition = BlockToWorld(mBlockPosOld);

		mInput.Threshold = BlockStepUnit;
	}

	protected override void UpdateCore() {
		if (mState != null) {
			mState.Execute();
		}
	}

	private int StateMain() {
		mElapsedTime += Time.deltaTime;
		if (!CommandManager.PreviewMode) {
			mInput.Update();
		}

		if (mInput.Slide == KyInputCrossKey.Direction.Left) {
			mBlockPos.x -= 1;
		} else if (mInput.Slide == KyInputCrossKey.Direction.Right) {
			mBlockPos.x += 1;
		} else if (mInput.Slide == KyInputCrossKey.Direction.Down) {
			mBlockPos.y -= 1;
			mElapsedTime = 0;
		}

		if (mElapsedTime >= BlockStepTime) {
			mElapsedTime = 0;
			mBlockPos.y -= 1;
		}
		
		if (mBlockPos != mBlockPosOld) {
			bool ground = false;
			if (mBlockPos.y < 0) {
				ground = true;
			}
			//	ステージ内にクランプ
			mBlockPos.x = Mathf.Clamp(mBlockPos.x, 0, StageWidth - 1);
			mBlockPos.y = Mathf.Clamp(mBlockPos.y, 0, StageHeight - 1);
			//	ブロック内部には不可侵
			if (HitTest(new Vector2(mBlockPos.x, mBlockPosOld.y))) {
				mBlockPos.x = mBlockPosOld.x;
			}
			if (mBlockField[(int)mBlockPos.y, (int)mBlockPosOld.x] == 1) {
				mBlockPos.y = mBlockPosOld.y;
				ground = true;
			}

			mBlockPosOld = mBlockPos;
			//	接地
			if (ground) {
				if (GameEngine != null) {
					 CommandManager.MoveFrame(2000, 0);
					int tag = mBlockField[(int)mBlockPosOld.y, (int)mBlockPosOld.x];
					if (tag == 3) {
						CommandManager.MoveFrame(3000, 0);
					} else if (tag == 4) {
						CommandManager.MoveFrame(4000, 0);
					} else {
						CommandManager.MoveFrame(2000, 0);
					}
				}
				Sprite sprite = Player.GetComponent<Sprite>();
				sprite.FrameIndex = 1;
				mState.ChangeState(null);
			}
			mBlockPosOld = mBlockPos;
			Player.transform.localPosition = BlockToWorld(mBlockPosOld);
		}

		if (mBlockField[(int)mBlockPosOld.y, (int)mBlockPosOld.x] == 2) {
			CommandManager.MoveFrame(1000, 0);
			Sprite sprite = Player.GetComponent<Sprite>();
			sprite.FrameIndex = 1;
			mState.ChangeState(null);
		}

		return 0;
	}

	/// <summary>
	/// ブロック単位をワールド座標単位に変換する。
	/// </summary>
	protected Vector3 BlockToWorld(Vector2 block) {
		Vector3 world = new Vector3();
		world.x = (block.x - StageWidth / 2) * BlockStepUnit;
		world.y = (block.y - StageHeight / 2) * BlockStepUnit;
		world.z = 0;
		return world;
	}

	protected bool HitTest(Vector2 pos) {
		int width = mBlockPlayer.GetLength(1);
		int height = mBlockPlayer.GetLength(0);
		for (int j = 0; j < height; ++j) {
			for (int i = 0; i < width; ++i) {
				if (mBlockPlayer[j, i] == 1) {
					int bx = (int)(pos.x + i - mBlockOrigin.x);
					int by = (int)(pos.y + j - mBlockOrigin.y);
					if (bx < 0 || StageWidth <= bx) {
						return true;
					}
					if (by < 0) {
						return true;
					}
					if (StageHeight <= by) {
						continue;
					}
					if (mBlockField[by,bx] == 1) {
						return true;
					}
				}
			}
		}
		return false;
	}

	public int StageType = 0;
	public GameObject Player = null;

	private const float BlockStepTime = 1.0f;	//	ブロックが落ちる時間間隔
	private const float BlockStepUnit = 24.0f;	//	ブロックの一辺の長さ＝移動間隔
	private const int StageWidth = 12;
	private const int StageHeight = 15;

	private KyStateManager mState = new KyStateManager();	//	ステート管理。
	private float mElapsedTime = 0.0f;				//	経過時間(ブロックの落下に使用)。
	private Vector2 mBlockPosOld = new Vector2(0, 0);	//	現在のブロックの位置(ブロック単位)。
	private Vector2 mBlockPos = new Vector2(0, 0);
	private byte[,] mBlockField = null;		//	積まれているブロックを表す。
	private byte[,] mBlockPlayer = null;
	private Vector2 mBlockOrigin = new Vector2(0, 0);
	private KyInputCrossKey mInput = new KyInputCrossKey();
	//private KyGameEngine mEngine = null;
}
