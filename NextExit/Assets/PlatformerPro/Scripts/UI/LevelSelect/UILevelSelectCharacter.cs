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
        /// If true character automatically moves to the end of collider.
        /// </summary>
        [Tooltip (" If true character automatically moves to the end of collider.")]
        public bool autoMove = true;

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
		const float leeway = 0.005f;

		/// <summary>
		/// Are we currently moving?
		/// </summary>
		protected bool isMoving;

		/// <summary>
		/// Cached input reference.
		/// </summary>
		protected Input input;

        /// <summary>
        /// Tile we are moving on.
        /// </summary>
        protected UILevelSelectTile autoMoveTile;

        /// <summary>
        /// Position we are moving towards.
        /// </summary>
        protected Vector2 targetPosition;

        /// <summary>
        /// Track directionw e are moving in. -1,1 = Left right. -10,10 = down/up
        /// </summary>
        protected int currentDirection;

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
            if (useJumpButton && input.JumpButton == ButtonState.DOWN)
            {
                if (TryEnterLevel()) return;
            }
            Vector3 originalPosition = transform.position;
            if (!autoMove)
            {
                // If its not auto move then we need to keep holding in direction to continue move
                if (currentDirection == 1 && input.HorizontalAxisDigital != 1) autoMoveTile = null;
                if (currentDirection == -1 && input.HorizontalAxisDigital != -1) autoMoveTile = null;
                if (currentDirection == 10 && input.VerticalAxisDigital != 1) autoMoveTile = null;
                if (currentDirection == -10 && input.VerticalAxisDigital != -1) autoMoveTile = null;
            }
            if (autoMoveTile != null)
            {
                Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
                float distance = Vector2.Distance(transform.position, targetPosition);
                float amount = speed * Time.deltaTime;
                if (amount > distance)
                {
                    transform.position = targetPosition;
                    autoMoveTile = null;
                } 
                else 
                { 
                    transform.Translate(direction * amount);
                }
            }
            else
            {
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, leeway);
                if (hitColliders.Length == 0) return;
                if (input.HorizontalAxisDigital != 0)
                {
                    foreach (Collider2D hit in hitColliders)
                    {
                        UILevelSelectTile currentTile = hit.GetComponent<UILevelSelectTile>();
                        if (currentTile != null && currentTile.AllowsMove(input.HorizontalAxisDigital, 0))
                        {
                            targetPosition = currentTile.TargetPosition(input.HorizontalAxisDigital, 0);
                            if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
                            {
   
                            }
                            else 
                            {
                                autoMoveTile = currentTile;
                                currentDirection = input.HorizontalAxisDigital;
                                return;
                            }
                        }
                    }
                }
                if (input.VerticalAxisDigital != 0)
                {
                    foreach (Collider2D hit in hitColliders)
                    {
                        UILevelSelectTile currentTile = hit.GetComponent<UILevelSelectTile>();
                        if (currentTile != null && currentTile.AllowsMove(0, input.VerticalAxisDigital))
                        {
                            targetPosition = currentTile.TargetPosition(0, input.VerticalAxisDigital);
                            if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
                            {

                            }
                            else
                            {
                                autoMoveTile = currentTile;
                                currentDirection = input.VerticalAxisDigital * 10;
                                return;
                            }
                        }
                    }
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
		virtual protected bool TryEnterLevel()
		{
			Collider2D[] hitColliders = Physics2D.OverlapCircleAll (transform.position, leeway);
			if (hitColliders.Length > 0) 
			{
				foreach (Collider2D hit in hitColliders)
				{
					UILevelSelectTile currentTile =	hit.GetComponent<UILevelSelectTile>();
					if (currentTile != null && currentTile.AllowsEnter()) {
						currentTile.EnterLevel();
                        return true;
					}
				}
			}
            return false;
		}
	}
}