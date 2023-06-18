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

    void Start()  //get behavior type
    {
        this.behaviorType = GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().BehaviorType;
    }

    void Update()   
    {
        if (this.behaviorType == Unity.MLAgents.Policies.BehaviorType.HeuristicOnly)
        {
            // if heuristic react to key presses
            // Debug.Log("some button is pressed");
            RequestDecision();
        }
    }

    public override void OnEpisodeBegin()      
    {

    }

    public override void CollectObservations(VectorSensor sensor)    
    {

    }

    public override void OnActionReceived(ActionBuffers actions)     //depends on state
    {
        int mainThrusterOn = actions.DiscreteActions[0];

        if (mainThrusterOn == 1) {
            rocket.AddRelativeForce(Vector3.up * force);
            fuel -= 1 * Time.deltaTime;
            Debug.Log(fuel);
        }
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
}
