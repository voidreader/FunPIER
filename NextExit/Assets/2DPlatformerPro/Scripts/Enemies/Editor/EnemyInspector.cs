#if UNITY_EDITOR
using UnityEditor;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{
	/// <summary>
	/// Inspector for enemies.
	/// </summary>
	[CustomEditor(typeof(Enemy), false)]
	public class EnemyInspector : Editor
	{
		/// <summary>
		/// Are we editing feet colliders?
		/// </summary>
		protected bool editFeet;

		/// <summary>
		/// Are we editing side colliders?
		/// </summary>
		protected bool editSides;

		/// <summary>
		/// The move button style.
		/// </summary>
		protected static GUIStyle moveButtonStyle;

		/// <summary>
		/// The color of the button handles.
		/// </summary>
		protected static Color mouseOverColor = new Color(0.7f, 0.9f, 1.0f, 1.0f);

		/// <summary>
		/// If true show the enemy debugger.
		/// </summary>
		protected bool debugEnemy;

		/// <summary>
		/// The previous state, stored in debug mode.
		/// </summary>
		protected EnemyState debugPreviousAiState;

		/// <summary>
		/// The previous animation state, stored in debug mode.
		/// </summary>
		protected AnimationState debugPreviousAnimationState;

		/// <summary>
		/// The state, stored in debug mode.
		/// </summary>
		protected EnemyState debugAiState;
		
		/// <summary>
		/// The  animation state, stored in debug mode.
		/// </summary>
		protected AnimationState debugAnimationState;

		/// <summary>
		/// Current movement, stored in debug mode.
		/// </summary>
		protected string debugMovement;

		/// <summary>
		/// Previous movement, stored in debug mode.
		/// </summary>
		protected string debugPreviousMovement;

		/// <summary>
		/// The scene view inspector identifier.
		/// </summary>
		protected static int sceneViewInspectorId = -1;

		/// <summary>
		/// Unity enable hook.
		/// </summary>
		void OnEnable()
		{
			if (moveButtonStyle == null)
			{
				moveButtonStyle = new GUIStyle();
				moveButtonStyle.normal.background = (Texture2D) Resources.Load("MoveButton", typeof(Texture2D));
			}
		}

		/// <summary>
		/// Draw the inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			sceneViewInspectorId = this.GetInstanceID ();

			EditorGUILayout.HelpBox("The base enemy class, can be used for all kinds of enemies from simple to complex.", MessageType.Info, true);
			GUILayout.Space (10);
			DrawDefaultInspector();
			DrawFallInspector ((Enemy) target);
			ShowWarnings();
			DrawEnemyDebugger ((Enemy) target);

		}

		/// <summary>
		/// Draw the scene GUI (i.e. draw collider editors if they are active)
		/// </summary>
		void OnSceneGUI()
		{
			DoSceneGui ();
		}
		/// <summary>
		/// Does the scene GUI.
		/// </summary>
		virtual protected void DoSceneGui()
		{
			if (sceneViewInspectorId != this.GetInstanceID ()) 
			{
				// For some reason Unity creates multiple isntances and uses one to draw the scene view and another to draw the inspector
				// This is a workaround for that
				EnemyInspector c = (EnemyInspector) EditorUtility.InstanceIDToObject(sceneViewInspectorId);
				if (c != null) 
				{
					c.OnSceneGUI();
					return;
				}
			}

			if (editFeet)
			{
				ShowEditFeet((Enemy) target);
			}
			else if (editSides)
			{
				ShowEditSides((Enemy) target);
			}
		}

		/// <summary>
		/// Draws the fall inspector.
		/// </summary>
		virtual protected void DrawFallInspector(Enemy enemy)
		{
			Undo.RecordObject(enemy, "Enemy Update");
			bool newCharacterCanFall = EditorGUILayout.Toggle (new GUIContent ("Can Character Fall", "If true character will use raycasts to check for platforms below the character and fall if they are not found. Elsewise the character will not check for platforms or fall."), enemy.characterCanFall);
			if (newCharacterCanFall != enemy.characterCanFall)
			{
				enemy.characterCanFall = newCharacterCanFall;
				EditorUtility.SetDirty(enemy);
			}

			if (enemy.characterCanFall)
			{
				float newTerminalVelocity = EditorGUILayout.FloatField (new GUIContent ("Terminal Velocity", "Maximum speed the Enemy can fall at."), enemy.terminalVelocity);
				if (newTerminalVelocity != enemy.terminalVelocity)
				{
					enemy.terminalVelocity = newTerminalVelocity;
					EditorUtility.SetDirty(enemy);
				}
				float newGroundedLookAhead = EditorGUILayout.FloatField (new GUIContent ("Grounded Lookahead", "How much further to lookahead when determining if we are grounded."), enemy.groundedLookAhead);
				if (newGroundedLookAhead != enemy.groundedLookAhead)
				{
					enemy.groundedLookAhead = newGroundedLookAhead;
					EditorUtility.SetDirty(enemy);
				}
				bool newUseCharacterGravity = EditorGUILayout.Toggle (new GUIContent ("Use Character Gravity", "If true a Character will be searched for on Start() and that characters gravity setting will be used, be careful of movements that change gravity."), enemy.useCharacterGravity);
				if (newUseCharacterGravity != enemy.useCharacterGravity)
				{
					enemy.useCharacterGravity = newUseCharacterGravity;
					EditorUtility.SetDirty(enemy);
				}
				if (!enemy.useCharacterGravity)
				{
					float newCustomGravity = EditorGUILayout.FloatField(new GUIContent ("Custom Gravity", "The gravity to use for this enemy (must be negative)."), enemy.customGravity);
                    if (newCustomGravity != enemy.customGravity)
   			        {
						enemy.customGravity = newCustomGravity;
						if (enemy.customGravity > 0.0f) enemy.customGravity = 0.0f;
						EditorUtility.SetDirty(enemy);
					}
				}
			}

			bool newDetectSideCollisions = EditorGUILayout.Toggle (new GUIContent ("Side Collisions", "If true character will use raycasts to check for platformsto the right and left of the character. False and the character will walk ignore them."), enemy.detectSideCollisions);
			if (newDetectSideCollisions != enemy.detectSideCollisions)
			{
				enemy.detectSideCollisions = newDetectSideCollisions;
				EditorUtility.SetDirty(enemy);
			}
			
			if (enemy.detectSideCollisions)
			{
				bool newswitchDirectionOnSideHit= EditorGUILayout.Toggle (new GUIContent ("Switch on Side Hit", "If true character will switch direction when hittings sides."), enemy.switchDirectionOnSideHit);
				if (newswitchDirectionOnSideHit != enemy.switchDirectionOnSideHit)
				{
					enemy.switchDirectionOnSideHit = newswitchDirectionOnSideHit;
					EditorUtility.SetDirty(enemy);
				}
			}
			
			// Handles for feet
			EditorGUILayout.BeginHorizontal ();
			GUI.enabled = enemy.characterCanFall;
			GUI.color = (editFeet && enemy.characterCanFall) ? Color.red: Color.white;
			if (GUILayout.Button("Edit Feet")) 
			{
				editFeet = !editFeet;
				if (editFeet)
				{
					editSides = false;
				}
				SceneView.RepaintAll();
			}
			GUI.enabled = enemy.detectSideCollisions;
			GUI.color = (editSides && enemy.detectSideCollisions) ? Color.red: Color.white;
			if (GUILayout.Button("Edit Sides")) 
			{
				editSides = !editSides;
				if (editSides)
				{
					editFeet = false;
				}
				SceneView.RepaintAll();
			}
			EditorGUILayout.EndHorizontal ();
			GUI.enabled = true;
			GUI.color = Color.white;
		}

		/// <summary>
		/// Shows any high level warnings which may suggest misconfigured characters.
		/// </summary>
		virtual protected void ShowWarnings()
		{
			GUILayout.Space (10);
			GUI.color = Color.white;
			GUILayout.Label( "Warnings and Helpers", EditorStyles.boldLabel);
			// Enemy has a hazard
			EnemyHazard hazard = ((Enemy)target).gameObject.GetComponentInChildren<EnemyHazard>();
			if (hazard == null)
			{
				Hazard plainHazard = ((Enemy)target).gameObject.GetComponentInChildren<Hazard>();

				if (plainHazard == null)
				{
					EditorGUILayout.HelpBox("This enemy does not have an EnemyHazard attached. It won't be able to damage the character.", MessageType.Info);
					if (GUILayout.Button("Add a Hazard"))
					{
						AddHazard();
					}
				}
				else
				{
					EditorGUILayout.HelpBox("This enemy has a Hazard but not an EnemyHazard attached. It won't be informed that it has hit a character.", MessageType.Warning);
				}
			}
			// Hazard has a collider
			else
			{
				if (hazard.GetComponent<Collider2D>() == null)
				{
					EditorGUILayout.HelpBox("The Hazard does not have a collider attached. It won't be able to damage the character.", MessageType.Warning);
					if (GUILayout.Button("Add a BoxCollider2D"))
					{
						AddCollider(hazard);
					}
				}
			}
			// Hurt box
			EnemyHurtBox hurtBox = ((Enemy)target).gameObject.GetComponentInChildren<EnemyHurtBox>();
			if (hurtBox == null)
			{
				EditorGUILayout.HelpBox("This enemy does not have an EnemyHurtBox attached. The character wont be able to damage it.", MessageType.Info);
				if (GUILayout.Button("Add a HurtBox"))
				{
					AddHurtBox();
				}
			}
			// Hurt box has a collider
			else
			{
				if (hurtBox.GetComponent<Collider2D>() == null)
				{
					EditorGUILayout.HelpBox("The HurtBox does not have a collider attached. The character wont be able to damage it", MessageType.Warning);
					if (GUILayout.Button("Add a BoxCollider2D"))
					{
						AddCollider(hurtBox);
					}
				}
			}
		}

		/// <summary>
		/// Show the scene view editor for editing feet.
		/// </summary>
		virtual protected void ShowEditFeet(Enemy enemy)
		{
			Vector3 rightResult = DrawMoveHandles (enemy, enemy.feetWidth, enemy.feetHeight);
			Vector3 leftResult = DrawMoveHandles (enemy, -enemy.feetWidth, enemy.feetHeight);
			float newWidth = rightResult.x - enemy.transform.position.x;
			if (newWidth != enemy.feetWidth)
			{
				if (newWidth < 0.0f) newWidth *= -1;
				enemy.feetWidth = newWidth;
				EditorUtility.SetDirty(enemy);
			}
			else
			{
				newWidth = leftResult.x - enemy.transform.position.x;
				if (newWidth != enemy.feetWidth)
				{
					if (newWidth < 0.0f) newWidth *= -1;
					enemy.feetWidth = newWidth;
					EditorUtility.SetDirty(enemy);
				}
			}
			float newHeight = rightResult.y - enemy.transform.position.y;
			if (newHeight != enemy.feetHeight)
			{
				enemy.feetHeight = newHeight;
				EditorUtility.SetDirty(enemy);
			}
			else
			{
				newHeight = leftResult.y - enemy.transform.position.y;
				if (newHeight != enemy.feetHeight)
				{
					enemy.feetHeight = newHeight;
					EditorUtility.SetDirty(enemy);
				}
			}
		}

		/// <summary>
		/// Show the scene view editor for editing sides.
		/// </summary>
		virtual protected void ShowEditSides(Enemy enemy)
		{
			Vector3 rightResult = DrawMoveHandles (enemy, enemy.sideWidth, enemy.sideHeight);
			Vector3 leftResult = DrawMoveHandles (enemy, -enemy.sideWidth, enemy.sideHeight);
			float newWidth = rightResult.x - enemy.transform.position.x;
			if (newWidth != enemy.sideWidth)
			{
				if (newWidth < 0.0f) newWidth *= -1;
				enemy.sideWidth = newWidth;
				EditorUtility.SetDirty(enemy);
			}
			else
			{
				newWidth = leftResult.x - enemy.transform.position.x;
				if (newWidth != enemy.sideWidth)
				{
					if (newWidth < 0.0f) newWidth *= -1;
					enemy.sideWidth = newWidth;
					EditorUtility.SetDirty(enemy);
				}
			}
			float newHeight = rightResult.y - enemy.transform.position.y;
			if (newHeight != enemy.sideHeight)
			{
				enemy.sideHeight = newHeight;
				EditorUtility.SetDirty(enemy);
			}
			else
			{
				newHeight = leftResult.y - enemy.transform.position.y;
				if (newHeight != enemy.sideHeight)
				{
					enemy.sideHeight = newHeight;
					EditorUtility.SetDirty(enemy);
				}
			}
		}

		/// <summary>
		/// Adds a hazard to the enemy
		/// </summary>
		virtual protected void AddHazard()
		{
			GameObject go = new GameObject();
			go.transform.parent = ((Enemy)target).gameObject.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.name = "EnemyHazard";
			int layer = LayerMask.NameToLayer ("Hazard");
			if (layer <= 0) 
			{
				Debug.LogWarning ("No Hazard layer found, using Default layer.");
			}
			else
			{
				go.layer = layer;
			}
			Hazard hazard = go.AddComponent<EnemyHazard>();
			hazard.damageAmount = 1;
			BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
			collider.isTrigger = true;
		}

		/// <summary>
		/// Adds a hazard to the enemy
		/// </summary>
		virtual protected void AddHurtBox()
		{
			GameObject go = new GameObject();
			go.transform.parent = ((Enemy)target).gameObject.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.name = "EnemyHurtBox";
			go.layer = ((Enemy)target).gameObject.layer;
			go.AddComponent<EnemyHurtBox>();
			BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
			collider.isTrigger = true;
		}

		/// <summary>
		/// Adds a collider to a component.
		/// </summary>
		virtual protected void AddCollider(Component component)
		{
			BoxCollider2D collider = component.gameObject.AddComponent<BoxCollider2D>();
			collider.isTrigger = true;
		}

		/// <summary>
		/// Draws a move handle.
		/// </summary>
		/// <returns>The new location.</returns>
		/// <param name="collider">Collider to draw the handle for.</param>
		virtual protected Vector3 DrawMoveHandles(Enemy enemy, float xOffset, float yOffset)
		{
			// Positions
			Vector3 pos = enemy.transform.position + new Vector3 (xOffset, yOffset, 0);
			Vector3 guiPos = Camera.current.WorldToScreenPoint(pos);
			guiPos.y = Screen.height - guiPos.y;
			
			// Adjusment Handles
			Handles.color = new Color(0,0,0,0);
			Vector3 result = Handles.FreeMoveHandle(
				pos, 
				Quaternion.identity,
				0.3f * HandleUtility.GetHandleSize(pos),
				Vector3.zero, Handles.SphereCap);
			
			// Button
			Handles.BeginGUI();
			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
			Rect rect = new Rect(guiPos.x - 24, guiPos.y - 60, 48, 48);
			if (rect.Contains(Event.current.mousePosition)) GUI.backgroundColor = mouseOverColor;
			GUI.Box(rect, " ", moveButtonStyle);
			Handles.EndGUI();
			
			return result;
		}

		virtual protected void DrawEnemyDebugger(Enemy enemy)
		{
			if (debugEnemy) GUI.color = Color.red;
			bool pressed = GUILayout.Button ("Debug Enemy");
			GUI.color = Color.white;
			if (pressed) {
				debugEnemy = !debugEnemy;
				if (debugEnemy)  EditorApplication.update += ForceRepaint;
				else EditorApplication.update -= ForceRepaint;
			}
			if (debugEnemy)
			{
				// headers
				GUILayout.BeginHorizontal ();
				GUILayout.Label("");
				GUILayout.FlexibleSpace ();
				GUILayout.Label("Current",  EditorStyles.boldLabel, GUILayout.Width( 100 ));
				GUILayout.Label("Previous",  EditorStyles.boldLabel, GUILayout.Width( 100 ));
				GUILayout.EndHorizontal ();
				GUILayout.Box("",GUILayout.ExpandWidth(true), GUILayout.Height(1));

				DrawPrefixAndValue("AI State", debugAiState.ToString(), debugPreviousAiState.ToString());
				DrawPrefixAndValue("Movement", debugMovement, debugPreviousMovement);
				DrawPrefixAndValue("Animation State", debugAnimationState.AsString(), debugPreviousAnimationState.AsString());
				DrawPrefixAndValue("Velocity", enemy.Velocity.ToString(), enemy.PreviousVelocity.ToString());
				DrawPrefixAndValue("Target Rotation", enemy.SlopeTargetRotation.ToString(), enemy.PreviousRotation.ToString());
				GUILayout.Space (10);

				// Update states
				if (debugAiState != enemy.State)
				{
					debugPreviousAiState = debugAiState;
					debugAiState = enemy.State;
				}
				if (debugAnimationState != enemy.AnimationState)
				{
					debugPreviousAnimationState = debugAnimationState;
					debugAnimationState = enemy.AnimationState;
				}
				string enemyMovementString = enemy.CurrentMovement == null ? "FALLING" : ((MovementInfo)enemy.CurrentMovement.GetType().GetProperty("Info").GetValue(null, null)).Name;
				if (debugMovement != enemyMovementString)
				{
					debugPreviousMovement = debugMovement;
					debugMovement = enemyMovementString;
				}
			}
		}

		/// <summary>
		/// On disable make sure we stop responding to update.
		/// </summary>
		void OnDisable()
		{
			EditorApplication.update -= ForceRepaint;
		}

		/// <summary>
		/// Forces the repaint.
		/// </summary>
		virtual protected void ForceRepaint()
		{
			Repaint ();
		}

		/// <summary>
		/// Draws a row in the enemy debugger.
		/// </summary>
		/// <param name="prefix">Prefix.</param>
		/// <param name="value">Value.</param>
		/// <param name="previous">Previous.</param>
		virtual protected void DrawPrefixAndValue(string prefix, string value, string previous)
		{
			GUILayout.BeginHorizontal ();
			GUILayout.Label(prefix + ": ", EditorStyles.boldLabel);
			GUILayout.FlexibleSpace ();
			GUILayout.Label(value, GUILayout.Width( 100 ));
			GUILayout.Label(previous != null ? previous : "",  GUILayout.Width( 100 ));
			GUILayout.EndHorizontal ();
		}
	}


}
#endif