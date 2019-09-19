using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkChecker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void netStatusChanged(InternetReachabilityVerifier.Status newStatus) {
        Debug.Log("netStatusChanged: new InternetReachabilityVerifier.Status = " + newStatus);
    }
}
