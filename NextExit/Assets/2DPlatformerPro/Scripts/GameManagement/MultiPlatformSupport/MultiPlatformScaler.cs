using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

namespace PlatformerPro
{
	/// <summary>
	/// ESets an objects scale based on the platform.
	/// </summary>
	[System.Serializable]
	public class MultiPlatformScaler : MonoBehaviour
	{
		/// <summary>
		/// How much should the object be scaled?
		/// </summary>
		[Tooltip ("How much should the object be scaled?")]
		public Vector3 scalingFactor = Vector3.one;
		
		/// <summary>
		/// The conditions to check.
		/// </summary>
		[Tooltip ("The conditions to check.")]
		public MultiPlatformEnablerCondition[] conditions;
		
		void Awake()
		{
			DoActions ();
		}
		
		/// <summary>
		/// Do the actionsfor the matched conditions.
		/// </summary>
		protected virtual void DoActions()
		{
			foreach (MultiPlatformEnablerCondition c in conditions)
			{
				if (IsMatched(c))
				{
					c.target.transform.localScale = new Vector3(c.target.transform.localScale.x * scalingFactor.x, 
					                                            c.target.transform.localScale.y * scalingFactor.y, 
					                                            c.target.transform.localScale.z * scalingFactor.z);
				}
			}
		}
		
		/// <summary>
		/// Returns true if the condition applies.
		/// </summary>
		/// <returns><c>true</c> if the condition is matched; otherwise, <c>false</c>.</returns>
		/// <param name="c">Condition.</param>
		virtual protected bool IsMatched(MultiPlatformEnablerCondition c)
		{
			#if UNITY_EDITOR
			if (c.forceMatch) return true;
			#endif
			if (c.useDeviceType && SystemInfo.deviceType != c.deviceType) return false;
			if (c.usePlatform && Application.platform != c.platform) return false;
			if (c.largeScreen && !MultiPlatformEnabler.IsLargeTouchScreen ()) return false;
			if (c.modelStringRegex != null && c.modelStringRegex != "") 
			{
				string model = SystemInfo.deviceModel;
				Regex regex = new Regex(c.modelStringRegex);
				Match match = regex.Match(model);
				if (!match.Success) return false;
			}
			return true;
		}

	}
	
}
