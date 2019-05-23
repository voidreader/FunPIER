using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 問題「回転寿司」の寿司管理用スクリプト。
/// </summary>
public class KySusiKaiten : KyScriptObject {

	public enum SushiNeta {
		Ebi = 0,
		Kanpati,
		Toro,
		Gyoku,
	}

	protected override void Start () {
		base.Start();
		for (int i = 0; i <= 10; ++i) {
			GenerateSushi((SushiNeta)(i % 3), 15 - i * GenerateFreq);
			mSushiCount++;
		}
	}

	protected override void UpdateCore() {
		if (!mEnabled) { return; }
		mElapsedTime += DeltaTime;
		mGameTime += DeltaTime;
		//	寿司生成
		if (mElapsedTime >= GenerateFreq) {
			int sushi = 0;
			if (mGameTime < EnableTime) {
				if (Random.Range(0.0f, 1.0f) < GyokuRate) {
					sushi = 3;
				} else {
					sushi = mSushiCount % 4;
				}
				if (sushi != (int)SushiNeta.Gyoku) {
					mNotGyokuCount++;
				} else {
					mNotGyokuCount = 0;
				}
			} else {
				sushi = Random.Range(1, 3);
			}
			GenerateSushi((SushiNeta)sushi, 0);
			mElapsedTime = 0;
			mSushiCount++;
			mGenCount[sushi]++;
		}
		//	画面外で寿司消滅
		if (mSushiList.Count > 0) {
			GameObject head = mSushiList[0];
			if (head.GetComponent<KyTweener>() == null) {
				GameObject.Destroy(head);
				mSushiList.RemoveAt(0);
			}
		}
		//	寿司キャッチ判定
		if (mCatching) {
			int count = Mathf.Min(mSushiList.Count, 3);
			for (int i = mSushiList.Count - 1; i >= mSushiList.Count - count; --i) {
				GameObject sushi = mSushiList[i];
				float y = mSushiList[i].transform.localPosition.y;
				if (CatchYMin < y && y < CatchYMax) {
					Destroy(sushi);
					mSushiList.RemoveAt(i);
					mCatching = false;
					CommandManager.MoveFrame(2000, 0);
					int sushiType = 0;
					sushiType = (int)System.Enum.Parse(typeof(SushiNeta), sushi.name);
					mCatchCount[sushiType]++;
					break;
				}
			}
		}
		//	玉子逃し判定
		if (!mGyokuLeft) {
			int count = Mathf.Min(mSushiList.Count, 3);
			for (int i = mSushiList.Count - 1; i >= mSushiList.Count - count; --i) {
				GameObject sushi = mSushiList[i];
				if (sushi.name == "Gyoku") {
					float y = mSushiList[i].transform.localPosition.y;
					if (y > CatchYMax) {
						mGyokuLeft = true;
						//Debug.Log("Gyoku Sai");
						break;
					}
				}
			}
		}
	}

	private GameObject GenerateSushi(SushiNeta sushi, float startTime) {
		GameObject go = CommandManager.CreateKyObject(System.Enum.GetName(typeof(SushiNeta), sushi), 
			SushiNetaName[(int)sushi]);
		go.transform.parent = transform;
		Vector3 pos = new Vector3(-60, -180, -mSushiCount * 0.01f);
		go.transform.localPosition = pos;

		KyTweener tweener1 = go.AddComponent<KyTweener>();
		tweener1.UseFlag = KyTweener.UseFlags.UsePosition | KyTweener.UseFlags.UseScale | KyTweener.UseFlags.UsePers | KyTweener.UseFlags.UseStart;
		tweener1.StartPosition = pos;
		tweener1.EndPosition = new Vector3(-60, -180, 0);
		tweener1.Duration = 15.0f;
		tweener1.IgnoreZ = true;
		tweener1.AutoDestroy = true;
		tweener1.ElapsedTime = startTime;
		tweener1.StartPersZ = 0.7f;
		tweener1.EndPersZ = 4.0f;
		tweener1.UseScriptTime = true;
		tweener1.VPoint = new Vector3(240, 130, 0);

		KyTweener tweener2 = go.AddComponent<KyTweener>();
		tweener2.UseFlag = KyTweener.UseFlags.UsePosition | KyTweener.UseFlags.UseScale | KyTweener.UseFlags.UsePers | KyTweener.UseFlags.UseStart;
		tweener2.StartPosition = pos;
		tweener2.EndPosition = new Vector3(300, -180, 0);
		tweener2.Duration = 3.0f;
		tweener2.IgnoreZ = true;
		tweener2.AutoDestroy = true;
		tweener2.ElapsedTime = 0;
		tweener2.StartPersZ = 4.0f;
		tweener2.EndPersZ = 4.0f;
		tweener2.UseScriptTime = true;
		tweener2.VPoint = new Vector3(240, 130, 0);
		tweener2.enabled = false;

		tweener1.Successor = tweener2;

		mSushiList.Add(go);

		return go;
	}

	public void OnCatch() {
		mCatching = true;
	}

	public void OnJudge() {
		mEnabled = false;
		for (int i = 0; i < 4; ++i) {
			DebugUtil.Log(System.Enum.GetName(typeof(SushiNeta), (SushiNeta)i) + ":" +
				mCatchCount[i] + "/" + mGenCount[i]);
		}
		if (mCatchCount[(int)SushiNeta.Ebi] >= mGenCount[(int)SushiNeta.Ebi] - 1 &&
			mCatchCount[(int)SushiNeta.Kanpati] == 0 &&
			mCatchCount[(int)SushiNeta.Toro] == 0 &&
			mCatchCount[(int)SushiNeta.Gyoku] == 0) {
			CommandManager.SetVariable("secret", 1);
		}
		if (mCatchCount[(int)SushiNeta.Gyoku] >= mGenCount[(int)SushiNeta.Gyoku] - 1) {
			CommandManager.SetVariable("result", 1);
		} else {
			CommandManager.SetVariable("result", 0);
		}
	}

	public float GenerateFreq = 2.0f;
	public float CatchYMin = -150;
	public float CatchYMax = -120;
	public float GyokuRate = 0.3f;
	public float EnableTime = 10.0f;

	private string[] SushiNetaName = {
		"susi/susiSusiEbi",
		"susi/susiSusiKanpati",
		"susi/susiSusiToro",
		"susi/susiSusiGyoku",
	};
	private bool mEnabled = true;
	private float mElapsedTime = 0;
	private float mGameTime = 0;
	private int mSushiCount = 0;
	private bool mCatching = false;
	private int mNotGyokuCount = 0;
	private bool mGyokuLeft = false;
	private List<GameObject> mSushiList = new List<GameObject>();
	private int[] mGenCount = new int[4];
	private int[] mCatchCount = new int[4];
}
