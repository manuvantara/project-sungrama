using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // limits of the camera movement
    public float zMin = -17.5f;
    public float zMax = 0f;
    public float speed = 0.2f;

    void FixedUpdate()
    {
        // camera controlled with s and w keys and moves up and down
        if (Input.GetKey(KeyCode.W) && transform.position.z < zMax)
        {
            // change global position of camera
            transform.Translate(0, 0, speed, Space.World);
        }

        if (Input.GetKey(KeyCode.S) && transform.position.z > zMin)
        {
            transform.Translate(0, 0, -speed, Space.World);
        }


    }
}
