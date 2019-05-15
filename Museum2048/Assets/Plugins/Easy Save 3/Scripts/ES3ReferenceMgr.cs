using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ES3Internal;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Reflection;
using System;
#endif

public class ES3ReferenceMgr : ES3ReferenceMgrBase, ISerializationCallbackReceiver 
{
	public void OnBeforeSerialize()
	{
		#if UNITY_EDITOR
		if(BuildPipeline.isBuildingPlayer)
		{
			// This is called before building.
			RemoveNullValues();
		}
		#endif
	}

	public void OnAfterDeserialize(){}

	#if UNITY_EDITOR

	public void RefreshDependencies()
	{
		var gos = EditorSceneManager.GetActiveScene().GetRootGameObjects();
		// Remove Easy Save 3 Manager from dependency list
		AddDependencies(gos);
	}

	public void AddDependencies(UnityEngine.Object[] objs, float timeoutSecs=2)
	{
		var startTime = Time.realtimeSinceStartup;
        
        foreach(var obj in objs)
        {
        	if(Time.realtimeSinceStartup - startTime > timeoutSecs)
        		break;
        	
        	if(obj.name == "Easy Save 3 Manager")
        		 continue;
        	
    	    var dependencies = EditorUtility.CollectDependencies(new UnityEngine.Object[]{obj});
    
    		foreach(var dependency in dependencies)
    		{
    			if(dependency == null || !CanBeSaved(dependency))
    				continue;
    
    			Add(dependency);
    		}
        }
        Undo.RecordObject(this, "Update Easy Save 3 Reference List");
	}

	// Adds prefabs to the prefab manager.
	// Note: this assumes that the object is actually a prefab, so no checks
	// are performed to check that this is a normal GameObject or prefab instance.
	/*public void AddPrefabDependencies(ES3Prefab es3Prefab)
	{
		AddPrefab(es3Prefab);

		// Get GameObject and it's children and add them to the reference list.
		foreach(var obj in EditorUtility.CollectDependencies(new UnityEngine.Object[]{es3Prefab.gameObject}))
		{
			if(obj == null || !CanBeSaved(obj))
				continue;
			
			if(es3Prefab.Get(obj) == -1)
			{
				es3Prefab.Add(obj);
				Undo.RecordObject(es3Prefab, "Update Easy Save 3 Prefab");
			}
		}
	}

	public void RefreshPrefabDependencies()
	{
		foreach (var kvp in idRef)
		{
			var obj = kvp.Value;
			// If object in dependency list is a prefab in the Assets folder, process it.
			//If this object is a prefab, generate references for it.
			#if UNITY_2018_OR_NEWER
			if(obj.GetType() == typeof(GameObject) && PrefabUtility.IsPartOfPrefabAsset(obj))
			#else
			if(obj.GetType() == typeof(GameObject) && PrefabUtility.GetPrefabType(obj) == PrefabType.Prefab)
			#endif
			{
				var es3Prefab = ((GameObject)obj).GetComponent<ES3Prefab>();
				if(es3Prefab != null)
					AddPrefabDependencies(es3Prefab);
			}
		}
	}*/

	public void GeneratePrefabReferences()
	{
		bool undoRecorded = false;

		if(this.prefabs.RemoveAll(item => item == null) > 0)
		{
			Undo.RecordObject(this, "Update Easy Save 3 Reference List");
			undoRecorded = true;
		}

		var es3Prefabs = Resources.FindObjectsOfTypeAll<ES3Prefab>();

		if(es3Prefabs.Length == 0)
			return;

		foreach(var es3Prefab in es3Prefabs)
		{
			#if UNITY_2018_3_OR_NEWER
			var prefabType = PrefabUtility.GetPrefabInstanceStatus(es3Prefab.gameObject);
			if(prefabType != PrefabInstanceStatus.NotAPrefab && prefabType != PrefabInstanceStatus.MissingAsset)
				continue;
			#else
					var prefabType = PrefabUtility.GetPrefabType(es3Prefab.gameObject);
					if(prefabType != PrefabType.Prefab && prefabType != PrefabType.MissingPrefabInstance)
						continue;
			#endif

			if(GetPrefab(es3Prefab) == -1)
			{
				AddPrefab(es3Prefab);
				if(!undoRecorded)
				{
					Undo.RecordObject(this, "Update Easy Save 3 Reference List");
					undoRecorded = true;
				}
			}

			bool prefabUndoRecorded = false;

			// Get GameObject and it's children and add them to the reference list.
			foreach(var obj in EditorUtility.CollectDependencies(new UnityEngine.Object[]{es3Prefab}))
			{
				if(obj == null || !CanBeSaved(obj))
					continue;

				if(es3Prefab.Get(obj) == -1)
				{
					es3Prefab.Add(obj);
					if(!prefabUndoRecorded)
					{
						Undo.RecordObject(es3Prefab, "Update Easy Save 3 Prefab");
						prefabUndoRecorded = true;
					}
				}
			}
		}
    }

    public static bool CanBeSaved(UnityEngine.Object obj)
	{
		// Check if any of the hide flags determine that it should not be saved.
		if(	(((obj.hideFlags & HideFlags.DontSave) == HideFlags.DontSave) || 
		     ((obj.hideFlags & HideFlags.DontSaveInBuild) == HideFlags.DontSaveInBuild) ||
		     ((obj.hideFlags & HideFlags.DontSaveInEditor) == HideFlags.DontSaveInEditor) ||
		     ((obj.hideFlags & HideFlags.HideAndDontSave) == HideFlags.HideAndDontSave)))
		{
			var type = obj.GetType();
			// Meshes are marked with HideAndDontSave, but shouldn't be ignored.
			if(type != typeof(Mesh) && type != typeof(Material))
				return false;
		}
		return true;
	}

	#endif
}
