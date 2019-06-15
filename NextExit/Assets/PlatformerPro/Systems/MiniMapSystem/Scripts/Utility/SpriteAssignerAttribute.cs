using UnityEngine;

public class SpriteAssignerAttribute : PropertyAttribute
{
	public string nameProperty;
	public SpriteAssignerAttribute(string nameProperty) { 
		this.nameProperty = nameProperty;
	}
}

