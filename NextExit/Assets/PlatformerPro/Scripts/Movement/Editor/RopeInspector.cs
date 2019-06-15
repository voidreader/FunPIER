#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace PlatformerPro
{
	/// <summary>
	/// Custom inspector for ropes which allows you to create ropes
	/// </summary>
	[CustomEditor(typeof(Rope), false)]
	public class RopeInspector : Editor
	{
		
		/// <summary>
		/// Draw the inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			Rope myTarget = (Rope)target;
			myTarget.length = EditorGUILayout.FloatField(new GUIContent("Length","How long is the rope?"), myTarget.length);
			if (myTarget.length < 0.25f) myTarget.length = 1.0f;
			myTarget.segments = EditorGUILayout.IntField(new GUIContent("Segments","How many segments make up the rope?"), myTarget.segments);
			if (myTarget.segments < 1) myTarget.segments = 1;
			myTarget.ropeSectionPrefab = (GameObject) EditorGUILayout.ObjectField(new GUIContent("Prefab","Prefab to use for rope sections, add sprite and the like here."), myTarget.ropeSectionPrefab, typeof(GameObject), false);
			myTarget.usedFixedPosition = EditorGUILayout.Toggle(new GUIContent("Fixed Grab Position","If true the rope can't be climbed and has only one fixed grab position at the bottom of the rope."), myTarget.usedFixedPosition);
			GUILayout.Label ("Physics Details", EditorStyles.boldLabel);
			myTarget.ropeMass = EditorGUILayout.FloatField(new GUIContent("Segment Mass","The mass of each rope segment (except the last which has mass = ropeMass * 5). Heavy ropes swing less wildly."), myTarget.ropeMass);
			if (myTarget.ropeMass < 0.25f) myTarget.ropeMass = 1.0f;

			myTarget.angleLimits = EditorGUILayout.FloatField(new GUIContent("Limit Angles ","Amount rope swing angles should be hard limited."), myTarget.angleLimits);
			if (myTarget.angleLimits < 0) myTarget.angleLimits = 0;

			myTarget.angularDrag = EditorGUILayout.FloatField(new GUIContent("Angular Drag ","Amount of angular drag to apply to rope. Use this to soft limit angles."), myTarget.angularDrag);
			if (myTarget.angularDrag < 0) myTarget.angularDrag = 0;


			if (GUILayout.Button("Update Now")) 
			{
				CreateRope(myTarget);
			}
		}

		/// <summary>
		/// Create a new rope.
		/// </summary>
		/// <param name="myTarget">The rope object.</param>
		protected void CreateRope(Rope myTarget)
		{
			if (myTarget.transform.parent != null)
			{
				Debug.LogWarning ("Rope creation may not work correctly if the Rope is a child of another object.");
			}

			float segmentLength = myTarget.length / (float)myTarget.segments;
			List<GameObject> objectsToDelete = new List<GameObject> ();
			// Delete all children
			foreach (Transform t in myTarget.gameObject.transform)
			{
				objectsToDelete.Add(t.gameObject);
			}

			foreach (GameObject g in objectsToDelete)
			{
				if (g != null) GameObject.DestroyImmediate(g);
			}

			// Ensure we are scaled at 1,1,1
			myTarget.transform.localScale = Vector3.one;

			// Create Anchor
			GameObject anchor = new GameObject ();
			anchor.transform.parent = myTarget.transform;
			anchor.transform.localPosition = Vector3.zero;
			anchor.name = "RopeAnchor";
			Rigidbody2D anchorRigidbody = anchor.AddComponent<Rigidbody2D> ();
			anchorRigidbody.isKinematic = true;
			anchor.AddComponent<RopeAnchor> ();

			// Create segments
			float currentLocalYPos = -segmentLength / 2.0f;
			Rigidbody2D nextRigidbodyToConnectTo = anchorRigidbody;

			for (int i = 0; i < myTarget.segments; i++)
			{
				GameObject ropeSectionGo;
				if (myTarget.ropeSectionPrefab != null)
				{
					ropeSectionGo = (GameObject) GameObject.Instantiate(myTarget.ropeSectionPrefab);
				}
				else
				{
					ropeSectionGo = new GameObject();

				}
				ropeSectionGo.name = "RopeSection_" + i;
				ropeSectionGo.layer = myTarget.gameObject.layer;

				// Set length and position
				ropeSectionGo.transform.localScale = new Vector3(ropeSectionGo.transform.localScale.x, segmentLength, ropeSectionGo.transform.localScale.z);
				ropeSectionGo.transform.parent = myTarget.transform;
				ropeSectionGo.transform.localPosition = new Vector3(0, currentLocalYPos, 0);

				// Check Rigidbody 2D
				Rigidbody2D ropeSectionRigidbody;
				if (ropeSectionGo.GetComponent<Rigidbody2D>() == null)
				{
					ropeSectionRigidbody = ropeSectionGo.AddComponent<Rigidbody2D>();
				}
				else
				{
					ropeSectionRigidbody = ropeSectionGo.GetComponent<Rigidbody2D>();
				}
				ropeSectionRigidbody.mass = myTarget.ropeMass * (i == (myTarget.segments - 1) ? 5 : 1 );

				// Check Collider
				if (ropeSectionGo.GetComponent<Collider2D>() == null)
				{
					if (!myTarget.usedFixedPosition)
					{
						BoxCollider2D boxCollider = ropeSectionGo.AddComponent<BoxCollider2D>();
						// Default to a 0.5f wide box collider
						boxCollider.size = new Vector2(0.5f, 1.0f);
					}
				}
				if (myTarget.usedFixedPosition && i == (myTarget.segments - 1))
				{
					BoxCollider2D boxCollider = ropeSectionGo.AddComponent<BoxCollider2D>();
					// Default to a 0.5f wide box collider
					boxCollider.size = new Vector2(0.5f, 0.5f / segmentLength);
#if UNITY_5_3_OR_NEWER
					boxCollider.offset = new Vector2(0, -0.5f);
#else
					boxCollider.center = new Vector2(0, -0.5f);
#endif

				}

				// Check Hinge Joint
				HingeJoint2D hingeJoint;
				if (ropeSectionGo.GetComponent<HingeJoint2D>() == null)
				{
					hingeJoint = ropeSectionGo.AddComponent<HingeJoint2D>();
				}
				else
				{
					hingeJoint = ropeSectionGo.GetComponent<HingeJoint2D>();
				}
				hingeJoint.anchor = new Vector2(0, 0.5f);
				hingeJoint.connectedAnchor = new Vector2(0, i == 0 ? 0.0f : -0.5f);
				hingeJoint.connectedBody = nextRigidbodyToConnectTo;
				if (myTarget.angleLimits > 0)
				{
					hingeJoint.useLimits = true;
					JointAngleLimits2D limits = new JointAngleLimits2D();
					limits.min = myTarget.angleLimits;
					limits.max = -myTarget.angleLimits;
					hingeJoint.limits = limits;
				}

				// Check Rope Section
				RopeSection ropeSection;
				if (ropeSectionGo.GetComponent<RopeSection>() == null)
				{
					ropeSection = ropeSectionGo.AddComponent<RopeSection>();
				}
				else
				{
					ropeSection = ropeSectionGo.GetComponent<RopeSection>();
				}
				if (myTarget.usedFixedPosition)
				{
					if (i == (myTarget.segments - 1)) 
					{
						ropeSection.usedFixedPosition = true;
					}
					else
					{
						DestroyImmediate(ropeSection);
					}
				}

				// Update for next loop
				currentLocalYPos -= segmentLength;
				nextRigidbodyToConnectTo = ropeSectionGo.GetComponent<Rigidbody2D>();
			}
		}
	}
#endif
}
