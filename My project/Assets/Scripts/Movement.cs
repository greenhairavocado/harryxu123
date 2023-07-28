using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float force = 1.0f;
    public float fuel = 100f;
    public float rotationSpeed = 5f;
    public float fuelEfficiencyCoefficient = 1.0f;
    Rigidbody rocket;

    // Start is called before the first frame update
    void Start()
    {
        rocket = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.UpArrow) && fuel > 0) {  //while pressed
            rocket.AddRelativeForce(Vector3.up * force);
            fuel -= 1 * Time.deltaTime;
            Debug.Log(fuel);
        } else if (fuel <= 0) {       //downward force from gravity
            Debug.Log("Out of fuel!");
        }
        // if (Input.GetKeyDown(KeyCode.UpArrow)) {  // pressed

        // }
        // if (Input.GetKeyUp(KeyCode.UpArrow)) {  //released

        // }


        // Check for rotations
        float rotateX = 0f;
        float rotateY = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            rotateX += 1;
        }

        if (Input.GetKey(KeyCode.S))      
        {
            rotateX -= 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            rotateY += 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            rotateY -= 1;
        }

        transform.Rotate(rotateX * rotationSpeed * Time.deltaTime, rotateY * rotationSpeed * Time.deltaTime, 0);
    }
}
