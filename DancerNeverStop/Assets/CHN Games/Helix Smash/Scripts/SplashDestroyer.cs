using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashDestroyer : MonoBehaviour
{
    int _180degreeCounter=0; // Hold number of 180 degree turns.
    float lastAngle = -99999; // Initial value of last angle, the initial angle can not be -99999, so that means, it must be assigned to start angles of the splash sprite. Start method is not used, because its angle information is not consistently.
    void Update()
    {
        float angle = Vector3.SignedAngle(-Vector3.forward, transform.position,Vector3.up); // Calculate the angle of the splash object around to the y axis and relative to the z axis.
        if (lastAngle == -99999) lastAngle = angle; // Set initial value of last angle.
        if (lastAngle * angle < 0) // If last angle * angle < 0, that means, only one is must be negative and I used signed angle to calculate angle. So, there is four cases like that => (last angle < 0 and angle > 0), (last angle > 0 and angle < 0), (last angle < 180 and angle > -180), (last angle > -180 and angle < 180)
        { 
            _180degreeCounter++; // Increase by one 180 degree counter.
            if(_180degreeCounter == 3) Destroy(gameObject); // If the splash turns 540 degree (180 * 3), destroy it.
        }
        lastAngle = angle; // To measure angle difference between two frames, last angle must be assigned to angle.
    }
}
