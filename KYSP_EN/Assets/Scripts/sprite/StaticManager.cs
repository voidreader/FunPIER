using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// シングルトンプレハブを管理するクラスです。
/// インスタンスの数が１つ以下であることが保証されている
/// プレハブをシングルトンプレハブと呼びます。
/// その保証と管理を行うのがこのクラスです。
/// 生成されたシングルトンプレハブのインスタンスは、
/// このStaticManagerコンポーネントを持つGameObjectの子供として常駐します。
/// </summary>
public class StaticManager : MonoBehaviour {

	#region MonoBehaviour Methods

	void Awake() {
		if (sInstance == null) {
			sInstance = this;
		}
	}

	#endregion

	#region Methods

	public static GameObject GetInstanceOf(GameObject prefab) {
		//	プレハブの同一性検査はnameで行います。
		if (sInstance == null) { Debug.LogWarning("instance is null"); }
		foreach (Transform child in sInstance.transform) {
			if (child.gameObject.name == prefab.name) {
				return child.gameObject;
			}
		}
		GameObject instance = (GameObject)Object.Instantiate(prefab);
		instance.name = prefab.name;
		instance.transform.parent = sInstance.transform;
		DebugUtil.Log("instantiate " + instance.name);
		return instance;
	}

	#endregion

	#region Fields

	private static StaticManager sInstance;



    #endregion
}
