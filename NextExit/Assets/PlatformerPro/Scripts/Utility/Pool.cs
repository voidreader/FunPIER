using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PlatformerPro
{
	/// <summary>
	/// A simple pool of objects.
	/// </summary>
	public class Pool : MonoBehaviour
	{
		[Header ("Pool")]

		/// <summary>
		/// The pooled objects. You can assign the pool here, this can be useful if you
		/// don't want all objects to be the same.
		/// </summary>
		[Tooltip ("The pooled objects. You can assign the pool here, this can be useful if you don't want all objects to be the same.")]
		public List<GameObject> pooledObjects;


		/// <summary>
		/// Prefab to use to create new pool objects.
		/// </summary>
		[Tooltip ("Prefab to use to create new pool objects.")]
		public GameObject poolPrefab;

		/// <summary>
		/// Starting size of pool.
		/// </summary>
		[Tooltip ("Starting size of pool.")]
		[Range(0,999)]
		public int startingSize;

		/// <summary>
		/// Should we create more objects if pool is exhausted or should we throw an error.
		/// </summary>
		[Tooltip ("If true we create more objects if pool is exhausted, if false we raise a warning.")]
		public bool createMoreIfExhausted;

		/// <summary>
		/// Current position in pool.
		/// </summary>
		protected int currentPosition;

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		public void Start()
		{
			Init ();
		}

		/// <summary>
		/// Initialise pool.
		/// </summary>
		virtual protected void Init()
		{
			if (pooledObjects == null || pooledObjects.Count < startingSize)
			{
				PopulatePool();
			}
		}

		/// <summary>
		/// Initialise the pool.
		/// </summary>
		virtual protected void PopulatePool()
		{
			if (pooledObjects == null) pooledObjects = new List<GameObject> ();
			int numberToCreate = startingSize - pooledObjects.Count;
			if (numberToCreate < 0) numberToCreate = 0;
			for (int i = 0; i < numberToCreate; i++)
			{
				CreateInstance();
			}
			currentPosition = 0;
		}

		/// <summary>
		/// Gets an instance form the pool, or null if no instances available.
		/// </summary>
		/// <returns>The instance or null.</returns>
		virtual public GameObject GetInstance()
		{
			GameObject instance = null;

			// Special case error message
			if (pooledObjects.Count == 0)
			{
				if (!createMoreIfExhausted)
				{
					Debug.LogWarning("Create more if exhausted is false but the pools starting size is 0, no object will ever be available");
				}
			}

			// Find free object
	
			for (int i = 0; i < pooledObjects.Count ; i++)
			{
				if (!pooledObjects[currentPosition].activeSelf)
				{
					instance = pooledObjects[currentPosition];
					currentPosition++;
					if (currentPosition >= pooledObjects.Count) currentPosition = 0;
					break;
				}
				currentPosition++;
				if (currentPosition >= pooledObjects.Count) currentPosition = 0;
			}

			// Create if needed
			if (instance == null)
			{
				if (createMoreIfExhausted)
				{
					instance = CreateInstance();
				}
				else
				{
#if UNITY_EDITOR
					// Only show this in the editor as in some cases it may be a desired state
					Debug.Log("Tried to get instance from pool, but pool was exhausted.");
#endif
					return null;
				}
			}
			instance.SetActive(true);
			Reset (instance);
			return instance;
		}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <returns>The instance.</returns>
		virtual protected GameObject CreateInstance()
		{
			GameObject instance = (GameObject) GameObject.Instantiate (poolPrefab);
			instance.transform.position = transform.position;
			instance.SetActive (false);
			pooledObjects.Add (instance);
			return instance;
		}

		/// <summary>
		/// Reset the specified instance. Specific implementations should override this.
		/// </summary>
		/// <param name="instance">Instance.</param>
		virtual protected void Reset(GameObject instance)
		{

		}
	}
}
