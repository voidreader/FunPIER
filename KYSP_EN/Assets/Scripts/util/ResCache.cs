using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResCache {

	public static bool AddResource(string path, Object obj) {
		if (sResDict.ContainsKey(path)) {
			return false;
		} else {
			sResDict.Add(path, obj);
			return true;
		}
	}

	/*public static Material GetMaterialPerTexture(string path) {

		Material material = new Material();
	}*/

	public static Object GetResource(string path) {
		Object obj = null;
		if (!sResDict.TryGetValue(path, out obj)) {
			obj = Resources.Load(path);
			sResDict.Add(path, obj);
		}
		return obj;
	}

	public static void ClearCache() {
		sResDict.Clear();
	}

	public static int ResCount {
		get { return sResDict.Count; }
	}

	private static Dictionary<string, Object> sResDict = new Dictionary<string, Object>();
}
