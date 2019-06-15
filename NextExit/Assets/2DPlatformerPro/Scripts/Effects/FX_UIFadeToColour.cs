using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Simple FX class which sfdes colour of a UI object.
	/// </summary>
	public class FX_UIFadeToColour : FX_Base
	{
		/// <summary>
		/// Target color.
		/// </summary>
		public Color fadeColour;

		/// <summary>
		/// How long to fade for.
		/// </summary>
		public float fadeTime;

		/// <summary>
		/// List of the objects that will be faded.
		/// </summary>
		[Tooltip ("List of the objects that will be faded. If blank all children of type Grpahic will be faded")]
		public Graphic[] targets;

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake()
		{
			if (playOnAwake && (targets == null || targets.Length < 1)) Debug.LogError("You can't play on awake with no targets set as the children may not be created at Awake() time.");
			else if (playOnAwake) StartEffect();
		}

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{

		}

		/// <summary>
		/// Do the effect.
		/// </summary>
		override protected void DoEffect()
		{
			Graphic[] myTargets = targets;
			if (targets == null || targets.Length < 1) myTargets = GetComponentsInChildren<Graphic> ();
			foreach (Graphic target in myTargets) target.CrossFadeColor (fadeColour, fadeTime, true, true);
		}
	}
}