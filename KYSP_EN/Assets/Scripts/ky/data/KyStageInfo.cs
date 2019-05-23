using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 스테이지 정보를 집약·관리하는 클래스.
/// </summary>
public class KyStageInfo {

	public class Stage {
		public int MainID = 0;
		public int StageNo = 0;
		public int StageNo2 = 0;
		public int Score = 0;
		public string Title = "";
		public string Comment = "";
		public string Secret = "";
	}

	private void Initialize() {
		mStages = new Dictionary<int, Stage>();
		mStageList1 = new Stage[KyConst.StageCountYomu];
		mStageList2 = new Stage[KyConst.StageCountYomanai];
		mStagesCategory1 = new int[(int)KyConst.ScoreCategory.Count];
		mStagesCategory2 = new int[(int)KyConst.ScoreCategory.Count];
	}

	public void Load(string path) {
		TextAsset asset = Resources.Load(path) as TextAsset;
		if (asset == null) {
			Debug.LogWarning("stage info cannot be loaded. : " + path);
			return;
		}
		Load(asset);
	}

	public void Load(TextAsset asset) {
		Initialize();
		using (Stream ms = new MemoryStream(asset.bytes))
		using (BinaryReader br = new BinaryReader(ms)) {
			br.ReadByte();
			try {
				while (true) {
					Stage stage = new Stage();
					stage.MainID = br.ReadByte();
					stage.StageNo = br.ReadByte();
					stage.StageNo2 = br.ReadByte();
					stage.Score = br.ReadByte();
					stage.Title = br.ReadString();
					stage.Comment = br.ReadString();
					stage.Secret = br.ReadString();
					
					if (0 < stage.MainID) {
						mStages.Add(stage.MainID, stage);
					} else {
						continue;
					}
					if (0 < stage.StageNo && stage.StageNo <= KyConst.StageCountYomu) {
						mStageList1[stage.StageNo - 1] = stage;
						for (int i = 0; i < (int)KyConst.ScoreCategory.Count; ++i) {
							if ((stage.Score & (1 << i)) != 0) { mStagesCategory1[i]++; } 
						}
					}
					if (0 < stage.StageNo2 && stage.StageNo2 <= KyConst.StageCountYomanai) {
						mStageList2[stage.StageNo2 - 1] = stage;
						for (int i = 0; i < (int)KyConst.ScoreCategory.Count; ++i) {
							if ((stage.Score & (1 << i)) != 0) { mStagesCategory2[i]++; }
						}
					}
				}
			}
			catch (System.Exception) {
				return;
			}
		}
	}

	public Dictionary<int, Stage> Stages {
		get { return mStages; }
	}

	public Stage[] StageList1 {
		get { return mStageList1; }
	}

	public Stage[] StageList2 {
		get { return mStageList2; }
	}

	public int[] StagesCategory1 {
		get { return mStagesCategory1; }
	}

	public int[] StagesCategory2 {
		get { return mStagesCategory2; }
	}

	private Dictionary<int, Stage> mStages;     //	문제 정보 마스터 [문제 ID→ 스테이지 정보]
    private Stage[] mStageList1;                //	공기읽기.문제순서트 [문제번호→ 스테이지정보]
    private Stage[] mStageList2;                //	읽지 않는다. 문제순서트 [문제번호→ 스테이지 정보]
    private int[] mStagesCategory1;             //	공기읽기. 각문제 카테고리의 문제수. [카테고리 번호→문제수]
    private int[] mStagesCategory2;				//	읽지않는다. 각 문제 카테고리의 문제수. [카테고리 번호→문제수]

}
