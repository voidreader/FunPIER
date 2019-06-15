using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlatformerPro
{
	/// <summary>
	/// Component to attach to an object if you wish its state to be saved.
	/// </summary>
	[ExecuteInEditMode]
	public class PersistableObject : PlatformerProMonoBehaviour 
	{

		/// <summary>
		/// Should we enable persistence?
		/// </summary>
		[HideInInspector]
		[Tooltip ("Should we enable persistence?")]
		public bool enablePersistence;

		/// <summary>
		/// Unique ID for this object. You shoulnd't need to edit this, use editor debug mode if you must.
		/// </summary>
		[HideInInspector]
		[Tooltip (" Unique ID for this object. You shoulnd't need to edit this, use editor debug mode if you must.")]
		public string guid = System.Guid.NewGuid().ToString();

		/// <summary>
		/// GameObject to apply persitence settings to, defaults to self.
		/// </summary>
		[HideInInspector]
		[Tooltip ("GameObject to apply persitence settings to, defaults to self.")]
		public GameObject target;

		/// <summary>
		/// How do we implement the persistence settings.
		/// </summary>
		[HideInInspector]
		[Tooltip ("How do we implement the persistence settings.")]
		public PersistableObjectType persistenceImplementation;

		/// <summary>
		/// If true then the obejct starts disabled.
		/// </summary>
		[HideInInspector]
		[Tooltip ("If true then the obejct starts disabled.")]
		public bool defaultStateIsDisabled;


		void OnEnable()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				UpdateGuid(this);
			}
			#endif
		}

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start ()
		{
			#if UNITY_EDITOR
			if (Application.isPlaying)
			{
				PostInit ();
			}
			#else
			PostInit ();
			#endif
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void PostInit () 
		{
			if (target == null) target = gameObject;
			ProcessState ();
		}

		/// <summary>
		/// Sets the persistence defaults.
		/// </summary>
		virtual protected void SetPersistenceDefaults()
		{
			enablePersistence = PlatformerProGameManager.Instance.persistObjectsInLevel;
			persistenceImplementation = PersistableObjectType.CUSTOM;
			target = gameObject;
			defaultStateIsDisabled = false;
		}

		/// <summary>
		/// Sets the persistence state.
		/// </summary>
		/// <param name="state">State to set.</param>
		public void SetPersistenceState(bool state)
		{
			if (!enablePersistence) return;
			PlatformerProGameManager.Instance.PersistableObjectManager.SetState (guid, state, ExtraPersistenceData);
		}

		/// <summary>
		/// Sets the persistence state.
		/// </summary>
		/// <param name="state">State to set.</param>
		public void SetPersistenceState(bool state, string extraData)
		{
			if (!enablePersistence) return;
			PlatformerProGameManager.Instance.PersistableObjectManager.SetState (guid, state, extraData);
		}

		/// <summary>
		/// Processes the persisted state.
		/// </summary>
		virtual protected void ProcessState()
		{
			if (!enablePersistence) return;
			PersistableObjectData data = PlatformerProGameManager.Instance.PersistableObjectManager.GetState (guid, defaultStateIsDisabled);
			if (data.state)
			{
				switch (persistenceImplementation)
				{
				case PersistableObjectType.ACTIVATE_DEACTIVATE:
					target.SetActive (true);
					break;
				// Skip destroy there is no corresponding 'undestroy'
				case PersistableObjectType.SEND_MESSAGE:
					target.SendMessage ("SetPersistenceState", true, SendMessageOptions.RequireReceiver);
					break;
				case PersistableObjectType.CUSTOM:
					ApplyCustomPersistence (data);
					break;
				}
			} 
			else
			{
				switch (persistenceImplementation)
				{
				case PersistableObjectType.ACTIVATE_DEACTIVATE:
					target.SetActive (false);
					break;
				case PersistableObjectType.DESTROY:
					Destroy (target);
					break;
				case PersistableObjectType.SEND_MESSAGE:
					target.SendMessage ("SetPersistenceState", false, SendMessageOptions.RequireReceiver);
					break;
				case PersistableObjectType.CUSTOM:
					ApplyCustomPersistence (data);
					break;
				
				}
			}
		}

		/// <summary>
		/// Custom persistable implementation. Override to customise.
		/// </summary>
		/// <param name="data">Data.</param>
		virtual protected void ApplyCustomPersistence(PersistableObjectData data)
		{
			if (!enablePersistence) return;
			Debug.LogWarning("This persistable object doesn't support a custom persistence implementaiton.");
		}

		/// <summary>
		/// Gets the extra persistence data.
		/// </summary>
		/// <value>The extra persistence data.</value>
		virtual protected string ExtraPersistenceData
		{
			get
			{
				return null;
			}
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Try to ensure we have unique guids.
		/// </summary>
		/// <param name="target">Target.</param>
		public static Dictionary <string, PersistableObjectReference> guidRegister = new Dictionary <string, PersistableObjectReference> ();

		public static void UpdateGuid(PersistableObject target)
		{
			if (!Application.isPlaying)
			{
				if (target.guid == null || target.guid == "")
					target.guid = System.Guid.NewGuid ().ToString ();
				PersistableObjectReference por = new PersistableObjectReference (target.transform.position, target.gameObject.name, target.gameObject.scene.name);
				if (guidRegister.ContainsKey (target.guid) && !por.Equals (guidRegister [target.guid]))
				{
					target.guid = System.Guid.NewGuid ().ToString ();
					guidRegister.Add (target.guid, por);
					EditorUtility.SetDirty (target);
					UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEngine.SceneManagement.SceneManager.GetActiveScene ());
				} else if (!guidRegister.ContainsKey (target.guid))
				{
					guidRegister.Add (target.guid, por);
				}
				// GetPrefabParent replaced with GetCorrespondingObjectFromSource
				Object prefab = PrefabUtility.GetCorrespondingObjectFromSource (target.gameObject);
				if (prefab != null && prefab is GameObject )
				{
					PersistableObject po = ((GameObject)prefab).GetComponent<PersistableObject> ();
					if (po != null && po.guid != null && po.guid.Equals (target.guid))
					{
						target.guid = System.Guid.NewGuid ().ToString ();
						guidRegister.Add (target.guid, por);

						EditorUtility.SetDirty (target);
						UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEngine.SceneManagement.SceneManager.GetActiveScene ());
					}
				}
			}
		}

		public class PersistableObjectReference
		{
			Vector3 position;
			string sceneName;
			string objectName;

			public PersistableObjectReference(Vector3 position, string objectName, string sceneName)
			{
				this.position = position;
				this.sceneName = sceneName;
				this.objectName = objectName;
			}

			public override bool Equals (object obj)
			{
				if (obj == null)
					return false;
				if (ReferenceEquals (this, obj))
					return true;
				if (obj.GetType () != typeof(PersistableObjectReference))
					return false;
				PersistableObjectReference other = (PersistableObjectReference)obj;
				return Vector3.Distance (position, other.position) < 0.001f && objectName == other.objectName && sceneName == other.sceneName;
			}

			public override int GetHashCode ()
			{
				unchecked
				{
					return position.GetHashCode () ^ (sceneName != null ? sceneName.GetHashCode () : 0);
				}
			}

			public override string ToString ()
			{
				return string.Format ("[PersistableObjectReference: position={0}, sceneName={1}, objectName={2}]", position, sceneName, objectName);
			}
			
		}
		#endif
	}
}
