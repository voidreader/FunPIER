using System;

[Serializable]
public class ShootLevel : ICloneable
{
	public int Level;

	public int Speedlevel;

	public int N2;

	public int N4;

	public int N8;

	public int N16;

	public int N32;

	public int N64;

	public int N128;

	public int N256;

	public int N512;

	public int N1024;

	public int N2048;

	public int N4096;

	public int N8192;

	public string Picture;

	public float Time;

	public Missiontype type;

	public string detail;

	public int Reward;

	public object Clone()
	{
		return base.MemberwiseClone();
	}
}
