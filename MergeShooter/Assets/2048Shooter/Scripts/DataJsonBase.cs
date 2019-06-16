using System;
using System.Collections.Generic;

[Serializable]
public class DataJsonBase<T> where T : class, ICloneable
{
  public List<T> Data;

  public void AddData(object obj)
  {
    if (!(obj is T))
      return;
    if (this.Data == null)
      this.Data = new List<T>();
    this.Data.Add(obj as T);
  }

  public DataJsonBase<T> Clone()
  {
    if (this.Data == null || this.Data.Count <= 0)
      return (DataJsonBase<T>) null;
    List<T> objList = new List<T>();
    foreach (T obj in this.Data)
      objList.Add((T) obj.Clone());
    DataJsonBase<T> instance = Activator.CreateInstance(this.GetType()) as DataJsonBase<T>;
    instance.Data = objList;
    return instance;
  }
}