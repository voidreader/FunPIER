#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{
	/// <summary>
	/// Inspector for characters classes.
	/// </summary>
	[CustomEditor(typeof(Character), false)]
	public class CharacterInspector : Editor
	{

		protected static int sceneViewInspectorId = -1;

		#region constants

		/// <summary>
		/// Sides should always be at least this distance above the feet.
		/// </summary>
		protected const float MinHeightOfSideRelativeToFeet = 0.05f;
		
		/// <summary>
		/// Sides should always be a little separated.
		/// </summary>
		protected const float MinYDistancebetweenSideColliders = 0.2f;

		#endregion

		#region members

		/// <summary>
		/// Show layers foldout?
		/// </summary>
		protected bool layersFoldOut;

		/// <summary>
		/// Show slopes foldout?
		/// </summary>
		protected bool slopesFoldOut;

		/// <summary>
		/// Show advanced details foldout?
		/// </summary>
		protected bool advancedFoldOut;

		/// <summary>
		/// Show collider editor options foldout
		/// </summary>
		protected bool colliderEditorFoldOut;

		/// <summary>
		/// Are we editing feet colliders?
		/// </summary>
		protected bool editFeet;

		/// <summary>
		/// Are we editing side colliders?
		/// </summary>
		protected bool editSides;

		/// <summary>
		/// Are we editing head colliders?
		/// </summary>
		protected bool editHead;

		/// <summary>
		/// Are we showing but not editing colliders?
		/// </summary>
		protected bool showColliders;

		/// <summary>
		/// Should the character use the smart feet colliders?
		/// </summary>
		protected bool smartFeet;

		/// <summary>
		/// Are we showing the collider debug gizmos.
		/// </summary>
		protected static bool debugColliders;
		
		/// <summary>
		/// Are we sending animation states to the debug window.
		/// </summary>
		protected static bool debugAnimations;
		
		/// <summary>
		/// Are we sending state information to the debug window.
		/// </summary>
		protected static bool debugMovement;

		/// <summary>
		/// How should we detect boundaries?
		/// </summary>
		protected BoundaryDetectionMode boundaryDetectionMode = BoundaryDetectionMode.NONE;

		/// <summary>
		/// Has the user clicked a reset button?
		/// </summary>
		protected bool isAboutToReset;

		/// <summary>
		/// Store previous animation state for use when debugging.
		/// </summary>
		protected AnimationState previousAnimationState = AnimationState.NONE;

		/// <summary>
		/// The add button style.
		/// </summary>
		protected static GUIStyle addButtonStyle;

		/// <summary>
		/// The remove button style.
		/// </summary>
		protected static GUIStyle removeButtonStyle;

		/// <summary>
		/// The move button style.
		/// </summary>
		protected static GUIStyle moveButtonStyle;

		/// <summary>
		/// The default color of the handles.
		/// </summary>
		protected static Color handleColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);

		/// <summary>
		/// The color of the button handles.
		/// </summary>
		protected static Color mouseOverColor = new Color(0.7f, 0.9f, 1.0f, 1.0f);

		/// <summary>
		/// Should colliders be evenly spaced around characters centre.
		/// </summary>
		protected static bool symmetrical;

		/// <summary>
		/// Stores current movement for debugging
		/// </summary>
		protected static string debugCurrentMovement;

		/// <summary>
		/// Stores previous movement for debugging
		/// </summary>
		protected static string debugPreviousMovement;

		/// <summary>
		/// Stores current movement type for debugging
		/// </summary>
		protected static string debugCurrentMovementType;

		/// <summary>
		/// Stores previous movement type for debugging
		/// </summary>
		protected static string debugPreviousMovementType;

		#endregion

		#region serialized properties

		// Layer properties
		protected SerializedProperty layerMask;
		protected SerializedProperty geometryLayerMask;
		protected SerializedProperty passthroughLayerMask;

		// Advanced properties
		protected SerializedProperty feetLookAhead;
		protected SerializedProperty groundedLookAhead;
		protected SerializedProperty groundedLeeway;
		protected SerializedProperty sideLookAhead;
		protected SerializedProperty fallSpeedForIgnoreHeadCollisions;
		protected SerializedProperty terminalVelocity;
		protected SerializedProperty switchCollidersOnDirectionChange;
		protected SerializedProperty passthroughLeeway;

		// Slope properties
		protected SerializedProperty calculateSlopes;
		protected SerializedProperty rotateToSlopes;
		protected SerializedProperty maxSlopeRotation;
		protected SerializedProperty rotationSpeed;
		protected SerializedProperty sideAngle;
		protected SerializedProperty characterRotationType;

		// Ladder detection properties
		protected SerializedProperty detectLaddersByTag;
		protected SerializedProperty ladderLayerOrTagName;

		// Wall detection properties
		protected SerializedProperty detectAllWalls;
		protected SerializedProperty detectWallsByTag;
		protected SerializedProperty wallLayerOrTagName;

		// Input properties
		protected SerializedProperty input;

		protected List<BasicRaycast> feetColliders;
		protected List<BasicRaycast> leftColliders;
		protected List<BasicRaycast> rightColliders;
		protected List<BasicRaycast> headColliders;

		#endregion

		#region Unity hooks

		void OnEnable()
		{
			layerMask = serializedObject.FindProperty("layerMask");
			geometryLayerMask = serializedObject.FindProperty("geometryLayerMask");
			passthroughLayerMask = serializedObject.FindProperty("passthroughLayerMask");

			feetLookAhead = serializedObject.FindProperty("feetLookAhead");
			groundedLookAhead = serializedObject.FindProperty("groundedLookAhead");
			groundedLeeway = serializedObject.FindProperty("groundedLeeway");
			sideLookAhead = serializedObject.FindProperty("sideLookAhead");
			fallSpeedForIgnoreHeadCollisions = serializedObject.FindProperty("fallSpeedForIgnoreHeadCollisions");
			terminalVelocity = serializedObject.FindProperty("terminalVelocity");
			switchCollidersOnDirectionChange = serializedObject.FindProperty("switchCollidersOnDirectionChange");
			passthroughLeeway = serializedObject.FindProperty("passthroughLeeway");

			calculateSlopes = serializedObject.FindProperty("calculateSlopes");
			rotateToSlopes = serializedObject.FindProperty("rotateToSlopes");
			maxSlopeRotation = serializedObject.FindProperty("maxSlopeRotation");
			rotationSpeed = serializedObject.FindProperty("rotationSpeed");
			sideAngle = serializedObject.FindProperty("sideAngle");
			characterRotationType = serializedObject.FindProperty("characterRotationType");

			detectLaddersByTag = serializedObject.FindProperty("detectLaddersByTag");
			ladderLayerOrTagName = serializedObject.FindProperty("ladderLayerOrTagName");

			detectAllWalls = serializedObject.FindProperty("detectAllWalls");
			detectWallsByTag = serializedObject.FindProperty("detectWallsByTag");
			wallLayerOrTagName= serializedObject.FindProperty("wallLayerOrTagName");

			input = serializedObject.FindProperty("input");

			EnsureInitialised ();

			if (addButtonStyle == null)
			{
				addButtonStyle = new GUIStyle();
				addButtonStyle.normal.background = (Texture2D) Resources.Load("AddButton", typeof(Texture2D));
			}
			if (removeButtonStyle == null)
			{
				removeButtonStyle = new GUIStyle();
				removeButtonStyle.normal.background = (Texture2D) Resources.Load("RemoveButton", typeof(Texture2D));
			}
			if (moveButtonStyle == null)
			{
				moveButtonStyle = new GUIStyle();
				moveButtonStyle.normal.background = (Texture2D) Resources.Load("MoveButton", typeof(Texture2D));
			}

		}

		/// <summary>
		/// Ensures all the lists of colliders are non null.
		/// </summary>

		protected virtual void EnsureInitialised()
		{
			if (((Character)target).feetColliders == null) ((Character)target).feetColliders = new BasicRaycast[0];
			if (((Character)target).leftColliders == null) ((Character)target).leftColliders = new BasicRaycast[0];
			if (((Character)target).rightColliders == null) ((Character)target).rightColliders = new BasicRaycast[0];
			if (((Character)target).headColliders == null) ((Character)target).headColliders = new BasicRaycast[0];

			if (feetColliders == null) feetColliders = ((Character)target).feetColliders.ToList();
			if (leftColliders == null) leftColliders = ((Character)target).leftColliders.ToList();
			if (rightColliders == null) rightColliders = ((Character)target).rightColliders.ToList();
			if (headColliders == null) headColliders = ((Character)target).headColliders.ToList();

		}

		/// <summary>
		/// Draw the inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			sceneViewInspectorId = this.GetInstanceID ();

			serializedObject.Update();

			EditorGUILayout.PropertyField(input, new GUIContent("Input", "Characters input. Can be left empty if there is only one Input in the scene."));

			DrawLayersFoldout();

			DrawSlopesFoldout();

			DrawAvancedSettingsFoldout();

			DrawColliderEditor();

			serializedObject.ApplyModifiedProperties ();

			GUI.color = Color.white;

			DrawDebugOptions();

			if (debugMovement) DrawDebugMovement ();
		}

		/// <summary>
		/// Draw the scene GUI (i.e. draw collider editors if they are active)
		/// </summary>
		void OnSceneGUI()
		{
			if (sceneViewInspectorId != this.GetInstanceID ()) 
			{
				// For some reason Unity creates multiple isntances and uses one to draw the scene view and another to draw the inspector
				// This is a workaround for that
				CharacterInspector c = (CharacterInspector) EditorUtility.InstanceIDToObject(sceneViewInspectorId);
				if (c != null) 
				{
					c.OnSceneGUI();
					return;
				}
			}

			EnsureTwoFeetColliders();
			EnsureTwoHeadColliders();
			EnsureSideColliders();
			EnsureFeetSpacing();

			if (!Application.isPlaying)
			{
				if (editFeet)
				{
					ShowSides();
					ShowHead();
					ShowEditFeet();
				}
				if  (editSides)
				{
					ShowFeet();
					ShowHead();
					ShowEditSides();
				}
				if  (editHead)
				{

					ShowSides();
					ShowFeet();
					ShowEditHead();
				}
			}
			if (showColliders)
			{
				ShowSides();
				ShowFeet();
				ShowHead();
			}

			if (debugColliders) DrawDebugColliders();

			SceneView.RepaintAll();
		}
	

		#endregion

		#region methods

		/// <summary>
		/// Draws layer editors.
		/// </summary>
		virtual protected void DrawLayersFoldout()
		{
			layersFoldOut = EditorGUILayout.Foldout(layersFoldOut, "Layer Settings");
			if(layersFoldOut)
			{
				EditorGUILayout.PropertyField(layerMask, new GUIContent("Layer Mask", "All layers that are considered by any collider (automatically includes geometry and passthrough layers)."));
				EditorGUILayout.PropertyField(geometryLayerMask,new GUIContent("Geometry Layer Mask", "The layers that are solid impassable geometry."));
				EditorGUILayout.PropertyField(passthroughLayerMask,new GUIContent("Passthrough Layer Mask", "The layers that can be jumped through but still stood on."));
				int newLayerMask = layerMask.intValue | geometryLayerMask.intValue | passthroughLayerMask.intValue;
				if (newLayerMask != layerMask.intValue) {
					layerMask.intValue = newLayerMask;
					serializedObject.ApplyModifiedProperties();
				}
				if (((Component)target).gameObject.GetComponentInChildren<ClimbMovement>() != null)
				{
					DrawLadderSettingsFoldout();
				}
				if (((Component)target).gameObject.GetComponentInChildren<WallMovement>() != null)
				{
					DrawWallSettingsFoldout();
				}
				if ((geometryLayerMask.intValue & layerMask.intValue) != geometryLayerMask.intValue) EditorGUILayout.HelpBox("Geometry layers MUST be in the overall layer mask.", MessageType.Error);
				else if ((passthroughLayerMask.intValue & layerMask.intValue) != passthroughLayerMask.intValue)EditorGUILayout.HelpBox("Passthrough layers MUST be in the overall layer mask.", MessageType.Error);
				else if (geometryLayerMask.intValue == 0)
				{
					EditorGUILayout.HelpBox("No geometry layers have been set.", MessageType.Warning);
				}
				for (int i = NoAllocationRaycast.MAX_LAYER; i < NoAllocationRaycast.MAX_LAYER + 16; i++)
				{
					if ((layerMask.intValue & (1 << i)) == (1 << i))
					{
						EditorGUILayout.HelpBox("For performance reasons layers higher than the MAX_LAYER defined in NoAllocationRaycast will be ignored. The default is 16 and you should only change it if you know what you are doing.", MessageType.Warning);
					}
				}
			}
		}

		/// <summary>
		/// Draws detail editors.
		/// </summary>
		virtual protected void DrawSlopesFoldout()
		{
			slopesFoldOut = EditorGUILayout.Foldout(slopesFoldOut, "Slopes and Angles");
			if(slopesFoldOut)
			{
				EditorGUILayout.PropertyField(calculateSlopes, new GUIContent("Calculate Slopes", "Should we calculate slopes for use by movements."));
				if (calculateSlopes.boolValue)
				{
					EditorGUILayout.PropertyField(rotateToSlopes, new GUIContent("Rotate to Slopes", "Does the character rotate to match slopes? Note you can walk on slopes without rotation (be sure to set the Minimum Wall Angle)."));
					if (!rotateToSlopes.boolValue) 
					{
						EditorGUILayout.HelpBox("Although rotation is off the following values will be used if for example you parent to a rotating platform.", MessageType.Info);
					}
					EditorGUILayout.PropertyField(maxSlopeRotation, new GUIContent("Max Slope Rotation", "How much in degrees can the character be rotated. Slopes steeper than this will be ignored."));

					EditorGUILayout.PropertyField(rotationSpeed, new GUIContent("Rotation Speed", "How fast in degrees per second the character will rotate to match a slope."));
					// Check for warnings
					if (rotationSpeed.floatValue < 360 && ((Character)target).gameObject.GetComponentInChildren<DontRotateSprite>() != null)
					{
						EditorGUILayout.HelpBox("You are using a DontRotateSprite script so it is recommended you set rotation speed to a very high value (360+).", MessageType.Warning);
					}
					else if (rotationSpeed.floatValue < 120)
					{
						EditorGUILayout.HelpBox("In most cases rotation speed should be 120 or higher (180 is the default).", MessageType.Info);
					}

					EditorGUILayout.PropertyField(characterRotationType, new GUIContent("Rotation Type", "Defines how the slope rotation is applied to the character."));
					EditorGUILayout.HelpBox(((CharacterRotationType)characterRotationType.enumValueIndex).GetDescription(), MessageType.Info);

				}
				else
				{
					rotateToSlopes.boolValue = false;
				}

				EditorGUILayout.PropertyField(sideAngle, new GUIContent("Minimum Wall Angle", "What angle does the geometry need to be at before its considered a wall. Side colliders will ignore geometry that isn't a wall."));
				// Check for warnings
				if (sideAngle.floatValue > 30 && feetColliders.Count < 4)
				{
					EditorGUILayout.HelpBox("It is recommended to use at least 5 feet colliders if your minimum wall angle is greater than 30 degrees", MessageType.Warning);
				}
				else if (sideAngle.floatValue > 0 && feetColliders.Count < 3)
				{
					EditorGUILayout.HelpBox("It is recommended to use at least 3 feet colliders if your minimum wall angle is greater than zero", MessageType.Warning);
				}
				

			}
		}

		virtual protected void DrawLadderSettingsFoldout()
		{
			// Tag or layer based detection
			EditorGUILayout.PropertyField(detectLaddersByTag, new GUIContent("Detect Climbables by Tags", "If true use tags to detect ladders and other climbable, else use layers. Tags can cause an allocation which may be a problem on mobile devices."));

			if (detectLaddersByTag.boolValue)
			{
				string newTagName = 
					EditorGUILayout.TagField(
					new GUIContent("Climbable Tag", "The tag to use for ladder and other climbable detection."), ladderLayerOrTagName.stringValue);
				if (newTagName != ladderLayerOrTagName.stringValue)
				{
					ladderLayerOrTagName.stringValue = newTagName;
					EditorUtility.SetDirty(target);
				}
			}
			else
			{
				string newLadderLayerOrTagName = LayerMask.LayerToName(
					EditorGUILayout.LayerField(
					new GUIContent("Climbable Layer", "The layer to use for ladder and other climbable detection."), LayerMask.NameToLayer(ladderLayerOrTagName.stringValue)));
				if (newLadderLayerOrTagName != ladderLayerOrTagName.stringValue)
				{
					ladderLayerOrTagName.stringValue = newLadderLayerOrTagName;
					EditorUtility.SetDirty(target);
				}
			}
		}

		virtual protected void DrawWallSettingsFoldout()
		{
			// Works on all walls
			EditorGUILayout.PropertyField(detectAllWalls, new GUIContent("Use All Walls", "If true all walls will be considered for wall slide, ledge climb, etc. Otherwise only specific walls will apply. Note you can further filter walls in some movements."));

			if (!detectAllWalls.boolValue)
			{
				// Tag or layer based detection
				EditorGUILayout.PropertyField(detectWallsByTag, new GUIContent("Detect Walls by Tags", "If true use tags to detect walls and ledges, else use layers. Tags can cause an allocation which may be a problem on mobile devices."));
				
				if (detectWallsByTag.boolValue)
				{
					string newTagName = 
						EditorGUILayout.TagField(
							new GUIContent("Wall Tag", "The tag to use for wall and ledge detection."), wallLayerOrTagName.stringValue);
					if (newTagName != wallLayerOrTagName.stringValue)
					{
						wallLayerOrTagName.stringValue = newTagName;
						EditorUtility.SetDirty(target);
					}
				}
				else
				{
					string newWallLayerOrTagName = LayerMask.LayerToName(
						EditorGUILayout.LayerField(
						new GUIContent("Wall Layer", "The layer to use for wall and ledge detection."), LayerMask.NameToLayer(wallLayerOrTagName.stringValue)));
					if (newWallLayerOrTagName != wallLayerOrTagName.stringValue)
					{
						wallLayerOrTagName.stringValue = newWallLayerOrTagName;
						EditorUtility.SetDirty(target);
					}
				}
			}
		}

		/// <summary>
		/// Draws detail editors.
		/// </summary>
		virtual protected void DrawAvancedSettingsFoldout()
		{
			advancedFoldOut = EditorGUILayout.Foldout(advancedFoldOut, "Advanced Settings");
			if(advancedFoldOut)
			{
				// Reset button
				if (GUILayout.Button(new GUIContent("Reset To Defaults","Resets all advanced settings to default values.")))
				{
					serializedObject.Update();
					feetLookAhead.floatValue = Character.DefaultFeetLookAhead;
					groundedLookAhead.floatValue = Character.DefaultGroundedLookAhead;
					groundedLeeway.floatValue = Character.DefaultGroundedLeeway;
					passthroughLeeway.floatValue = Character.DefaultPassthroughLeeway;
					sideLookAhead.floatValue = Character.DefaultSideLookAhead;
					fallSpeedForIgnoreHeadCollisions.floatValue = Character.DefaultFallSpeedForIgnoreHeadCollisions;
					terminalVelocity.floatValue = Character.DefaultTerminalVelocity;
					switchCollidersOnDirectionChange.boolValue = false;
				}
				EditorGUILayout.PropertyField(terminalVelocity, new GUIContent("Terminal Velocity", "The fastest speed the character can move downwards at."));
				EditorGUILayout.PropertyField(feetLookAhead, new GUIContent("Feet Look Ahead", "How much further, beyond the bottom of the feet, to cast the ground colliders. Used for ladder detection and landing detection."));
				EditorGUILayout.PropertyField(groundedLeeway, new GUIContent("Grounded Leeway", "The time after leaving the ground for which the character is still considered to be grounded."));
				EditorGUILayout.PropertyField(groundedLookAhead, new GUIContent("Grounded Look Ahead", "How much further, beyond the bottom of the feet, to check when considering the character to be grounded."));
				EditorGUILayout.PropertyField(sideLookAhead, new GUIContent("Side Look Ahead", "How much further, beyond the sides of the character, to cast the side colliders. Used for wall and ledge detection."));
				EditorGUILayout.PropertyField(fallSpeedForIgnoreHeadCollisions, new GUIContent("Speed For Ingore Head", "The downward speed at which head collisions are completely ignored. Lower values give smoother movement, but wont work with fast moving vertical platforms."));
				EditorGUILayout.PropertyField(passthroughLeeway, new GUIContent("Passthrough Leeway", "At what point will a passthrough collider consider itself grounded. Too small and you fall through platforms. Too large and you snap oddly. Usually the default is okay unless you change gravity quite a bit."));
				EditorGUILayout.PropertyField(switchCollidersOnDirectionChange, new GUIContent("Flip Raycasts", "Should we switch raycast collider direction when the character changes direction (used for asymmetric characters)."));

			}
		}

		/// <summary>
		/// Draws the editor controls for editing colliders.
		/// </summary>
		virtual protected void DrawColliderEditor()
		{
			
			GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
			GUILayout.Label ("Collider Editor");
			GUI.color = Color.white;
			
			GUI.enabled = !isAboutToReset;
			if (GUILayout.Button(new GUIContent("Reset Colliders", "Reset the characters raycast colliders.")))
			{
				isAboutToReset = true;
				showColliders = true;
				editSides = false;
				editHead = false;
				editFeet = false;
			}
			GUI.enabled = true;

			if (isAboutToReset)
			{
				// Smart sprite
				SpriteRenderer spriteRenderer = ((Character)target).gameObject.GetComponentInChildren<SpriteRenderer>();
				if (spriteRenderer != null && spriteRenderer.sprite != null)
				{
					string texturePath = AssetDatabase.GetAssetPath(spriteRenderer.sprite.texture);
					TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(texturePath);
					if (textureImporter.isReadable) {
						EditorGUILayout.BeginHorizontal();
						GUILayout.Space(10);
						GUILayout.Label("Use smart sprite detection");
						GUILayout.FlexibleSpace();
						if (GUILayout.Button("Reset"))
						{
							isAboutToReset = false;
							boundaryDetectionMode = BoundaryDetectionMode.SMART_SPRITE;
							ResetAllColliders();
						}
						EditorGUILayout.EndHorizontal();
					}
					else
					{
						EditorGUILayout.HelpBox("Sprite is not readable, smart sprite based option disabled.", MessageType.Info);
					}
					
					// Sprite
					EditorGUILayout.BeginHorizontal();
					if (((Character)target).gameObject.GetComponentInChildren<SpriteRenderer>() != null)
					{
						GUILayout.Space(10);
						GUILayout.Label("Use basic sprite detection");
						GUILayout.FlexibleSpace();
						if (GUILayout.Button("Reset"))
						{
							isAboutToReset = false;
							boundaryDetectionMode = BoundaryDetectionMode.SPRITE;
							ResetAllColliders();
						}
					}
					EditorGUILayout.EndHorizontal();
					
				}
				
				// Collider
				if (((Character)target).gameObject.GetComponentInChildren<Collider2D>() != null)
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(10);
					GUILayout.Label("Use collider based detection");
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Reset"))
					{
						isAboutToReset = false;
						boundaryDetectionMode = BoundaryDetectionMode.COLLIDER;
						ResetAllColliders();
					}
					EditorGUILayout.EndHorizontal();
				}
				
				// Mesh
				if (((Character)target).gameObject.GetComponentInChildren<MeshFilter>() != null)
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(10);;
					GUILayout.Label("Use mesh based detection");
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Reset"))
					{
						isAboutToReset = false;
						boundaryDetectionMode = BoundaryDetectionMode.MESH;
						ResetAllColliders();
					}
					EditorGUILayout.EndHorizontal();
				}
				
				// Default
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10);;
				GUILayout.Label("Reset to default position");
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Reset"))
				{
					isAboutToReset = false;
					boundaryDetectionMode = BoundaryDetectionMode.DEFAULT;
					ResetAllColliders();
				}
				EditorGUILayout.EndHorizontal();
				
				// Cancel
				EditorGUILayout.BeginHorizontal();
				
				GUILayout.Space(10);
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Cancel"))
				{
					isAboutToReset = false;
				}
				EditorGUILayout.EndHorizontal();
				
			}

			GUI.enabled = !isAboutToReset;

			// Edit Buttons
			EditorGUILayout.BeginHorizontal();
			if (((Character)target).gameObject.transform.rotation != Quaternion.identity) GUI.enabled = false;
			GUI.color = editFeet ? Color.red: Color.white;
			if (GUILayout.Button("Edit Feet")) 
			{
				editFeet = !editFeet;
				if (editFeet)
				{
					editSides = false;
					editHead = false;
					showColliders = false;
					symmetrical = true;
				}
			}
			GUI.color = editSides ? Color.red: Color.white;
			if (GUILayout.Button("Edit Sides"))
			{
				editSides = !editSides;
				if (editSides)
				{
					editFeet = false;
					editHead = false;
					showColliders = false;
					symmetrical = true;
				}
			}
			GUI.color = editHead ? Color.red: Color.white;
			if (GUILayout.Button("Edit Head"))
			{
				editHead = !editHead;
				if (editHead)
				{
					editSides = false;
					editFeet = false;
					showColliders = false;
					symmetrical = true;
				}
			}

			GUI.enabled = true;
			GUI.color = showColliders ? Color.red: Color.white;
			if (GUILayout.Button("Show Only"))
			{
				showColliders = !showColliders;
				if (showColliders)
				{
					editSides = false;
					editFeet = false;
					editHead = false;
				}
			}
			EditorGUILayout.EndHorizontal();

			GUI.enabled = true;

			if (((Character)target).gameObject.transform.rotation != Quaternion.identity) {
				if (editSides || editFeet || editHead)
				{
					showColliders = true;
					editSides = false;
					editFeet = false;
					editHead = false;
				}
				GUI.color = Color.white;
				EditorGUILayout.HelpBox("You cannot edit colliders on a character that is rotated.", MessageType.Warning);
			}

			if (((Character)target).gameObject.transform.rotation != Quaternion.identity) {
				if (editSides || editFeet || editHead)
				{
					isAboutToReset = false;
					editSides = false;
					editFeet = false;
					editHead = false;
				}
				GUI.color = Color.white;
				EditorGUILayout.HelpBox("You cannot edit colliders on a character that is rotated.", MessageType.Warning);
			}

			GUI.color = Color.white;

			if (editFeet) DrawFeetGui();

			if (editSides) DrawSidesGui();

			if (editHead) DrawHeadGui();


		}

		/// <summary>
		/// Draws the debug options buttons.
		/// </summary>
		virtual protected void DrawDebugOptions()
		{
			GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
			GUILayout.Label ("Debug Options");
			// Edit Buttons
			EditorGUILayout.BeginHorizontal();
			GUI.color = debugColliders ? Color.red: Color.white;
			if (GUILayout.Button("Colliders")) 
			{
				debugColliders = !debugColliders;
			}
			GUI.color = debugAnimations ? Color.red: Color.white;
			if (GUILayout.Button("Animations")) 
			{
				debugAnimations = !debugAnimations;
				if (debugAnimations) AddAnimationDebugger();
				else RemoveAnimationDebugger();
			}
			GUI.color = debugMovement ? Color.red: Color.white;
			if (GUILayout.Button("Movement")) 
			{
				debugMovement = !debugMovement;
				if (debugMovement)  EditorApplication.update += ForceRepaint;
				else EditorApplication.update -= ForceRepaint;
			}
			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal();
		}

		/// <summary>
		/// Draws the GUI for editing feet.
		/// </summary>
		virtual protected void DrawFeetGui()
		{
			EnsureTwoFeetColliders();

			EditorGUILayout.HelpBox("Drag handles to move.", MessageType.Info);

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(new GUIContent("Add Collider", "Adds another foot collider.")))
			{
				feetColliders.Insert(1, CreateDefaultCollider(RaycastType.FOOT, new Vector2((feetColliders[0].Extent.x + feetColliders[feetColliders.Count - 1].Extent.x) / 2.0f, feetColliders[0].Extent.y)));
				UpdateColliders();
			}
			if (feetColliders.Count <= 2) GUI.enabled = false;
			if (GUILayout.Button(new GUIContent("Remove Collider", "Removes a foot collider.")))
			{
				feetColliders.RemoveAt (1);
				UpdateColliders();
			}
			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();
			
			symmetrical = EditorGUILayout.ToggleLeft(new GUIContent("Reflect changes to both sides", "Should the feet extents be evenly spaced around the centre of the character."), symmetrical);
			
			EnsureFeetSpacing();
		}

		/// <summary>
		/// Resets all colliders.
		/// </summary>
		virtual protected void ResetAllColliders()
		{
			// Feet
			isAboutToReset = false;

			feetColliders.Clear ();
			EnsureTwoFeetColliders ();

			// Head
			isAboutToReset = false;
			headColliders.Clear ();
			EnsureTwoHeadColliders();

			// Sides
			leftColliders.Clear();
			rightColliders.Clear();
			EnsureSideColliders();

		}

		/// <summary>
		/// Order and save colliders and set an undo point
		/// </summary>
		virtual protected void UpdateColliders()
		{
			((Character)target).feetColliders = feetColliders.OrderBy (f => f.Extent.x).ToArray ();
			((Character)target).leftColliders = leftColliders.OrderBy(f => f.Extent.y).ToArray ();
			((Character)target).rightColliders = rightColliders.OrderBy (f => f.Extent.y).ToArray ();
			((Character)target).headColliders = headColliders.OrderBy (f => f.Extent.x).ToArray ();
			EnsureHeadLength ();
			EditorUtility.SetDirty (target);
		}

		/// <summary>
		/// Draws the GUI for editing sides.
		/// </summary>
		virtual protected void DrawSidesGui()
		{
			EnsureSideColliders();

			EditorGUILayout.HelpBox("Drag handles to move. Use (+) to add new colliders. Hold SHIFT to delete colliders.", MessageType.Info);

			GUILayout.BeginHorizontal();
			if (GUILayout.Button(new GUIContent("Align in X", "Align the x value of all the side colliders (uses bottom as a reference).")))
			{
				for (int i = 1; i < leftColliders.Count; i++)
				{
					leftColliders[i].Extent = new Vector2(leftColliders[0].Extent.x ,leftColliders[i].Extent.y); 
				}
				for (int i = 1; i < rightColliders.Count; i++)
				{
					if (rightColliders[i].Extent != new Vector2(rightColliders[0].Extent.x, rightColliders[i].Extent.y))
					{
						rightColliders[i].Extent = new Vector2(rightColliders[0].Extent.x, rightColliders[i].Extent.y);
						UpdateColliders();
					}
				}
			}
			if (GUILayout.Button(new GUIContent("Space Evenly", "Space the side colliders evely in Y.")))
			{
				float spacing = Mathf.Abs(((leftColliders[0].Extent.y - leftColliders[leftColliders.Count - 1].Extent.y)) / (leftColliders.Count - 1));
				for (int i = 1; i < leftColliders.Count; i++)
				{
					if (leftColliders[i].Extent != new Vector2(leftColliders[i].Extent.x, leftColliders[0].Extent.y + spacing * i))
				    {
						leftColliders[i].Extent = new Vector2(leftColliders[i].Extent.x, leftColliders[0].Extent.y + spacing * i);
						UpdateColliders();
					}
				}

				spacing = Mathf.Abs (((rightColliders[0].Extent.y - rightColliders[rightColliders.Count - 1].Extent.y)) / (rightColliders.Count - 1));
				for (int i = 1; i < leftColliders.Count; i++)
				{
					if (rightColliders[i].Extent != new Vector2(rightColliders[i].Extent.x, rightColliders[0].Extent.y + spacing * i))
					{
						rightColliders[i].Extent = new Vector2(rightColliders[i].Extent.x, rightColliders[0].Extent.y + spacing * i);
						UpdateColliders();
					}
				}

			}
			GUILayout.EndHorizontal();
			symmetrical = EditorGUILayout.ToggleLeft(new GUIContent("Reflect changes to both sides", "Should the sides extents be evenly spaced around the centre of the character."), symmetrical);

		}

		/// <summary>
		/// Draws the GUI for editing head.
		/// </summary>
		virtual protected void DrawHeadGui()
		{
			EnsureTwoHeadColliders();

			EditorGUILayout.HelpBox("Drag handles to move. Use (+) to add new colliders. Hold SHIFT to delete colliders.", MessageType.Info);

			GUILayout.BeginHorizontal();
			if (GUILayout.Button(new GUIContent("Align Top", "Align the y extentof all the head colliders (uses position 0 as a reference).")))
			{
				for (int i = 1; i < headColliders.Count; i++)
				{
					if (headColliders[i].Extent != new Vector2(headColliders[i].Extent.x ,headColliders[0].Extent.y))
					{
						headColliders[i].Extent = new Vector2(headColliders[i].Extent.x ,headColliders[0].Extent.y); 
						UpdateColliders();
					}
				}
			}
			if (GUILayout.Button(new GUIContent("Space Evenly", "Space the side colliders evenly in X.")))
			{
				float spacing = Mathf.Abs(((headColliders[0].Extent.x - headColliders[headColliders.Count - 1].Extent.x)) / (headColliders.Count - 1));
				for (int i = 1; i < headColliders.Count; i++)
				{
					if (headColliders[i].Extent != new Vector2(headColliders[0].Extent.x + spacing * i,headColliders[i].Extent.y)) 
					{
						headColliders[i].Extent = new Vector2(headColliders[0].Extent.x + spacing * i,headColliders[i].Extent.y);
						UpdateColliders();
					}
				}
			}
			GUILayout.EndHorizontal();
			symmetrical = EditorGUILayout.ToggleLeft(new GUIContent("Reflect changes to both sides", "Should the head extents be evenly spaced around the centre of the character."), symmetrical);
		}

		/// <summary>
		/// Show the scene view editor for editing feet.
		/// </summary>
		virtual protected void ShowEditFeet()
		{
			GUIStyle centeredLabelStyle = GUI.skin.GetStyle("Label");
			centeredLabelStyle.alignment = TextAnchor.UpperCenter;

			// If we have some feet we can now draw them
			if (feetColliders.Count >= 2)
			{
				BasicRaycast leftFoot = feetColliders[0];
				BasicRaycast rightFoot = feetColliders[feetColliders.Count - 1];

				Handles.color = handleColor;

				// Draw uneditable GUI
				for (int i = 1; i < feetColliders.Count - 1; i++)
				{
					DrawImmovableHandle(feetColliders[i]);
				}

				Handles.DrawLine(leftFoot.Transform.position + (Vector3)leftFoot.Extent, rightFoot.Transform.position + (Vector3)rightFoot.Extent);

				// Draw editable GUI

				Vector2 newPositionLeft = DrawMoveHandle(leftFoot);
				Vector2 newPositionRight = DrawMoveHandle(rightFoot);

				float newY = leftFoot.Extent.y;
				if (newPositionRight.y != newY) newY = newPositionRight.y;
				else if (newPositionLeft.y != newY) newY = newPositionLeft.y;

				if (newPositionLeft.x != leftFoot.Extent.x) 
				{
					leftFoot.Extent = new Vector2(newPositionLeft.x, newY);
					if (symmetrical)
					{
						rightFoot.Extent = new Vector2(-newPositionLeft.x, newY);
					} 
					else
					{
						rightFoot.Extent = new Vector2(newPositionRight.x, newY);
					}
					UpdateColliders();
				}
				else if (newPositionRight.x != rightFoot.Extent.x) 
				{
					rightFoot.Extent = new Vector2(newPositionRight.x, newY);
					if (symmetrical)
					{
						leftFoot.Extent = new Vector2(-newPositionRight.x, newY);
					} 
					else
					{
						leftFoot.Extent = new Vector2(newPositionLeft.x, newY);
					}
					UpdateColliders ();
				}
				else
				{
					if (rightFoot.Extent != new Vector2(newPositionRight.x, newY))
					{
						rightFoot.Extent = new Vector2(newPositionRight.x, newY);
						UpdateColliders();
					}
					if (leftFoot.Extent != new Vector2(newPositionLeft.x, newY)) 
					{
						leftFoot.Extent = new Vector2(newPositionLeft.x, newY);
						UpdateColliders ();
					}
				}
				EnsureFeetSpacing();
			}
		}

		/// <summary>
		/// Show the feet in the scene view (not editabe)
		/// </summary>
		virtual protected void ShowFeet()
		{
			GUIStyle centeredLabelStyle = GUI.skin.GetStyle("Label");
			centeredLabelStyle.alignment = TextAnchor.UpperCenter;
			
			// If we have some feet we can now draw them
			if (feetColliders.Count >= 2)
			{
				BasicRaycast leftFoot = feetColliders[0];
				BasicRaycast rightFoot = feetColliders[feetColliders.Count - 1];
				
				Handles.color = handleColor;
				Handles.DrawLine(leftFoot.WorldExtent, rightFoot.WorldExtent);
				
				Vector3 pointLeft = Camera.current.WorldToScreenPoint(leftFoot.WorldExtent);
				Vector3 pointRight = Camera.current.WorldToScreenPoint(rightFoot.WorldExtent);

				if (pointRight.x < pointLeft.x)
				{
					Vector3 tmp = pointRight; pointRight = pointLeft; pointLeft = tmp;
				}
				
				Handles.BeginGUI();
				GUI.color = handleColor;
				GUILayout.BeginArea(new Rect(pointLeft.x, Screen.height - pointLeft.y - 32, pointRight.x - pointLeft.x, 16));
				GUILayout.Label("Feet", centeredLabelStyle);
				GUILayout.EndArea();
				Handles.EndGUI();
			}
		}

		/// <summary>
		/// Show the scene view editor for editing sides.
		/// </summary>
		virtual protected void ShowEditSides()
		{
			GUIStyle centeredLabelStyle = GUI.skin.GetStyle("Label");
			centeredLabelStyle.alignment = TextAnchor.UpperCenter;
			
			// If we have some sides we can now draw them
			if (leftColliders.Count >= 2)
			{
				BasicRaycast previous = null;
				for (int i = 0; i < leftColliders.Count; i++)
				{
					BasicRaycast left = leftColliders[i];
					// Draw line and add handle
					if (previous != null)
					{
						Handles.color = handleColor;
						Handles.DrawLine(left.Transform.position + (Vector3)left.Extent, previous.Transform.position + (Vector3)previous.Extent);

						if (!Event.current.shift)
						{
							if (DrawAddHandle(left, previous))
							{
								leftColliders.Insert (i,CreateDefaultCollider(RaycastType.SIDE_LEFT, (leftColliders[i-1].Extent + leftColliders[i].Extent) / 2.0f)); 
								// Always add to other side too
								rightColliders.Insert(i, CreateDefaultCollider(RaycastType.SIDE_RIGHT, (rightColliders[i-1].Extent + rightColliders[i].Extent) / 2.0f)); 
								UpdateColliders();
								break;
							}
						}
					}
					previous = left;
				}

				for (int i = 0; i < leftColliders.Count; i++)
				{
					BasicRaycast left = leftColliders[i];
					// Draw handle
					if (Event.current.shift && leftColliders.Count > 2) 
					{
						if (DrawRemoveHandle(left))
						{
							leftColliders.RemoveAt(i);
							rightColliders.RemoveAt(i);
							UpdateColliders();
						}
					}
					else
					{
						Vector2 newPosition = DrawMoveHandle(left);
						if (newPosition != left.Extent) 
						{
							if (newPosition.y < feetColliders[0].Extent.y + // feetColliders[0].Transform.y  + 
									    MinHeightOfSideRelativeToFeet) 
							{
								newPosition.y = feetColliders[0].Extent.y + MinHeightOfSideRelativeToFeet;
								UpdateColliders();
							}
							left.Extent = newPosition;
							if (symmetrical && rightColliders.Count > i)
							{
								rightColliders[i].Extent = new Vector2(-newPosition.x, newPosition.y);
								UpdateColliders();
							}
						}
					}

					// Ensure left is to the left and right to the right
					if (i < rightColliders.Count)
					{
						if (rightColliders[i].Extent.x < leftColliders[i].Extent.x)
						{
							BasicRaycast tmp = rightColliders[i];
							rightColliders[i] = leftColliders[i];
							leftColliders[i] = tmp;
							leftColliders[i].RaycastType = RaycastType.SIDE_LEFT;
							rightColliders[i].RaycastType = RaycastType.SIDE_RIGHT;
							UpdateColliders();
						}
					}
				}

			}

			// If we have some sides we can now draw them
			if (rightColliders.Count >= 2)
			{
				BasicRaycast previous = null;
				for (int i = 0; i < rightColliders.Count; i++)
				{
					BasicRaycast right = rightColliders[i];
					// Draw line and add handle
					if (previous != null)
					{
						Handles.color = handleColor;
						Handles.DrawLine(right.Transform.position + (Vector3)right.Extent, previous.Transform.position + (Vector3)previous.Extent);
						if (!Event.current.shift)
						{
							if (DrawAddHandle(right, previous))
							{
								rightColliders.Insert(i-1, CreateDefaultCollider(RaycastType.SIDE_RIGHT, (rightColliders[i-1].Extent + rightColliders[i].Extent) / 2.0f)); 
								// Always add to other side too
								leftColliders.Insert (i-1,  CreateDefaultCollider(RaycastType.SIDE_LEFT, (leftColliders[i-1].Extent + leftColliders[i].Extent) / 2.0f)); 
								UpdateColliders ();
								break;
							}
						}
					}
					previous = right;
				}
				
				for (int i = 0; i < rightColliders.Count; i++)
				{
					BasicRaycast right = rightColliders[i];
					// Draw handle
					if (Event.current.shift && rightColliders.Count > 2) 
					{
						if (DrawRemoveHandle(right)) 
						{
							rightColliders.RemoveAt(i);
							leftColliders.RemoveAt(i);
							UpdateColliders();
						}
					}
					else
					{
						Vector2 newPosition = DrawMoveHandle(right);
						if (newPosition != right.Extent) 
						{
							right.Extent = newPosition;
							if (symmetrical && leftColliders.Count > i)
							{
								leftColliders[i].Extent = new Vector2(-newPosition.x, newPosition.y);
							}
						}
					}
				}
				
			}
		}

		
		/// <summary>
		/// Show the sides in the scene view (not editable).
		/// </summary>
		virtual protected void ShowSides()
		{
			GUIStyle centeredLabelStyle = GUI.skin.GetStyle("Label");
			centeredLabelStyle.alignment = TextAnchor.UpperCenter;

			Vector3 pointBottom = Vector3.zero;
			Vector3 pointTop = Vector3.zero;

			// If we have some sides we can now draw them
			if (leftColliders.Count >= 2)
			{

				BasicRaycast previous = null;
				for (int i = 0; i < leftColliders.Count; i++)
				{
					BasicRaycast left = leftColliders[i];
					if (i == 0) pointBottom = Camera.current.WorldToScreenPoint(left.WorldExtent);
					if (i == headColliders.Count - 1) pointTop = Camera.current.WorldToScreenPoint(left.WorldExtent);

					// Draw line
					if (previous != null)
					{
						Handles.color = handleColor;
						Handles.DrawLine(left.WorldExtent, previous.WorldExtent);
					}
					previous = left;
				}

				if (pointBottom.y > pointTop.y)
				{
					Vector3 tmp = pointTop; pointTop = pointBottom; pointBottom = tmp;
				}

			}

			// If we have some feet we can now draw them
			if (rightColliders.Count >= 2)
			{
				BasicRaycast previous = null;

				for (int i = 0; i < rightColliders.Count; i++)
				{
					BasicRaycast right = rightColliders[i];
					if (i == 0) pointBottom = Camera.current.WorldToScreenPoint(right.WorldExtent);
					if (i == headColliders.Count - 1) pointTop = Camera.current.WorldToScreenPoint(right.WorldExtent);
					// Draw line
					if (previous != null)
					{
						Handles.color = handleColor;
						Handles.DrawLine(right.WorldExtent, previous.WorldExtent);
					}
					previous = right;
				}

				if (pointBottom.y > pointTop.y)
				{
					Vector3 tmp = pointTop; pointTop = pointBottom; pointBottom = tmp;
				}
			}

		}

		/// <summary>
		/// Show the scene view editor for editing head.
		/// </summary>
		virtual protected void ShowEditHead()
		{
			GUIStyle centeredLabelStyle = GUI.skin.GetStyle("Label");
			centeredLabelStyle.alignment = TextAnchor.UpperCenter;
			
			// If we have some head colliders we can now draw them
			if (headColliders.Count >= 2)
			{
				BasicRaycast previous = null;
				for (int i = 0; i < headColliders.Count; i++)
				{
					BasicRaycast head = headColliders[i];
					// Draw line and add handle
					if (previous != null)
					{
						Handles.color = handleColor;
						Handles.DrawLine(head.Transform.position + (Vector3)head.Extent, previous.Transform.position + (Vector3)previous.Extent);
						
						if (!Event.current.shift)
						{
							if (DrawAddHandle(head, previous))
							{
								headColliders.Insert (i, CreateDefaultCollider(RaycastType.HEAD, (headColliders[i-1].Extent + headColliders[i].Extent) / 2.0f)); 
								UpdateColliders();
								break;
							}
						}
					}
					previous = head;
				}
				
				for (int i = 0; i < headColliders.Count; i++)
				{
					BasicRaycast head = headColliders[i];
					// Draw handle
					if (Event.current.shift && headColliders.Count > 2) 
					{
						if (DrawRemoveHandle(head))
						{
							headColliders.RemoveAt(i);
							UpdateColliders();
						}
					}
					else
					{
						Vector2 newPosition = DrawMoveHandle(head);
						if (newPosition != head.Extent) 
						{
							head.Extent = newPosition;
							if (symmetrical)
							{
								int j = headColliders.Count - i - 1;
								if (j != i)
								{
									headColliders[j].Extent = new Vector2(-headColliders[i].Extent.x, headColliders[i].Extent.y);
								}
							}
						}

					}
				}
				
			}
		}

		/// <summary>
		/// Show the head colliders in scene view (not editable)
		/// </summary>
		virtual protected void ShowHead()
		{
			GUIStyle centeredLabelStyle = GUI.skin.GetStyle("Label");
			centeredLabelStyle.alignment = TextAnchor.UpperCenter;
			Vector3 pointLeft = Vector3.zero;
			Vector3 pointRight = Vector3.zero;

			// If we have some head colliders we can now draw them
			if (headColliders.Count >= 2)
			{
				BasicRaycast previous = null;
				for (int i = 0; i < headColliders.Count; i++)
				{
					BasicRaycast head = headColliders[i];
					if (i == 0) pointLeft = Camera.current.WorldToScreenPoint(head.WorldExtent);
					if (i == headColliders.Count - 1) pointRight = Camera.current.WorldToScreenPoint(head.WorldExtent);
					// Draw line and add handle
					if (previous != null)
					{
						Handles.color = handleColor;
						Handles.DrawLine(head.WorldExtent, previous.WorldExtent);
					}
					previous = head;
				}

				if (pointRight.x < pointLeft.x)
				{
					Vector3 tmp = pointRight; pointRight = pointLeft; pointLeft = tmp;
				}
				
				Handles.BeginGUI();
				GUI.color = handleColor;
				GUILayout.BeginArea(new Rect(pointLeft.x, Screen.height - pointLeft.y - 64, pointRight.x - pointLeft.x, 16));
				GUILayout.Label("Head", centeredLabelStyle);
				GUILayout.EndArea();
				Handles.EndGUI();
			}
		}


		/// <summary>
		/// Creates the default collider for the given type.
		/// </summary>
		/// <returns>The default collider.</returns>
		/// <param name="type">Type.</param>
		/// <param name="position">Position of feet.</param>
		virtual protected BasicRaycast CreateDefaultCollider(RaycastType type, Vector2 position)
		{
			// Create a smart foot collider
			if (type == RaycastType.FOOT)
			{
				NoAllocationSmartFeetcast foot = new NoAllocationSmartFeetcast();
				foot.Mob = (IMob)target;
				foot.RaycastType = RaycastType.FOOT;
				foot.Extent = position;
				foot.Transform = ((Character)target).transform;
				return foot;
			}
			// Create a side collider
			if ((type & RaycastType.SIDES) == type)
			{
				NoAllocationSmartSidecast side = new NoAllocationSmartSidecast();
				side.Mob = (IMob)target;
				side.RaycastType = type;
				side.Extent = position;
				side.Transform = ((Character)target).transform;
				side.Length = Mathf.Abs(position.x);
				return side;
			}
			// Create a head collider
			if (type == RaycastType.HEAD)
			{
				NoAllocationSmartHeadcast head = new NoAllocationSmartHeadcast();
				head.Mob = (IMob)target;
				head.RaycastType = RaycastType.HEAD;
				head.Extent = position;
				head.Transform = ((Character)target).transform;
				return head;
			}
			return null;
		}

		/// <summary>
		/// Ensures there are at least two feet colliders.
		/// </summary>
		virtual protected void EnsureTwoFeetColliders() 
		{
			EnsureInitialised ();

			// Esnure at least 2
			if (feetColliders == null || feetColliders.Count < 2)
			{
				Rect bounds = GetBounds(RaycastType.FOOT);
				feetColliders = new List<BasicRaycast>();
				feetColliders.Add ( CreateDefaultCollider(RaycastType.FOOT, new Vector2(bounds.xMin, bounds.yMin)));
				feetColliders.Add(CreateDefaultCollider(RaycastType.FOOT, new Vector2(bounds.xMax, bounds.yMin)));
				UpdateColliders();
			}
		}

		/// <summary>
		/// Ensures there are at least two head colliders.
		/// </summary>
		virtual protected void EnsureTwoHeadColliders() 
		{
			EnsureInitialised ();

			// Ensure at least 2
			if (headColliders == null || headColliders.Count < 2)
			{
				Rect bounds = GetBounds(RaycastType.HEAD);
				headColliders = new List<BasicRaycast>();
				headColliders.Add(CreateDefaultCollider(RaycastType.HEAD, new Vector2(bounds.xMin, bounds.yMax)));
				headColliders.Add (CreateDefaultCollider(RaycastType.HEAD, new Vector2(bounds.xMax, bounds.yMax)));
				UpdateColliders();
			}
		}

		/// <summary>
		/// Ensures the feet colliders are evenly spaced.
		/// </summary>
		virtual protected void EnsureFeetSpacing()
		{
			EnsureInitialised ();

			// Equidistant - assume feet collider list is sorted
			float horizontalSpacing = (feetColliders[feetColliders.Count - 1].Extent.x - feetColliders[0].Extent.x) / (float)(feetColliders.Count - 1);
			for (int i = 1; i < feetColliders.Count - 1; i++) 
			{
				if (feetColliders[i].Extent != new Vector2(feetColliders[0].Extent.x + horizontalSpacing * i, feetColliders[0].Extent.y))
				{
					feetColliders[i].Extent = new Vector2(feetColliders[0].Extent.x + horizontalSpacing * i, feetColliders[0].Extent.y);
					UpdateColliders();
				}
			}
		}

		
		/// <summary>
		/// Ensures the head colliders are half the size of the character.
		/// </summary>
		private void EnsureHeadLength()
		{
			EnsureInitialised ();
			EnsureTwoFeetColliders ();

			// Equidistant - assume feet collider list is sorted
			float length = (headColliders[0].Extent.y - feetColliders[0].Extent.y) / 2.0f;
			for (int i = 0; i < headColliders.Count; i++) 
			{
				if (headColliders[i].Length != length)
				{
					headColliders[i].Length = length;
					// This should be called by UpdateColldiers so doesn't itself update the colliders.
				}
			}
		}

		/// <summary>
		/// Ensures there are at least four side colliders and checks there positions relateive to head and feet.
		/// </summary>
		virtual protected void EnsureSideColliders() 
		{
			EnsureInitialised ();	
			if (leftColliders.Count < 2)
			{
				Rect bounds = GetBounds (RaycastType.SIDE_LEFT);
				leftColliders.Clear();
				leftColliders.Add ( CreateDefaultCollider(RaycastType.SIDE_LEFT, new Vector2(bounds.xMin, bounds.yMin + MinHeightOfSideRelativeToFeet)));
				leftColliders.Add ( CreateDefaultCollider(RaycastType.SIDE_LEFT,new Vector2(bounds.xMin, bounds.yMax)));
				UpdateColliders();
			}
			if (rightColliders.Count < 2) 
			{
				Rect bounds = GetBounds (RaycastType.SIDE_RIGHT);
				rightColliders.Clear();
				rightColliders.Add ( CreateDefaultCollider(RaycastType.SIDE_RIGHT, new Vector2(bounds.xMax, bounds.yMin + MinHeightOfSideRelativeToFeet)));
				rightColliders.Add ( CreateDefaultCollider(RaycastType.SIDE_RIGHT, new Vector2(bounds.xMax, bounds.yMax)));
				UpdateColliders();
			}

			// Don't change colliders once we have started the game
			if (!Application.isPlaying)
			{
				// Order by Y
				leftColliders = leftColliders.OrderBy(f => f.Extent.y).ToList ();
				rightColliders = rightColliders.OrderBy(f => f.Extent.y).ToList ();

				Vector2 cachedExtent;
				// Always have a minimum y distance between colliders
				for (int i = 1; i < leftColliders.Count; i++)
				{
					cachedExtent = leftColliders[i].Extent;
					// Check height is lower than head
					if (headColliders.Count > 0) 
					{
						if (headColliders[0] != null && leftColliders[i].Extent.y > headColliders[0].Extent.y)
						{
							leftColliders[i].Extent = new Vector2(leftColliders[i].Extent.x, headColliders[0].Extent.y);
						}
					}
					// Check height heigher than feet
					if (feetColliders.Count > 0) 
					{
						if (leftColliders[i].Extent.y < feetColliders[0].Extent.y + MinHeightOfSideRelativeToFeet)
						{
							leftColliders[i].Extent = new Vector2(leftColliders[i].Extent .x, feetColliders[0].Extent.y + MinHeightOfSideRelativeToFeet);
						}
					}
					if (leftColliders[i-1].Extent.y + MinYDistancebetweenSideColliders > leftColliders[i].Extent.y)
					{
						leftColliders[i].Extent = new Vector2(leftColliders[i].Extent.x, leftColliders[i-1].Extent.y + MinYDistancebetweenSideColliders );
					}
					if (cachedExtent != leftColliders[i].Extent) UpdateColliders();
				}
				for (int i = 1; i < rightColliders.Count; i++)
				{
					cachedExtent = rightColliders[i].Extent;
					// Check height is lower than head
					if (headColliders.Count > 0) 
					{
						
						if (headColliders[headColliders.Count - 1] != null && rightColliders[i].Extent.y > headColliders[headColliders.Count - 1].Extent.y)
						{
							rightColliders[i].Extent = new Vector2(rightColliders[i].Extent .x, headColliders[headColliders.Count - 1].Extent.y);
						}
						
					}
					// Check height heigher than feet
					if (feetColliders.Count > 0) 
					{
						if (rightColliders[i].Extent.y < feetColliders[0].Extent.y + MinHeightOfSideRelativeToFeet)
						{
							rightColliders[i].Extent = new Vector2(rightColliders[i].Extent .x, feetColliders[0].Extent.y + MinHeightOfSideRelativeToFeet);
							
						}
					}
					if (rightColliders[i-1].Extent.y + MinYDistancebetweenSideColliders > rightColliders[i].Extent.y)
					{
						rightColliders[i].Extent = new Vector2(rightColliders[i].Extent.x, rightColliders[i-1].Extent.y + MinYDistancebetweenSideColliders );

					}
					if (cachedExtent != rightColliders[i].Extent) UpdateColliders();
				}
			}
		}

		/// <summary>
		/// Draws a move handle.
		/// </summary>
		/// <returns>The new location.</returns>
		/// <param name="collider">Collider to draw the handle for.</param>
		virtual protected Vector3 DrawMoveHandle(BasicRaycast collider)
		{

			// Positions
			Vector3 pos = collider.Transform.position + (Vector3)collider.Extent;
			Vector3 guiPos = Camera.current.WorldToScreenPoint(pos);
			guiPos.y = Screen.height - guiPos.y;

			// Adjusment Handles
			Handles.color = new Color(0,0,0,0);
			Vector3 result = Handles.FreeMoveHandle(
				collider.Transform.position + (Vector3)collider.Extent, 
				Quaternion.identity,
				0.3f * HandleUtility.GetHandleSize(collider.Transform.position + (Vector3)collider.Extent),
				Vector3.zero, Handles.SphereCap) - collider.Transform.position;

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

		/// <summary>
		/// Draws a add handle.
		/// </summary>
		/// <returns>True if the handle is clicked.</returns>
		/// <param name="a">Collider to draw the handle from.</param>
		/// <param name="b">Collider to draw the handle to.</param>
		virtual protected bool DrawAddHandle(BasicRaycast a, BasicRaycast b)
		{
			// Positions
			Vector3 pos = (a.Transform.position + (Vector3)a.Extent + b.Transform.position + (Vector3)b.Extent) / 2.0f;
			Vector3 guiPos = Camera.current.WorldToScreenPoint(pos);
			guiPos.y = Screen.height - guiPos.y;

			// Button
			Handles.BeginGUI();
			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
			Rect rect = new Rect(guiPos.x - 17, guiPos.y - 52, 32, 32);
			if (rect.Contains(Event.current.mousePosition)) GUI.backgroundColor = mouseOverColor;
			GUI.Box(rect, " ", addButtonStyle);
			Handles.EndGUI();
			if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				Event.current.Use();
				return true;
			}
			else if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.Layout)
			{
				HandleUtility.AddDefaultControl(0);
			}
			return false;
		}

		/// <summary>
		/// Draws a remove handle.
		/// </summary>
		/// <returns>True if the handle is clicked.</returns>
		/// <param name="collider">Collider to draw the handle for.</param>
		virtual protected bool DrawRemoveHandle(BasicRaycast collider)
		{
			// Positions
			Vector3 pos = collider.Transform.position + (Vector3)collider.Extent;
			Vector3 guiPos = Camera.current.WorldToScreenPoint(pos);
			guiPos.y = Screen.height - guiPos.y;
			
			// Button
			Handles.BeginGUI();
			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
			Rect rect = new Rect(guiPos.x - 24, guiPos.y - 60, 48, 48);
			if (rect.Contains(Event.current.mousePosition)) GUI.backgroundColor = Color.red;
			GUI.Box(rect, " ", removeButtonStyle);
			Handles.EndGUI();
			if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				Event.current.Use();
				return true;
			}
			else if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.Layout)
			{
				HandleUtility.AddDefaultControl(0);
			}
			return false;
		}

		/// <summary>
		/// Draws a handle that shouldn't move.
		/// </summary>
		/// <returns>The new location.</returns>
		/// <param name="collider">Collider to draw the handle for.</param>
		virtual protected void DrawImmovableHandle(BasicRaycast collider)
		{
			Handles.color = handleColor;
			Handles.FreeMoveHandle(
				collider.Transform.position + (Vector3)collider.Extent, 
				Quaternion.identity,
				0.1f * HandleUtility.GetHandleSize(collider.Transform.position + (Vector3)collider.Extent),
				Vector3.zero, Handles.SphereCap);
		}

		/// <summary>
		/// Gets the bounds.
		/// </summary>
		/// <returns>The bounds as a rect in a space local to the character transform.</returns>
		virtual protected Rect GetBounds(RaycastType type)
		{
			// Collider Bounds
			if (boundaryDetectionMode == BoundaryDetectionMode.COLLIDER)
			{
				if (((Character)target).gameObject.GetComponentInChildren<Collider2D>() != null)
				{
					return GetColliderBounds(type);
				}
			}

			// Sprite bounds - will also be used as default if sprite is found
			SpriteRenderer spriteRenderer = ((Character)target).gameObject.GetComponentInChildren<SpriteRenderer>();
			if (spriteRenderer != null && spriteRenderer.sprite != null)
			{
				if (boundaryDetectionMode == BoundaryDetectionMode.SMART_SPRITE) return GetSmartSpriteBounds(type);
				else if (boundaryDetectionMode == BoundaryDetectionMode.SPRITE) return GetSpriteBounds(type);
				else if (boundaryDetectionMode == BoundaryDetectionMode.NONE) return GetSpriteBounds(type);
			}

			// Mesh bounds
			if (boundaryDetectionMode == BoundaryDetectionMode.MESH)
			{
				if (((Character)target).gameObject.GetComponentInChildren<MeshFilter>() != null)
				{
					return GetMeshBounds(type);
				}
			}

			// Default
			return new Rect(-0.5f, 0.75f, 1f, 1.5f);
		}

		/// <summary>
		/// Gets the raw sprite bounds.
		/// </summary>
		/// <returns>The smart sprite bounds.</returns>
		/// <param name="type">Type.</param>
		virtual protected Rect GetSpriteBounds(RaycastType type)
		{
			Sprite sprite = ((Character)target).gameObject.GetComponentInChildren<SpriteRenderer>().sprite;
			Transform t = ((Character)target).gameObject.GetComponentInChildren<SpriteRenderer>().transform;
			if (sprite != null) 
			{
				return new Rect(t.localPosition.x + sprite.bounds.min.x, t.localPosition.y + sprite.bounds.min.y, sprite.bounds.extents.x * 2, sprite.bounds.extents.y * 2);
			}
			// Default
			return new Rect(-0.5f, 0.75f, 1f, 1.5f);
		}

		/// <summary>
		/// Gets the sprite bounds by trying to determine where transparent pixels lie.
		/// </summary>
		/// <returns>The sprite bounds.</returns>
		/// <param name="type">The type will affect how the pixels are read. </param>
		virtual protected Rect GetSmartSpriteBounds(RaycastType type)
		{
			Sprite sprite = ((Character)target).gameObject.GetComponentInChildren<SpriteRenderer>().sprite;
			if (sprite != null) {
				string texturePath = AssetDatabase.GetAssetPath(sprite.texture);
				TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(texturePath);
				if(textureImporter.isReadable)
				{
					Transform t = ((Character)target).gameObject.GetComponentInChildren<SpriteRenderer>().transform;

					if (type == RaycastType.FOOT)
					{
						int width = Mathf.FloorToInt(sprite.texture.width);
						int height = Mathf.FloorToInt(sprite.texture.height);
						Color[] pixels = sprite.texture.GetPixels(0, 0, width / 2, height / 2);
						for (int y = 0 ; y < height / 2; y++)
						{
							for (int x = 0 ; x < width / 2; x++)
							{
								if (pixels[((width/2)*y) + x].a > 0.01f)
								{
									return new Rect(t.localPosition.x + sprite.bounds.min.x + (((float)x/(float)width) * sprite.bounds.extents.x * 2), 
									                t.localPosition.y - sprite.bounds.min.y + (((float)y/(float)width) * sprite.bounds.extents.y * 2),
									                (sprite.bounds.extents.x - ((float)x/(float)width) * sprite.bounds.extents.x * 2) * 2,
									                (sprite.bounds.extents.y - ((float)y/(float)width) * sprite.bounds.extents.y * 2) * 2);
								}
							}
						}
					}
					else if (type == RaycastType.HEAD)
					{
						int width = Mathf.FloorToInt(sprite.texture.width);
						int height = Mathf.FloorToInt(sprite.texture.height);
						Color[] pixels = sprite.texture.GetPixels(0, height / 2, width / 2, height - (height / 2));
						for (int y = height - (height / 2) - 1; y >= 0; y--)
						{
							for (int x = 0 ; x < width / 2; x++)
							{
								if (pixels[((width/2) * y) + x].a > 0.01f)
								{
									return new Rect(t.localPosition.x + sprite.bounds.min.x, 
									                t.localPosition.y - sprite.bounds.min.y + (((float)(y + height - (height / 2) - 1)/(float)width) * sprite.bounds.extents.y * 2),
									                sprite.bounds.extents.x * 2,
									                (sprite.bounds.extents.y - ((float)y/(float)width) * sprite.bounds.extents.y * 2) * 2);
								}
							}
						}
					}
					else
					{
						int width = Mathf.FloorToInt(sprite.texture.width);
						int height = Mathf.FloorToInt(sprite.texture.height);
						Color[] pixels = sprite.texture.GetPixels(0, 0, width / 2, height);
						int cx = width / 2;
						for (int y = 0 ; y < height; y++)
						{
							for (int x = 0 ; x < width / 2; x++)
							{
								if (pixels[((width/2)*y) + x].a > 0.01f)
								{
									if (x < cx) cx = x;
								}
							}
						}
						return new Rect(t.localPosition.x + sprite.bounds.min.x + (((float)cx/(float)width) * sprite.bounds.extents.x * 2), 
						                t.localPosition.y - sprite.bounds.min.y,
						                (sprite.bounds.extents.x - ((float)cx/(float)width) * sprite.bounds.extents.x * 2) * 2,
						                sprite.bounds.extents.y * 2);
					}
				}
			}
			// Default
			return new Rect(-0.5f, 0.75f, 1f, 1.5f);
		}

		/// <summary>
		/// Gets the bounds based on a child collider.
		/// </summary>
		/// <returns>The bounds.</returns>
		/// <param name="type">Type of raycast to get bounds for.</param>
		virtual protected Rect GetColliderBounds(RaycastType type)
		{
			Collider2D[] colliders = ((Character)target).gameObject.GetComponentsInChildren<Collider2D> ();
			Collider2D bestCollider = null;
			int colliderLevel = 0;
			// Try to work out which collider to use we use the following order: CharacterRef > CharacterHurtBox > Any Other Collider > CharacterHitBox
			foreach (Collider2D c in colliders)
			{
				if (colliderLevel < 1) 
				{
					bestCollider = c;
				}
				if (c.gameObject.GetComponent<CharacterHitBox>() == null)
				{
					if (colliderLevel < 2) 
					{
						colliderLevel = 2;
						bestCollider = c;
					}
					if (c.gameObject.GetComponent<CharacterReference>() != null)
					{
						colliderLevel = 3;
						bestCollider = c;
						if (colliderLevel < 4 && c.gameObject.GetComponent<CharacterHurtBox>() == null) 
						{
							colliderLevel = 4;
							bestCollider = c;
						}
					}
				}
			}
			if (bestCollider != null)
			{
				Transform t = ((Character)target).gameObject.transform;
				return new Rect( bestCollider.bounds.min.x - t.position.x, bestCollider.bounds.min.y -  t.position.y, bestCollider.bounds.size.x, bestCollider.bounds.size.y );
			}
			// Default
			return new Rect(-0.5f, 0.75f, 1f, 1.5f);
		}

		/// <summary>
		/// Gets the bounds based on a child mesh.
		/// </summary>
		/// <returns>The bounds.</returns>
		/// <param name="type">Type of raycast to get bounds for.</param>
		virtual protected Rect GetMeshBounds(RaycastType type)
		{
			// Default
			return new Rect(-0.5f, 0.75f, 1f, 1.5f);
		}

		
		/// <summary>
		/// Draws arrows showing collider direction, hit point, etc.
		/// </summary>
		virtual protected void DrawDebugColliders() {
			for (int i = 0; i < feetColliders.Count; i++)
			{
				if (feetColliders[i] != null)
					feetColliders[i].DrawGizmos((Character)target);
			}
			
			for (int i = 0; i < leftColliders.Count; i++)
			{
				if (leftColliders[i] != null)
					leftColliders[i].DrawGizmos((Character)target);
			}
			
			for (int i = 0; i < rightColliders.Count; i++)
			{
				if (rightColliders[i] != null)
					rightColliders[i].DrawGizmos((Character)target);
			}
			
			for (int i = 0; i < headColliders.Count; i++)
			{
				if (headColliders[i] != null)
					headColliders[i].DrawGizmos((Character)target);
			}
		}

		/// <summary>
		/// Draws the debug animations (which is just a console log at this stage)
		/// </summary>
		virtual protected void AddAnimationDebugger ()
		{
			if (Application.isPlaying)
			{
				AnimationBridge_DebugLog bridge = ((Character)target).GetComponentInChildren<AnimationBridge_DebugLog>();
				if (bridge == null)
				{
					((Character)target).gameObject.AddComponent<AnimationBridge_DebugLog>();
				}
				else
				{
					bridge.enabled = true;
				}
			}
		}

		/// <summary>
		/// Draws the debug animations (which is just a console log at this stage)
		/// </summary>
		virtual protected void RemoveAnimationDebugger ()
		{
			if (Application.isPlaying)
			{
				AnimationBridge_DebugLog bridge = ((Character)target).GetComponentInChildren<AnimationBridge_DebugLog>();
				if (bridge != null)
				{
					bridge.enabled = false;
				}
			}
		}

		/// <summary>
		/// Draws the debug movement.
		/// </summary>
		virtual protected void DrawDebugMovement ()
		{
			if (Application.isPlaying)
			{
				Character characterTarget = (Character)target;

				// headers
				GUILayout.BeginHorizontal ();
				GUILayout.Label("");
				GUILayout.FlexibleSpace ();
				GUILayout.Label("Current",  EditorStyles.boldLabel, GUILayout.Width( 100 ));
				GUILayout.Label("Previous",  EditorStyles.boldLabel, GUILayout.Width( 100 ));
				GUILayout.EndHorizontal ();
				GUILayout.Box("",GUILayout.ExpandWidth(true), GUILayout.Height(1));

				DrawPrefixAndValue("Movement", debugCurrentMovement, debugPreviousMovement);
				DrawPrefixAndValue(" (Class)", debugCurrentMovementType, debugPreviousMovementType);
				DrawPrefixAndValue("Velocity", characterTarget.Velocity.ToString(), characterTarget.PreviousVelocity.ToString());
				DrawPrefixAndValue("Target Rotation", characterTarget.SlopeTargetRotation.ToString(), characterTarget.PreviousRotation.ToString());
				GUILayout.Space (10);
				
				// Update states
				string movementName = "NONE";
				if (characterTarget.ActiveMovement != null) 
				{
					System.Type t = characterTarget.ActiveMovement.GetType();
					movementName = ((MovementInfo)t.GetProperty ("Info").GetValue (null, null)).Name;
					if (debugCurrentMovement != movementName || debugCurrentMovementType != t.Name)
					{
						debugPreviousMovement = debugCurrentMovement;
						debugCurrentMovement = movementName;
						debugPreviousMovementType = debugCurrentMovementType;
						debugCurrentMovementType = t.Name;
					}
				}

			}
			else
			{
				GUILayout.Label("You cant debug movements in Editor mode, press Play");
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
			GUILayout.Label(value, GUILayout.Width( 120 ));
			GUILayout.Label(previous != null ? previous : "",  GUILayout.Width( 120 ));
			GUILayout.EndHorizontal ();
		}

		#endregion
	}

	/// <summary>
	/// Boundary detection mode.
	/// </summary>
	public enum BoundaryDetectionMode
	{
		NONE,
		DEFAULT,
		SPRITE,
		SMART_SPRITE,
		MESH,
		COLLIDER
	}
}