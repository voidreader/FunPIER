#if UNITY_EDITOR
using UnityEditor;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{
	
	/// <summary>
	/// Inspector for sequence driven enemies.
	/// </summary>
	[CustomEditor(typeof(SequenceDrivenEnemy), true)]
	public class SequenceDrivenEnemyInspector : EnemyInspector
	{
		/// <summary>
		/// We use a serialised propety for layer mask so we can draw a layer mask selector (cheeky).
		/// </summary>
		protected SerializedProperty sightLayerMask;

		/// <summary>
		/// We use a serialised propety for layer mask so we can draw a layer mask selector (cheeky).
		/// </summary>
		protected SerializedProperty hearingLayerMask;

		/// <summary>
		/// Show basic fold out.
		/// </summary>
		protected bool foldOut;
		
		/// <summary>
		/// Show sequence fold out.
		/// </summary>
		protected bool sequenceFoldOut;

		/// <summary>
		/// Currently selected phase.
		/// </summary>
		public int currentPhase;
		
		/// <summary>
		/// State of each state foldout.
		/// </summary>
		protected List<bool> foldOutState;

		protected List<bool> gotoFoldOutStates;

		/// <summary>
		/// Unity enable hook.
		/// </summary>
		override protected void OnEnable()
		{
			sightLayerMask = serializedObject.FindProperty("sightLayers");
			hearingLayerMask = serializedObject.FindProperty("hearingLayers");

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
			// Unity says we don't need to do this, but if we don't do this then the automatic serialised object updates get the same name as the last object recorded
			Undo.FlushUndoRecordObjects ();
			Undo.RecordObject (target, "Sequenced Enemy Update");

			EditorGUILayout.HelpBox("The sequence driven enemy allows you to specify a sequence of actions (with optional loops) and is well suited to bosses.", MessageType.Info, true);
			GUILayout.Space (10);
			foldOut = EditorGUILayout.Foldout(foldOut, "Basic Settings");
			if (foldOut)
			{
				DrawDefaultInspector ();
				DrawFallInspector ((Enemy)target);
				DrawSenseInspector ((SequenceDrivenEnemy)target);
			}
			sequenceFoldOut = EditorGUILayout.Foldout(sequenceFoldOut, "Sequence");
			if (!Application.isPlaying)
			{
				if (sequenceFoldOut)
				{
					DrawSequenceInspector ((SequenceDrivenEnemy) target);
				}
			}
			else
			{
				DrawPlayModeSequenceInspector ((SequenceDrivenEnemy) target);
			}
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
		/// Draws the senses inspector.
		/// </summary>
		virtual protected void DrawSenseInspector(SequenceDrivenEnemy enemy)
		{
			Undo.RecordObject(enemy, "Enemy Update");
			GUILayout.Space (10);
			GUI.color = Color.white;
			GUILayout.Label( "Senses", EditorStyles.boldLabel);
			
			float newSightDistance = EditorGUILayout.FloatField (new GUIContent ("Sight Distance", "Distance the enemy can see."), enemy.sightDistance);
			if (newSightDistance != enemy.sightDistance)
			{
				enemy.sightDistance = newSightDistance;
			}
			if (enemy.sightDistance > 0) 
			{
				float sightYOffset = EditorGUILayout.FloatField (new GUIContent ("Sight Y Offset", "Y position of the characters 'eyes'."), enemy.sightYOffset);
				if (sightYOffset != enemy.sightYOffset)
				{
					enemy.sightYOffset = sightYOffset;
				}
				EditorGUILayout.PropertyField(sightLayerMask, new GUIContent ("Sight Layers", "Layers to check for obstacle and characters. The enemy will be be able to 'see through' anything not in this layer mask."));
			}
			float newProximityRadius = EditorGUILayout.FloatField (new GUIContent ("Hearing Distance", "Radius the enemy can hear/sense ."), enemy.hearingRadius);
			if (newProximityRadius != enemy.hearingRadius)
			{
				enemy.hearingRadius = newProximityRadius;
			}
			if (enemy.hearingRadius > 0) 
			{
				Vector2 hearingOffset = EditorGUILayout.Vector2Field (new GUIContent ("Hearing Offset", "Vector offset position for hearing/proximity."), enemy.hearingOffset);
				if (hearingOffset != enemy.hearingOffset)
				{
					enemy.hearingOffset = hearingOffset;
				}
				EditorGUILayout.PropertyField(hearingLayerMask, new GUIContent ("Hearing Layers", "Layers to check for characters when listening/proximity sensing."));
			}
			// TODO Add some different target losing options
			if (enemy.sightDistance > 0 || enemy.hearingRadius > 0)
			{
				float maxTargetDistance = EditorGUILayout.FloatField (new GUIContent ("Max Target Distance", "Once targetted how far away the target needs to be before it is lost."), enemy.maxTargetDistance);
				// Ensure this is bigger than the sight and hearing radius
				if (maxTargetDistance < enemy.sightDistance) maxTargetDistance = enemy.sightDistance;
				if (maxTargetDistance < enemy.hearingRadius) maxTargetDistance = enemy.hearingRadius;
				if (maxTargetDistance != enemy.maxTargetDistance)
				{
					enemy.maxTargetDistance = maxTargetDistance;
				}
			}
		}
		
		/// <summary>
		/// Draws the sequence inspector.
		/// </summary>
		virtual protected void DrawSequenceInspector(SequenceDrivenEnemy enemy)
		{
			if (enemy.phaseInfo == null || enemy.phaseInfo.Count < 1)
			{
				enemy.phaseInfo = new List<EnemyPhase>();
				EnemyPhase phase = new EnemyPhase("New Phase");
				EnemyStateInfo newState = new EnemyStateInfo();
				newState.stateName = "New State";
				newState.gotoState = -1;
				newState.gotoPhase = -1;
				phase.stateInfo.Add (newState);
				enemy.phaseInfo.Add (phase);
			}
			
			Undo.RecordObject(enemy, "Enemy Update");
			if (currentPhase >= enemy.phaseInfo.Count) currentPhase = -1;
			
			EditorGUI.indentLevel++;
			
			for (int i = 0; i < enemy.phaseInfo.Count; i++)
			{
				DrawPhaseInspector(enemy, enemy.phaseInfo[i], i);
			}

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(new GUIContent("Add Phase", "Adds a new phase"), EditorStyles.miniButton))
			{
				EnemyPhase phase = new EnemyPhase("New Phase");
				EnemyStateInfo newState = new EnemyStateInfo();
				newState.stateName = "New State";
				newState.gotoState = -1;
				newState.gotoPhase = -1;
				phase.stateInfo.Add (newState);
				enemy.phaseInfo.Add (phase);
			}
			GUILayout.EndHorizontal();

			GUILayout.FlexibleSpace();

			// DrawGlobalGotoInspector (enemy);

			EditorGUI.indentLevel--;
		}
		
		/// <summary>
		/// Draws the sequence inspector.
		/// </summary>
		virtual protected void DrawPhaseInspector(SequenceDrivenEnemy enemy, EnemyPhase phase, int index)
		{

			GUI.color = Color.white;
			GUILayout.BeginVertical(EditorStyles.textArea);

			if (phase.stateInfo == null) phase.stateInfo = new List<EnemyStateInfo> ();

			if (GUILayout.Button(new GUIContent (phase.name)))
			{
				if (currentPhase == index) currentPhase = -1;
				else currentPhase = index;
			}

			EditorGUI.indentLevel++;
			
			if (currentPhase == index) 
			{
				if (foldOutState == null)
				{
					foldOutState = new List<bool>(new bool[phase.stateInfo.Count]);
				}
				else if (foldOutState.Count != phase.stateInfo.Count)
				{
					while (foldOutState.Count < phase.stateInfo.Count)
					{
						foldOutState.Add (false);
					}
				}
				
				GUILayout.Label( "Properties", EditorStyles.boldLabel);
				string newPhaseName = EditorGUILayout.TextField (new GUIContent ("Name", "Phase name."), phase.name);
				if (newPhaseName != phase.name)
				{
					phase.name = newPhaseName;
				}
				
				GUILayout.Label( "States", EditorStyles.boldLabel);
				for (int i = 0; i < phase.stateInfo.Count; i++)
				{
					DrawStateInfo(enemy, phase, phase.stateInfo[i], index, i);
				}
				
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(new GUIContent("Add State", "Adds a new state"), EditorStyles.miniButton))
				{
					EnemyStateInfo newState = new EnemyStateInfo();
					newState.stateName = "New State";
					newState.gotoState = -1;
					newState.gotoPhase = -1;
					phase.stateInfo.Add (newState);
				}
				if (GUILayout.Button(new GUIContent("Add Goto", "Adds a new goto"), EditorStyles.miniButton))
				{
					EnemyStateInfo newState = new EnemyStateInfo();
					newState.stateName = "New Goto";
					newState.gotoState = 0;
					newState.gotoPhase = index;
					phase.stateInfo.Add (newState);
				}
				GUILayout.EndHorizontal();
				
				GUILayout.Label( "Exit", EditorStyles.boldLabel);
				EnemyPhaseExitType exitType = (EnemyPhaseExitType) EditorGUILayout.EnumPopup (new GUIContent ("Exit Type", "How we decide to exit this phase."), phase.exitType);
				if (exitType != phase.exitType)
				{
					phase.exitType = exitType;
				}
				float exitSupportingData;
				float exitSupportingDataAlt;
				switch (phase.exitType)
				{
				case EnemyPhaseExitType.TIMER:
					exitSupportingData = EditorGUILayout.FloatField (new GUIContent ("Exit Time", "Time to spend in state before exiting."), phase.exitSupportingData);
					if (exitSupportingData != phase.exitSupportingData)
					{
						phase.exitSupportingData = exitSupportingData;
					}
					break;
				case EnemyPhaseExitType.TIMER_PLUS_RANDOM:
					exitSupportingData = EditorGUILayout.FloatField (new GUIContent ("Exit Time", "Time to spend in state before exiting."), phase.exitSupportingData);
					if (exitSupportingData != phase.exitSupportingData)
					{
						phase.exitSupportingData = exitSupportingData;
					}
					int altAsInt = (int)phase.exitSupportingDataAlt;
					altAsInt = EditorGUILayout.IntSlider(new GUIContent ("Random Chance", "Chance that we will exit state after timer is expired."), altAsInt, 0, 100);
					exitSupportingDataAlt = (float)altAsInt;
					if (exitSupportingDataAlt != phase.exitSupportingDataAlt)
					{
						phase.exitSupportingDataAlt = exitSupportingDataAlt;
					}
					break;
				case EnemyPhaseExitType.HEALTH_PERCENTAGE:
					int healthAsInt = (int)(phase.exitSupportingData * 100.0f);
					healthAsInt = EditorGUILayout.IntSlider(new GUIContent ("Percentage", "Health percentage, state will exit when health is below this."), healthAsInt, 0, 100);
					exitSupportingData = ((float)healthAsInt) / 100.0f;
					if (exitSupportingData != phase.exitSupportingData)
					{
						phase.exitSupportingData = exitSupportingData;
					}
					break;
				case EnemyPhaseExitType.TARGET_WITHIN_RANGE:
					exitSupportingData = EditorGUILayout.FloatField (new GUIContent ("Range", "Range target must be within."), phase.exitSupportingData);
					if (exitSupportingData != phase.exitSupportingData)
					{
						phase.exitSupportingData = exitSupportingData;
					}
					exitSupportingDataAlt = (EditorGUILayout.Toggle (new GUIContent ("Must Be Visible", "If true enemy must have a clear line of sight to the target."), phase.exitSupportingDataAlt == 1.0f) ? 1.0f : 0.0f);
					if (exitSupportingDataAlt != phase.exitSupportingDataAlt)
					{
						phase.exitSupportingDataAlt = exitSupportingDataAlt;
					}
					break;
				case EnemyPhaseExitType.NUMBER_OF_HITS:
					exitSupportingData = (float)EditorGUILayout.IntField (new GUIContent ("Number of Hits", "Number of hits before exiting to the next phase"), (int)phase.exitSupportingData);
					if (exitSupportingData != phase.exitSupportingData)
					{
						phase.exitSupportingData = exitSupportingData;
					}
					DamageType exitSupportingDamageType = (DamageType) EditorGUILayout.EnumPopup(new GUIContent ("Damage Type", "If not set to none, only damage of this type will count towards number of hits."), phase.exitSupportingDamageType);
					if (exitSupportingDamageType != phase.exitSupportingDamageType)
					{
						phase.exitSupportingDamageType = exitSupportingDamageType;
					}
					break;	
				case EnemyPhaseExitType.NUMBER_OF_LOOPS:
					exitSupportingData = (float)EditorGUILayout.IntField (new GUIContent ("Number of Loops", "Number of loops before exiting to the next phase"), (int)phase.exitSupportingData);
					if (exitSupportingData != phase.exitSupportingData)
					{
						phase.exitSupportingData = exitSupportingData;
					}
					break;	
				}
				
				GUILayout.Space (4);
				
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (index == 0) GUI.enabled = false;
				if (GUILayout.Button(new GUIContent("Move Up", "Move this phase up."), EditorStyles.miniButton))
				{
					enemy.phaseInfo[index] = enemy.phaseInfo[index - 1];
					enemy.phaseInfo[index - 1] = phase;
				}
				GUI.enabled = true;
				if (index >= enemy.phaseInfo.Count - 1) GUI.enabled = false;
				if (GUILayout.Button(new GUIContent("Move Down", "Move this phase down."), EditorStyles.miniButton))
				{
					enemy.phaseInfo[index] = enemy.phaseInfo[index + 1];
					enemy.phaseInfo[index + 1] = phase;
				}
				GUI.enabled = true;
				if (enemy.phaseInfo.Count <= 1) GUI.enabled = false;
				if (GUILayout.Button(new GUIContent("Delete", "Delete this phase."), EditorStyles.miniButton))
				{
					enemy.phaseInfo.Remove(phase);
				}
				GUI.enabled = true;
				GUILayout.EndHorizontal();
			}

			EditorGUI.indentLevel--;

			GUILayout.EndVertical ();

			GUILayout.Space (4);


		}

		/// <summary>
		/// Draws the sequence inspector.
		/// </summary>
		virtual protected void DrawGlobalGotoInspector(SequenceDrivenEnemy enemy)
		{
			
			GUI.color = Color.white;
			GUILayout.BeginVertical(EditorStyles.textArea);
			EditorGUI.indentLevel++;

			if (enemy.globalGotos == null) enemy.globalGotos = new List<EnemyStateInfo> ();
			if (gotoFoldOutStates == null)
			{
				gotoFoldOutStates = new List<bool>(new bool[enemy.globalGotos.Count]);
			}
			else if (foldOutState.Count != enemy.globalGotos.Count)
			{
				while (foldOutState.Count < enemy.globalGotos.Count)
				{
					foldOutState.Add (false);
				}
			}

				
			GUILayout.Label( "Global Gotos", EditorStyles.boldLabel);
			for (int i = 0; i < enemy.globalGotos.Count; i++)
			{
				DrawStateInfo(enemy, null, enemy.globalGotos[i], 0, i);
			}
				
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(new GUIContent("Add Goto", "Adds a new goto"), EditorStyles.miniButton))
			{
				EnemyStateInfo newState = new EnemyStateInfo();
				newState.stateName = "New Goto";
				newState.gotoState = 0;
				newState.gotoPhase = 0;
				enemy.globalGotos.Add (newState);
			}
			GUILayout.EndHorizontal();

			GUILayout.Space (4);

			EditorGUI.indentLevel--;

			GUILayout.EndVertical();

			GUILayout.Space (4);
			
			
		}

		virtual protected void DrawStateInfo(SequenceDrivenEnemy enemy, EnemyPhase phase, EnemyStateInfo info, int phaseIndex, int index)
		{
			GUI.color = (info.gotoState >= 0) ? Color.yellow : (info.assignedMovement == null ) ? Color.red : Color.green;
			GUILayout.BeginVertical (EditorStyles.textArea);
			GUI.color = Color.white;

			if (phase == null)
			{
				gotoFoldOutStates [index] = EditorGUILayout.Foldout (gotoFoldOutStates [index], new GUIContent (info.stateName));
			}
			else
			{
				foldOutState [index] = EditorGUILayout.Foldout (foldOutState [index], new GUIContent (info.stateName));
			}
			
			if ((phase == null && gotoFoldOutStates[index] ) || (phase != null && foldOutState[index]))
			{
				
				string newStateName = EditorGUILayout.TextField (new GUIContent ("Name", "State name."), info.stateName);
				if (newStateName != info.stateName)
				{
					info.stateName = newStateName;
				}
				
				if (info.gotoState >= 0)
				{
					int gotoPhase = EditorGUILayout.IntField (new GUIContent ("Go To Phase", "State we go back to until exit is reached."), info.gotoPhase);
					if (gotoPhase >= 0 && gotoPhase < enemy.phaseInfo.Count)
					{
						if (gotoPhase != info.gotoPhase)
						{
							info.gotoPhase = gotoPhase;
						}
					}
					int gotoState = EditorGUILayout.IntField (new GUIContent ("Go To State", "State we go back to until exit is reached."), info.gotoState);
					if (gotoState >= 0 && (info.gotoPhase != phaseIndex || gotoState != index) && gotoState < enemy.phaseInfo[info.gotoPhase].stateInfo.Count)
					{
						if (gotoState != info.gotoState)
						{
							info.gotoState = gotoState;
						}
					}

					EnemyStateExitType exitType = (EnemyStateExitType) EditorGUILayout.EnumPopup (new GUIContent ("Go To when...", "What condition causes us to run this Go To."), info.exitType);
					if (exitType != info.exitType)
					{
						info.exitType = exitType;
					}
					if (info.exitType == EnemyStateExitType.NONE) EditorGUILayout.HelpBox("The type NONE means this goto will never execute", MessageType.Warning);
				}
				else
				{
					EnemyMovement newAssignedMovement = (EnemyMovement) EditorGUILayout.ObjectField(new GUIContent ("Movement", "Assocaited enemy movement."),  info.assignedMovement, typeof (EnemyMovement), true);
					if (newAssignedMovement != info.assignedMovement)
					{
						info.assignedMovement = newAssignedMovement;
					}
					EditorGUILayout.HelpBox(info.assignedMovement == null ? "No movement set" : info.assignedMovement.GetType().Name, MessageType.None);

					EnemyStateExitType exitType = (EnemyStateExitType) EditorGUILayout.EnumPopup (new GUIContent ("Exit Type", "How we decide to exit this state."), info.exitType);
					if (exitType != info.exitType)
					{
						info.exitType = exitType;
					}
					if (info.exitType == EnemyStateExitType.ALWAYS) EditorGUILayout.HelpBox("The type ALWAYS is mainly meant for GOTO states. In this case it means this state will always exit instantly.", MessageType.Warning);
				}

				float exitSupportingData;
				float exitSupportingDataAlt;
				switch (info.exitType)
				{
				case EnemyStateExitType.TIMER:
					exitSupportingData = EditorGUILayout.FloatField (new GUIContent ("Exit Time", "Time to spend in state before exiting."), info.exitSupportingData);
					if (exitSupportingData != info.exitSupportingData)
					{
						info.exitSupportingData = exitSupportingData;
					}
					break;
				case EnemyStateExitType.TIMER_PLUS_RANDOM:
					exitSupportingData = EditorGUILayout.FloatField (new GUIContent ("Exit Time", "Time to spend in state before exiting."), info.exitSupportingData);
					if (exitSupportingData != info.exitSupportingData)
					{
						info.exitSupportingData = exitSupportingData;
					}
					int altAsInt = (int)info.exitSupportingDataAlt;
					altAsInt = EditorGUILayout.IntSlider(new GUIContent ("Random Chance", "Chance that we will exit state after timer is expired."), altAsInt, 0, 100);
					exitSupportingDataAlt = (float)altAsInt;
					if (exitSupportingDataAlt != info.exitSupportingDataAlt)
					{
						info.exitSupportingDataAlt = exitSupportingDataAlt;
					}
					break;
				case EnemyStateExitType.HEALTH_PERCENTAGE:
					int healthAsInt = (int)(info.exitSupportingData * 100.0f);
					healthAsInt = EditorGUILayout.IntSlider(new GUIContent ("Percentage", "Health percentage, state will exit when health is below this."), healthAsInt, 0, 100);
					exitSupportingData = ((float)healthAsInt) / 100.0f;
					if (exitSupportingData != info.exitSupportingData)
					{
						info.exitSupportingData = exitSupportingData;
					}
					break;
				case EnemyStateExitType.TARGET_WITHIN_RANGE:
					exitSupportingData = EditorGUILayout.FloatField (new GUIContent ("Range", "Range target must be within."), info.exitSupportingData);
					if (exitSupportingData != info.exitSupportingData)
					{
						info.exitSupportingData = exitSupportingData;
					}
					exitSupportingDataAlt = (EditorGUILayout.Toggle (new GUIContent ("Must Be Visible", "If true enemy must have a clear line of sight to the target."), info.exitSupportingData == 1.0f) ? 1.0f : 0.0f);
					if (exitSupportingDataAlt != info.exitSupportingDataAlt)
					{
						info.exitSupportingDataAlt = exitSupportingDataAlt;
					}
					break;
				case EnemyStateExitType.NUMBER_OF_HITS:
					exitSupportingData = (float)EditorGUILayout.IntField (new GUIContent ("Number of Hits", "Number of hits before exiting to the next state."), (int)info.exitSupportingData);
					if (exitSupportingData != info.exitSupportingData)
					{
						info.exitSupportingData = exitSupportingData;
					}
					DamageType exitSupportingDamageType = (DamageType) EditorGUILayout.EnumPopup(new GUIContent ("Damage Type", "If not set to NONE, only damage of this type will count towards number of hits."), info.exitSupportingDamageType);
					if (exitSupportingDamageType != info.exitSupportingDamageType)
					{
						info.exitSupportingDamageType = exitSupportingDamageType;
					}
					break;		
				}
				
				GUILayout.Space (4);

				if (phase != null)
				{
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (index == 0) GUI.enabled = false;
					if (GUILayout.Button(new GUIContent("Move Up", "Move this state up."), EditorStyles.miniButton))
					{
						phase.stateInfo[index] = phase.stateInfo[index - 1];
						phase.stateInfo[index - 1] = info;
					}
					GUI.enabled = true;
					if (index >= phase.stateInfo.Count - 1) GUI.enabled = false;
					if (GUILayout.Button(new GUIContent("Move Down", "Move this state down."), EditorStyles.miniButton))
					{
						phase.stateInfo[index] = phase.stateInfo[index + 1];
						phase.stateInfo[index + 1] = info;
					}
					GUI.enabled = true;
					if (phase.stateInfo.Count <= 1) GUI.enabled = false;
					if (GUILayout.Button(new GUIContent("Delete", "Delete this state."), EditorStyles.miniButton))
					{
						phase.stateInfo.Remove(info);
					}
					GUI.enabled = true;
					GUILayout.EndHorizontal();
				}
				else
				{
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (index == 0) GUI.enabled = false;
					if (GUILayout.Button(new GUIContent("Move Up", "Move this state up."), EditorStyles.miniButton))
					{
						enemy.globalGotos[index] = enemy.globalGotos[index - 1];
						enemy.globalGotos[index - 1] = info;
					}
					GUI.enabled = true;
					if (index >= enemy.globalGotos.Count - 1) GUI.enabled = false;
					if (GUILayout.Button(new GUIContent("Move Down", "Move this state down."), EditorStyles.miniButton))
					{
						enemy.globalGotos[index] = enemy.globalGotos[index + 1];
						enemy.globalGotos[index + 1] = info;
					}
					GUI.enabled = true;
					if (enemy.globalGotos.Count <= 1) GUI.enabled = false;
					if (GUILayout.Button(new GUIContent("Delete", "Delete this state."), EditorStyles.miniButton))
					{
						enemy.globalGotos.Remove(info);
					}
					GUI.enabled = true;
					GUILayout.EndHorizontal();
				}
			}

			GUILayout.EndVertical ();

			GUILayout.Space (4);
			
		}
		
		/// <summary>
		/// Draws the play mode sequence inspector.
		/// </summary>
		virtual protected void DrawPlayModeSequenceInspector(SequenceDrivenEnemy enemy)
		{
			currentPhase = enemy.CurrentPhase;
			
			EditorGUI.indentLevel++;
			
			for (int i = 0; i < enemy.phaseInfo.Count; i++)
			{
				DrawPlayModePhaseInspector(enemy, enemy.phaseInfo[i], i);
			}
			
			EditorGUI.indentLevel--;
		}
		
		/// <summary>
		/// Draws the sequence inspector.
		/// </summary>
		virtual protected void DrawPlayModePhaseInspector(SequenceDrivenEnemy enemy, EnemyPhase phase, int index)
		{

			GUILayout.BeginVertical (EditorStyles.textArea);
			GUI.color = (index == enemy.CurrentPhase) ? Color.green : Color.white;
			GUILayout.Button (new GUIContent (phase.name));			
			GUI.color = Color.white;
			
			EditorGUI.indentLevel++;
			
			if (currentPhase == index) 
			{
				if (foldOutState == null || foldOutState.Count != phase.stateInfo.Count)
				{
					foldOutState = new List<bool>(new bool[phase.stateInfo.Count]);
				}
				
				for (int i = 0; i < phase.stateInfo.Count; i++)
				{
					DrawPlayModeStateInfo(enemy, phase, phase.stateInfo[i], i);
				}
			}
			EditorGUI.indentLevel--;
			GUILayout.EndVertical ();
			GUILayout.Space (4);
		}
		
		virtual protected void DrawPlayModeStateInfo(SequenceDrivenEnemy enemy, EnemyPhase phase, EnemyStateInfo info, int index)
		{
			GUI.color = (index == enemy.CurrentState) ? Color.green : Color.white;
			GUILayout.BeginVertical (EditorStyles.textArea);
			GUI.color = Color.white;

			EditorGUILayout.Foldout(foldOutState[index], new GUIContent (info.stateName));

			GUILayout.EndVertical ();
			
			GUILayout.Space (4);

		}
		
		
	}
	
}
#endif