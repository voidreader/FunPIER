using UnityEngine;
using System.Collections;

public static class KyUtil {

	/// <summary>
	/// �w���GameObject�y�т��̂��ׂĂ̎qGameObject�̕\����Ԃ�ύX����B
	/// </summary>
	/// <param name="obj">�Ώۂ�GameObject</param>
	/// <param name="visible">���E�s��</param>
	public static void SetVisibleWithChildren(GameObject obj, bool visible) {
		if (obj.GetComponent<Renderer>() != null) {
			obj.GetComponent<Renderer>().enabled = visible;
		}
		Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
		foreach (Renderer renderer in renderers) {
			renderer.enabled = visible;
		}
	}

	public static void SetActiveWithChildren(GameObject obj, bool active) {
		Transform[] transforms = obj.GetComponentsInChildren<Transform>(true);
		foreach (Transform transform in transforms) {
			transform.gameObject.active = active;
		}
	}

	public static T GetComponentInChild<T>(GameObject obj, string name) 
		where T : Component {
		Transform child = obj.transform.Find(name);
		if (child != null) {
			return child.GetComponent<T>();
		} else {
			return null;
		}
	}

	public static GameObject FindChild(GameObject obj, string name) {
		Transform child = obj.transform.Find(name);
		if (child != null) {
			return child.gameObject;
		} else {
			return null;
		}
	}

	public static GameObject FindChildRecursively(GameObject obj, string name) {
		foreach (Transform child in obj.transform) {
			if (child.gameObject.name == name) {
				return child.gameObject;
			} else {
				GameObject found = FindChildRecursively(child.gameObject, name);
				if (found != null) {
					return found;
				}
			}
		}
		return null;
	}

	public static GameObject FindSibling(GameObject obj, string name) {
		Transform parent = obj.transform.parent;
		if (parent == null) {
			return null;
		}
		Transform sibling = parent.transform.Find(name);
		if (sibling != null) {
			return sibling.gameObject;
		} else {
			return null;
		}
	}

	public static bool ContainsIn(GameObject obj, Rect rect, Vector3 pos) {
		if (obj == null) { return false; }
		Rect objrect = new Rect(
			rect.xMin + obj.transform.position.x,
			rect.yMin + obj.transform.position.y,
			rect.width,
			rect.height
		);
		return objrect.Contains(pos);
	}

	public static Vector3 ClampByRect(Vector3 vec, Rect rect) {
		vec.x = Mathf.Clamp(vec.x, rect.xMin, rect.xMax);
		vec.y = Mathf.Clamp(vec.y, rect.yMin, rect.yMax);
		return vec;
	}

	public static void SetText(GameObject obj, string childName, string text) {
		SpriteTextCustom sprite = GetComponentInChild<SpriteTextCustom>(obj, childName);
		if (sprite != null) {
			sprite.Text = text;
			sprite.UpdateAll();
		}
	}

	public static void AlignTop(GameObject obj, float offset) {
		Vector3 pos = obj.transform.localPosition;
		pos.y = Camera.main.orthographicSize + offset;
		obj.transform.localPosition = pos;
	}

	public static void AlignBottom(GameObject obj, float offset) {
		Vector3 pos = obj.transform.localPosition;
		pos.y = -Camera.main.orthographicSize + offset;
		obj.transform.localPosition = pos;
	}

	public delegate void ChildHandler(GameObject obj);

	public static void ForEachChildIn(GameObject obj, ChildHandler handler, bool recursive) {
		if (recursive) {
			Transform[] children = obj.GetComponentsInChildren<Transform>(true);
			foreach (Transform child in children) {
				handler(child.gameObject);
			}
		} else {
			foreach (Transform child in obj.transform) {
				handler(child.gameObject);
			}
		}
	}
}
