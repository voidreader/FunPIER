using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// テキストテーブルを管理するクラス。
/// 各種ヘルパーメソッドも用意。
/// </summary>
public class KyText {

	#region Singleton

	private static KyText sInstance;

	public static KyText Instance {
		get {
			if (sInstance == null) {
				sInstance = new KyText();
			}
			return sInstance;
		}
	}

	#endregion

	private KyText() {
		mTextTable = new Dictionary<int, string>();
	}

	public void Load(string path) {
		TextAsset asset = Resources.Load(path) as TextAsset;
		if (asset == null) {
			Debug.LogWarning("text table cannot be loaded. : " + path);
			return;
		}
		Load(asset);
	}

	public void Load(TextAsset asset) {
		using (Stream ms = new MemoryStream(asset.bytes))
		using (BinaryReader br = new BinaryReader(ms)) {
			br.ReadByte();
			try {
				while (true) {
					int textId = br.ReadInt32();
					string text = br.ReadString();
					sInstance.mTextTable.Add(textId, text);
				}
			}
			catch (System.Exception) {
				return;
			}
		}
	}

	public static string GetText(int textId) {
		string text = "";
		sInstance.mTextTable.TryGetValue(textId, out text);
		if (text == null) { text = ""; }
		DebugUtil.Log("get text:" + text);
		return text;
	}

	public static string BuildTwitterMessage(int type, int score, int category, int rank, int comment) {
		if (type == 0) {
			int cattext = (rank == 0 || rank == 5) ? 0 : category;
			return string.Format(GetText(23000),
				score,
				GetText(21000 + category),
				GetText(21010 + rank * 10 + cattext),
				GetText(23010 + comment)) 
				+ GetText(23100);
		} else if (type == 1) {
			int cattext = (rank == 0 || rank == 5) ? 0 : category;
			return string.Format(GetText(23001),
				score,
				GetText(21000 + category),
				GetText(21100 + rank * 10 + cattext),
				GetText(23020 + comment))
				+ GetText(23100);
		} else if (type == 2) {
			return string.Format(GetText(23002),
				GetText(21081 + rank),
				GetText(23030 + comment))
				+ GetText(23100);
		} else if (type == 3) {
			return string.Format(GetText(23003),
				score,
				GetText(20052 + rank),
				GetText(23040 + comment))
				+ GetText(23100);
		}
		return "";
	}

	private Dictionary<int, string> mTextTable;

}
