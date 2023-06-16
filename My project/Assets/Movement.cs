using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float force = 1.0f;
    public float fuel = 100f;
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
        if (Input.GetKey(KeyCode.UpArrow)) {  //while pressed
            rocket.AddRelativeForce(Vector3.up * force);
            fuel -= 1 * Time.deltaTime;
            Debug.Log(fuel);
        }
        // if (Input.GetKeyDown(KeyCode.UpArrow)) {  // pressed

        // }
        // if (Input.GetKeyUp(KeyCode.UpArrow)) {  //released

        // }
    }
}
