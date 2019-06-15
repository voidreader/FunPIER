using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Multi platform enabler condition.
	/// </summary>
	[System.Serializable]
	public class MultiPlatformEnablerCondition
	{

		/// <summary>
		/// Force a match (editor only).
		/// </summary>
		[Tooltip ("Always match this element. Used for testing in the editor, ignored outside of the editor.")]
		public bool forceMatch;

		/// <summary>
		/// Should we check device type.
		/// </summary>
		[TooltipAttribute (" Should we check device type.")]
		public bool useDeviceType;
		
		/// <summary>
		/// High level device type.
		/// </summary>
		[TooltipAttribute ("High level device type.")]
		public DeviceType deviceType;
		
		/// <summary>
		/// Should we check platform
		/// </summary>
		[TooltipAttribute ("Should we check platform")]
		public bool usePlatform;
		
		/// <summary>
		/// Platform this applies to.
		/// </summary>
		[TooltipAttribute ("Platform this applies to.")]
		public RuntimePlatform platform;
		
		/// <summary>
		/// Regex which will be macthed against the model string.
		/// </summary>
		[TooltipAttribute ("Regex which will be macthed against the model string.")]
		public string modelStringRegex;
		
		/// <summary>
		///  Only apply if screen is large.
		/// </summary>
		[TooltipAttribute ("Only apply if screen is large like a tablet")]
		public bool largeScreen;
		
		/// <summary>
		/// The GO to enable or disable.
		/// </summary>
		[TooltipAttribute ("The GO to enable or disable.")]
		public GameObject target;
	}
}