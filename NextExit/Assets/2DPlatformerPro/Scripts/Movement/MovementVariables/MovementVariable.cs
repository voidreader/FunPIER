using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Collections.Generic;
#endif

namespace PlatformerPro
{
	/// <summary>
	/// Data for a movement variable
	/// </summary>
	[System.Serializable]
	public class MovementVariable 
	{
		/// <summary>
		/// The float value.
		/// </summary>
		[SerializeField]
		protected float floatValue;
		
		/// <summary>
		/// The int value.
		/// </summary>
		[SerializeField]
		protected int intValue;
		
		/// <summary>
		/// The string value.
		/// </summary>
		[SerializeField]
		protected string stringValue;

		/// <summary>
		/// The bool value.
		/// </summary>
		[SerializeField]
		protected bool boolValue;
		
		/// <summary>
		/// The vector2 value.
		/// </summary>
		[SerializeField]
		protected Vector2 vector2Value;
		
		/// <summary>
		/// The game object value.
		/// </summary>
		[SerializeField]
		protected GameObject gameObjectValue;

		/// <summary>
		/// The gcurve value.
		/// </summary>
		[SerializeField]
		protected AnimationCurve curveValue;


		/// <summary>
		/// Gets or sets the float value.
		/// </summary>
		public float FloatValue
		{
			get
			{
				return floatValue;
			}
			set
			{
				floatValue = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the int value.
		/// </summary>
		public int IntValue
		{
			get
			{
				return intValue;
			}
			set
			{
				intValue = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the string value.
		/// </summary>
		public string StringValue
		{
			get
			{
				return stringValue;
			}
			set
			{
				stringValue = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the bool value.
		/// </summary>
		public bool BoolValue
		{
			get
			{
				return boolValue;
			}
			set
			{
				boolValue = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the vector2 value.
		/// </summary>
		public Vector2 Vector2Value
		{
			get 
			{
				return vector2Value;
			}
			set
			{
				vector2Value = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the GameObject value
		/// </summary>
		public GameObject GameObjectValue
		{
			get
			{
				return gameObjectValue;
			}
			set
			{
				gameObjectValue = value;
			}
		}

		/// <summary>
		/// Gets or sets the curve value
		/// </summary>
		public AnimationCurve CurveValue
		{
			get
			{
				return curveValue;
			}
			set
			{
				curveValue = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.MovementVariable"/> class.
		/// </summary>
		public MovementVariable ()
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.MovementVariable"/> class.
		/// </summary>
		public MovementVariable (float floatValue)
		{
			this.floatValue = floatValue;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.MovementVariable"/> class.
		/// </summary>
		public MovementVariable (bool boolValue)
		{
			this.boolValue = boolValue;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.MovementVariable"/> class.
		/// </summary>
		public MovementVariable (string stringValue)
		{
			this.stringValue = stringValue;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.MovementVariable"/> class.
		/// </summary>
		public MovementVariable (int intValue)
		{
			this.intValue = intValue;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.MovementVariable"/> class.
		/// </summary>
		/// <param name="vector2Value">Vector2 value.</param>
		public MovementVariable (Vector2 vector2Value)
		{
			this.vector2Value = vector2Value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.MovementVariable"/> class.
		/// </summary>
		/// <param name="gameObjectValue">GameObject value.</param>
		public MovementVariable (GameObject gameObjectValue)
		{
			this.gameObjectValue = gameObjectValue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.MovementVariable"/> class.
		/// </summary>
		/// <param name="curveValue">Curve value.</param>
		public MovementVariable (AnimationCurve curveValue)
		{
			this.curveValue = curveValue;
		}

		/// <summary>
		/// Clone constructor. Initializes a new instance of the <see cref="PlatformerPro.MovementVariable"/> class.
		/// </summary>
		/// <param name="originalVariable">Variable to clone values from.</param>
		public MovementVariable (MovementVariable originalVariable)
		{
			if (originalVariable != null)
			{
				this.floatValue = originalVariable.floatValue;
				this.intValue = originalVariable.intValue;
				this.stringValue = originalVariable.stringValue;
				this.boolValue = originalVariable.boolValue;
				this.vector2Value = originalVariable.vector2Value;
				this.gameObjectValue = originalVariable.gameObjectValue;
			}
		}
		
		
		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="PlatformerPro.MovementVariable"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="PlatformerPro.MovementVariable"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="PlatformerPro.MovementVariable"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals (this, obj))
				return true;
			if (obj.GetType () != typeof(MovementVariable))
				return false;
			MovementVariable other = (MovementVariable)obj;
			return floatValue == other.floatValue && intValue == other.intValue && stringValue == other.stringValue  && boolValue == other.boolValue &&  vector2Value.x == other.vector2Value.x && vector2Value.y == other.vector2Value.y ;
		}
		
		/// <summary>
		/// Serves as a hash function for a <see cref="PlatformerPro.MovementVariable"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode ()
		{
			unchecked
			{
				return floatValue.GetHashCode () ^ intValue.GetHashCode () ^ (stringValue != null ? stringValue.GetHashCode () : 0) ^ boolValue.GetHashCode () ^ (vector2Value.x.GetHashCode ())  ^ (vector2Value.y.GetHashCode ())  ;
			}
		}



	}

#if UNITY_EDITOR
	[System.Serializable]
	public class SavableMovementVariable : System.Runtime.Serialization.ISerializable
	{
		/// <summary>
		/// The float value.
		/// </summary>
		protected float floatValue;
		
		/// <summary>
		/// The int value.
		/// </summary>
		protected int intValue;
		
		/// <summary>
		/// The string value.
		/// </summary>
		protected string stringValue;
		
		/// <summary>
		/// The bool value.
		/// </summary>
		protected bool boolValue;
		
		/// <summary>
		/// The vector2 value.
		/// </summary>
		protected Vector2 vector2Value;
		
		/// <summary>
		/// The int value.
		/// </summary>
		protected GameObject gameObjectValue;


		public SavableMovementVariable()
		{
		}

		/// <summary>
		/// Clone constructor. Initializes a new instance of the <see cref="PlatformerPro.MovementVariable"/> class.
		/// </summary>
		/// <param name="originalVariable">Variable to clone values from.</param>
		public SavableMovementVariable (MovementVariable originalVariable)
		{
			this.floatValue = originalVariable.FloatValue;
			this.intValue = originalVariable.IntValue;
			this.stringValue = originalVariable.StringValue;
			this.boolValue = originalVariable.BoolValue;
			this.vector2Value = originalVariable.Vector2Value;
			this.gameObjectValue = originalVariable.GameObjectValue;
		}

		protected SavableMovementVariable(SerializationInfo info, StreamingContext context)
		{
			floatValue = (float)info.GetDouble("floatValue");
			intValue = info.GetInt32("intValue");
			stringValue = info.GetString("stringValue");
			boolValue = info.GetBoolean("boolValue");
			vector2Value = new Vector2((float)info.GetDouble("vector2ValueX"), (float)info.GetDouble("vector2ValueY"));
			//gameObjectValue = info.GetInt32("i");
		}

		public virtual void GetObjectData(SerializationInfo info,  StreamingContext context)
		{
			info.AddValue("floatValue", (double)floatValue);
			info.AddValue("intValue", intValue);
			info.AddValue("stringValue", stringValue);
			info.AddValue("boolValue", boolValue);
			info.AddValue("vector2ValueX", (double)vector2Value.x);
			info.AddValue("vector2ValueY", (double)vector2Value.y);
			
			//			info.AddValue("j", n2);
			//			info.AddValue("k", str);
		}

		public MovementVariable Convert()
		{
			MovementVariable result = new MovementVariable ();
			result.FloatValue = this.floatValue;
			result.IntValue = this.intValue;
			result.StringValue = this.stringValue; 
			result.BoolValue = this.boolValue;
			result.Vector2Value = this.vector2Value;
			result.GameObjectValue = this.gameObjectValue;
			return result;
		}



		/// <summary>
		/// Loads input data from file.
		/// </summary>
		/// <returns>The loaded data or null if data not loaded.</returns>
		/// <param name="fullPath">Full path.</param>
		public static MovementVariable[] LoadFromFile(string fullPath)
		{
			if (fullPath.Length != 0) {
				using (Stream reader = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.None))
				{
					IFormatter formatter = new BinaryFormatter();
					List<SavableMovementVariable> inputData = (List<SavableMovementVariable> ) formatter.Deserialize(reader);
					return inputData.Select(m => m.Convert()).ToArray();
				}
			}
			else
			{
				Debug.LogError ("Tried to load movement data from file but the file path was empty");
			}
			return null;
		}
		
		public static void SaveToFile(string fullPath, MovementVariable[] data)
		{
			if (fullPath.Length != 0) {
				//using (StreamWriter writer = new StreamWriter(fullPath))
				using (Stream writer = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					IFormatter formatter = new BinaryFormatter();
					formatter.Serialize(writer, data.ToList ().ConvertAll(m => new SavableMovementVariable(m)));
				}
			}
			else
			{
				Debug.LogError ("Tried to save movement data to file but no input path was specified.");
			}
		}
	}
#endif
	
}