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
	/// Inspector for ladder and other climbing movememt classes.
	/// </summary>
	[CustomEditor(typeof(ClimbMovement), false)]
	public class ClimbMovementInspector : BaseMovementInspector <ClimbMovement>
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