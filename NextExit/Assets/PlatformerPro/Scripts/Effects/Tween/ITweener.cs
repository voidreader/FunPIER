using UnityEngine;
using System;
using System.Collections;

namespace PlatformerPro.Tween
{
	/// <summary>
	/// Interface for things that can tween.
	/// </summary>
	public interface ITweener <T, P>
	{
		/// <summary>
		/// Tweens target to destination over time.
		/// </summary>
		/// <param name="mode">how to tween.</param>
		/// <param name="target">Target object.</param>
		/// <param name="destination">Destination position, final value, etc.</param>
		/// <param name="time">Time to take to do the tween.</param>
		/// <param name="callback">Called when tween completes.</param>
		void TweenWithTime(TweenMode mode, T target, P destination, float time, Action<T,P> callback);

		/// <summary>
		/// Tweens target to destination at rate.
		/// </summary>
		/// <param name="mode">how to tween.</param>
		/// <param name="target">Target object.</param>
		/// <param name="destination">Destination position, final value, etc.</param>
		/// <param name="rate">How fast to do the tween.</param>
		/// <param name="callback">Called when tween completes.</param>
		void TweenWithRate(TweenMode mode, T target, P destination, float rate, Action<T,P> callback);

		/// <summary>
		/// Is tweener currently tweening.
		/// </summary>
		bool Active
		{
			get;
		}

		/// <summary>
		/// If true use TimeManager.FrameTime false use Unitys Time.deltaTime
		/// </summary>
		/// <value><c>true</c> if use game time; otherwise, <c>false</c>.</value>
		bool UseGameTime
		{
			get; set;
		}

		/// <summary>
		/// Stops the active tween.
		/// </summary>
		void Stop ();
	}
}
