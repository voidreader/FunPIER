using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{

	/// <summary>
	/// A trigger that only fires when you walk through in a certain direction (with a minimum speed).
	/// </summary>
	public class OneWayTrigger : Trigger
	{

		/// <summary>
		/// The height. Note that height is evaluated against the position of the
		/// characters transform (it doesn't take character height in to account).
		/// </summary>
		[Tooltip ("The height. Note that height is evaluated against the position of the characters transform (it doesn't take character height in to account).")]
		public float height;

		/// <summary>
		/// The required velocity (speed and direction). Use 'speed < 0' for right to left, and 'speed > 0' for left to right.
		/// </summary>
		[Tooltip ("The required velocity (speed and direction). Use 'speed < 0' for right to left, and 'speed > 0' for left to right.")]
		public float velocity;

		/// <summary>
		/// Characters to left.
		/// </summary>
		protected List<Character> charactersOnTriggeringSide;

		/// <summary>
		/// If we are rotated by 90 degrees the trigger is up to down or down to up, instead of left to right/right to left.
		/// </summary>
		protected bool isUpAndDownTrigger;

		/// <summary>
		/// How close before we start tracking the cahracter. Larger numbers support higher speeds, but 
		/// have more performance cost.
		/// </summary>
		protected const float Proximity = 0.5f;

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start () {
			Init();
		}
		
		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update () {
			CheckMoveThrough();
		}

		/// <summary>
		/// Initialise the sensor.
		/// </summary>
		override protected void Init()
		{
			base.Init();			
			charactersOnTriggeringSide = new List<Character>();
			if (!Mathf.Approximately(transform.eulerAngles.z, 90.0f) && transform.eulerAngles.z != 0.0f)
			{
				Debug.LogWarning("One way trigger may be rotated by 0 or 90 degrees, no other values are supported");
			}
			else if(Mathf.Approximately(transform.eulerAngles.z, 90.0f))
			{
				isUpAndDownTrigger = true;
			}
		}

//		/// <summary>
//		/// Unity OnDestroy() hook.
//		/// </summary>
//		void OnDestroy()
//		{
//			base.OnDestroy ();
//		}

		/// <summary>
		/// Checks the move through.
		/// </summary>
		virtual protected void CheckMoveThrough()
		{
			if (allCharacters != null)
			{
				for (int i = 0; i < allCharacters.Count; i++)
				{
					if (isUpAndDownTrigger)
					{
						if (!charactersOnTriggeringSide.Contains(allCharacters[i]))
						{
							// Add characters close to the triggering side
							if (velocity > 0 && allCharacters[i].transform.position.y - transform.position.y < 0 && 
							    allCharacters[i].transform.position.y - transform.position.y > -Proximity &&
							    (allCharacters[i].transform.position.x - transform.position.x) >= 0 && 
							    (allCharacters[i].transform.position.x - transform.position.x - height ) <= 0)
							{
								charactersOnTriggeringSide.Add(allCharacters[i]);
							}
							if (velocity < 0 && allCharacters[i].transform.position.y - transform.position.y > 0 && 
							    allCharacters[i].transform.position.y - transform.position.y < Proximity &&
							    (allCharacters[i].transform.position.x - transform.position.x) >= 0 && 
							    (allCharacters[i].transform.position.x - transform.position.x - height ) <= 0)
							{
								charactersOnTriggeringSide.Add(allCharacters[i]);
							}
						}
						else
						{
							// Check for cross - we can salefy ignore height because if they were in the bounds last frame
							// then they will be very close next frame
							if (velocity > 0 && allCharacters[i].transform.position.y - transform.position.y > 0) EnterTrigger(allCharacters[i]); 
							if (velocity < 0 && allCharacters[i].transform.position.y - transform.position.y < 0) EnterTrigger(allCharacters[i]);
							
							// Remove if no longer close
							if (velocity > 0 && (allCharacters[i].transform.position.y - transform.position.y >= 0 || 
							                     allCharacters[i].transform.position.y - transform.position.y <= -Proximity ||
							                     (allCharacters[i].transform.position.x - transform.position.x) < 0 || 
							                     (allCharacters[i].transform.position.x - transform.position.x - height ) >= 0))
							{
								charactersOnTriggeringSide.Remove(allCharacters[i]);
							}
							if (velocity < 0 && (allCharacters[i].transform.position.y - transform.position.y <= 0 || 
							                     allCharacters[i].transform.position.y - transform.position.y >= Proximity ||
							                     (allCharacters[i].transform.position.x - transform.position.x) < 0 || 
							                     (allCharacters[i].transform.position.x - transform.position.x - height ) >= 0))
							{
								charactersOnTriggeringSide.Remove(allCharacters[i]);
							}
						}
					}
					else
					{
						if (!charactersOnTriggeringSide.Contains(allCharacters[i]))
						{
							// Add characters close to the triggering side
							if (velocity > 0 && allCharacters[i].transform.position.x - transform.position.x < 0 && 
							    allCharacters[i].transform.position.x - transform.position.x > -Proximity &&
							    (allCharacters[i].transform.position.y - transform.position.y) >= 0 && 
							    (allCharacters[i].transform.position.y - transform.position.y - height ) <= 0)
							{
								charactersOnTriggeringSide.Add(allCharacters[i]);
							}
							if (velocity < 0 && allCharacters[i].transform.position.x - transform.position.x > 0 && 
							    allCharacters[i].transform.position.x - transform.position.x < Proximity &&
							    (allCharacters[i].transform.position.y - transform.position.y) >= 0 && 
							    (allCharacters[i].transform.position.y - transform.position.y - height ) <= 0)
							{
								charactersOnTriggeringSide.Add(allCharacters[i]);
							}
						}
						else
						{
							// Check for cross - we can salefy ignore height because if they were in the bounds last frame
							// then they will be very close next frame
							if (velocity > 0 && allCharacters[i].transform.position.x - transform.position.x > 0) EnterTrigger(allCharacters[i]); 
							if (velocity < 0 && allCharacters[i].transform.position.x - transform.position.x < 0) EnterTrigger(allCharacters[i]);

							// Remove if no longer close
							if (velocity > 0 && (allCharacters[i].transform.position.x - transform.position.x >= 0 || 
							                     allCharacters[i].transform.position.x - transform.position.x <= -Proximity ||
							                     (allCharacters[i].transform.position.y - transform.position.y) < 0 || 
							                     (allCharacters[i].transform.position.y - transform.position.y - height ) >= 0))
							{
								charactersOnTriggeringSide.Remove(allCharacters[i]);
							}
							if (velocity < 0 && (allCharacters[i].transform.position.x - transform.position.x <= 0 || 
							                     allCharacters[i].transform.position.x - transform.position.x >= Proximity ||
							                     (allCharacters[i].transform.position.y - transform.position.y) < 0 || 
							                     (allCharacters[i].transform.position.y - transform.position.y - height ) >= 0))
							{
								charactersOnTriggeringSide.Remove(allCharacters[i]);
							}
						}
					}
				}
			}
		}

#if UNITY_EDITOR

		/// <summary>
		/// Unity gizmo hook, draw the height.
		/// </summary>
		void OnDrawGizmos()
		{
			UnityEditor.Handles.color = Trigger.GizmoColor;
			Vector3[] verts = new Vector3[4];

			if (Mathf.Approximately(transform.eulerAngles.z, 90.0f))
			{
				verts[0] = transform.position + new Vector3(0, -0.05f, 0);
				verts[1] = transform.position + new Vector3(height, -0.05f,  0);
				verts[2] = transform.position + new Vector3(height, 0.05f, 0);
				verts[3] = transform.position + new Vector3(0, 0.05f, 0);
			}
			else
			{
				verts[0] = transform.position + new Vector3(-0.05f, 0, 0);
				verts[1] = transform.position + new Vector3(-0.05f, height, 0);
				verts[2] = transform.position + new Vector3( 0.05f, height, 0);
				verts[3] = transform.position + new Vector3( 0.05f, 0, 0);
			}
			UnityEditor.Handles.DrawSolidRectangleWithOutline(verts, new Color(1,0,0,0.5f), new Color(1,0,0,0.5f));

			Gizmos.color = Trigger.GizmoColor;
			if (Mathf.Approximately(transform.eulerAngles.z, 90.0f))
			{
				if (velocity > 0)
				{
					Gizmos.DrawLine(transform.position + new Vector3(height / 2.0f, -0.33f, 0), transform.position + new Vector3(height / 2.0f, -0.05f, 0)); 
					Gizmos.DrawLine(transform.position + new Vector3(height / 2.0f + 0.05f, -0.1f, 0), transform.position + new Vector3(height / 2.0f, -0.05f, 0));
					Gizmos.DrawLine(transform.position + new Vector3(height / 2.0f - 0.05f, -0.1f, 0), transform.position + new Vector3(height / 2.0f, -0.05f, 0));
					
					Gizmos.DrawLine(transform.position + new Vector3(height - 0.1f, -0.33f, 0), transform.position + new Vector3(height - 0.1f, -0.05f, 0)); 
					Gizmos.DrawLine(transform.position + new Vector3(height - 0.05f, -0.1f,  0), transform.position + new Vector3(height - 0.1f, -0.05f, 0));
					Gizmos.DrawLine(transform.position + new Vector3(height - 0.15f, -0.1f,  0), transform.position + new Vector3(height - 0.1f, -0.05f, 0));
					
					Gizmos.DrawLine(transform.position + new Vector3(0.1f, -0.33f, 0), transform.position + new Vector3(0.1f, -0.05f, 0)); 
					Gizmos.DrawLine(transform.position + new Vector3(0.15f, -0.1f, 0), transform.position + new Vector3(0.1f, -0.05f, 0));
					Gizmos.DrawLine(transform.position + new Vector3(0.05f, -0.1f, 0), transform.position + new Vector3(0.1f, -0.05f, 0));
				}
				else if (velocity < 0)
				{
					Gizmos.DrawLine(transform.position + new Vector3(height / 2.0f, 0.33f, 0), transform.position + new Vector3(height / 2.0f, 0.05f, 0)); 
					Gizmos.DrawLine(transform.position + new Vector3(height / 2.0f + 0.05f, 0.1f, 0), transform.position + new Vector3(height / 2.0f, 0.05f, 0));
					Gizmos.DrawLine(transform.position + new Vector3(height / 2.0f - 0.05f, 0.1f, 0), transform.position + new Vector3(height / 2.0f, 0.05f, 0));
					
					Gizmos.DrawLine(transform.position + new Vector3(height - 0.1f, 0.33f, 0), transform.position + new Vector3(height - 0.1f, 0.05f, 0)); 
					Gizmos.DrawLine(transform.position + new Vector3(height - 0.05f, 0.1f,  0), transform.position + new Vector3(height - 0.1f, 0.05f, 0));
					Gizmos.DrawLine(transform.position + new Vector3(height - 0.15f, 0.1f,  0), transform.position + new Vector3(height - 0.1f, 0.05f, 0));
					
					Gizmos.DrawLine(transform.position + new Vector3(0.1f, 0.33f, 0), transform.position + new Vector3(0.1f, 0.05f, 0)); 
					Gizmos.DrawLine(transform.position + new Vector3(0.15f, 0.1f, 0), transform.position + new Vector3(0.1f, 0.05f, 0));
					Gizmos.DrawLine(transform.position + new Vector3(0.05f, 0.1f, 0), transform.position + new Vector3(0.1f, 0.05f, 0));
				}
			}
			else
			{
				if (velocity > 0)
				{
					Gizmos.DrawLine(transform.position + new Vector3(-0.33f, height / 2.0f, 0), transform.position + new Vector3(-0.05f, height / 2.0f, 0)); 
					Gizmos.DrawLine(transform.position + new Vector3(-0.1f, height / 2.0f + 0.05f, 0), transform.position + new Vector3(-0.05f, height / 2.0f, 0));
					Gizmos.DrawLine(transform.position + new Vector3(-0.1f, height / 2.0f - 0.05f, 0), transform.position + new Vector3(-0.05f, height / 2.0f, 0));

					Gizmos.DrawLine(transform.position + new Vector3(-0.33f, height - 0.1f, 0), transform.position + new Vector3(-0.05f, height - 0.1f, 0)); 
					Gizmos.DrawLine(transform.position + new Vector3(-0.1f,  height - 0.05f, 0), transform.position + new Vector3(-0.05f, height - 0.1f, 0));
					Gizmos.DrawLine(transform.position + new Vector3(-0.1f,  height - 0.15f, 0), transform.position + new Vector3(-0.05f, height - 0.1f, 0));

					Gizmos.DrawLine(transform.position + new Vector3(-0.33f, 0.1f, 0), transform.position + new Vector3(-0.05f, 0.1f, 0)); 
					Gizmos.DrawLine(transform.position + new Vector3(-0.1f, 0.15f, 0), transform.position + new Vector3(-0.05f, 0.1f, 0));
					Gizmos.DrawLine(transform.position + new Vector3(-0.1f, 0.05f, 0), transform.position + new Vector3(-0.05f, 0.1f, 0));
				}
				else if (velocity < 0)
				{
					Gizmos.DrawLine(transform.position + new Vector3(0.33f, height / 2.0f, 0), transform.position + new Vector3(0.05f, height / 2.0f, 0)); 
					Gizmos.DrawLine(transform.position + new Vector3(0.1f, height / 2.0f + 0.05f, 0), transform.position + new Vector3(0.05f, height / 2.0f, 0));
					Gizmos.DrawLine(transform.position + new Vector3(0.1f, height / 2.0f - 0.05f, 0), transform.position + new Vector3(0.05f, height / 2.0f, 0));
					
					Gizmos.DrawLine(transform.position + new Vector3(0.33f, height - 0.1f, 0), transform.position + new Vector3(0.05f, height - 0.1f, 0)); 
					Gizmos.DrawLine(transform.position + new Vector3(0.1f,  height - 0.05f, 0), transform.position + new Vector3(0.05f, height - 0.1f, 0));
					Gizmos.DrawLine(transform.position + new Vector3(0.1f,  height - 0.15f, 0), transform.position + new Vector3(0.05f, height - 0.1f, 0));
					
					Gizmos.DrawLine(transform.position + new Vector3(0.33f, 0.1f, 0), transform.position + new Vector3(0.05f, 0.1f, 0)); 
					Gizmos.DrawLine(transform.position + new Vector3(0.1f, 0.15f, 0), transform.position + new Vector3(0.05f, 0.1f, 0));
					Gizmos.DrawLine(transform.position + new Vector3(0.1f, 0.05f, 0), transform.position + new Vector3(0.05f, 0.1f, 0));
				}
			}
			
			if (receivers != null)
			{
				Gizmos.color = Trigger.GizmoColor;
				foreach (TriggerTarget receiver in receivers) 
				{
					if (receiver != null && receiver.receiver != null) Gizmos.DrawLine(transform.position + new Vector3(0, (height/2), 0), receiver.receiver.transform.position);
				}
			}
		}
#endif

	}
}