
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweMouseAim : MonoBehaviour
{
    public float lookSpeed;
    public LayerMask layers;

    private Vector3 lookPoint;
    private Vector3 lookDirection;
    private Quaternion lookRotation;
    private Vector3 baseLocation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //This script points the the scripts transform towards the mouse cursor.

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        
        if (Physics.Raycast(ray, out hit, 1000f, layers))
        {
            lookPoint = new Vector3(hit.point.x, gameObject.transform.position.y, hit.point.z);

        }


        lookDirection = (lookPoint - gameObject.transform.position).normalized;
        lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, lookSpeed * Time.deltaTime);
    }
}
