using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class LandingAI : Agent
{
    public float force = 1.0f;
    public float fuel = 100f;
    public float rotationSpeed = 5f;
    public float fuelEfficiencyCoefficient = 1.0f;
    Rigidbody rocket;
    Unity.MLAgents.Policies.BehaviorType behaviorType;
    public Transform initialGoal;
    public Transform goalPosition;
    public Transform lowestValidLocation;
    int mainThrusterOn;
    bool isControlledByPlayer;

    void Start()  //get behavior type
    {
        this.behaviorType = GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().BehaviorType;
        rocket = GetComponent<Rigidbody>();
    }

    void Update()   
    {
        if (this.behaviorType == Unity.MLAgents.Policies.BehaviorType.HeuristicOnly)
        {
            // if heuristic react to key presses
            // Debug.Log("some button is pressed");
            isControlledByPlayer = true;
            RequestDecision();
        }

        if (fuel <= 0)
        {
            SetReward(-1f);
            EndEpisode();
        }

        if (transform.localPosition.y < lowestValidLocation.localPosition.y)
        {
            SetReward(-1);
            EndEpisode();
        }

        if (!isControlledByPlayer && goalPosition.localPosition.y > transform.localPosition.y && rocket.velocity.y < -0.5f)
        {
            SetReward(-1);
            EndEpisode();
        }
        else if (goalPosition.localPosition.y < transform.localPosition.y && rocket.velocity.y > 0.5f && mainThrusterOn == 1)
        {
            SetReward(-1);
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()      
    {
        // Debug.Log($"New Episode Started {rocket.velocity}");
        transform.localPosition = new Vector3(-2f, -4.15f, -2f);
        transform.rotation = Quaternion.identity;
        rocket.velocity = Vector3.zero;
        rocket.angularVelocity = Vector3.zero;
        goalPosition = initialGoal;
        fuel = 100f;
    }

    public override void CollectObservations(VectorSensor sensor)    
    {
        sensor.AddObservation(rocket.velocity);
        sensor.AddObservation(goalPosition.localPosition);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(fuel);
    }

    public override void OnActionReceived(ActionBuffers actions)     //depends on state
    {
        mainThrusterOn = actions.DiscreteActions[0];
        int verticalSteer = actions.DiscreteActions[1];
        int horizontalSteer = actions.DiscreteActions[2];

        // Debug.Log(mainThrusterOn);

        if (mainThrusterOn == 1 && fuel > 0) {
            rocket.AddRelativeForce(Vector3.up * force);
            fuel -= 1 * Time.deltaTime;
            // Debug.Log(fuel);
        }

        float rotateX = 0f;
        float rotateY = 0f;

        if (verticalSteer == 1)
        {
            rotateX += 1;
        }
        else if (verticalSteer == 2)
        {
            rotateX -= 1;
        }

        if (horizontalSteer == 1)
        {
            rotateY += 1;
        }
        else if (horizontalSteer == 2)
        {
            rotateY -= 1;
        }

        // transform.Rotate(rotateX * rotationSpeed * Time.deltaTime, rotateY * rotationSpeed * Time.deltaTime, 0);
    }

    public override void Heuristic(in ActionBuffers action)    
    {
        ActionSegment<int> discreteActions = action.DiscreteActions;
        /*
        if (Input.GetKey(KeyCode.UpArrow) && fuel > 0) {  //while pressed
            rocket.AddRelativeForce(Vector3.up * force);
            fuel -= 1 * Time.deltaTime;
            Debug.Log(fuel);
        } else if (fuel <= 0) {       //downward force from gravity
            Debug.Log("Out of fuel!");
        }
        */

        if (Input.GetKey(KeyCode.UpArrow))     
        {
            discreteActions[0] = 1;
        }

        if (Input.GetKey(KeyCode.W))
        {
            discreteActions[1] = 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            discreteActions[1] = 2;
        }

        if (Input.GetKey(KeyCode.A))
        {
            discreteActions[2] = 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            discreteActions[2] = 2;
        }


    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.transform} {goalPosition}");
        if (other.TryGetComponent<checkpoint>(out checkpoint goal) && other.transform == goalPosition)
        {
            if (other.GetComponent<checkpoint>().isLast && rocket.velocity.y >= -4 && rocket.velocity.y < 0)
            {
                SetReward(1f);
                Debug.Log("Success");
                EndEpisode();
            }
            else if (rocket.velocity.y < 4 && rocket.velocity.y >= 0)
            {
                SetReward(1f);
                Transform newGoal = other.GetComponent<checkpoint>().next;
                goalPosition = newGoal;
                Debug.Log("Initial Success");
            }
            else
            {
                SetReward(-1f);
                Debug.Log("Failure");
                EndEpisode();
            }
        }
    }

    // void OnColliderEnter(Collider other)
    // {
    //     Debug.Log("physical collision");
    //     if (other.TryGetComponent<checkpoint>(out checkpoint goal))
    //     {
    //         if (other.GetComponent<checkpoint>().isLast && rocket.velocity.y >= -4 && rocket.velocity.y < 0)
    //         {
    //             SetReward(1f);
    //             Debug.Log("Success");
    //             EndEpisode();
    //         }
    //         else
    //         {
    //             SetReward(-1f);
    //             Debug.Log("Failure");
    //             EndEpisode();
    //         }
    //     }
    // }
}
