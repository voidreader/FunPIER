using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{

	/// <summary>
	/// A character in a top down level select screen.
	/// </summary>
	public class UILevelSelectCharacter : MonoBehaviour
	{
		/// <summary>
		/// How fast does the character move (in game units per second).
		/// </summary>
		[Tooltip ("Speed at which the character moves (in game units per second).")]
		public float speed;

		/// <summary>
		/// Use jump button to enter level.
		/// </summary>
		[Tooltip ("Does pressing the JUMP button enter the level?")]
		public bool useJumpButton = true;

		/// <summary>
		/// The visible componenent of this character.
		/// </summary>
		[Tooltip ("The visible componenent of this character.")]
		public GameObject visibleComponenent;

		/// <summary>
		/// Leeway to allow when finding the path.
		/// </summary>
		const float leeway = 0.001f;

		/// <summary>
		/// Are we currently moving?
		/// </summary>
		protected bool isMoving;

		/// <summary>
		/// Tile we are currently standing on.
		/// </summary>
		protected UILevelSelectTile currentTile;

		/// <summary>
		/// Cached input reference.
		/// </summary>
		protected Input input;


		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			DoMove ();
		}


		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected  void Init()
		{
			input = (Input) GameObject.FindObjectOfType (typeof(Input));
			if (input == null) Debug.LogWarning("Couldn't find an input to control the UILevelSelectCharacter");
			if (LevelManager.PreviousLevel != null && LevelManager.PreviousLevel != "")
			{
				UILevelSelectTile[] tiles = FindObjectsOfType<UILevelSelectTile> ();
				foreach (UILevelSelectTile tile in tiles)
				{
					if (LevelManager.PreviousLevel == tile.levelLoadSceneName) transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, transform.position.z);
				}
			}
			if (visibleComponenent != null) visibleComponenent.SetActive (true);
		}

		virtual protected void DoMove()
		{
			Vector3 originalPosition = transform.position;
			if (useJumpButton && input.JumpButton == ButtonState.DOWN)
			{
				TryEnterLevel();
			}
			else if (input.HorizontalAxisDigital != 0)
			{
				transform.Translate(input.HorizontalAxisDigital * speed * Time.deltaTime, 0, 0, Space.World);
				if (!TileAllowsMove(input.HorizontalAxisDigital, 0)) 
				{
					transform.position = originalPosition;
				}
			}
			else if (input.VerticalAxisDigital != 0)
			{
				transform.Translate(0, input.VerticalAxisDigital * speed * Time.deltaTime, 0, Space.World);
				if (!TileAllowsMove(0, input.VerticalAxisDigital)) 
				{
					transform.position = originalPosition;
				}
			}
		}

		/// <summary>
		/// Checks for tile underneath the character.
		/// </summary>
		/// <returns>The level select tile if found, or the tile the character is moving on.</returns>
		virtual protected bool TileAllowsMove(int x, int y)
		{
			Collider2D[] hitColliders = Physics2D.OverlapCircleAll (transform.position, leeway);
			if (hitColliders.Length > 0) 
			{
				foreach (Collider2D hit in hitColliders)
				{
					UILevelSelectTile currentTile =	hit.GetComponent<UILevelSelectTile>();
					if (currentTile != null && currentTile.AllowsMove(x,y)) {
						// Snap to centre
						currentTile.Snap(this);
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Try to enter a nearby level.
		/// </summary>
		virtual protected void TryEnterLevel()
		{
			Collider2D[] hitColliders = Physics2D.OverlapCircleAll (transform.position, leeway);
			if (hitColliders.Length > 0) 
			{
				foreach (Collider2D hit in hitColliders)
				{
					UILevelSelectTile currentTile =	hit.GetComponent<UILevelSelectTile>();
					if (currentTile != null && currentTile.AllowsEnter()) {
						currentTile.EnterLevel();
					}
				}
			}
		}
	}
}