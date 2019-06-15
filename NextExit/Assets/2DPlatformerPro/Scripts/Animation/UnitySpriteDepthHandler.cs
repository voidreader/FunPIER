using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	/// <summary>
	/// Changes sprite rendering order based on character depth. Usually used for complex platforms that can go in
	/// front of and behind eah other (i.e. loops).
	/// </summary>
	public class UnitySpriteDepthHandler : MonoBehaviour
	{
		public int multipler;
		
		/// <summary>
		/// The character reference.
		/// </summary>
		protected IMob animatable;

		/// <summary>
		/// Cached reference to the renderer.
		/// </summary>
		protected SpriteRenderer spriteRenderer;

		/// <summary>
		/// Stores the offset from default layer.
		/// </summary>
		protected int offset;

		/// <summary>
		/// The current Z layer.
		/// </summary>
		protected int currentZLayer;

		/// <summary>UnitySpriteDepthHandler
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			// This is not elegant but its a simple and effective way to handle interfaces in Unity
			animatable = (IMob)gameObject.GetComponentInParent (typeof(IMob));
			if (animatable == null) Debug.LogError ("Depth Handler facer can't find the animatable reference");
			spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
			if (spriteRenderer == null) Debug.LogError("The UnitySpriteDepthHandler should be placed on the same game object as the SpriteRenderer");
			offset = spriteRenderer.sortingOrder - (animatable.ZLayer * multipler);
		}

		/// <summary>
		/// Unity update hook, face correct direction.
		/// </summary>
		void Update ()
		{
			if (animatable.ZLayer != currentZLayer)
			{
				spriteRenderer.sortingOrder = (animatable.ZLayer * multipler) + offset;
				currentZLayer = animatable.ZLayer;
			}
			
		}

	}

}