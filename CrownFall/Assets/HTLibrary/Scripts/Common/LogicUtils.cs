using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class LogicUtils
	{
		//---------------------------------------
		/// <summary>
		/// 특정 GameObject의 시야 각도 내에 Target이 들어오는지 확인합니다.
		/// </summary>
		/// <param name="pObject">시야 확인 기준 GameObject</param>
		/// <param name="pTarget">시야 확인 대상</param>
		/// <param name="fMaxDistance">시야 최대 거리</param>
		/// <param name="fMaxDegree">시야 범위</param>
		static public bool CheckSight(GameObject pObject, GameObject pTarget, float fMaxDistance, float fMaxDegree)
		{
			if (pObject == null || pTarget == null)
				return false;
			
			return CheckSight(pObject.transform.position, pObject.transform.forward, pTarget.transform.position, fMaxDistance, fMaxDegree);
		}

		static public bool CheckSight(Transform pObject, Transform pTarget, float fMaxDistance, float fMaxDegree)
		{
			if (pObject == null || pTarget == null)
				return false;

			return CheckSight(pObject.position, pObject.forward, pTarget.position, fMaxDistance, fMaxDegree);
		}

		static public bool CheckSight(Vector3 vObjPos, Vector3 vForward, Vector3 vTgtPos, float fMaxDistance, float fMaxDegree)
		{
			if (IsInRange(vObjPos, vTgtPos, fMaxDistance) == false)
				return false;

			float fInnerDegree = MathUtils.GetInnerDegree(vForward, (vTgtPos - vObjPos).normalized);
			if (fInnerDegree > fMaxDegree)
				return false;

			return true;
		}

		//---------------------------------------
		/// <summary>
		/// 특정 GameObject의 시야 범위 내에 Target이 들어오는지 확인합니다.
		/// </summary>
		/// <param name="pObject">시야 확인 기준 GameObject</param>
		/// <param name="pTarget">시야 확인 대상</param>
		/// <param name="fMaxDistance">확인 범위</param>
		static public bool IsInRange(GameObject pObject, GameObject pTarget, float fRange)
		{
			if (pObject == null || pTarget == null)
				return false;

			Vector3 vObjPos = pObject.transform.position;
			Vector3 vTgtPos = pTarget.transform.position;
			if (IsInRange(vObjPos, vTgtPos, fRange))
				return false;

			return true;
		}

		/// <summary>
		/// 특정 Postion의 범위 내에 Target Position이 들어오는지 확인합니다.
		/// </summary>
		/// <param name="pObject">시야 확인 기준 Position</param>
		/// <param name="pTarget">시야 확인 대상 Position</param>
		/// <param name="fMaxDistance">확인 범위</param>
		static public bool IsInRange(Vector3 vObjPos, Vector3 vTargetPos, float fRange)
		{
			if (Vector3.Distance(vObjPos, vTargetPos) >= fRange)
				return false;

			return true;
		}


		/////////////////////////////////////////
		//---------------------------------------
		/// <summary>
		/// 해당 Position이 Navigation Mesh 내에 포함되는지 확인합니다.
		/// </summary>
		/// <param name="vPos">확인 대상 Position</param>
		/// <param name="fRadius">NavMesh 확인 범위</param>
		static public bool IsNavigationAvailable(Vector3 vPos, float fRadius, out NavMeshHit pHitInfo)
		{
			if (NavMesh.Raycast(vPos + new Vector3(fRadius, 0.0f, 0.0f), vPos + new Vector3(-fRadius, 0.0f, 0.0f), out pHitInfo, NavMesh.AllAreas))
				return false;

			if (NavMesh.Raycast(vPos + new Vector3(0.0f, 0.0f, fRadius), vPos + new Vector3(0.0f, 0.0f, -fRadius), out pHitInfo, NavMesh.AllAreas))
				return false;

			if (NavMesh.SamplePosition(vPos, out pHitInfo, fRadius, NavMesh.AllAreas) == false)
				return false;

			return true;
		}

		/////////////////////////////////////////
		//---------------------------------------
		//Find the line of intersection between two planes.	The planes are defined by a normal and a point on that plane.
		//The outputs are a point on the line and a vector which indicates it's direction. If the planes are not parallel, 
		//the function outputs true, otherwise false.
		public static bool PlanePlaneIntersection(out Vector3 linePoint, out Vector3 lineVec, Vector3 plane1Normal, Vector3 plane1Position, Vector3 plane2Normal, Vector3 plane2Position)
		{
			linePoint = Vector3.zero;
			lineVec = Vector3.zero;

			//We can get the direction of the line of intersection of the two planes by calculating the 
			//cross product of the normals of the two planes. Note that this is just a direction and the line
			//is not fixed in space yet. We need a point for that to go with the line vector.
			lineVec = Vector3.Cross(plane1Normal, plane2Normal);

			//Next is to calculate a point on the line to fix it's position in space. This is done by finding a vector from
			//the plane2 location, moving parallel to it's plane, and intersecting plane1. To prevent rounding
			//errors, this vector also has to be perpendicular to lineDirection. To get this vector, calculate
			//the cross product of the normal of plane2 and the lineDirection.		
			Vector3 ldir = Vector3.Cross(plane2Normal, lineVec);

			float denominator = Vector3.Dot(plane1Normal, ldir);

			//Prevent divide by zero and rounding errors by requiring about 5 degrees angle between the planes.
			if (Mathf.Abs(denominator) > 0.006f)
			{
				Vector3 plane1ToPlane2 = plane1Position - plane2Position;
				float t = Vector3.Dot(plane1Normal, plane1ToPlane2) / denominator;
				linePoint = plane2Position + t * ldir;

				return true;
			}
			else
				return false;
		}

		//Get the intersection between a line and a plane. 
		//If the line and plane are not parallel, the function outputs true, otherwise false.
		public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
		{
			float length;
			float dotNumerator;
			float dotDenominator;
			Vector3 vector;
			intersection = Vector3.zero;

			//calculate the distance between the linePoint and the line-plane intersection point
			dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
			dotDenominator = Vector3.Dot(lineVec, planeNormal);

			//line and plane are not parallel
			if (dotDenominator != 0.0f)
			{
				length = dotNumerator / dotDenominator;

				//create a vector from the linePoint to the intersection point
				vector = MathUtils.SetVectorLength(lineVec, length);

				//get the coordinates of the line-plane intersection point
				intersection = linePoint + vector;

				return true;
			}
			else
				return false;
		}

		//Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
		//Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
		//same plane, use ClosestPointsOnTwoLines() instead.
		public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
		{
			Vector3 lineVec3 = linePoint2 - linePoint1;
			Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
			Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

			float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

			//is coplanar, and not parrallel
			if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
			{
				float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
				intersection = linePoint1 + (lineVec1 * s);
				return true;
			}
			else
			{
				intersection = Vector3.zero;
				return false;
			}
		}

		//Two non-parallel lines which may or may not touch each other have a point on each line which are closest
		//to each other. This function finds those two points. If the lines are not parallel, the function 
		//outputs true, otherwise false.
		public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
		{
			closestPointLine1 = Vector3.zero;
			closestPointLine2 = Vector3.zero;

			float a = Vector3.Dot(lineVec1, lineVec1);
			float b = Vector3.Dot(lineVec1, lineVec2);
			float e = Vector3.Dot(lineVec2, lineVec2);

			float d = a * e - b * b;

			//lines are not parallel
			if (d != 0.0f)
			{

				Vector3 r = linePoint1 - linePoint2;
				float c = Vector3.Dot(lineVec1, r);
				float f = Vector3.Dot(lineVec2, r);

				float s = (b * f - c * e) / d;
				float t = (a * f - c * b) / d;

				closestPointLine1 = linePoint1 + lineVec1 * s;
				closestPointLine2 = linePoint2 + lineVec2 * t;

				return true;
			}
			else
				return false;
		}

		//Returns true if a line segment (made up of linePoint1 and linePoint2) is fully or partially in a rectangle
		//made up of RectA to RectD. The line segment is assumed to be on the same plane as the rectangle. If the line is 
		//not on the plane, use ProjectPointOnPlane() on linePoint1 and linePoint2 first.
		public static bool IsLineInRectangle(Vector3 linePoint1, Vector3 linePoint2, Vector3 rectA, Vector3 rectB, Vector3 rectC, Vector3 rectD)
		{

			bool pointAInside = false;
			bool pointBInside = false;

			pointAInside = IsPointInRectangle(linePoint1, rectA, rectC, rectB, rectD);

			if (!pointAInside)
				pointBInside = IsPointInRectangle(linePoint2, rectA, rectC, rectB, rectD);

			//none of the points are inside, so check if a line is crossing
			if (!pointAInside && !pointBInside)
			{
				bool lineACrossing = MathUtils.AreLineSegmentsCrossing(linePoint1, linePoint2, rectA, rectB);
				bool lineBCrossing = MathUtils.AreLineSegmentsCrossing(linePoint1, linePoint2, rectB, rectC);
				bool lineCCrossing = MathUtils.AreLineSegmentsCrossing(linePoint1, linePoint2, rectC, rectD);
				bool lineDCrossing = MathUtils.AreLineSegmentsCrossing(linePoint1, linePoint2, rectD, rectA);

				if (lineACrossing || lineBCrossing || lineCCrossing || lineDCrossing)
					return true;
				else
					return false;
			}
			else
				return true;
		}

		//Returns true if "point" is in a rectangle mad up of RectA to RectD. The line point is assumed to be on the same 
		//plane as the rectangle. If the point is not on the plane, use ProjectPointOnPlane() first.
		public static bool IsPointInRectangle(Vector3 point, Vector3 rectA, Vector3 rectC, Vector3 rectB, Vector3 rectD)
		{
			Vector3 vector;
			Vector3 linePoint;

			//get the center of the rectangle
			vector = rectC - rectA;
			float size = -(vector.magnitude / 2f);
			vector = MathUtils.AddVectorLength(vector, size);
			Vector3 middle = rectA + vector;

			Vector3 xVector = rectB - rectA;
			float width = xVector.magnitude / 2f;

			Vector3 yVector = rectD - rectA;
			float height = yVector.magnitude / 2f;

			linePoint = MathUtils.ProjectPointOnLine(middle, xVector.normalized, point);
			vector = linePoint - point;
			float yDistance = vector.magnitude;

			linePoint = MathUtils.ProjectPointOnLine(middle, yVector.normalized, point);
			vector = linePoint - point;
			float xDistance = vector.magnitude;

			if ((xDistance <= width) && (yDistance <= height))
				return true;
			else
				return false;
		}


		/////////////////////////////////////////
		//---------------------------------------
		public static bool IsPointInRect(Vector3 point, Rect rect)
		{
			if (point.x < rect.xMin || point.x > rect.xMax)
				return false;

			if (point.y < rect.yMin || point.y > rect.yMax)
				return false;

			return true;
		}

		/////////////////////////////////////////
		//---------------------------------------
	}
}
