using System;

[Serializable]
public struct ValuePer
{
	public int Value;

	public int Per;

	public ValuePer(int value, int per)
	{
		this.Value = value;
		this.Per = per;
	}
}
