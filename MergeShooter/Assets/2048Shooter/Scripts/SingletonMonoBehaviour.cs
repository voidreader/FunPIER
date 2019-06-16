using UnityEngine;
using System.Collections;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    Debug.LogError(typeof(T) + "is nothing");
                    // GameObject singleton = new GameObject();
                    // instance = singleton.AddComponent<T>();
                    // singleton.name = typeof(T).ToString();

                }
            }

            return instance;
        }
    }

    public virtual void Awake()
    {
        CheckInstance();
    }

    protected bool CheckInstance()
    {
        if (this == Instance) { return true; }
        Destroy(this);
        return false;
    }
}