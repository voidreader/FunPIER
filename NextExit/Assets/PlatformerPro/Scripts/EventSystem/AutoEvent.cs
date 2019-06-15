using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Attach this to an object to raise an event from standard Unity hooks (awake, start, enable, etc).
	/// </summary>
	public class AutoEvent : MonoBehaviour 
	{

		/// <summary>
		/// Rasie event on Start.
		/// </summary>
		public bool raiseOnStart;

		/// <summary>
		/// Raise event on enable.
		/// </summary>
		public bool raiseOnEnable;

		/// <summary>
		/// Raise event on destroy.
		/// </summary>
		public bool raiseOnDestroy;

		/// <summary>
		/// Generic event raised by this object.
		/// </summary>
		public event System.EventHandler<System.EventArgs> GenericEvent;

		/// <summary>
		/// Raises the generic event event.
		/// </summary>
		protected void OnGenericEvent()
		{
			if (GenericEvent != null) GenericEvent (this, null);
		}

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start () 
		{
			if (raiseOnStart) RaiseEvent ();
		}

		/// <summary>
		/// Unity enable hook.
		/// </summary>
		void OnEnable () 
		{
			StopCoroutine (CheckEnableAfterYield ());
			StartCoroutine (CheckEnableAfterYield ());
		}

		/// <summary>
		/// Delay enabled check by one frame to avoid issues with unintialised variables on activation.
		/// </summary>
		/// <returns>The enable after yield.</returns>
		protected IEnumerator CheckEnableAfterYield()
		{
			yield return true;
			if (raiseOnEnable) RaiseEvent ();
		}

		/// <summary>
		/// Unity destroy hook.
		/// </summary>
		void OnDestroy () 
		{
			if (raiseOnDestroy) RaiseEvent ();
		}

		/// <summary>
		/// Raises the event.
		/// </summary>
		public void RaiseEvent()
		{
			OnGenericEvent ();
		}

	}
}