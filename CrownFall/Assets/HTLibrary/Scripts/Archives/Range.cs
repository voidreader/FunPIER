using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace HT
{
	[Serializable]
	public struct Range<T>
	{
		public T min;
		public T max;

		public Range(T min_, T max_)
		{
			this.min = min_;
			this.max = max_;
		}
	}

	public static class TRangeEx
	{
		public static int RandomRange(this Range<int> target)
		{
			return UnityEngine.Random.Range(target.min, target.max);
		}

		public static float RandomRange(this Range<float> target)
		{
			return UnityEngine.Random.Range(target.min, target.max);
		}

		public static bool IsInRange(this Range<int> target, int value)
		{
			return value >= target.min && target.max >= value;
		}

		public static bool IsInRange(this Range<float> target, float value)
		{
			return value >= target.min && target.max >= value;
		}

		public static bool IsInRange(this Range<double> target, double value)
		{
			return value >= target.min && target.max >= value;
		}
	}

	[Serializable]
	public struct RangeFloat
	{
		public float min;
		public float max;

		public float Size
		{
			get { return max - min; }
		}

		public RangeFloat(float min_, float max_)
		{
			this.min = min_;
			this.max = max_;
		}

		public float RandomRange()
		{
			return UnityEngine.Random.Range(min, max);
		}
	}

	[Serializable]
	public struct RangeValueFloat
	{
		public float min;
		public float max;
		public float value;

		public float Size
		{
			get { return max - min; }
		}

		public RangeValueFloat(float min_, float max_, float value_)
		{
			this.min = min_;
			this.max = max_;
			this.value = Math.Min(Math.Max(min_, value_), max_);
		}

		public float SetupRandomValue()
		{
			return value = MakeRandomValue();
		}

		public float MakeRandomValue()
		{
			return UnityEngine.Random.Range(min, max);
		}

		public float IncreaseValue(float increaseValue)
		{
			return this.value = Math.Min(Math.Max(min, value + increaseValue), max);
		}
	}
}