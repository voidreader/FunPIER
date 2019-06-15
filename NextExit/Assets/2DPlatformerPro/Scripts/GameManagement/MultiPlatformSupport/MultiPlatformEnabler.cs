using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

namespace PlatformerPro
{
	/// <summary>
	/// Enables or Disables items based on the platform.
	/// </summary>
	[System.Serializable]
	public class MultiPlatformEnabler : MonoBehaviour
	{
		/// <summary>
		/// What does this enabler do?
		/// </summary>
		[Tooltip ("What does this enabler do?")]
		public MultiPlatformEnablerType enablerType;

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
					switch (enablerType)
					{
						case MultiPlatformEnablerType.ENABLE_ALL_MATCHES:
							c.target.SetActive(true);
							break;
						case MultiPlatformEnablerType.ENABLE_FIRST_MATCH:
							c.target.SetActive(true);
							return;
						case MultiPlatformEnablerType.DISABLE_ALL_MATCHES:
							c.target.SetActive(false);
							break;
						case MultiPlatformEnablerType.DISABLE_FIRST_MATCH:
							c.target.SetActive(false);
							return;
					}
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
			if (c.largeScreen && !IsLargeTouchScreen ()) return false;
			if (c.modelStringRegex != null && c.modelStringRegex != "") 
			{
				string model = SystemInfo.deviceModel;
				Regex regex = new Regex(c.modelStringRegex);
				Match match = regex.Match(model);
				if (!match.Success) return false;
			}
			return true;
		}

	    /// <summary>
	    /// Returns true if the screen is large touch screen (i.e. a tablet). You often want to draw things like controls differently in this case.
	    /// </summary>
	    /// <returns><c>true</c> if is large touch screen; otherwise, <c>false</c>.</returns>
	    public static bool IsLargeTouchScreen()
	    {
			// TODO Add various checks here.
			return false;
		}
	}

	/// <summary>
	/// Different actions the enabler can perform.
	/// </summary>
	public enum MultiPlatformEnablerType
	{
		ENABLE_FIRST_MATCH,
		ENABLE_ALL_MATCHES,
		DISABLE_FIRST_MATCH,
		DISABLE_ALL_MATCHES
	}


}
