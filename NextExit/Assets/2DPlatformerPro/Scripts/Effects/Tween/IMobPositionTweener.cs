using UnityEngine;
using System;
using System.Collections;

namespace PlatformerPro.Tween
{
	/// <summary>
	/// A tweener that tweens position using an IMobs translate method, it also sets velocity.
	/// </summary>
	public class IMobPositionTweener : MonoBehaviour, ITweener <IMob, Vector3>
	{

		protected Vector3 destination;
		protected float speed;
		protected IMob target;
		protected Action<IMob, Vector3> callback;
		protected Func<float, float> easingFunction;

		/// <summary>
		/// Tweens target transform to destination position over time.
		/// </summary>
		/// <param name="mode">how to tween.</param>
		/// <param name="target">Target imob.</param>
		/// <param name="destination">Final position.</param>
		/// <param name="time">Time to take to do the tween.</param>
		/// <param name="callback">Called when tween completes.</param>
		/// <returns>The isntance doing the tween.</returns>
		public void TweenWithTime (TweenMode tweenMode, IMob target, Vector3 destination, float time, Action<IMob, Vector3> callback)
		{
			// Early exits
			if (!(target is Component))  Debug.LogWarning("IMobPositionTweener doesn't now how to tween a target that isn't a Component");

			if (Active) Stop ();
			this.target = target;
			this.destination = destination;
			this.callback = callback;

			/// Not actually tweening
			if (tweenMode == TweenMode.SNAP)
			{
				((Component)target).transform.position = destination;
				if (callback != null) callback(target, destination);
			}
			// Tween by fixed speed
			else 
			{
				float distance = Vector3.Distance (((Component)target).transform.position, destination);
				speed = distance/ time;
				// Use an easing function
				switch(tweenMode)
				{
					case TweenMode.LINEAR:break;
					default: Debug.Log ("No tween function available for mode: " + tweenMode); break;
				}
				StartCoroutine(DoTween());
			}
		}

		/// <summary>
		/// Tweens target transform to destination position with given rate (speed).
		/// </summary>
		/// <param name="mode">How to tween.</param>
		/// <param name="target">Target imob.</param>
		/// <param name="finalPosition">Final position.</param>
		/// <param name="speed">How fast to move.</param>
		/// <param name="callback">Callback.</param>
		/// <returns>The isntance doing the tween.</returns>
		public void TweenWithRate (TweenMode tweenMode, IMob target, Vector3 destination, float speed, Action<IMob, Vector3> callback)
		{
			// Early exits
			if (speed == 0) Debug.LogWarning("Can't tween with speed of zero");
			if (!(target is Component)) Debug.LogWarning("IMobPositionTweener doesn't now how to tween a target that isn't a Component");


			if (Active)Stop ();
			this.target = target;
			this.destination = destination;
			this.callback = callback;
			this.speed = speed;

			if (speed == 0) Debug.LogWarning("Can't tween with speed of zero");

			/// Not actually tweening
			if (tweenMode == TweenMode.SNAP)
			{
				((Component)target).transform.position = destination;
				if (callback != null) callback(target, destination);
			}
			// Tween by fixed speed
			else
			{
				switch(tweenMode)
				{
					case TweenMode.LINEAR:break;
					default: Debug.Log ("No tween function available for mode: " + tweenMode); break;
				}

				StartCoroutine(DoTween());
			}
		}

		/// <summary>
		/// Does the tween using an easing function.
		/// </summary>
		/// <returns>The eased tween.</returns>
		protected IEnumerator DoTween()
		{
			Active = true;
			Vector3 currentPosition;
			Vector3 delta = Vector3.zero;
			float frameMagnitude = 0;

			currentPosition = ((Component)target).transform.position;
			delta = currentPosition - destination;

			while (delta.sqrMagnitude > frameMagnitude)
			{
				currentPosition = ((Component)target).transform.position;
				delta = currentPosition - destination;
				Vector3 dir = delta.normalized;
				float frameTime = UseGameTime ? TimeManager.FrameTime : Time.deltaTime;
				frameMagnitude = ((frameTime * speed ) * (frameTime * speed));
				target.SetVelocityX(dir.x * speed);
				target.SetVelocityY(dir.y * speed);
				target.Translate(-dir.x * frameTime * speed, -dir.y * frameTime * speed, true);
				yield return true;
			}
			((Component)target).transform.position = destination;
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
	}
}
