using System;

public abstract class Singleton<T> where T : class, new()
{
  protected static T _instance;

  public static T instance
  {
    get
    {
      if ((object) Singleton<T>._instance == null)
        Singleton<T>._instance = Activator.CreateInstance<T>();
      return Singleton<T>._instance;
    }
  }
}
