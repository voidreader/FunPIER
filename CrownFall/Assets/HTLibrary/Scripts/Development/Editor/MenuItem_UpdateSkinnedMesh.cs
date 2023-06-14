using UnityEngine;
using UnityEditor;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class UpdateSkinnedMeshWindow : EditorWindow
	{
		//---------------------------------------
		[MenuItem("Window/HTLibrary/Update Skinned Mesh Bones")]
		public static void OpenWindow()
		{
			var window = GetWindow<UpdateSkinnedMeshWindow>();
			window.titleContent = new GUIContent("HT Skinned Mesh Updater");
		}

		//---------------------------------------
		private SkinnedMeshRenderer _targetSkin = null;
		private SkinnedMeshRenderer _originalSkin = null;

		//---------------------------------------
		private void OnGUI()
		{
			_targetSkin = EditorGUILayout.ObjectField("TargetSkin", _targetSkin, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
			_originalSkin = EditorGUILayout.ObjectField("OriginalSkin", _originalSkin, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;

			//-----
			GUI.enabled = (_targetSkin != null && _originalSkin != null)? true : false;

			//-----
			if (GUILayout.Button("Update Skinned Mesh Renderer"))
				ResourceUtils.RefreshSkinnedMeshBones(_originalSkin, _targetSkin);
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}