using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float maxThrust = 15f;
    public float maxRotationSpeed = 1f;
    public float fuel = 100f;

    private Rigidbody rb;

    // Indicates whether the rocket has landed or crashed.
    private bool isLanded = false;
    private bool hasCrashed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Reset(bool random=false)
    {
        if (random)
        {
            // Randomize position and rotation.
            transform.localPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(60f, 80f), Random.Range(-5f, 5f));
            transform.rotation = Quaternion.Euler(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
        }
        else
        {
            // Reset position and rotation.
            transform.localPosition = new Vector3(0f, 25f, 0f);
            transform.rotation = Quaternion.identity;
        }
        // Reset position, rotation, and velocities.
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        fuel = 100f;

        // Reset status flags.
        isLanded = false;
        hasCrashed = false;
    }

    public void ApplyThrust(float thrust)
    {
        // Make sure thrust is within acceptable range and apply force.
        thrust = Mathf.Clamp(thrust, 0, 1);
        rb.AddRelativeForce(Vector3.up * thrust * maxThrust);
    }

    // public void ApplyRotation(Vector3 targetRotation)
    // {
    //     // Apply a torque to rotate the rocket towards the target rotation.
    //     Vector3 torque = Vector3.Cross(transform.up, targetRotation.normalized);
    //     rb.AddTorque(torque * maxRotationSpeed);
    // }
    public void ApplyRotation(Vector3 rotation)
    {
        // Assume that rotation.x, rotation.y, and rotation.z are inputs between -1 and 1.
        // Multiply by some constant to control how fast the rocket can rotate.
        float roll = rotation.x * maxRotationSpeed;
        float pitch = rotation.y * maxRotationSpeed;
        float yaw = rotation.z * maxRotationSpeed;

        // Apply the rotations. Depending on how your rocket is set up, you may need to switch
        // which axes the roll, pitch, and yaw are applied to.
        rb.AddTorque(transform.right * pitch);
        rb.AddTorque(transform.up * yaw);
        rb.AddTorque(-transform.forward * roll);
    }


    public bool HasCrashed()
    {
        return hasCrashed;
    }

    public bool HasLanded()
    {
        return isLanded;
    }

    public void Land()
    {
        isLanded = true;
    }

    public void Crash()
    {
        hasCrashed = true;
    }

    public void ConsumeFuel(float amount)
    {
        fuel -= amount;
    }

    // Define some kind of collision handler to set hasCrashed.
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "LandingPad")
        {
            hasCrashed = true;
        }
    }

    // Add properties to access Rigidbody velocity and angular velocity.
    public Vector3 Velocity
    {
        get { return rb.velocity; }
    }

    public Vector3 AngularVelocity
    {
        get { return rb.angularVelocity; }
    }

    public float RemainingFuel
    {
        get { return fuel; }
    }
}
