using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Base component which supports showing a property of a player with a given id.
	/// </summary>
	public abstract class UIValueProvider : PlatformerProMonoBehaviour
	{

		/// <summary>
		/// The player to listen for.
		/// </summary>
		public int playerId;

		/// <summary>
		/// Cached health reference.
		/// </summary>
		protected CharacterHealth characterHealth;

		/// <summary>
		/// List of all the renderers that draw something.
		/// </summary>
		protected List<IValueRenderer> renderers;

		/// <summary>
		/// Unity Start() hook.
		/// </summary>
		void Start()
		{
			PostInit ();
		}

		/// <summary>
		/// Unity On Destory hook
		/// </summary>
		void OnDestroy()
		{
			if (PlatformerProGameManager.Instance != null) PlatformerProGameManager.Instance.CharacterLoaded -= HandleCharacterLoaded;
			if (characterHealth != null) 
			{
				characterHealth.Died -= HandleChange;
				characterHealth.Loaded -= HandleChange;
				characterHealth.GainLives -= HandleChange;
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void PostInit()
		{
			PlatformerProGameManager.Instance.CharacterLoaded += HandleCharacterLoaded;
			renderers = GetComponentsInChildren (typeof(IValueRenderer)).Cast<IValueRenderer>().ToList ();
		}

		/// <summary>
		/// Get character reference when character loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		virtual protected void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			if (playerId == PlatformerProGameManager.ANY_PLAYER || playerId == e.Character.PlayerId)
			{
				characterHealth = e.Character.CharacterHealth;
				if (characterHealth != null)
				{
					characterHealth.Died += HandleChange;
					characterHealth.Loaded += HandleChange;
					characterHealth.GainLives += HandleChange;
					UpdateComponent ();
				} else
				{
					Debug.LogWarning ("Loaded character didn't have a CharacterHealth");
				}
			}
		}

		/// <summary>
		/// Handles any change event by updating UI. Override to add filters.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		virtual protected void HandleChange (object sender, System.EventArgs e)
		{
			UpdateComponent ();
		}

		/// <summary>
		/// Updates the visible part of the component.
		/// </summary>
		protected void UpdateComponent()
		{
			for (int i = 0; i < renderers.Count; i++)
			{
				renderers[i].Render(this);
			}
		}


		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public abstract object RawValue
		{
			get;
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		virtual public int IntValue
		{
			get
			{
				int result = 0;
				if (RawValue.GetType().IsAssignableFrom(typeof(int)))
				{
					return (int)RawValue;
				}
				if (RawValue is string)
				{
					int.TryParse ((string)RawValue, out result);
				}
				return result;
			}
		}

		/// <summary>
		/// Gets the maximum int value. For example maximum number of lives.
		/// </summary>
		/// <value>The max int value.</value>
		virtual public int IntMaxValue
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Gets the value as percentage between 0 (0%) and 1 (100%).
		/// </summary>
		/// <value>The value.</value>
		virtual public float PercentageValue
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Gets the value as a float.
		/// </summary>
		/// <value>The value.</value>
		virtual public float FloatValue
		{
			get
			{
				float result = 0;
				if (RawValue.GetType().IsAssignableFrom(typeof(float)))
				{
					return (float)RawValue;
				}
				if (RawValue is string)
				{
					float.TryParse ((string)RawValue, out result);
				}
				return result;
			}
		}

		/// <summary>
		/// Gets the value as a string. We provide a simple implementation to 
		/// make it easier to implements this class.
		/// </summary>
		/// <value>The value.</value>
		virtual public string StringValue
		{
			get
			{
				return RawValue.ToString ();
			}
		}


	}

}