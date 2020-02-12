using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Patterns;

namespace SA.Android.Utilities
{
    public abstract class AN_Singleton<T> : SA_Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake() {
            DontDestroyOnLoad(gameObject);
            gameObject.transform.SetParent(AN_Services.Parent);
        }
    }
}
