using UnityEngine;
using System.Collections;

public class SmoothFollowScript: MonoBehaviour {

    // The target we are following
    [SerializeField] Transform target;
    // The distance in the x-z plane to the target
    [SerializeField] float distance = 10.0f;
    // the height we want the camera to be above the target
    [SerializeField] float height = 10.0f;

    [SerializeField] LevelManager levelManager;

    void  LateUpdate ()
    {
        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        Vector3 newPos = new Vector3(target.position.x, target.position.y+height, target.position.z);
        transform.position = newPos;
        transform.position -= Vector3.forward * distance;       

        // Always look at the target
        transform.LookAt(target);
    }
}
