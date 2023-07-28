using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class joystick_debug : MonoBehaviour
{
    PlayerInput playerInput;
    public GameObject rocket;
    Rigidbody rb;

    public float torque = 0.15f;
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = rocket.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 steer = playerInput.actions["Steering"].ReadValue<Vector2>();
        Debug.Log(steer);

        float roll = steer.x;
        float pitch = steer.y;

        rb.AddTorque(transform.forward * roll * torque, ForceMode.Force);
        rb.AddTorque(transform.right * pitch * torque, ForceMode.Force);

    }
}
