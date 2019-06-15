using UnityEngine;

namespace PlatformerPro
{
	/// <summary>
	/// Attribute to indicate that a field should be hidden when another field has a specific boolean value.
	/// </summary>
	public class DontShowWhenAttribute : PropertyAttribute
	{
		/// <summary>
		/// Name of the other property.
		/// </summary>
		public string otherProperty;

		/// <summary>
		/// If true this property will be shown when other proprty is true.
		/// </summary>
		public bool showWhenTrue;

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.DontShowWhenAttribute"/> class.
		/// </summary>
		/// <param name="otherProperty">Other property.</param>
		public DontShowWhenAttribute(string otherProperty) 
		{
			this.otherProperty = otherProperty;
			showWhenTrue = false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.DontShowWhenAttribute"/> class.
		/// </summary>
		/// <param name="otherProperty">Other property.</param>
		/// <param name="showWhenTrue">If set to <c>true</c> show when true.</param>
		public DontShowWhenAttribute(string otherProperty, bool showWhenTrue) 
		{
			this.otherProperty = otherProperty;
			this.showWhenTrue = showWhenTrue;
		}
	}
}
