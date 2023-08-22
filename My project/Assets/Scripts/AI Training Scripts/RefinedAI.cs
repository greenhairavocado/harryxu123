// using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RefinedAI : Agent
{
    public simulationhandler simulationhandler;
    [Header("Specific to Rocket")]
    public Rocket rocket; // Assume a Rocket class is managing the physics and status of the rocket.
    public Transform landingPad;
    public float landingPadOffset = 0f;
    public float threshold = 0.1f;

    [Header("Specific to Training")]
    public bool randomPlatform;
    public bool randomSpawn;
    public bool enableSteering;
    public Material success;
    public Material failure;
    public MeshRenderer floor;
    public float fuelEfficiencyCoefficient = 1.0f;
    Vector3 preCollisionVelocity;
    Vector3 alignmentGoal;
    float lastDistance;
    float lastVelocity;
    float alignment;

    float startTime;

    void Start()
    {
        alignmentGoal = new Vector3(landingPad.localPosition.x, 100f, landingPad.localPosition.z);
        startTime = Time.time;
    }

    void FixedUpdate()
    {
        preCollisionVelocity = rocket.Velocity;

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.localRotation;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, Random.Range(95f, 135f) * Time.deltaTime);

        // move the x and z positions towards the landing pad
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(landingPad.localPosition.x, transform.localPosition.y, landingPad.localPosition.z), rocket.scaleFactor * Time.deltaTime);

        // Debug.Log(rocket.GetComponent<Rigidbody>().velocity);

        // if the rocket is falling faster than the last velocity, punish it
        // if (rocket.Velocity.y < lastVelocity && rocket.transform.localPosition.y < landingPad.localPosition.y + 400f)
        // {
        //     AddReward(-0.1f);
        // }
        // else
        // {
        //     AddReward(0.1f);
        // }
    }

    public override void OnEpisodeBegin()
    {
        // Reset rocket to some initial condition at the start of each episode.
        rocket.Reset(randomSpawn);

        landingPad.localPosition = new Vector3(landingPadOffset, 0f, 0f);
        // Move the landing pad to a new position.
        if (randomPlatform)
        {
            landingPad.localPosition = new Vector3(Random.Range(-5f * rocket.scaleFactor, 5f * rocket.scaleFactor), 0f, Random.Range(-5f * rocket.scaleFactor, 5f * rocket.scaleFactor));
        }

        alignmentGoal = new Vector3(landingPad.localPosition.x, 100f, landingPad.localPosition.z);

        lastDistance = Vector3.Distance(rocket.transform.localPosition, landingPad.localPosition);
        lastVelocity = rocket.Velocity.y;
        alignment = Vector3.Dot(rocket.transform.up, Vector3.up);
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Add rocket position relative to the landing pad.
        sensor.AddObservation(rocket.transform.localPosition - landingPad.localPosition);

        // Add rocket velocity and angular velocity.
        sensor.AddObservation(rocket.Velocity);
        sensor.AddObservation(rocket.AngularVelocity);

        // Add rocket rotation.
        sensor.AddObservation(rocket.transform.rotation);

        // If rocket fuel is a factor, add remaining fuel.
        sensor.AddObservation(rocket.RemainingFuel);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Assume two actions: one for controlling thrust, another for controlling rotation.
        var actions = actionBuffers.ContinuousActions;
        
        float thrust = actions[0];
        Vector3 targetRotation = new Vector3(actions[1], actions[2], actions[3]);

        // Apply actions to rocket.
        if (alignment > 0.75f) rocket.ApplyThrust(thrust);
        if (enableSteering) rocket.ApplyRotation(targetRotation);

        // Consume fuel.
        rocket.ConsumeFuel(fuelEfficiencyCoefficient * Time.fixedDeltaTime);

        // Calculate the dot product between the rocket's up vector and the world's up vector.
        float alignmentDotProduct = Vector3.Dot(rocket.transform.up, Vector3.up);

        // Calculate rewards.
        if (rocket.HasCrashed())
        {
            SetReward(-100 - (Mathf.Abs(preCollisionVelocity.y) * 10));
            // Debug.Log($"Crashed at a speed of {preCollisionVelocity.y} m/s");
            floor.material = failure;
            EndEpisode();
        }
        else if (rocket.HasLanded())
        {
            SetReward(1000 + (rocket.RemainingFuel * 10));
            // Debug.Log($"Landed with {rocket.RemainingFuel} fuel remaining at a speed of {preCollisionVelocity.y} m/s");
            floor.material = success;

            if (rocket.Velocity.y > -1f && rocket.Velocity.y < 1f)
            {
                // Debug.Log($"Less than 1! {rocket.Velocity.y} m/s");
                SetReward(500);
            }
            else if (rocket.Velocity.y > -2f && rocket.Velocity.y < 1f)
            {
                // Debug.Log($"Less than 2! {rocket.Velocity.y} m/s");
                SetReward(250);
            }
            else if (rocket.Velocity.y > -3f && rocket.Velocity.y < 1f)
            {
                // Debug.Log($"Less than 3! {rocket.Velocity.y} m/s");
                SetReward(100);
            }

            EndEpisode();
        }
        else if (rocket.RemainingFuel <= 0f)
        {
            SetReward(-100);
            floor.material = failure;
            EndEpisode();
        }
        else if (rocket.transform.localPosition.y < landingPad.localPosition.y)
        {
            SetReward(-100);
            floor.material = failure;
            EndEpisode();
        }
        // else if (lastVelocity + 10f < rocket.Velocity.y)
        // {
        //     Debug.Log($"Velocity increased from {lastVelocity} to {rocket.Velocity.y} m/s");
        //     SetReward(-100);
        //     floor.material = failure;
        //     EndEpisode();
        // }
        // else if (Vector3.Distance(rocket.transform.localPosition, landingPad.localPosition) > lastDistance)
        // {
        //     SetReward(-100);
        //     floor.material = failure;
        //     EndEpisode();
        // }
        else if (rocket.Velocity.y > 50f)
        {
            SetReward(-1000);
            floor.material = failure;
            EndEpisode();
        }
        else if (Vector3.Distance(rocket.transform.localPosition, landingPad.localPosition) > (100f * rocket.scaleFactor))
        {
            SetReward(-1000);
            floor.material = failure;
            EndEpisode();
        }
        lastDistance = Vector3.Distance(rocket.transform.localPosition, landingPad.localPosition);
        lastVelocity = rocket.Velocity.y;
        alignment = alignmentDotProduct;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Define some heuristic for testing, like keyboard controls.
        var actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetKey(KeyCode.Space) ? 1f : 0f; // Use space key for thrust.
        actions[1] = Input.GetAxis("Horizontal");
        actions[2] = Input.GetAxis("Vertical");
        actions[3] = Input.GetKey(KeyCode.Q) ? -1f : (Input.GetKey(KeyCode.E) ? 1f : 0f); // Use Q/E keys for roll control.
    }

    void OnCollisionEnter(Collision other)
    {
        // Check if the rocket lands on the platform slow enough to be safe.
        if (other.gameObject.tag == "LandingPad" && preCollisionVelocity.y <= 0 && preCollisionVelocity.y > -threshold)
        {
            rocket.Land();
            if (simulationhandler != null) simulationhandler.SubmitReport(true, rocket.RemainingFuel, Time.time - startTime);
        }
        else
        {
            rocket.Crash();
            if (simulationhandler != null) simulationhandler.SubmitReport(false, rocket.RemainingFuel, Time.time - startTime);
        }
        
    }
}
