#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{
	/// <summary>
	/// Inspector for ground movement classes.
	/// </summary>
	[CustomEditor(typeof(SpecialMovement), false)]
	public class SpecialMovementInspector : BaseMovementInspector <SpecialMovement>
	{

		#region Unity hooks

		/// <summary>
		/// When the component is accessed update.
		/// </summary>
		void OnEnable()
		{
			InitTypes ();
		}

		#endregion

	}
	
}