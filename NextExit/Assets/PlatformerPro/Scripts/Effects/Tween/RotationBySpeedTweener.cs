using UnityEngine;
using System;
using System.Collections;

namespace PlatformerPro.Tween
{
	/// <summary>
	/// A tweener that tweens rotation. Not intended for use to get to a certain rotation but instead to spin for a certain amount of time.
	/// </summary>
	public class RotationBySpeedTweener : MonoBehaviour, ITweener <Transform, Vector3>
	{

		protected Vector3 destination;
		protected float speed;
		protected float time;
		protected Transform target;
		protected Vector3 rotationRates;
		protected Action<Transform, Vector3> callback;
		protected Func<float, float> easingFunction;

		/// <summary>
		/// Tweens target transform to destination position over time.
		/// </summary>
		/// <param name="mode">how to tween.</param>
		/// <param name="target">Target object.</param>
		/// <param name="rotationRates">Rate of rotation in x,y,z.</param>
		/// <param name="time">Time to take to do the tween.</param>
		/// <param name="callback">Called when tween completes.</param>
		/// <returns>The isntance doing the tween.</returns>
		public void TweenWithTime (TweenMode tweenMode, Transform target, Vector3 rotationRates, float time, Action<Transform, Vector3> callback)
		{
			if (Active)
			{
				Debug.LogWarning("You should stop the active tween before starting a new one");
				Stop ();
			}
			this.time = time;
			this.target = target;
			this.rotationRates = rotationRates;
			this.callback = callback;

			/// Not actually tweening
			if (tweenMode == TweenMode.SNAP)
			{
				Debug.LogWarning("RotationBySpeedTweener doesn't support TweenMode.SNAP");
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
		public void TweenWithRate (TweenMode tweenMode, Transform target, Vector3 destination, float speed, Action<Transform, Vector3> callback)
		{
			Debug.LogWarning ("You can't tween with rate on the RotationBySpeedTweener. Use tween with time and set the rates in the rotationRates field.");
		}

		/// <summary>
		/// Does the tween using an easing function.
		/// </summary>
		/// <returns>The eased tween.</returns>
		protected IEnumerator DoEasedTween()
		{
			Active = true;
			float elapsedTime = 0;

			while (elapsedTime < time)
			{
				if (elapsedTime != 0)
				{
					float amount = easingFunction(elapsedTime / time) / (elapsedTime / time);
					Vector3 rates = amount * rotationRates * (UseGameTime ? TimeManager.FrameTime: Time.deltaTime);
					target.Rotate(rates, Space.World);
				}
				elapsedTime += UseGameTime ? TimeManager.FrameTime: Time.deltaTime;
				yield return true;
			}

			Active = false;
			if (callback != null) callback(target, destination);
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
			get; set;
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
