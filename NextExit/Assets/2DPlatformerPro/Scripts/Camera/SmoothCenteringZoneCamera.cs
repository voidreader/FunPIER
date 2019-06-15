using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A camera which smoothly centers on the character and also allows movement between zones.
	/// </summary>
	public class SmoothCenteringZoneCamera : FixedZoneCamera
	{
		#region members

		/// <summary>
		/// Follow settings for xAxis;
		/// </summary>
		public CameraAxisSettings xAxis;

		/// <summary>
		/// Follow settings for yAxis;
		/// </summary>
		public CameraAxisSettings yAxis;

		/// <summary>
		/// Reference to the character being followed.
		/// </summary>
		[Tooltip ("Reference to the character being followed.")]
		public Character character;

		/// <summary>
		/// If true the character will be completely frozen during the transition.
		/// </summary>
		[Tooltip ("If true the character will be completely frozen during transition.")]
		public bool freezePlayerDuringTransition;

		/// <summary>
		/// If the character is teleported or respawned should be snap to that position or animate to it?
		/// </summary>
		[Tooltip ("If the character is teleported or respawned should be snap to that position or animate to it?")]
		public bool snapOnRespawn;

		/// <summary>
		/// Is camera moving in x.
		/// </summary>
		protected bool movingInX;

		/// <summary>
		/// Is camera moving in y.
		/// </summary>
		protected bool movingInY;
		
		/// <summary>
		/// Reference to the character loader.
		/// </summary>
		protected CharacterLoader characterLoader;

		/// <summary>
		/// How far did we move last frame in X
		/// </summary>
		protected float distanceMovedLastFrameX;

		/// <summary>
		/// How far did we move last frame in X
		/// </summary>
		protected float distanceMovedLastFrameY;

		/// <summary>
		/// The actual smoothing factor in X.
		/// </summary>
		protected float actualSmoothingFactorX;

		/// <summary>
		/// The actual smoothing factor in Y.
		/// </summary>
		protected float actualSmoothingFactorY;

		/// <summary>
		/// never allow frames to move more than this factor relative to previous frame.
		/// </summary>
		protected const float SmoothingFactor = 1.125f;

		#endregion

		#region constants

		/// <summary>
		/// If camera movementn is smaller than this it is considered still.
		/// </summary>
		protected const float MAX_SPEED_FOR_STILL = 0.4f;

		#endregion 

		/// <summary>
		/// Unity OnDestroy hook.
		/// </summary>
		void OnDestroy()
		{
			DoDestroy ();
		}

		/// <summary>
		/// Initialise this instance.
		/// </summary>
		override public void Init()
		{
			base.Init ();
			if (character != null) characterLoader = CharacterLoader.GetCharacterLoaderForCharacter(character);
			if (characterLoader == null) characterLoader = CharacterLoader.GetCharacterLoader();
			if (characterLoader != null)
			{
				characterLoader.CharacterLoaded += HandleCharacterLoaded;
			}
			else if (character == null)
			{
				Character[] characters = GameObject.FindObjectsOfType<Character> ();
				if (characters.Length == 1)
				{
					character = characters[0];
				}
				else if (characters.Length == 0)
				{
					Debug.LogError("No characters or character loaders found, Camera cannot follow character.");
				}
				else
				{
					Debug.LogError("Too many characters found, Camera does not know which character to follow.");
				}
			}
			if (character != null && snapOnRespawn)
			{
				character.Respawned += HandleRespawned;
			}

			actualSmoothingFactorX = 1.0f + (SmoothingFactor - 1.0f) * xAxis.acceleration;
			actualSmoothingFactorY = 1.0f + (SmoothingFactor - 1.0f) * yAxis.acceleration;
		}

		/// <summary>
		/// Called when the zones the been changed.
		/// </summary>
		override public void ZoneHasBeenChanged(Transform t, Vector3 p)
		{
			base.ZoneHasBeenChanged (t, p);
			if (freezePlayerDuringTransition && character != null) character.enabled = true;
		}

		/// <summary>
		/// Do the destroy actions.
		/// </summary>
		override protected void DoDestroy()
		{
			base.DoDestroy ();
			if (characterLoader != null)
			{
				characterLoader.CharacterLoaded -= HandleCharacterLoaded;

			}
			if (character != null && snapOnRespawn)
			{
				character.Respawned -= HandleRespawned;
			}
		}

		/// <summary>
		/// Handles the character being loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		virtual protected void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			character = e.Character;
			transform.Translate(character.transform.position.x - transform.position.x, character.transform.position.y - transform.position.y, 0);
		}

		/// <summary>
		/// Handles the respawned event
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Even details.</param>
		virtual protected void HandleRespawned (object sender, CharacterEventArgs e)
		{
			transform.Translate(character.transform.position.x - transform.position.x, character.transform.position.y - transform.position.y, 0);
		}

		/// <summary>
		/// Unity LateUpdate hook.
		/// </summary>
		void LateUpdate() 
		{
			if (enabled && !isInTransition) DoMove ();
			if (isInTransition)
			{

			}
		}

		/// <summary>
		/// Move the camera
		/// </summary>
		protected void DoMove() 
		{
			if (character != null)
			{
				if (xAxis.moveOnAxis) MoveInX ();
				if (yAxis.moveOnAxis) MoveInY ();
				if (currentZone != null) LimitToBounds ();
			}
		}

		/// <summary>
		/// Moves the camera in x.
		/// </summary>
		protected void MoveInX() 
		{
			if (xAxis.hardFollow)
			{
				transform.Translate(character.transform.position.x - transform.position.x, 0, 0);
			}
			else
			{
				if (movingInX)
				{
					float xDiff = character.transform.position.x - transform.position.x;
					if (Mathf.Abs (xDiff) < (xAxis.minOffsetForMove * 0.5f))
					{
						movingInX = false;
						distanceMovedLastFrameX = 0.0f;
					} 
					else
					{
						// Alternative way to do this but it doesn't work so well.
//						float newPosition = Mathf.SmoothDamp(transform.position.x, character.transform.position.x, ref xVel, 0.75f);
//						transform.position = new Vector3(newPosition, transform.position.y, transform.position.z);
//					}
						float dist = xDiff * TimeManager.FrameTime * xAxis.acceleration;
						if (dist < 0 && dist < xDiff) dist = xDiff;
						else if (dist > 0 && dist > xDiff) dist = xDiff;
						// Try to smooth out frame time jumps (mainly for web player)
						if (distanceMovedLastFrameX < 0 && dist < (distanceMovedLastFrameX * actualSmoothingFactorX)) dist = distanceMovedLastFrameX * actualSmoothingFactorX;
						else if (distanceMovedLastFrameX > 0 && dist > (distanceMovedLastFrameX * actualSmoothingFactorX)) dist = distanceMovedLastFrameX * actualSmoothingFactorX;
						else if (distanceMovedLastFrameX < -0.05f && dist > (distanceMovedLastFrameX / actualSmoothingFactorX)) dist = distanceMovedLastFrameX / actualSmoothingFactorX;
						else if (distanceMovedLastFrameX > 0.05f && dist < (distanceMovedLastFrameX / actualSmoothingFactorX)) dist = distanceMovedLastFrameX / actualSmoothingFactorX;
						transform.Translate(dist, 0, 0);
						distanceMovedLastFrameX = dist;
					}
				}
				else if (Mathf.Abs (character.transform.position.x - transform.position.x) > xAxis.minOffsetForMove)
				{
					movingInX = true;
				}
			}
		}

		/// <summary>
		/// Moves the camera in y.
		/// </summary>
		protected void MoveInY() 
		{
			if (yAxis.hardFollow)
			{
				transform.Translate(0, character.transform.position.y - transform.position.y, 0);
			}
			else
			{
				if (movingInY)
				{
					float yDiff = character.transform.position.y - transform.position.y;
					if (Mathf.Abs (yDiff) < (yAxis.minOffsetForMove * 0.5f))
					{
						movingInY = false;
						distanceMovedLastFrameY = 0.0f;
					} 
					else
					{
						float dist = yDiff * TimeManager.FrameTime * yAxis.acceleration;
						if (dist < 0 && dist < yDiff) dist = yDiff;
						else if (dist > 0 && dist > yDiff) dist = yDiff;
						// Try to smooth out frame time jumps (mainly for web player)
						if (distanceMovedLastFrameY < 0 && dist < (distanceMovedLastFrameY * actualSmoothingFactorY)) dist = distanceMovedLastFrameY * actualSmoothingFactorY;
						else if (distanceMovedLastFrameY > 0 && dist > (distanceMovedLastFrameY * actualSmoothingFactorY)) dist = distanceMovedLastFrameY * actualSmoothingFactorY;
						else if (distanceMovedLastFrameY < -0.05f && dist > (distanceMovedLastFrameY / actualSmoothingFactorY)) dist = distanceMovedLastFrameY / actualSmoothingFactorY;
						else if (distanceMovedLastFrameY > 0.05f && dist < (distanceMovedLastFrameY / actualSmoothingFactorY)) dist = distanceMovedLastFrameY / actualSmoothingFactorY;
						transform.Translate(0, dist, 0);
						distanceMovedLastFrameY = dist;
					}
				}
				else if (Mathf.Abs (character.transform.position.y - transform.position.y) > yAxis.minOffsetForMove)
				{
					movingInY = true;
				}
			}
		}

		/// <summary>
		/// Limits camera position to the current zones bounds.
		/// </summary>
		protected void LimitToBounds()
		{
			if (currentZone != null)
			{
				// X Bounds
				if (transform.position.x > currentZone.Max(myCamera).x)
				{
					transform.Translate (currentZone.Max(myCamera).x - transform.position.x, 0, 0);
					movingInX = false;
				}
				else if (transform.position.x < currentZone.Min(myCamera).x)
				{
					transform.Translate (currentZone.Min(myCamera).x - transform.position.x, 0, 0);
					movingInX = false;
				}
				// Y Bounds
				if (transform.position.y > currentZone.Max(myCamera).y)
				{
					transform.Translate (0, currentZone.Max(myCamera).y - transform.position.y, 0);
					movingInY = false;
				}
				else if (transform.position.y < currentZone.Min(myCamera).y)
				{
					transform.Translate (0, currentZone.Min(myCamera).y - transform.position.y, 0);
					movingInY = false;
				}
			}
		}

		/// <summary>
		/// Changes the zone by immediately snapping to the new zones position. 
		/// Overridden so we can also clear any movement items.
		/// </summary>
		/// <param name="newZone">New zone.</param>
		override public void ChangeZone(CameraZone newZone)
		{
			base.ChangeZone (newZone);
			movingInX = false;
			movingInY = false;
			distanceMovedLastFrameX = 0.0f;
			distanceMovedLastFrameY = 0.0f;
			if (freezePlayerDuringTransition && character != null) character.enabled = false;
		}
	}

	/// <summary>
	/// Settings defining how a camera moves along an axis.
	/// </summary>
	[System.Serializable]
	public class CameraAxisSettings 
	{
		/// <summary>
		/// Does the camera move along this axis.
		/// </summary>
		public bool moveOnAxis = true;

		/// <summary>
		/// Does the camera transform exactly follow the character transform on this axis.
		/// </summary>
		public bool hardFollow;

		/// <summary>
		/// How much does the character need to move before the camera starts to move.
		/// Ignored if hardFollow is true;
		/// </summary>
		public float minOffsetForMove;

		/// <summary>
		/// The acceleration applied to the camera until it catches up to characer.
		/// Ignored if hardFollow is true;
		/// </summary>
		public float acceleration;

	}
}
