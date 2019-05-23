using UnityEngine;
using System.Collections;

/// <summary>
/// �������т̃O���t����уR�����g�̃R���g���[���B
/// </summary>
public class KyRecordView : MonoBehaviour {

	#region MonoBehaviour Methods

	void Start () {
		for (int i = 0; i < KyConst.ScoreCategoryCount; ++i) {
			mScoreSprite[i] = KyUtil.GetComponentInChild<SpriteTextCustom>(gameObject, "chart/score" + i);
			mScoreSprite[i].GetComponent<Renderer>().enabled = false;
		}
		ShowMessage(InitShowMessage);
	}
	
	void Update () {
		if (mChartAnimating) {
			mElapsedTime += Time.deltaTime;
			float t = mElapsedTime / ChartAnimDuration;
			UpdateChart(t);
			if (t >= 1.0f) {
				mChartAnimating = false;
				mElapsedTime = 0;
			}
		}
	}

	#endregion

	#region Methods

	public void SetupScore(float[] score) {
		System.Array.Copy(score, mScores, KyConst.ScoreCategoryCount);
		//	�J�e�S���Ԃł̍ő哾�_�Ƃ��̃J�e�S����Z�o
		RecordMaxCategory = 0;
		RecordMaxScore = 0;
		for (int i = 0; i < KyConst.ScoreCategoryCount; ++i) {
			if (score[i] > RecordMaxScore) {
				RecordMaxScore = score[i];
				RecordMaxCategory = i;
			}
		}
		RecordRank =
			RecordMaxScore <= 0.5 ? 0 :
			RecordMaxScore <= 0.62 ? 1 :
			RecordMaxScore <= 0.74 ? 2 :
			RecordMaxScore <= 0.86 ? 3 :
			RecordMaxScore < 1.00 ? 4 : 5;
		int category =
			(RecordRank == 0 || RecordRank == 5) ? 0 :
			RecordMaxCategory;

		KyUtil.SetText(gameObject, "msg/text1", string.Format(KyText.GetText(21070), KyText.GetText(21000 + RecordMaxCategory)));
		int baseTextId = mGameMode == KyConst.GameMode.KukiYomi ? 21010 : 21100;
		KyUtil.SetText(gameObject, "msg/text2", string.Format(KyText.GetText(21071), KyText.GetText(baseTextId + RecordRank * 10 + category)));
	}

	public void BeginAnimChart() {
		mChartAnimating = true;
		mElapsedTime = 0;
	}

	public void StopAnimChart() {
		mChartAnimating = false;
	}

	public void UpdateChart(float t) {
		ChartSprite.Vertices[0] = Vector3.zero;
		for (int i = 0; i <= KyConst.ScoreCategoryCount; ++i) {
			float u = Mathf.Min(t, mScores[i % KyConst.ScoreCategoryCount]);
			float v = Mathf.Max(u, 0.05f);
			ChartSprite.Vertices[i + 1] = Vector3.Lerp(Vector3.zero, MaxChartPoint[i % KyConst.ScoreCategoryCount], v);
			if (i < KyConst.ScoreCategoryCount) {
				mScoreSprite[i].GetComponent<Renderer>().enabled = true;
				mScoreSprite[i].Text = ((int)(u * 100)).ToString();
				mScoreSprite[i].UpdateAll();
			}
		}
		ChartSprite.UpdateVertices();
	}

	public void ShowMessage(bool show) {
		GameObject omae = KyUtil.FindChild(gameObject, "msg/omae");
		//DebugUtil.Log(omae);
		omae.GetComponent<Renderer>().enabled = show;
		if (show) {
			//	�\�����[�h(��������/�ߋ��̂��O)�ɂ���Đ؂�ւ���B
			Sprite sprite = omae.GetComponent<Sprite>();
			sprite.AnimationIndex = ViewMode == 0 ? 1 : 2;
		}
		KyUtil.FindChild(gameObject, "msg/window").GetComponent<Renderer>().enabled = show;
		KyUtil.FindChild(gameObject, "msg/text1").GetComponent<Renderer>().enabled = show;
		KyUtil.FindChild(gameObject, "msg/text2").GetComponent<Renderer>().enabled = show;
	}

	#endregion

	#region Properties

	public float[] Scores {
		set {
			System.Array.Copy(value, mScores, KyConst.ScoreCategoryCount);
		}
	}

	public bool ChartAnimating {
		get { return mChartAnimating; }
	}

	public KyConst.GameMode GameMode {
		get { return mGameMode; }
		set {
			mGameMode = value;
			string text = mGameMode == KyConst.GameMode.KukiYomi ? KyText.GetText(20000) :
				KyText.GetText(20002);
			KyUtil.SetText(gameObject, "mode/name", text);
		}
	}

	#endregion

	#region Fields

	public SpritePolygon ChartSprite = null;
	public int ViewMode = 1;
	public float ChartAnimDuration = 5.0f;
	public bool InitShowMessage = false;
	public float RecordMaxScore = 0;
	public int RecordMaxCategory = 0;
	public int RecordRank = 0;

	private readonly Vector3[] MaxChartPoint = new Vector3[KyConst.ScoreCategoryCount] {
		new Vector3(0, 148, 0), new Vector3( 128, 74, 0), new Vector3(128,-74, 0),
		new Vector3(0,-148, 0), new Vector3(-128,-74, 0), new Vector3(-128,74, 0)
	};

	private float[] mScores = new float[KyConst.ScoreCategoryCount];
	private SpriteTextCustom[] mScoreSprite = new SpriteTextCustom[KyConst.ScoreCategoryCount];
	private bool mChartAnimating = false;
	private float mElapsedTime = 0;
	private KyConst.GameMode mGameMode = KyConst.GameMode.KukiYomi;

	#endregion
}
