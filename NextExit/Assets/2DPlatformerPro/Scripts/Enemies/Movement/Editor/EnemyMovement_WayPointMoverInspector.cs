using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Inspector for characters classes.
	/// </summary>
	[CustomEditor(typeof(EnemyMovement_WaypointMover), false)]
	public class EnemyMovement_WaypointMoverInspector : Editor
	{
		/// <summary>
		/// Are we editing waypoints in scene view?
		/// </summary>
		protected bool editWaypoints;

		/// <summary>
		/// If od the inspector in scene view.
		/// </summary>
		protected static int sceneViewInspectorId = -1;

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
		protected static Color handleColor = Color.red; // new Color(0.7f, 0.7f, 0.7f, 1.0f);
		
		/// <summary>
		/// The color of the button handles.
		/// </summary>
		protected static Color mouseOverColor = new Color(0.7f, 0.9f, 1.0f, 1.0f);

		void OnEnable()
		{

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
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			//if(Event.current.type == EventType.MouseMove) SceneView.RepaintAll();
			sceneViewInspectorId = this.GetInstanceID ();

			// Ensure 1 waypoint
			if (((EnemyMovement_WaypointMover)target).wayPoints == null || ((EnemyMovement_WaypointMover)target).wayPoints.Count < 2)
			{
				List<Vector2> waypoints = new List<Vector2>();
				waypoints.Add (((EnemyMovement_WaypointMover)target).transform.position);
				waypoints.Add (((EnemyMovement_WaypointMover)target).transform.position + Vector3.one);
				((EnemyMovement_WaypointMover)target).wayPoints = waypoints;
				EditorUtility.SetDirty(target);
			}

			DrawDefaultInspector ();

			GUI.color = editWaypoints ? Color.red: Color.white;
			if (GUILayout.Button("Edit Waypoints in Scene")) 
			{
				editWaypoints = !editWaypoints;
				SceneView.RepaintAll();
			}
			GUI.color = Color.white;
		}

		/// <summary>
		/// Draw the scene GUI (i.e. draw collider editors if they are active)
		/// </summary>
		void OnSceneGUI()
		{			
			if(Event.current.type == EventType.MouseMove) SceneView.RepaintAll();

			if (sceneViewInspectorId != this.GetInstanceID ()) 
			{
				// For some reason Unity creates multiple isntances and uses one to draw the scene view and another to draw the inspector
				// This is a workaround for that
				EnemyMovement_WaypointMoverInspector c = (EnemyMovement_WaypointMoverInspector) EditorUtility.InstanceIDToObject(sceneViewInspectorId);
				if (c != null) 
				{
					c.OnSceneGUI();
					return;
				}
			}
			ShowWaypoints ((EnemyMovement_WaypointMover)target);
			if (!Application.isPlaying)
			{

				if (editWaypoints)
				{
					ShowEditWaypoints(((EnemyMovement_WaypointMover)target));
				}
			}
		}

		/// <summary>
		/// Show the scene view editor for editing waypoints.
		/// </summary>
		virtual protected void ShowWaypoints(EnemyMovement_WaypointMover movement)
		{
			GUIStyle centeredLabelStyle = GUI.skin.GetStyle("Label");
			centeredLabelStyle.alignment = TextAnchor.UpperCenter;
			Handles.color = handleColor;
			for (int i = 0; i < movement.wayPoints.Count; i++)
			{
				if (i == 0 && movement.loop)
				{
					Handles.DrawLine(movement.wayPoints[movement.wayPoints.Count - 1], movement.wayPoints[i]);
					Vector2 v = movement.wayPoints[i] - movement.wayPoints[movement.wayPoints.Count - 1];
					v = Vector3.Cross (v, Vector3.forward);
					v.Normalize();
					Vector2 a1 = (Vector2)(Quaternion.Euler(0,0,-60) * (0.5f * v)) + movement.wayPoints[i];
					Vector2 a2 = (Vector2)(Quaternion.Euler(0,0,60) * (-0.5f * v)) + movement.wayPoints[i];
					Handles.DrawLine( a1, movement.wayPoints[i]);
					Handles.DrawLine( a2, movement.wayPoints[i]);
				}
				if (i != 0)
				{
					Handles.DrawLine(movement.wayPoints[i-1], movement.wayPoints[i]);


					//Handles.DrawLine( movement.wayPoints[i], a2);

					Vector2 v = movement.wayPoints[i] - movement.wayPoints[i-1];
					v = Vector3.Cross (v, Vector3.forward);
					v.Normalize();
					
					Vector2 a1 = (Vector2)(Quaternion.Euler(0,0,-60) * (0.5f * v)) + movement.wayPoints[i];
					Vector2 a2 = (Vector2)(Quaternion.Euler(0,0,60) * (-0.5f * v)) + movement.wayPoints[i];
					Handles.DrawLine( a1, movement.wayPoints[i]);
					Handles.DrawLine( a2, movement.wayPoints[i]);
				}
			}
		}

		/// <summary>
		/// Show the scene view editor for editing waypoints.
		/// </summary>
		virtual protected void ShowEditWaypoints(EnemyMovement_WaypointMover movement)
		{
			GUIStyle centeredLabelStyle = GUI.skin.GetStyle("Label");
			centeredLabelStyle.alignment = TextAnchor.UpperCenter;

			for (int i = 0; i < movement.wayPoints.Count; i++)
			{
				if (Event.current.shift && movement.wayPoints.Count > 2)
				{
					if (DrawRemoveHandle(movement.wayPoints[i]))
					{
						movement.wayPoints.RemoveAt(i);
						EditorUtility.SetDirty(movement);
						return;
					}
				}
				else
				{
					Vector2 newPosition = DrawMoveHandle(movement.wayPoints[i]);
					if (newPosition != movement.wayPoints[i])
					{
						movement.wayPoints[i] = newPosition;
						EditorUtility.SetDirty(movement);
					}
				}
				if (i > 0)
				{
					if (DrawAddHandle(movement.wayPoints[i], movement.wayPoints[i - 1]))
					{
						movement.wayPoints.Insert(i , (movement.wayPoints[i] + movement.wayPoints[i - 1]) / 2.0f);
						EditorUtility.SetDirty(movement);
					}
				}
				else if (movement.loop)
				{
					if (DrawAddHandle(movement.wayPoints[i], movement.wayPoints[movement.wayPoints.Count - 1]))
					{
						movement.wayPoints.Add((movement.wayPoints[i] + movement.wayPoints[movement.wayPoints.Count - 1]) / 2.0f);
						EditorUtility.SetDirty(movement);
					}
	
				}
			}
		}
		
		/// <summary>
		/// Draws a move handle.
		/// </summary>
		/// <returns>The new location.</returns>
		/// <param name="position">Position of waypoint.</param>
		virtual protected Vector2 DrawMoveHandle(Vector3 position)
		{
			
			// Positions
			Vector3 guiPos = Camera.current.WorldToScreenPoint(position);
			guiPos.y = Screen.height - guiPos.y;
			
			// Adjusment Handles
			Handles.color = new Color(0,0,0,0);
			Vector3 result = Handles.FreeMoveHandle(
				position, 
				Quaternion.identity,
				0.3f * HandleUtility.GetHandleSize(position),
				Vector3.zero, Handles.SphereCap);
			
			// Button
			Handles.BeginGUI();
			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
			Rect rect = new Rect(guiPos.x - 24, guiPos.y - 60, 48, 48);
			if (rect.Contains(Event.current.mousePosition)) GUI.backgroundColor = mouseOverColor;
			GUI.Box(rect, " ", moveButtonStyle);
			Handles.EndGUI();
			
			return (Vector2)result;
		}

		virtual protected bool DrawAddHandle(Vector3 a, Vector3 b)
		{
			// Positions
			Vector3 pos = (a +  b) / 2.0f;
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
		/// <param name="position">Position to draw the handle at.</param>
		virtual protected bool DrawRemoveHandle(Vector3 position)
		{
			// Positions
			Vector3 guiPos = Camera.current.WorldToScreenPoint(position);
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
		

	}
}
