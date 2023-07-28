using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform rocketTransform;
    public Vector3 offset;

    public float rotateSpeed = 5f;
    float currentX = 0f;
    float currentY = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X") * rotateSpeed;
            currentY += Input.GetAxis("Mouse Y") * rotateSpeed;
            currentY = Mathf.Clamp(currentY, -90, 90);
            transform.rotation = Quaternion.Euler(currentY, currentX, 0);
        }

        transform.position = rocketTransform.position - (transform.rotation * offset);

        // transform.position = rocketTransform.position + rotation * offset;
        // rocketTransform.LookAt(rocketTransform.position);
    }
}
