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
	/// Inspector for wall movement classes.
	/// </summary>
	[CustomEditor(typeof(WallMovement), false)]
	public class WallMovementInspector : BaseMovementInspector <WallMovement>
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