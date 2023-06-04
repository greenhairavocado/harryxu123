using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class agents : Agent
{
    public float speed;
    public Transform goalPosition;
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(1f, 2f, 4f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(goalPosition.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float xVelocity = actions.ContinuousActions[0];
        float zVelocity = actions.ContinuousActions[1];
        transform.localPosition += new Vector3(xVelocity, 0, zVelocity) * Time.deltaTime * speed;
    }

    public override void Heuristic(in ActionBuffers action)
    {
        ActionSegment<float> continuousActions = action.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }


}
