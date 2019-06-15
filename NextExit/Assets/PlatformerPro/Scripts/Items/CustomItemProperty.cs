using UnityEngine;

/// <summary>
/// A custom property for an  Item.
/// </summary>
[System.Serializable]
public class CustomItemProperty {
	
	/// <summary>
	/// name of the property.
	/// </summary>
	public string name;

	/// <summary>
	/// The string value.
	/// </summary>
	public string stringValue;

	/// <summary>
	/// The int value.
	/// </summary>
	public int intValue;

	/// <summary>
	/// The float value.
	/// </summary>
	public float floatValue;

	/// <summary>
	/// The object value.
	/// </summary>
	[System.Xml.Serialization.XmlIgnore]
	public Object objectValue;

	/// <summary>
	/// Initializes a new instance of the <see cref="CustomItemProperty"/> class.
	/// </summary>
	public CustomItemProperty() {
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CustomItemProperty"/> class.
	/// </summary>
	/// <param name="name">Name.</param>
	public CustomItemProperty(string name) {
		this.name = name;
	}

	/// <summary>
	/// Clone constructor. Initializes a new instance of the <see cref="CustomItemProperty"/> class.
	/// </summary>
	/// <param name="other">Object to clone.</param>
	public CustomItemProperty(CustomItemProperty other) {
		this.name = other.name;
		this.stringValue = other.stringValue;
		this.intValue = other.intValue;
		this.floatValue = other.floatValue;
		this.objectValue = other.objectValue;
	}

}