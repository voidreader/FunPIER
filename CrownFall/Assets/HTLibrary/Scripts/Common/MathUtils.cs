using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class MathUtils
	{
		//---------------------------------------
		public const float Deg2Rad = 0.0174532924F;
		public const float Rad2Deg = 57.29578F;

		public const float GRAVITY = 9.8f;
		public const float GRAVITY_X2 = GRAVITY * 2;
		public const float GRAVITY_HALF = GRAVITY * 0.5f;

		//---------------------------------------
		public const float PI = Mathf.PI;
		public const float _2PI = PI * 2;
		public const float _HALF_PI = PI * 0.5f;


		/////////////////////////////////////////
		//---------------------------------------
		/// <summary>
		/// Vector의 Right Vector를 구합니다.
		/// </summary>
		static public Vector3 GetVectorRight(Vector3 vFront)
		{
			return new Vector3(vFront.z, vFront.y, -vFront.x);
		}

		/// <summary>
		/// Vector의 Left Vector를 구합니다.
		/// </summary>
		static public Vector3 GetVectorLeft(Vector3 vFront)
		{
			return new Vector3(-vFront.z, vFront.y, vFront.x);
		}

		static public float GetInnerDegree(Vector3 vVec1, Vector3 vVec2)
		{
			float fInner = Vector3.Dot(vVec1, vVec2);
			return 180.0f - (fInner * 180.0f);
		}


		/////////////////////////////////////////
		//---------------------------------------
		private static Transform tempChild = null;
		private static Transform tempParent = null;

		private static Vector3[] positionRegister;
		private static float[] posTimeRegister;
		private static int positionSamplesTaken = 0;

		private static Quaternion[] rotationRegister;
		private static float[] rotTimeRegister;
		private static int rotationSamplesTaken = 0;

		public static void Init()
		{
			tempChild = (new GameObject("Math3d_TempChild")).transform;
			tempParent = (new GameObject("Math3d_TempParent")).transform;

			tempChild.gameObject.hideFlags = HideFlags.HideAndDontSave;
			MonoBehaviour.DontDestroyOnLoad(tempChild.gameObject);

			tempParent.gameObject.hideFlags = HideFlags.HideAndDontSave;
			MonoBehaviour.DontDestroyOnLoad(tempParent.gameObject);

			//set the parent
			tempChild.parent = tempParent;
		}


		//increase or decrease the length of vector by size
		public static Vector3 AddVectorLength(Vector3 vector, float size)
		{
			//get the vector length
			float magnitude = Vector3.Magnitude(vector);

			//calculate new vector length
			float newMagnitude = magnitude + size;

			//calculate the ratio of the new length to the old length
			float scale = newMagnitude / magnitude;

			//scale the vector
			return vector * scale;
		}

		//create a vector of direction "vector" with length "size"
		public static Vector3 SetVectorLength(Vector3 vector, float size)
		{
			//normalize the vector
			Vector3 vectorNormalized = Vector3.Normalize(vector);

			//scale the vector
			return vectorNormalized *= size;
		}


		//caclulate the rotational difference from A to B
		public static Quaternion SubtractRotation(Quaternion B, Quaternion A)
		{
			Quaternion C = Quaternion.Inverse(A) * B;
			return C;
		}

		//Add rotation B to rotation A.
		public static Quaternion AddRotation(Quaternion A, Quaternion B)
		{
			Quaternion C = A * B;
			return C;
		}

		//Same as the build in TransformDirection(), but using a rotation instead of a transform.
		public static Vector3 TransformDirectionMath(Quaternion rotation, Vector3 vector)
		{
			Vector3 output = rotation * vector;
			return output;
		}

		//Same as the build in InverseTransformDirection(), but using a rotation instead of a transform.
		public static Vector3 InverseTransformDirectionMath(Quaternion rotation, Vector3 vector)
		{
			Vector3 output = Quaternion.Inverse(rotation) * vector;
			return output;
		}

		//Rotate a vector as if it is attached to an object with rotation "from", which is then rotated to rotation "to".
		//Similar to TransformWithParent(), but rotating a vector instead of a transform.
		public static Vector3 RotateVectorFromTo(Quaternion from, Quaternion to, Vector3 vector)
		{
			//Note: comments are in case all inputs are in World Space.
			Quaternion Q = SubtractRotation(to, from);              //Output is in object space.
			Vector3 A = InverseTransformDirectionMath(from, vector);//Output is in object space.
			Vector3 B = Q * A;                                      //Output is in local space.
			Vector3 C = TransformDirectionMath(from, B);            //Output is in world space.
			return C;
		}
		
		//This function returns a point which is a projection from a point to a line.
		//The line is regarded infinite. If the line is finite, use ProjectPointOnLineSegment() instead.
		public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
		{
			//get vector from point on line to point in space
			Vector3 linePointToPoint = point - linePoint;

			float t = Vector3.Dot(linePointToPoint, lineVec);

			return linePoint + lineVec * t;
		}

		//This function returns a point which is a projection from a point to a line segment.
		//If the projected point lies outside of the line segment, the projected point will 
		//be clamped to the appropriate line edge.
		//If the line is infinite instead of a segment, use ProjectPointOnLine() instead.
		public static Vector3 ProjectPointOnLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
		{
			Vector3 vector = linePoint2 - linePoint1;
			Vector3 projectedPoint = ProjectPointOnLine(linePoint1, vector.normalized, point);

			int side = PointOnWhichSideOfLineSegment(linePoint1, linePoint2, projectedPoint);

			//The projected point is on the line segment
			if (side == 0)
				return projectedPoint;

			if (side == 1)
				return linePoint1;

			if (side == 2)
				return linePoint2;

			//output is invalid
			return Vector3.zero;
		}

		//This function returns a point which is a projection from a point to a plane.
		public static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
		{
			float distance;
			Vector3 translationVector;

			//First calculate the distance from the point to the plane:
			distance = SignedDistancePlanePoint(planeNormal, planePoint, point);

			//Reverse the sign of the distance
			distance *= -1;

			//Get a translation vector
			translationVector = SetVectorLength(planeNormal, distance);

			//Translate the point to form a projection
			return point + translationVector;
		}

		//Projects a vector onto a plane. The output is not normalized.
		public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
		{
			return vector - (Vector3.Dot(vector, planeNormal) * planeNormal);
		}

		//Get the shortest distance between a point and a plane. The output is signed so it holds information
		//as to which side of the plane normal the point is.
		public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
		{
			return Vector3.Dot(planeNormal, (point - planePoint));
		}

		//This function calculates a signed (+ or - sign instead of being ambiguous) dot product. It is basically used
		//to figure out whether a vector is positioned to the left or right of another vector. The way this is done is
		//by calculating a vector perpendicular to one of the vectors and using that as a reference. This is because
		//the result of a dot product only has signed information when an angle is transitioning between more or less
		//than 90 degrees.
		public static float SignedDotProduct(Vector3 vectorA, Vector3 vectorB, Vector3 normal)
		{
			Vector3 perpVector;
			float dot;

			//Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
			perpVector = Vector3.Cross(normal, vectorA);

			//Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
			dot = Vector3.Dot(perpVector, vectorB);

			return dot;
		}

		public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal)
		{
			Vector3 perpVector;
			float angle;

			//Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
			perpVector = Vector3.Cross(normal, referenceVector);

			//Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
			angle = Vector3.Angle(referenceVector, otherVector);
			angle *= Mathf.Sign(Vector3.Dot(perpVector, otherVector));

			return angle;
		}

		//Calculate the angle between a vector and a plane. The plane is made by a normal vector.
		//Output is in radians.
		public static float AngleVectorPlane(Vector3 vector, Vector3 normal)
		{
			float dot;
			float angle;

			//calculate the the dot product between the two input vectors. This gives the cosine between the two vectors
			dot = Vector3.Dot(vector, normal);

			//this is in radians
			angle = (float)Math.Acos(dot);

			return 1.570796326794897f - angle; //90 degrees - angle
		}

		//Calculate the dot product as an angle
		public static float DotProductAngle(Vector3 vec1, Vector3 vec2)
		{
			double dot;
			double angle;

			//get the dot product
			dot = Vector3.Dot(vec1, vec2);

			//Clamp to prevent NaN error. Shouldn't need this in the first place, but there could be a rounding error issue.
			if (dot < -1.0f)
				dot = -1.0f;

			if (dot > 1.0f)
				dot = 1.0f;

			//Calculate the angle. The output is in radians
			//This step can be skipped for optimization...
			angle = Math.Acos(dot);

			return (float)angle;
		}

		//Convert a plane defined by 3 points to a plane defined by a vector and a point. 
		//The plane point is the middle of the triangle defined by the 3 points.
		public static void PlaneFrom3Points(out Vector3 planeNormal, out Vector3 planePoint, Vector3 pointA, Vector3 pointB, Vector3 pointC)
		{
			planeNormal = Vector3.zero;
			planePoint = Vector3.zero;

			//Make two vectors from the 3 input points, originating from point A
			Vector3 AB = pointB - pointA;
			Vector3 AC = pointC - pointA;

			//Calculate the normal
			planeNormal = Vector3.Normalize(Vector3.Cross(AB, AC));

			//Get the points in the middle AB and AC
			Vector3 middleAB = pointA + (AB / 2.0f);
			Vector3 middleAC = pointA + (AC / 2.0f);

			//Get vectors from the middle of AB and AC to the point which is not on that line.
			Vector3 middleABtoC = pointC - middleAB;
			Vector3 middleACtoB = pointB - middleAC;

			//Calculate the intersection between the two lines. This will be the center 
			//of the triangle defined by the 3 points.
			//We could use LineLineIntersection instead of ClosestPointsOnTwoLines but due to rounding errors 
			//this sometimes doesn't work.
			Vector3 temp;
			LogicUtils.ClosestPointsOnTwoLines(out planePoint, out temp, middleAB, middleABtoC, middleAC, middleACtoB);
		}

		//Returns the forward vector of a quaternion
		public static Vector3 GetForwardVector(Quaternion q)
		{
			return q * Vector3.forward;
		}

		//Returns the up vector of a quaternion
		public static Vector3 GetUpVector(Quaternion q)
		{
			return q * Vector3.up;
		}

		//Returns the right vector of a quaternion
		public static Vector3 GetRightVector(Quaternion q)
		{
			return q * Vector3.right;
		}

		//Gets a quaternion from a matrix
		public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
		{
			return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
		}

		//Gets a position from a matrix
		public static Vector3 PositionFromMatrix(Matrix4x4 m)
		{
			Vector4 vector4Position = m.GetColumn(3);
			return new Vector3(vector4Position.x, vector4Position.y, vector4Position.z);
		}

		//This is an alternative for Quaternion.LookRotation. Instead of aligning the forward and up vector of the game 
		//object with the input vectors, a custom direction can be used instead of the fixed forward and up vectors.
		//alignWithVector and alignWithNormal are in world space.
		//customForward and customUp are in object space.
		//Usage: use alignWithVector and alignWithNormal as if you are using the default LookRotation function.
		//Set customForward and customUp to the vectors you wish to use instead of the default forward and up vectors.
		public static void LookRotationExtended(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 customForward, Vector3 customUp)
		{
			//Set the rotation of the destination
			Quaternion rotationA = Quaternion.LookRotation(alignWithVector, alignWithNormal);

			//Set the rotation of the custom normal and up vectors. 
			//When using the default LookRotation function, this would be hard coded to the forward and up vector.
			Quaternion rotationB = Quaternion.LookRotation(customForward, customUp);

			//Calculate the rotation
			gameObjectInOut.transform.rotation = rotationA * Quaternion.Inverse(rotationB);
		}

		//This function transforms one object as if it was parented to the other.
		//Before using this function, the Init() function must be called
		//Input: parentRotation and parentPosition: the current parent transform.
		//Input: startParentRotation and startParentPosition: the transform of the parent object at the time the objects are parented.
		//Input: startChildRotation and startChildPosition: the transform of the child object at the time the objects are parented.
		//Output: childRotation and childPosition.
		//All transforms are in world space.
		public static void TransformWithParent(out Quaternion childRotation, out Vector3 childPosition, Quaternion parentRotation, Vector3 parentPosition, Quaternion startParentRotation, Vector3 startParentPosition, Quaternion startChildRotation, Vector3 startChildPosition)
		{
			childRotation = Quaternion.identity;
			childPosition = Vector3.zero;

			//set the parent start transform
			tempParent.rotation = startParentRotation;
			tempParent.position = startParentPosition;
			tempParent.localScale = Vector3.one; //to prevent scale wandering

			//set the child start transform
			tempChild.rotation = startChildRotation;
			tempChild.position = startChildPosition;
			tempChild.localScale = Vector3.one; //to prevent scale wandering

			//translate and rotate the child by moving the parent
			tempParent.rotation = parentRotation;
			tempParent.position = parentPosition;

			//get the child transform
			childRotation = tempChild.rotation;
			childPosition = tempChild.position;
		}

		//With this function you can align a triangle of an object with any transform.
		//Usage: gameObjectInOut is the game object you want to transform.
		//alignWithVector, alignWithNormal, and alignWithPosition is the transform with which the triangle of the object should be aligned with.
		//triangleForward, triangleNormal, and trianglePosition is the transform of the triangle from the object.
		//alignWithVector, alignWithNormal, and alignWithPosition are in world space.
		//triangleForward, triangleNormal, and trianglePosition are in object space.
		//trianglePosition is the mesh position of the triangle. The effect of the scale of the object is handled automatically.
		//trianglePosition can be set at any position, it does not have to be at a vertex or in the middle of the triangle.
		public static void PreciseAlign(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 alignWithPosition, Vector3 triangleForward, Vector3 triangleNormal, Vector3 trianglePosition)
		{
			//Set the rotation.
			LookRotationExtended(ref gameObjectInOut, alignWithVector, alignWithNormal, triangleForward, triangleNormal);

			//Get the world space position of trianglePosition
			Vector3 trianglePositionWorld = gameObjectInOut.transform.TransformPoint(trianglePosition);

			//Get a vector from trianglePosition to alignWithPosition
			Vector3 translateVector = alignWithPosition - trianglePositionWorld;

			//Now transform the object so the triangle lines up correctly.
			gameObjectInOut.transform.Translate(translateVector, Space.World);
		}


		//Convert a position, direction, and normal vector to a transform
		public static void VectorsToTransform(ref GameObject gameObjectInOut, Vector3 positionVector, Vector3 directionVector, Vector3 normalVector)
		{
			gameObjectInOut.transform.position = positionVector;
			gameObjectInOut.transform.rotation = Quaternion.LookRotation(directionVector, normalVector);
		}

		//This function finds out on which side of a line segment the point is located.
		//The point is assumed to be on a line created by linePoint1 and linePoint2. If the point is not on
		//the line segment, project it on the line using ProjectPointOnLine() first.
		//Returns 0 if point is on the line segment.
		//Returns 1 if point is outside of the line segment and located on the side of linePoint1.
		//Returns 2 if point is outside of the line segment and located on the side of linePoint2.
		public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
		{
			Vector3 lineVec = linePoint2 - linePoint1;
			Vector3 pointVec = point - linePoint1;

			float dot = Vector3.Dot(pointVec, lineVec);

			//point is on side of linePoint2, compared to linePoint1
			if (dot > 0)
			{
				//point is on the line segment
				if (pointVec.magnitude <= lineVec.magnitude)
					return 0;

				//point is not on the line segment and it is on the side of linePoint2
				else
					return 2;
			}

			//Point is not on side of linePoint2, compared to linePoint1.
			//Point is not on the line segment and it is on the side of linePoint1.
			else
				return 1;
		}


		//Returns the pixel distance from the mouse pointer to a line.
		//Alternative for HandleUtility.DistanceToLine(). Works both in Editor mode and Play mode.
		//Do not call this function from OnGUI() as the mouse position will be wrong.
		public static float MouseDistanceToLine(Vector3 linePoint1, Vector3 linePoint2)
		{
			Camera currentCamera;
			Vector3 mousePosition;

#if UNITY_EDITOR
			if (Camera.current != null)
				currentCamera = Camera.current;
			else
				currentCamera = Camera.main;

			//convert format because y is flipped
			mousePosition = new Vector3(Event.current.mousePosition.x, currentCamera.pixelHeight - Event.current.mousePosition.y, 0f);

#else
		currentCamera = Camera.main;
		mousePosition = Input.mousePosition;
#endif

			Vector3 screenPos1 = currentCamera.WorldToScreenPoint(linePoint1);
			Vector3 screenPos2 = currentCamera.WorldToScreenPoint(linePoint2);
			Vector3 projectedPoint = ProjectPointOnLineSegment(screenPos1, screenPos2, mousePosition);

			//set z to zero
			projectedPoint = new Vector3(projectedPoint.x, projectedPoint.y, 0f);

			Vector3 vector = projectedPoint - mousePosition;
			return vector.magnitude;
		}


		//Returns the pixel distance from the mouse pointer to a camera facing circle.
		//Alternative for HandleUtility.DistanceToCircle(). Works both in Editor mode and Play mode.
		//Do not call this function from OnGUI() as the mouse position will be wrong.
		//If you want the distance to a point instead of a circle, set the radius to 0.
		public static float MouseDistanceToCircle(Vector3 point, float radius)
		{

			Camera currentCamera;
			Vector3 mousePosition;

#if UNITY_EDITOR
			if (Camera.current != null)
				currentCamera = Camera.current;
			else
				currentCamera = Camera.main;

			//convert format because y is flipped
			mousePosition = new Vector3(Event.current.mousePosition.x, currentCamera.pixelHeight - Event.current.mousePosition.y, 0f);
#else
		currentCamera = Camera.main;
		mousePosition = Input.mousePosition;
#endif

			Vector3 screenPos = currentCamera.WorldToScreenPoint(point);

			//set z to zero
			screenPos = new Vector3(screenPos.x, screenPos.y, 0f);

			Vector3 vector = screenPos - mousePosition;
			float fullDistance = vector.magnitude;
			float circleDistance = fullDistance - radius;

			return circleDistance;
		}
		
		//Returns true if line segment made up of pointA1 and pointA2 is crossing line segment made up of
		//pointB1 and pointB2. The two lines are assumed to be in the same plane.
		public static bool AreLineSegmentsCrossing(Vector3 pointA1, Vector3 pointA2, Vector3 pointB1, Vector3 pointB2)
		{
			Vector3 closestPointA;
			Vector3 closestPointB;
			int sideA;
			int sideB;

			Vector3 lineVecA = pointA2 - pointA1;
			Vector3 lineVecB = pointB2 - pointB1;

			bool valid = LogicUtils.ClosestPointsOnTwoLines(out closestPointA, out closestPointB, pointA1, lineVecA.normalized, pointB1, lineVecB.normalized);

			//lines are not parallel
			if (valid)
			{
				sideA = PointOnWhichSideOfLineSegment(pointA1, pointA2, closestPointA);
				sideB = PointOnWhichSideOfLineSegment(pointB1, pointB2, closestPointB);

				if ((sideA == 0) && (sideB == 0))
					return true;
				else
					return false;
			}

			//lines are parallel
			else
				return false;
		}

		//This function calculates the acceleration vector in meter/second^2.
		//Input: position. If the output is used for motion simulation, the input transform
		//has to be located at the seat base, not at the vehicle CG. Attach an empty GameObject
		//at the correct location and use that as the input for this function.
		//Gravity is not taken into account but this can be added to the output if needed.
		//A low number of samples can give a jittery result due to rounding errors.
		//If more samples are used, the output is more smooth but has a higher latency.
		public static bool LinearAcceleration(out Vector3 vector, Vector3 position, int samples)
		{
			Vector3 averageSpeedChange = Vector3.zero;
			vector = Vector3.zero;
			Vector3 deltaDistance;
			float deltaTime;
			Vector3 speedA;
			Vector3 speedB;

			//Clamp sample amount. In order to calculate acceleration we need at least 2 changes
			//in speed, so we need at least 3 position samples.
			if (samples < 3)
				samples = 3;

			//Initialize
			if (positionRegister == null)
			{
				positionRegister = new Vector3[samples];
				posTimeRegister = new float[samples];
			}

			//Fill the position and time sample array and shift the location in the array to the left
			//each time a new sample is taken. This way index 0 will always hold the oldest sample and the
			//highest index will always hold the newest sample. 
			for (int i = 0; i < positionRegister.Length - 1; i++)
			{
				positionRegister[i] = positionRegister[i + 1];
				posTimeRegister[i] = posTimeRegister[i + 1];
			}
			positionRegister[positionRegister.Length - 1] = position;
			posTimeRegister[posTimeRegister.Length - 1] = Time.time;

			positionSamplesTaken++;

			//The output acceleration can only be calculated if enough samples are taken.
			if (positionSamplesTaken >= samples)
			{
				//Calculate average speed change.
				for (int i = 0; i < positionRegister.Length - 2; i++)
				{
					deltaDistance = positionRegister[i + 1] - positionRegister[i];
					deltaTime = posTimeRegister[i + 1] - posTimeRegister[i];

					//If deltaTime is 0, the output is invalid.
					if (deltaTime == 0)
						return false;

					speedA = deltaDistance / deltaTime;
					deltaDistance = positionRegister[i + 2] - positionRegister[i + 1];
					deltaTime = posTimeRegister[i + 2] - posTimeRegister[i + 1];

					if (deltaTime == 0)
						return false;

					speedB = deltaDistance / deltaTime;

					//This is the accumulated speed change at this stage, not the average yet.
					averageSpeedChange += speedB - speedA;
				}

				//Now this is the average speed change.
				averageSpeedChange /= positionRegister.Length - 2;

				//Get the total time difference.
				float deltaTimeTotal = posTimeRegister[posTimeRegister.Length - 1] - posTimeRegister[0];

				//Now calculate the acceleration, which is an average over the amount of samples taken.
				vector = averageSpeedChange / deltaTimeTotal;

				return true;
			}
			else
				return false;
		}


		/*
		//This function calculates angular acceleration in object space as deg/second^2, encoded as a vector. 
		//For example, if the output vector is 0,0,-5, the angular acceleration is 5 deg/second^2 around the object Z axis, to the left. 
		//Input: rotation (quaternion). If the output is used for motion simulation, the input transform
		//has to be located at the seat base, not at the vehicle CG. Attach an empty GameObject
		//at the correct location and use that as the input for this function.
		//A low number of samples can give a jittery result due to rounding errors.
		//If more samples are used, the output is more smooth but has a higher latency.
		//Note: the result is only accurate if the rotational difference between two samples is less than 180 degrees.
		//Note: a suitable way to visualize the result is:
		Vector3 dir;
		float scale = 2f;	
		dir = new Vector3(vector.x, 0, 0);
		dir = Math3d.SetVectorLength(dir, dir.magnitude * scale);
		dir = gameObject.transform.TransformDirection(dir);
		Debug.DrawRay(gameObject.transform.position, dir, Color.red);	
		dir = new Vector3(0, vector.y, 0);
		dir = Math3d.SetVectorLength(dir, dir.magnitude * scale);
		dir = gameObject.transform.TransformDirection(dir);
		Debug.DrawRay(gameObject.transform.position, dir, Color.green);	
		dir = new Vector3(0, 0, vector.z);
		dir = Math3d.SetVectorLength(dir, dir.magnitude * scale);
		dir = gameObject.transform.TransformDirection(dir);
		Debug.DrawRay(gameObject.transform.position, dir, Color.blue);	*/
		public static bool AngularAcceleration(out Vector3 vector, Quaternion rotation, int samples)
		{

			Vector3 averageSpeedChange = Vector3.zero;
			vector = Vector3.zero;
			Quaternion deltaRotation;
			float deltaTime;
			Vector3 speedA;
			Vector3 speedB;

			//Clamp sample amount. In order to calculate acceleration we need at least 2 changes
			//in speed, so we need at least 3 rotation samples.
			if (samples < 3)
				samples = 3;

			//Initialize
			if (rotationRegister == null)
			{
				rotationRegister = new Quaternion[samples];
				rotTimeRegister = new float[samples];
			}

			//Fill the rotation and time sample array and shift the location in the array to the left
			//each time a new sample is taken. This way index 0 will always hold the oldest sample and the
			//highest index will always hold the newest sample. 
			for (int i = 0; i < rotationRegister.Length - 1; i++)
			{
				rotationRegister[i] = rotationRegister[i + 1];
				rotTimeRegister[i] = rotTimeRegister[i + 1];
			}
			rotationRegister[rotationRegister.Length - 1] = rotation;
			rotTimeRegister[rotTimeRegister.Length - 1] = Time.time;

			rotationSamplesTaken++;

			//The output acceleration can only be calculated if enough samples are taken.
			if (rotationSamplesTaken >= samples)
			{

				//Calculate average speed change.
				for (int i = 0; i < rotationRegister.Length - 2; i++)
				{

					deltaRotation = SubtractRotation(rotationRegister[i + 1], rotationRegister[i]);
					deltaTime = rotTimeRegister[i + 1] - rotTimeRegister[i];

					//If deltaTime is 0, the output is invalid.
					if (deltaTime == 0)
						return false;

					speedA = RotDiffToSpeedVec(deltaRotation, deltaTime);
					deltaRotation = SubtractRotation(rotationRegister[i + 2], rotationRegister[i + 1]);
					deltaTime = rotTimeRegister[i + 2] - rotTimeRegister[i + 1];

					if (deltaTime == 0)
						return false;

					speedB = RotDiffToSpeedVec(deltaRotation, deltaTime);

					//This is the accumulated speed change at this stage, not the average yet.
					averageSpeedChange += speedB - speedA;
				}

				//Now this is the average speed change.
				averageSpeedChange /= rotationRegister.Length - 2;

				//Get the total time difference.
				float deltaTimeTotal = rotTimeRegister[rotTimeRegister.Length - 1] - rotTimeRegister[0];

				//Now calculate the acceleration, which is an average over the amount of samples taken.
				vector = averageSpeedChange / deltaTimeTotal;

				return true;
			}
			else
				return false;
		}

		//Get y from a linear function, with x as an input. The linear function goes through points
		//0,0 on the left ,and Qxy on the right.
		public static float LinearFunction2DBasic(float x, float Qx, float Qy)
		{
			float y = x * (Qy / Qx);
			return y;
		}

		//Get y from a linear function, with x as an input. The linear function goes through points
		//Pxy on the left ,and Qxy on the right.
		public static float LinearFunction2DFull(float x, float Px, float Py, float Qx, float Qy)
		{
			float y = 0f;

			float A = Qy - Py;
			float B = Qx - Px;
			float C = A / B;

			y = Py + (C * (x - Px));

			return y;
		}

		//Convert a rotation difference to a speed vector.
		//For internal use only.
		private static Vector3 RotDiffToSpeedVec(Quaternion rotation, float deltaTime)
		{
			float x;
			float y;
			float z;

			if (rotation.eulerAngles.x <= 180.0f)
				x = rotation.eulerAngles.x;
			else
				x = rotation.eulerAngles.x - 360.0f;

			if (rotation.eulerAngles.y <= 180.0f)
				y = rotation.eulerAngles.y;
			else
				y = rotation.eulerAngles.y - 360.0f;

			if (rotation.eulerAngles.z <= 180.0f)
				z = rotation.eulerAngles.z;

			else
				z = rotation.eulerAngles.z - 360.0f;

			return new Vector3(x / deltaTime, y / deltaTime, z / deltaTime);
		}


		/////////////////////////////////////////
		//---------------------------------------
		public static Vector3 SlerpPivot(Vector3 vPivot, Vector3 vA, Vector3 vB, float fT)
		{
			Vector3 vRelA = vA - vPivot;
			Vector3 vRelB = vB - vPivot;

			Vector3 vSlerp = Vector3.Slerp(vRelA, vRelB, fT);
			return vSlerp + vPivot;
		}

		//---------------------------------------
		public static Vector3 ScreenPointEdgeClamp(Vector2 screenPos, float edgeBuffer, out float angleDeg)
		{
			// Take the direction of the screen point from the screen center to push it out to the edge of the screen
			// Use the shortest distance from projecting it along the height and width
			Vector2 screenCenter = new Vector2(Screen.width, Screen.height) * 0.5f;
			Vector2 screenDir = (screenPos - screenCenter).normalized;
			float angleRad = Mathf.Atan2(screenDir.x, screenDir.y);
			float distHeight = Mathf.Abs((screenCenter.y - edgeBuffer) / Mathf.Cos(angleRad));
			float distWidth = Mathf.Abs((screenCenter.x - edgeBuffer) / Mathf.Cos(angleRad + (Mathf.PI * 0.5f)));
			float dist = Mathf.Min(distHeight, distWidth);
			angleDeg = angleRad * Mathf.Rad2Deg;
			return screenCenter + (screenDir * dist);
		}


		/////////////////////////////////////////
		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
	public class RandomUtils
	{
		//---------------------------------------
		public static int Range(int min, int max)
		{
			return UnityEngine.Random.Range(min, max);
		}

		public static float Range(float min, float max)
		{
			return UnityEngine.Random.Range(min, max);
		}

		public static int Range(int range, bool bUseNegative)
		{
			return UnityEngine.Random.Range((bUseNegative) ? -range : 0, range);
		}

		public static float Range(float range, bool bUseNegative)
		{
			return UnityEngine.Random.Range((bUseNegative)? -range : 0.0f, range);
		}

		public static int Range<T>(T[] vArray)
		{
			return Range(0, vArray.Length);
		}

		public static int Range<T>(List<T> vArray)
		{
			return Range(0, vArray.Count);
		}

		//---------------------------------------
		public static T Array<T>(T[] vArray)
		{
			return vArray[RandomUtils.Range(0, vArray.Length)];
		}

		public static T Array<T>(List<T> vArray)
		{
			return vArray[RandomUtils.Range(0, vArray.Count)];
		}

		//---------------------------------------
		public static Vector2 GetVector2(bool negativeX = true, bool negativeY = true)
		{
			float fXmin = (negativeX) ? -1.0f : 0.0f;
			float fYmin = (negativeY) ? -1.0f : 0.0f;
			return new UnityEngine.Vector2(Range(fXmin, 1.0f), Range(fYmin, 1.0f)).normalized;
		}

		public static Vector3 GetVector3(bool negativeX = true, bool negativeY = true, bool negativeZ = true)
		{
			float fXmin = (negativeX) ? -1.0f : 0.0f;
			float fYmin = (negativeY) ? -1.0f : 0.0f;
			float fZmin = (negativeZ) ? -1.0f : 0.0f;
			return new UnityEngine.Vector3(Range(fXmin, 1.0f), Range(fYmin, 1.0f), Range(fZmin, 1.0f)).normalized;
		}

		/// <summary>
		/// Vector의 Foward를 기준으로 임의의 Foward를 구합니다.
		/// </summary>
		static public Vector3 GetRandomVector(Vector3 vFoward, float fDegree)
		{
			float fHalfRadianRate = Mathf.Deg2Rad * (fDegree * 0.5f);

			Vector3 vRight = MathUtils.GetVectorRight(vFoward);
			vRight = Vector3.Lerp(vFoward, vRight, fHalfRadianRate);

			Vector3 vLeft = MathUtils.GetVectorLeft(vFoward);
			vLeft = Vector3.Lerp(vFoward, vLeft, fHalfRadianRate);

			return Vector3.Lerp(vRight, vLeft, Range(0.0f, 1.0f));
		}

		//---------------------------------------
		static public Color GetRandomColor(bool bRandomAlpha)
		{
			Color pColor = new Color();
			pColor.r = Range(1.0f, false);
			pColor.g = Range(1.0f, false);
			pColor.b = Range(1.0f, false);
			
			pColor.a = (bRandomAlpha)? Range(1.0f, false) : 1.0f;

			return pColor;
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}
