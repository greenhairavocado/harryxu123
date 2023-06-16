using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class agents : Agent
{
    public float speed;
    public Material success;
    public Material failure;
    public MeshRenderer floor;
    public Transform goalPosition;
    
    public override void OnEpisodeBegin()   //start position 
    {
        transform.localPosition = new Vector3(Random.Range(-8.4f, -1.4f), 0.35f, Random.Range(-1.6f, 11.9f));
        goalPosition.localPosition = new Vector3(Random.Range(-8.4f, -1.4f), 0.35f, Random.Range(-1.6f, 11.9f));
    }

    public override void CollectObservations(VectorSensor sensor)    //AI's vision
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(goalPosition.localPosition);      
    }

    public override void OnActionReceived(ActionBuffers actions)     //AI action
    {
        float xVelocity = actions.ContinuousActions[0];
        float zVelocity = actions.ContinuousActions[1];
        transform.localPosition += new Vector3(xVelocity, 0, zVelocity) * Time.deltaTime * speed;   
    }

    public override void Heuristic(in ActionBuffers action)   //User control
    {
        ActionSegment<float> continuousActions = action.ContinuousActions;     
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"We collided with {other}");
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Goal_tag")
        {
            SetReward(1f);
            floor.material = success;
            EndEpisode();
        }
        if (other.gameObject.tag == "wall") 
        {
            SetReward(-1f);
            floor.material = failure;
            EndEpisode();
        }
    }

}
