using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Dictionary<Transform,bool> touchingPlatforms = new Dictionary<Transform, bool>(); // Holds touched platforms as dictionary. (Key => step transform, value => is obstacle bool information)
    void OnTriggerEnter(Collider other)
    {
        TouchingAPlatform(other);
    }
    void OnTriggerStay(Collider other)
    {
        TouchingAPlatform(other);
    }
    /*void OnTriggerExit(Collider other)
    {
        var step = other.transform.parent.parent; // Gets transform of the step.
        if (step.tag == "WillBeDeletedStep") return; // If this step marked as WillBeDeletedStep, return. This is needed for not using the garbage steps.
        if (touchingPlatforms.ContainsKey(step)) // If this step is added before.
        {
            touchingPlatforms.Remove(step); // Remove this step from dictionary list, because the player is not touching it.
        }
    }*/
    void TouchingAPlatform(Collider other)
    {
        var step = other.transform.parent.parent; // Gets transform of the step.
        if (step.tag == "WillBeDeletedStep") return; // If this step marked as WillBeDeletedStep, return. This is needed for not using the garbage steps.
        if (!touchingPlatforms.ContainsKey(step)) // If this step is  not added before.
        {
            touchingPlatforms.Add(step, other.transform.parent.name.Split('_')[1] == "1"); // If the platform has "1" suffix in its name, it means this is an obstacle and the bool is true.
        }
        else // If this step is added before.
        {
            touchingPlatforms[step] = other.transform.parent.name.Split('_')[1] == "1"; // If the platform has "1" suffix in its name, it means this is an obstacle and the bool is true.
        }
    }
}
