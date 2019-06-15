using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// FX class which flashes a sprite by changing its colour.
	/// </summary>
	public class FX_Flash : FX_Base
	{
		/// <summary>
		/// Target color.
		/// </summary>
		public Color invisibleColour;

		/// <summary>
		/// How long to flash for.
		/// </summary>
		public float totalTime;

		/// <summary>
		/// How long to stay in the invisble colour.
		/// </summary>
		public float invisibleTime;

		/// <summary>
		/// How long to stay in the invisble colour.
		/// </summary>
		public float visibleTime;

		/// <summary>
		/// Target to fade.
		/// </summary>
		public Component fadeTarget;

		protected Color originalColor;

		/// <summary>
		/// Do the effect.
		/// </summary>
		override protected void DoEffect()
		{
			StopAllCoroutines ();
			SetOriginalColor (fadeTarget);
			StartCoroutine (Flash());
		}

		/// <summary>
		/// Flash the target.
		/// </summary>
		virtual protected IEnumerator Flash()
		{
			float timer = 0;
			float stateTimer = 0;
			bool isInvisble = false;
			while (timer < totalTime)
			{
				timer += Time.deltaTime;
				stateTimer += Time.deltaTime;
				yield return true;
				if (isInvisble)
				{
					if (stateTimer > invisibleTime)
					{
						stateTimer = 0;
						SetColorForComponent(fadeTarget, originalColor);
						isInvisble = false;
					}
				}
				else
				{
					if (stateTimer > visibleTime)
					{
						stateTimer = 0;
						SetColorForComponent(fadeTarget, invisibleColour);
						isInvisble = true;
					}
				}
			}
			SetColorForComponent(fadeTarget, originalColor);
		}
			
		/// <summary>
		/// Sets the original color by inspecting the relevant component.
		/// </summary>
		/// <param name="component">Component.</param>
		virtual protected void SetOriginalColor(Component component)
		{
			if (component is MeshRenderer)
			{
				originalColor = ((MeshRenderer)component).material.color;
				return;
			}
			else if (component is SpriteRenderer)
			{
				originalColor =  ((SpriteRenderer)component).color;
				return;
			}
			else if (component is Graphic)
			{
				originalColor = ((Graphic)component).color;
			}
			else
			{
				Debug.LogWarning ("FX_Flash does not not know how to flash " + component);
			}
		}

		/// <summary>
		/// Sets the color for component.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="color">Color.</param>
		virtual protected void SetColorForComponent(Component component, Color32 color)
		{
			if (component is MeshRenderer)
			{
				((MeshRenderer)component).material.color = color;
				return;
			}
			else if (component is SpriteRenderer)
			{
				((SpriteRenderer)component).color = color;
				return;
			}
			else if (component is Graphic)
			{
				((Graphic)component).color = color;
			}
			else
			{
				Debug.LogWarning ("FX_Flash does not not know how to flash " + component);
			}
		}

	}
}