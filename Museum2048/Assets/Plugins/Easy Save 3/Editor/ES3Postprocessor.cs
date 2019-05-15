using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Reflection;
using ES3Internal;

[InitializeOnLoad]
public class ES3Postprocessor : UnityEditor.AssetModificationProcessor
{
	public static ES3ReferenceMgr _refMgr;
	public static ES3AutoSaveMgr _autoSaveMgr;
	public static ES3DefaultSettings _defaultSettings;
	public static bool didGenerateReferences = false;
	public static ES3DefaultSettings settings;

	public static Queue<UnityEngine.Object> changedSinceLastSave = new Queue<UnityEngine.Object>();

	// This constructor is also called once when playmode is activated.
	static ES3Postprocessor()
	{
		ES3Editor.ES3Window.OpenEditorWindowOnStart();

		#if UNITY_2017_2_OR_NEWER
		EditorApplication.playModeStateChanged += PlaymodeStateChanged;
		#else
		EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
		#endif
		Undo.postprocessModifications += OnPostProcessModifications;
		EditorApplication.update += Update;
		#if UNITY_2018_1_OR_NEWER
		EditorApplication.hierarchyChanged += HierarchyChanged;
		#else
		EditorApplication.hierarchyWindowChanged += HierarchyChanged;
		#endif
	}

	static void Update()
	{
		var timeStarted = Time.realtimeSinceStartup;
		
		if(_defaultSettings == null)
			_defaultSettings = ES3Settings.GetDefaultSettings();

		if(_defaultSettings.addMgrToSceneAutomatically && _refMgr == null)
			AddManagerToScene();
			
		/* Ensure that the following code is always last in the Update() routine */
		
		if(_defaultSettings.autoUpdateReferences && _refMgr != null)
		{
			while(changedSinceLastSave.Count > 0 && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
				if(Time.realtimeSinceStartup - timeStarted > 0.02f)
					return;
				_refMgr.AddDependencies(new UnityEngine.Object[]{changedSinceLastSave.Dequeue()});
			}
		}
	}

	static void HierarchyChanged()
	{
		// If the hierarchy changed, an item might have been added, so it will need updating.
		if(Selection.activeGameObject != null)
			changedSinceLastSave.Enqueue(Selection.activeGameObject);
	}

	#if UNITY_2017_2_OR_NEWER
	static void PlaymodeStateChanged(PlayModeStateChange state)
	#else
	static void PlaymodeStateChanged()
	#endif
	{
		// This is called when we press the Play button, but before serialisation takes place.
		if(EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
		{
			if(_refMgr != null)
				_refMgr.RemoveNullValues();
		}
	}

	public static string[] OnWillSaveAssets(string[] paths)
	{
		return paths;
	}

	private static UndoPropertyModification[] OnPostProcessModifications(UndoPropertyModification[] propertyModifications)
	{
		// Ignore changes made during play mode.
		if(Application.isPlaying)
			return propertyModifications;
		
		foreach (UndoPropertyModification m in propertyModifications)
		{
			var obj = m.currentValue.target;
	
			// If this is a scene object, and it's not a reference manager, add it.
			if(!AssetDatabase.Contains(obj))
				changedSinceLastSave.Enqueue(m.currentValue.target);
		}
		return propertyModifications;
	}

	public static GameObject AddManagerToScene()
	{
		if(_refMgr != null)
			return _refMgr.gameObject;
		
		var mgr = GameObject.Find("Easy Save 3 Manager");

		if(mgr == null)
		{
			mgr = new GameObject("Easy Save 3 Manager");
			var inspectorInfo = mgr.AddComponent<ES3InspectorInfo>();
			inspectorInfo.message = "The Easy Save 3 Manager is required in any scenes which use Easy Save, and is automatically added to your scene when you enter Play mode.\n\nTo stop this from automatically being added to your scene, go to 'Window > Easy Save 3 > Settings' and deselect the 'Auto Add Manager to Scene' checkbox.";

			_refMgr = mgr.AddComponent<ES3ReferenceMgr>();
			_autoSaveMgr = mgr.AddComponent<ES3AutoSaveMgr>();

			foreach(var obj in EditorSceneManager.GetActiveScene().GetRootGameObjects())
				if(obj != mgr && !changedSinceLastSave.Contains(obj))
					changedSinceLastSave.Enqueue(obj);
					
			_refMgr.GeneratePrefabReferences();

			Undo.RegisterCreatedObjectUndo(mgr, "Enabled Easy Save for Scene");

		}
		else
		{
			_refMgr = mgr.GetComponent<ES3ReferenceMgr>();
			if(_refMgr == null)
			{
				_refMgr = mgr.AddComponent<ES3ReferenceMgr>();
				Undo.RegisterCreatedObjectUndo(_refMgr, "Enabled Easy Save for Scene");
			}

			_autoSaveMgr = mgr.GetComponent<ES3AutoSaveMgr>();
			if(_autoSaveMgr == null)
			{
				_autoSaveMgr = mgr.AddComponent<ES3AutoSaveMgr>();
				Undo.RegisterCreatedObjectUndo(_autoSaveMgr, "Enabled Easy Save for Scene");
			}
		}
		return mgr;
	}
}

// Used to initialise the reference manager for the first time.
// Displays a loading bar.
/*public class ES3ReferenceMgrInitialiser : EditorWindow
{
	ES3ReferenceMgr mgr = null;
	public Queue<GameObject> gos = new Queue<GameObject>();

	void Awake()
	{
		var go = ES3Postprocessor.AddManagerToScene();
		if(go == null)
			return;
		
		mgr = go.GetComponent<ES3ReferenceMgr>();
		if(mgr == null)
			return;

		if(mgr.IsInitialised)
			return;

		var list = new List<GameObject> ();
		EditorSceneManager.GetActiveScene().GetRootGameObjects(list);
		// Remove Easy Save 3 Manager from dependency list
		list.Remove(go);

		gos = new Queue<GameObject>(list);

		EditorApplication.update += OnUpdate;
	}

	public static void Init()
	{
		UnityEditor.EditorWindow window = GetWindow(typeof(ES3ReferenceMgrInitialiser));
		window.position = new Rect (256, 256, 256, 96);
		window.ShowUtility();
	}

	void OnUpdate()
	{
		if(gos.Count > 0)
		{
			mgr.AddDependencies(new Object[] { gos.Dequeue() });
			Repaint();
		}

		if(gos.Count == 0)
		{
			EditorApplication.update -= OnUpdate;
			mgr.GeneratePrefabReferences();
			this.Close();
		}
	}
		
	void OnGUI()
	{
		if (gos.Count > 0)
		{
			EditorGUILayout.LabelField("Adding references to Easy Save 3 Manager", EditorStyles.boldLabel);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(gos.Count+" GameObjects remaining");
			EditorGUILayout.Space();
		}

		if(GUILayout.Button("Cancel"))
		{
			this.Close();
			mgr.Clear();
		}
	}
}*/
