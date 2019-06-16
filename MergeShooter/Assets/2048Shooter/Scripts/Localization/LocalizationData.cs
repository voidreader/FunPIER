using System;

[Serializable]
public class LocalizationData : ICloneable
{
	public string Key;

	public string En;

	public string Zh;
	public string Ko;

	public object Clone()
	{
		return base.MemberwiseClone();
	}
}
