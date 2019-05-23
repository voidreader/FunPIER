using UnityEngine;
using System.Collections;
using System.IO;

public class KySaveData {

	public enum DataKind {
		Preferences,
		Record,
		Intermit1,
		Intermit2,
	}

	static readonly string[] FileNames = {
		"prefs.save",
		"record.save",
		"consider1.save",
		"intermit2.save",
	};

	/// <summary>
	/// 설정 데이터
	/// </summary>
	public class Preferences {
		public Preferences() {
			Initialize();
		}

		public void Initialize() {
			SoundVolume = 3;
			YuragiEnabled = true;
			TwitterID = "";
			TwitterName = "";
			TwitterToken = "";
			TwitterTokenSecret = "";
		}

		public void Save(BinaryWriter writer) {
			writer.Write((byte)SoundVolume);
			writer.Write(YuragiEnabled);
			writer.Write(TwitterID);
			writer.Write(TwitterName);
			writer.Write(TwitterToken);
			writer.Write(TwitterTokenSecret);
			DebugUtil.Log("Preferences Data Saved.");
		}

		public void Load(BinaryReader reader) {
			SoundVolume = reader.ReadByte();
			YuragiEnabled = reader.ReadBoolean();
			TwitterID = reader.ReadString();
			TwitterName = reader.ReadString();
			TwitterToken = reader.ReadString();
			TwitterTokenSecret = reader.ReadString();
			DebugUtil.Log("Preferences Data Loaded.");
		}

		public int SoundVolume; //	사운드 볼륨 6 단계 [0-5].
        public bool YuragiEnabled;  //	진동의 활성화 / 비활성화.

        public string TwitterID;	//	TwitterユーザID
		public string TwitterName;	//	Twitterユーザ名
		public string TwitterToken;	//	Twitterユーザトークン
		public string TwitterTokenSecret;	//	Twitterユーザトークンシークレット
	}

	/// <summary>
	/// 게임 기록 데이터 
	/// </summary>
	public class Record {
		public Record() {
			Initialize();
		}

		public void Initialize() {
			Scores = new byte[6][];
			for (int i = 0; i < 6; ++i) {
				Scores[i] = new byte[KyConst.ScoreCategoryCount];
			}
			SecretFlags = new BitArray(128);
			SecretFlags.SetAll(false);
			if (KyDebugPrefs.OpenAllSecrets) {
				SecretFlags.SetAll(true);
			}
			ExtraScores = new int[KyConst.ExtraStageCount];
			GameClearCount = new byte[3];
		}

		public void Save(BinaryWriter writer) {
			for (int i = 0; i < KyConst.ScoreCategoryCount; ++i) {
				writer.Write(Scores[i]);
			}
			byte[] secretFlags = new byte[16];
			SecretFlags.CopyTo(secretFlags, 0);
			writer.Write(secretFlags);
			for (int i = 0; i < ExtraScores.Length; ++i) {
				writer.Write(ExtraScores[i]);
			}
			writer.Write(GameClearCount, 0, GameClearCount.Length);
			DebugUtil.Log("Record Data Saved.");
		}

		public void Load(BinaryReader reader) {
			for (int i = 0; i < KyConst.ScoreCategoryCount; ++i) {
				Scores[i] = reader.ReadBytes(KyConst.ScoreCategoryCount);
			}
			byte[] secretFlags = reader.ReadBytes(16);
			SecretFlags = new BitArray(secretFlags);
			for (int i = 0; i < ExtraScores.Length; ++i) {
				ExtraScores[i] = reader.ReadInt32();
			}
			GameClearCount = reader.ReadBytes(3);
			DebugUtil.Log("Record Data Loaded.");
		}

		public byte[] GetScores(int mode, int type) {
			return Scores[mode * 3 + type];
		}

		public int GetScore(int mode, int type, int category) {
			return Scores[mode * 3 + type][category];
		}

		public int GetScoreSum(int mode, int type) {
			int sum = 0;
			byte[] score = GetScores(mode, type);
			for (int i = 0; i < 6; ++i) {
				sum += score[i];
			}
			return sum;
		}

		public int GetScoreSumInPercent(int mode, int type) {
			int stageCount = mode == 0 ? KyConst.StageCountYomu : KyConst.StageCountYomanai;
			return GetScoreSum(mode, type) * 100 / stageCount;
		}

		public float[] GetScoresInRate(int mode, int type) {
			float[] rates = new float[KyConst.ScoreCategoryCount];
			int[] category = mode == 0 ? 
				KyApplication.StageInfo.StagesCategory1 : 
				KyApplication.StageInfo.StagesCategory2;
			byte[] scores = GetScores(mode, type);
			for (int i = 0; i < (int)KyConst.ScoreCategory.Count; ++i) {
				rates[i] = (float)scores[i] / category[i];
			}
			return rates;
		}

		public void SetScoresByResults(int mode, int type, BitArray results) {
			byte[] scores = GetScores(mode, type);
			for (int i = 0; i < KyConst.ScoreCategoryCount; ++i) {
				scores[i] = 0;
			}
			KyStageInfo.Stage[] stages = mode == 0 ? 
				KyApplication.StageInfo.StageList1 :
				KyApplication.StageInfo.StageList2;
			int count = Mathf.Min(stages.Length, results.Length);
			for (int i = 0; i < count; ++i) {
				if (results[i]) {
					KyStageInfo.Stage stage = stages[i];
					for (int j = 0; j < KyConst.ScoreCategoryCount; ++j) {
						if ((stage.Score & (1 << j)) != 0) {
							scores[j]++;
						}
					}
				}
			}
			DebugUtil.Log("scores:"
				+ scores[0] + "," + scores[1] + "," + scores[2] + ","
				+ scores[3] + "," + scores[4] + "," + scores[5]);
		}

		public byte[][] Scores;				//	최근 성적
		public BitArray SecretFlags;		//	시크릿 달성 플래그
		public int[] ExtraScores;           //	엑스트라 모드의 최고 점수.
        public byte[] GameClearCount;       //	각 모드의 게임을 클리어 한 횟수입니다.
        public BitArray MiscFlags;
	}

	/// <summary>
	/// 진지하게 모드 중단 데이터 
	/// </summary>
	public class Intermit {
		public Intermit() {
			StageResults = new BitArray(128);
			Initialize();
		}

		public void Initialize() {
			StageIndex = 0;
			StageResults.SetAll(false);
		}

        /// <summary>
        ///  신규 저장 
        /// </summary>
        /// <param name="writer"></param>
        public void SaveNew(BinaryWriter writer) {
            writer.Write((byte)0);
            byte[] secretFlags = new byte[16];
            StageResults.CopyTo(secretFlags, 0);
            writer.Write(secretFlags);
            DebugUtil.Log("Intermit Data Saved NEW!!!.");
        }

		public void Save(BinaryWriter writer) {
			writer.Write((byte)StageIndex);
			byte[] secretFlags = new byte[16];
			StageResults.CopyTo(secretFlags, 0);
			writer.Write(secretFlags);
			DebugUtil.Log("Intermit Data Saved.");
		}

		public void Load(BinaryReader reader) {
			StageIndex = reader.ReadByte();
			byte[] secretFlags = reader.ReadBytes(16);
			StageResults = new BitArray(secretFlags);

            Debug.Log(" >>> Intermit StageIndex :: " + StageIndex);

			DebugUtil.Log("Intermit Data Loaded.");
		}

		public int StageIndex;				//	다음 스테이지 번호
		public BitArray StageResults;		//	각 스테이지 결과
	}

    /// <summary>
    ///	게임 중에 필요한 컨텍스트 정보 데이터
	/// </summary>
	public class Context {

		public Context() {
			Initialize();
		}

		public void Initialize() {
			ChapterStageNo = 0;
			SecretStageNo = 0;
		}

		public int ChapterStageNo = 0;
		public int SecretStageNo = 0;
		public bool RecordBetterThanLast = false;
		public bool RecordNewRecord = false;
		public bool RecordPerfect = false;
	}

	private KySaveData() {
		mPrefs = new Preferences();
		mRecord = new Record();
		mIntermit1 = new Intermit();
		mIntermit2 = new Intermit();
		mContext = new Context();
		//Initialize();
	}

	public void Initialize() {
		mPrefs.Initialize();
		mRecord .Initialize();
		mIntermit1.Initialize();
		mIntermit2.Initialize();
		mContext.Initialize();
	}

	public void Save(DataKind kind) {
		string path = GetFilePath(kind);
		DebugUtil.Log("save file path : " + path);
		using (FileStream fs = File.Open(path, FileMode.Create))
		using (BinaryWriter writer = new BinaryWriter(fs)) {
			switch (kind) {
			case DataKind.Preferences:
				mPrefs.Save(writer); break;
			case DataKind.Record:
				mRecord.Save(writer); break;
			case DataKind.Intermit1:
				mIntermit1.Save(writer); break;
			case DataKind.Intermit2:
				mIntermit2.Save(writer); break;
			}
		}
	}


    public void SaveNew(DataKind kind) {
        string path = GetFilePath(kind);
        DebugUtil.Log("save file path : " + path);
        using (FileStream fs = File.Open(path, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(fs)) {
            switch (kind) {
                case DataKind.Preferences:
                    mPrefs.Save(writer); break;
                case DataKind.Record:
                    mRecord.Save(writer); break;
                case DataKind.Intermit1:
                    mIntermit1.SaveNew(writer); break;
                case DataKind.Intermit2:
                    mIntermit2.SaveNew(writer); break;
            }
        }
    }


    public void SaveAll() {
		Save(DataKind.Preferences);
		Save(DataKind.Record);
		Save(DataKind.Intermit1);
		Save(DataKind.Intermit2);
	}

	public void Load(DataKind kind) {
		string path = GetFilePath(kind);
		DebugUtil.Log("load file path : " + path);
		if (!File.Exists(path)) {

            Debug.Log(">> No exists :: " + kind.ToString() +  " << ");

            SaveNew(kind);
		}
		using (FileStream fs = File.Open(path, FileMode.Open))
		using (BinaryReader reader = new BinaryReader(fs)) {
			switch (kind) {
			case DataKind.Preferences:
				mPrefs.Load(reader); break;
			case DataKind.Record:
				mRecord.Load(reader); break;
			case DataKind.Intermit1:
                    Debug.Log(">> Intermit1 Data Load  << ");
				mIntermit1.Load(reader); break;
			case DataKind.Intermit2:
				mIntermit2.Load(reader); break;
			}
		}
	}

	public void LoadAll() {

        Debug.Log(" >>> Load ALL Data <<< ");

		Load(DataKind.Preferences);
		Load(DataKind.Record);
		Load(DataKind.Intermit1);
		Load(DataKind.Intermit2);
	}

	public string GetFilePath(DataKind kind) {
		string path;
		if (Application.platform == RuntimePlatform.WindowsEditor) {
			path = Directory.GetParent(Application.dataPath) + "/" + FileNames[(int)kind];
		} else if (Application.platform == RuntimePlatform.WindowsPlayer) {
			path = "./" + FileNames[(int)kind];
		} else {
			path = Application.persistentDataPath + "/" + FileNames[(int)kind];
		}
		return path;
	}

	public static KySaveData Instance {
		get {
			if (mInstance == null) {
				mInstance = new KySaveData();
			}
			return mInstance;
		}
	}

	public static Preferences PrefsData {
		get { return mInstance.mPrefs; }
	}

	public static Record RecordData {
		get { return mInstance.mRecord; }
	}

	public static Intermit IntermitData1 {
		get { return mInstance.mIntermit1; }
	}

	public static Intermit IntermitData2 {
		get { return mInstance.mIntermit2; }
	}

	public static Context ContextData {
		get { return mInstance.mContext; }
	}

	private static KySaveData mInstance;
	private Preferences mPrefs;
	private Record mRecord;
	private Intermit mIntermit1;
	private Intermit mIntermit2;
	private Context mContext;
}
