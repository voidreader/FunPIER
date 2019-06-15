using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace PlatformerPro.Tween
{
	/// <summary>
	/// A tweener that tweens position.
	/// </summary>
	public class ColorTweener : MonoBehaviour, ITweener <Component, Color32>
	{

		[SerializeField]
		protected bool useGameTime;

		protected Color32 targetColor;
		protected float speed;
		protected float time;
		protected Component target;
		protected Action<Component, Color32> callback;
		protected Func<float, float> easingFunction;

		/// <summary>
		/// Tweens target transform to destination position over time.
		/// </summary>
		/// <param name="mode">how to tween.</param>
		/// <param name="target">Target object.</param>
		/// <param name="targetColor">Target color.</param>
		/// <param name="time">Time to take to do the tween.</param>
		/// <param name="callback">Called when tween completes.</param>
		/// <returns>The isntance doing the tween.</returns>
		public void TweenWithTime (TweenMode tweenMode, Component target, Color32 targetColor, float time, Action<Component, Color32> callback)
		{
			if (Active)
			{
//				Debug.LogWarning("You should stop the active tween before starting a new one");
				Stop ();
			}
			this.time = time;
			this.target = target;
			this.callback = callback;
			this.targetColor = targetColor;

			/// Not actually tweening
			if (tweenMode == TweenMode.SNAP)
			{
				SetColorForComponent(target, targetColor);
				if (callback != null) callback(target, targetColor);
			}
			// Tween by fixed speed
			else 
			{
				// Use an easing function
				switch(tweenMode)
				{
					case TweenMode.LINEAR: easingFunction = Linear; break;
					case TweenMode.EASE_IN_OUT: easingFunction = BezierBlend; break;
					default: Debug.Log ("No tween function available for mode: " + tweenMode); break;
				}
				StartCoroutine(DoEasedTween());
			}
		}

		/// <summary>
		/// Tweens target transform to destination position with given rate (speed).
		/// </summary>
		/// <param name="mode">How to tween.</param>
		/// <param name="target">Target object.</param>
		/// <param name="finalPosition">Final position.</param>
		/// <param name="speed">How fast to move.</param>
		/// <param name="callback">Callback.</param>
		/// <returns>The isntance doing the tween.</returns>
		public void TweenWithRate (TweenMode tweenMode, Component target, Color32 targetColor, float speed, Action<Component, Color32> callback)
		{
			Debug.LogWarning ("ColorTweener does not support tween by rate");
		}

		/// <summary>
		/// Returns true if the given component can be colored.
		/// </summary>
		/// <returns><c>true</c> if this instance can color component the specified component; otherwise, <c>false</c>.</returns>
		/// <param name="component">Component.</param>
		virtual protected bool CanColorComponent(Component component)
		{
			if (component is MeshRenderer) return true;
			if (component is SpriteRenderer) return true;
			if (component is Graphic) return true;
			return false;
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
			if (component is SpriteRenderer)
			{
				((SpriteRenderer)component).color = color;
				return;
			}
			if (component is Graphic)
			{
				((Graphic)component).color = color;
			}
		}

		/// <summary>
		/// Gets the color for component.
		/// </summary>
		/// <returns>The color for component. Undetermined if we cannot get color for this component.</returns>
		virtual protected Color32 GetColorForComponent(Component component)
		{
			if (component is MeshRenderer)
			{
				return ((MeshRenderer)component).material.color;
			}
			if (component is SpriteRenderer)
			{
				return ((SpriteRenderer)component).color;
			}
			if (component is Graphic)
			{
				return ((Graphic)component).color;
			}
			return new Color32();
		}

		/// <summary>
		/// Does the tween using an easing function.
		/// </summary>
		/// <returns>The eased tween.</returns>
		virtual protected IEnumerator DoEasedTween()
		{
			Active = true;
			float elapsedTime = 0;
			Color32 originalColor = GetColorForComponent (target);

			while (elapsedTime < time)
			{

				byte r = (byte) Mathf.Lerp(originalColor.r, targetColor.r, easingFunction(elapsedTime / time));
				byte g = (byte) Mathf.Lerp(originalColor.g, targetColor.g, easingFunction(elapsedTime / time));
				byte b = (byte) Mathf.Lerp(originalColor.b, targetColor.b, easingFunction(elapsedTime / time));
				byte a = (byte) Mathf.Lerp(originalColor.a, targetColor.a, easingFunction(elapsedTime / time));
				Color32 frameColor = new Color32(r,g,b,a);
				SetColorForComponent(target, frameColor);
				elapsedTime += UseGameTime ? TimeManager.FrameTime: Time.deltaTime;
				yield return true;
			}

			SetColorForComponent (target, targetColor);
			Active = false;
			if (callback != null) callback(target, targetColor);
		}

		/// <summary>
		/// Is tweener currently tweening.
		/// </summary>
		/// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
		public bool Active
		{
			get; protected set;
		}

		/// <summary>
		/// Stops the active tween.
		/// </summary>
		public void Stop()
		{
			StopAllCoroutines ();
			Active = false;
		}

		/// <summary>
		/// If true use TimeManager.FrameTime false use Unitys Time.deltaTime
		/// </summary>
		/// <value><c>true</c> if use game time; otherwise, <c>false</c>.</value>
		public bool UseGameTime
		{
			get
			{
				return useGameTime;
			}
			set 
			{
				useGameTime = value;
			}
		}

		// TODO Move these to shared utility class

		protected float BezierBlend(float t)
		{
			return t * t * (3.0f - 2.0f * t);
		}

		protected float Linear(float t)
		{
			return t;
		}
	}
}
