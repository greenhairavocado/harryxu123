using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RefinedAI : Agent
{
    [Header("Specific to Rocket")]
    public Rocket rocket; // Assume a Rocket class is managing the physics and status of the rocket.
    public Transform landingPad;
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
    float alignment;

    void Start()
    {
        alignmentGoal = new Vector3(landingPad.localPosition.x, 100f, landingPad.localPosition.z);
    }

    void FixedUpdate()
    {
        preCollisionVelocity = rocket.Velocity;

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.localRotation;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, Random.Range(95f, 135f) * Time.deltaTime);

        // move the x and z positions towards the landing pad
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(landingPad.localPosition.x, transform.localPosition.y, landingPad.localPosition.z), 2f * Time.deltaTime);
    }

    public override void OnEpisodeBegin()
    {
        // Reset rocket to some initial condition at the start of each episode.
        rocket.Reset(randomSpawn);

        // Move the landing pad to a new position.
        if (randomPlatform)
        {
            landingPad.localPosition = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
        }

        alignmentGoal = new Vector3(landingPad.localPosition.x, 100f, landingPad.localPosition.z);

        lastDistance = Vector3.Distance(rocket.transform.localPosition, landingPad.localPosition);
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
            SetReward(-100);
            floor.material = failure;
            EndEpisode();
        }
        else if (rocket.HasLanded())
        {
            SetReward(100 + rocket.RemainingFuel);
            floor.material = success;
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
        else if (Vector3.Distance(rocket.transform.localPosition, landingPad.localPosition) > 75f)
        {
            SetReward(-100);
            floor.material = failure;
            EndEpisode();
        }
        lastDistance = Vector3.Distance(rocket.transform.localPosition, landingPad.localPosition);
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
        }
        else
        {
            rocket.Crash();
        }
    }
}
