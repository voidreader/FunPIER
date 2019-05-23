using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// デザインパラメータを集約・管理するクラス。
/// </summary>
public class KyDesignParams {

	#region Methods

	private void Initialize() {
		mDesignParams = new Dictionary<int, int>();
	}

	public void Load(string path) {
		TextAsset asset = Resources.Load(path) as TextAsset;
		if (asset == null) {
			Debug.LogWarning("design params cannot be loaded. : " + path);
			return;
		}
		Load(asset);
	}

	public void Load(TextAsset asset) {
		Initialize();
		int lastId = 0;
		using (Stream ms = new MemoryStream(asset.bytes))
		using (BinaryReader br = new BinaryReader(ms)) {
			br.ReadByte();
			try {
				while (true) {
					int id = br.ReadInt16();
					int param = br.ReadInt16();
					mDesignParams.Add(id, param);
					lastId = id;
				}
			}
			catch (System.Exception) {
				Debug.LogWarning("lastId : " + lastId);
				return;
			}
		}
	}

	public static int GetParam(int id) {
		int param = 0;
		Instance.mDesignParams.TryGetValue(id, out param);
		return param;
	}

	#endregion

	#region Properties

	public static KyDesignParams Instance {
		get {
			if (sInstance == null) {
				sInstance = new KyDesignParams();
			}
			return sInstance; 
		}
	}

	#endregion

	#region Fields

	private static KyDesignParams sInstance = null;
	private Dictionary<int, int> mDesignParams;	//	デザインパラメータマスター[ID⇒パラメータ]

	#endregion
}
