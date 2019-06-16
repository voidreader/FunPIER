using System;

public class SingleInstance<T> where T : new()
{
	public static T m_Instance;

	private static object m_lockObj = new object();

	public static T Instance
	{
		get
		{
			return SingleInstance<T>.GetInstance();
		}
	}

	public static T GetInstance()
	{
		object lockObj = SingleInstance<T>.m_lockObj;
		lock (lockObj)
		{
			if (SingleInstance<T>.m_Instance == null)
			{
				SingleInstance<T>.m_Instance = Activator.CreateInstance<T>();
			}
		}
		return SingleInstance<T>.m_Instance;
	}

	public static void Release()
	{
		SingleInstance<T>.m_Instance = default(T);
	}
}
