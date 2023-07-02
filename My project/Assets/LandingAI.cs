using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class LandingAI : Agent
{
    public Material success;
    public Material progress;
    public Material failure;
    public MeshRenderer floor;
    public float force = 1.0f;
    public float fuel = 100f;
    public float rotationSpeed = 5f;
    public float fuelEfficiencyCoefficient = 1.0f;
    float lastY;
    Rigidbody rocket;
    Unity.MLAgents.Policies.BehaviorType behaviorType;
    public Transform initialGoal;
    public Transform goalPosition;
    public Transform lowestValidLocation;
    int mainThrusterOn;
    bool isControlledByPlayer;
    bool isOnLastGoal;

    public float inertiaTimer;

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
            floor.material = failure;
            SetReward(-1f);
            Debug.Log("A Failure");
            EndEpisode();
        }

        if (transform.localPosition.y < lowestValidLocation.localPosition.y)
        {
            floor.material = failure;
            Debug.Log("B Failure");
            SetReward(-1);
            EndEpisode();
        }

        if (goalPosition.localPosition.y > transform.localPosition.y && transform.localPosition.y < lastY && inertiaTimer <= 0f)
        {
            floor.material = failure;
            Debug.Log("C Failure");
            SetReward(-1);
            EndEpisode();
        }
        else if (goalPosition.localPosition.y < transform.localPosition.y && transform.localPosition.y > lastY && inertiaTimer <= 0f)
        {
            floor.material = failure;
            SetReward(-1);
            EndEpisode();
        }

        if (inertiaTimer > 0)
        {
            inertiaTimer -= Time.deltaTime;
        }

        

        // if (!isControlledByPlayer && goalPosition.localPosition.y > transform.localPosition.y && rocket.velocity.y < -0.5f)
        // {
        //     SetReward(-1);
        //     EndEpisode();
        // }
        // else if (goalPosition.localPosition.y < transform.localPosition.y && rocket.velocity.y > 0.5f && mainThrusterOn == 1)
        // {
        //     SetReward(-1);
        //     EndEpisode();
        // }

        lastY = transform.localPosition.y;
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
        inertiaTimer = 0.5f;
        isOnLastGoal = false;
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
        if (other.TryGetComponent<checkpoint>(out checkpoint goal))
        {
            Debug.Log($"MY INFO: {rocket.velocity.y}");
            if (isOnLastGoal && rocket.velocity.y >= -4 && rocket.velocity.y <= 0)
            {
                SetReward(1f);
                floor.material = success;
                Debug.Log("Success");
                EndEpisode();
            }
            else if (!other.GetComponent<checkpoint>().isLast && rocket.velocity.y < 4 && rocket.velocity.y >= 0)
            {
                SetReward(1f);
                isOnLastGoal = true;
                floor.material = progress;
                Transform newGoal = other.GetComponent<checkpoint>().next;
                goalPosition = newGoal;
                Debug.Log("Initial Success");
                inertiaTimer = 2f;
            }
            else if (!(rocket.velocity.y >= -4 && rocket.velocity.y <= 0))
            {
                floor.material = failure;
                SetReward(-1f);
                Debug.Log($"Crash Failure {rocket.velocity.y}");
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
