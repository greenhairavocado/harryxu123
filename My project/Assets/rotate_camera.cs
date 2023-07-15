using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_camera : MonoBehaviour
{
    public float rotationSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        float horizontalRotation = rotationSpeed * Time.deltaTime;
        transform.Rotate(0f, horizontalRotation, 0f);
    }
}
