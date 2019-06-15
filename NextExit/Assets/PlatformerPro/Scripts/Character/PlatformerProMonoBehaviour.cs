using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlatformerPro
{
	/// <summary>
	/// Extends MonoBehaviour to primarily provide a better editor experience.
	/// </summary>
	public abstract class PlatformerProMonoBehaviour : MonoBehaviour 
	{
		
		/// <summary>
		/// Gets the header string used to describe the component.
		/// </summary>
		/// <value>The header.</value>
		virtual public string Header
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Gets a link to a youtube video.
		/// </summary>
		/// <value>The header.</value>
		virtual public string DocLink
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Gets a link to a youtube video.
		/// </summary>
		/// <value>The header.</value>
		virtual public string VideoLink
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// If non-null this component is deprecated. The string shows a message
		/// indicating how it should be replaced.
		/// </summary>
		virtual public string Deprecated
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Override this method if you want to provide custom validation. The actual code should be sorrounded by
		/// the if UNITY_EDITOR / endif directive.
		/// </summary>
		virtual public void Validate(PlatformerProMonoBehaviour myTarget)
		{
#if UNITY_EDITOR
			hasShownValidationHeader = false;
#endif
		}
		
#if UNITY_EDITOR
		private bool hasShownValidationHeader = false;
		/// <summary>
		/// Shows a validation header if not shown
		/// </summary>
		virtual protected void ShowValidationHeader()
		{
			if (!hasShownValidationHeader)
			{
				EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);
				hasShownValidationHeader = true;
			}
		}
#endif
	}
}