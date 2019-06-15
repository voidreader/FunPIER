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
	/// Inspector for damage movement classes.
	/// </summary>
	[CustomEditor(typeof(DamageMovement), false)]
	public class DamageMovementInspector : BaseMovementInspector <DamageMovement>
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