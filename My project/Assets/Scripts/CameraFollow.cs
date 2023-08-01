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
    public Rect touchableArea = new Rect(0, 0, Screen.width, Screen.height - 400);

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
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Check if touch position is within the touchable area
            if (touchableArea.Contains(touch.position))
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    currentX += touch.deltaPosition.x * rotateSpeed * Time.deltaTime;
                    currentY -= touch.deltaPosition.y * rotateSpeed * Time.deltaTime;
                    currentY = Mathf.Clamp(currentY, -90, 90);
                    transform.rotation = Quaternion.Euler(currentY, currentX, 0);
                }
            }
        }

        transform.position = rocketTransform.position - (transform.rotation * offset);

    }
}
