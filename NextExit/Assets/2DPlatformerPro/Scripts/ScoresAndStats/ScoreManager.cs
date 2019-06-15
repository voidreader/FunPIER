using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Very simple class for managing a basic score or statistic.
	/// </summary>
	public class ScoreManager 
	{
		/// <summary>
		/// Uniqifier for the save player prefs key.
		/// </summary>
		protected const string PP_SCORE_ID = "PP_SCOREMANAGER";

		/// <summary>
		/// The type of the score.
		/// </summary>
		public string scoreType;

		/// <summary>
		/// Should we save the score?
		/// </summary>
		public bool saveScoreToPreferences;

		/// <summary>
		/// Stores the current score.
		/// </summary>
		protected int currentScore;

		/// <summary>
		/// Cached copy of the score event args.
		/// </summary>
		protected ScoreEventArgs scoreEventArgs;

		#region events

		/// <summary>
		/// Occurs when score changes.
		/// </summary>
		public event System.EventHandler <ScoreEventArgs> ScoreChanged;

		/// <summary>
		/// Raises the score changed event.
		/// </summary>
		/// <param name="change">Change.</param>
		protected void OnScoreChanged(int change)
		{
			if (ScoreChanged != null)
			{
				scoreEventArgs.UpdateScore(change, currentScore);
				ScoreChanged(this, scoreEventArgs);
			}
		}

		#endregion

		/// <summary>
		/// Gets the score.
		/// </summary>
		/// <value>The score.</value>
		public int Score
		{
			get
			{
				return currentScore;
			}
		}

		/// <summary>
		/// Init the instance.
		/// </summary>
		/// <param name="scoreType">Score type.</param>
		protected void Init(string scoreType)
		{
			this.scoreType = scoreType;
			scoreEventArgs = new ScoreEventArgs (scoreType);
			if (saveScoreToPreferences) Load ();
		}

		/// <summary>
		/// Adds to the score. Use a negative number to subtract.
		/// </summary>
		/// <param name="amount">Amount.</param>
		public void AddScore(int amount)
		{
			currentScore += amount;
			if (saveScoreToPreferences) Save ();
			OnScoreChanged (amount);
		}

		/// <summary>
		/// Resets the score.
		/// </summary>
		public void ResetScore()
		{
			int tmpCurrentScore = currentScore;
			currentScore = 0;
			if (saveScoreToPreferences) Save ();
			OnScoreChanged (-tmpCurrentScore);
		}

		/// <summary>
		/// Save the score to player prefs, override if you want to save elsewhere.
		/// </summary>
		virtual protected void Save()
		{
			PlayerPrefs.SetInt (PP_SCORE_ID + scoreType, currentScore);
		}

		/// <summary>
		/// Load the score from player prefs, override if you want to load from elsewhere.
		/// </summary>
		virtual protected void Load()
		{
			currentScore = PlayerPrefs.GetInt (PP_SCORE_ID + scoreType, 0);
		}

		#region static methods and members
		
		/// <summary>
		/// The score managers.
		/// </summary>
		protected static List<ScoreManager> instances;

		/// <summary>
		/// The score manager types corresponds to the order of score managers.
		/// </summary>
		protected static List<string> instanceTypes;

		/// <summary>
		/// Get the instance
		/// </summary>
		public static ScoreManager GetInstanceForType(string scoreType)
		{

			if (instances == null)
			{
				instances = new List<ScoreManager>();
				instanceTypes = new List<string>();
			}
			if (instanceTypes.Contains(scoreType))
			{
				return instances[instanceTypes.IndexOf(scoreType)];
			}
			else
			{
				ScoreManager newScoreManager = CreateNewScoreManager(scoreType);
				instanceTypes.Add (scoreType);
				instances.Add (newScoreManager);
				return newScoreManager;
			}
		}

		/// <summary>
		/// Call this to change persistence for the given score manager. By default scores are not saved
		/// so will be reset when player exits the game.
		/// </summary>
		/// <param name="scoreType">Score type.</param>
		/// <param name="persist">If set to <c>true</c> then persist, otherwise don't.</param>
		public static void PersistScores(string scoreType, bool persist)
		{
			ScoreManager scoreManager = GetInstanceForType (scoreType);
			scoreManager.saveScoreToPreferences = persist;
		}

		/// <summary>
		/// Creates the score manager.
		/// </summary>
		protected static ScoreManager CreateNewScoreManager(string scoreType)
		{
			ScoreManager result;
			result = new ScoreManager ();
			result.Init (scoreType);
			return result;
		}

		#endregion
	
	}
}