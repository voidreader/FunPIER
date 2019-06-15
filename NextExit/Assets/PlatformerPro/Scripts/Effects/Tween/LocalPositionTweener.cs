using UnityEngine;
using System;
using System.Collections;

namespace PlatformerPro.Tween
{
	/// <summary>
	/// A tweener that tweens position.
	/// </summary>
	public class LocalPositionTweener : PositionTweener
	{

		/// <summary>
		/// Tweens target transform to destination position over time.
		/// </summary>
		/// <param name="mode">how to tween.</param>
		/// <param name="target">Target object.</param>
		/// <param name="destination">Final position.</param>
		/// <param name="time">Time to take to do the tween.</param>
		/// <param name="callback">Called when tween completes.</param>
		/// <returns>The isntance doing the tween.</returns>
		override public void TweenWithTime (TweenMode tweenMode, Transform target, Vector3 destination, float time, Action<Transform, Vector3> callback)
		{
			if (Active) Stop ();
			this.time = time;
			this.target = target;
			this.destination = destination;
			this.callback = callback;

			/// Not actually tweening
			if (tweenMode == TweenMode.SNAP)
			{
				target.localPosition = destination;
				if (callback != null) callback(target, destination);
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
				direction = (destination - target.localPosition).normalized;
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
		override public void TweenWithRate (TweenMode tweenMode, Transform target, Vector3 destination, float speed, Action<Transform, Vector3> callback)
		{
			if (Active)Stop ();
			this.target = target;
			this.destination = destination;
			this.callback = callback;
			this.speed = speed;

			/// Not actually tweening
			if (tweenMode == TweenMode.SNAP)
			{
				transform.localPosition = destination;
				if (callback != null) callback(target, destination);
			}
			// Tween by fixed speed
			else
			{
				switch(tweenMode)
				{
					case TweenMode.LINEAR: easingFunction = Linear; break;
					case TweenMode.EASE_IN_OUT: easingFunction = BezierBlend; break;
					default: Debug.Log ("No tween function available for mode: " + tweenMode); break;
				}

				// How long do we have
				float distance = Vector3.Distance (target.localPosition, destination);
				time = distance / speed;
				direction = (destination - target.localPosition).normalized;
				StartCoroutine(DoEasedTween());
			}
		}

		/// <summary>
		/// Does the tween using an easing function.
		/// </summary>
		/// <returns>The eased tween.</returns>
		override protected IEnumerator DoEasedTween()
		{
			Active = true;
			float elapsedTime = 0;
			Vector3 originalPosition = target.localPosition;

			while (elapsedTime < time)
			{
				target.localPosition = Vector3.Lerp(originalPosition, destination, easingFunction(elapsedTime / time));
				elapsedTime += UseGameTime ? TimeManager.FrameTime: Time.deltaTime;
				yield return true;
			}

			target.localPosition = destination;
			Active = false;
			if (callback != null) callback(target, destination);
		}

	}
}
