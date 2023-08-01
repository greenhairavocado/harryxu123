using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Handlers;
using Managers;
public class ChallengeAI : Agent
{
    public gamehandler game;
    public GameObject particles;
    public float speed = 5f;
    public Material success;
    public Material progress;
    public Material failure;
    public MeshRenderer floor;
    public float force = 1.0f;
    public float fuel = 100f;
    public float rotationSpeed = 5f;
    public float fuelEfficiencyCoefficient = 1.0f;
    bool landing;
    bool done;
    float lastY;
    Rigidbody rocket;
    Unity.MLAgents.Policies.BehaviorType behaviorType;
    public Transform initialGoal;
    public Transform goalPosition;
    public Transform lowestValidLocation;
    int mainThrusterOn;
    // bool isControlledByPlayer;
    bool isOnLastGoal;

    [Header("New Control Scheme")]
    public bool thrusting;
    public bool playerPrivileges;


    public float inertiaTimer;

    void Start()  //get behavior type
    {
        this.behaviorType = GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().BehaviorType;
        rocket = GetComponent<Rigidbody>();

        if (MainManager.Instance.difficulty == 0)
        {
            speed = 5;
        }
        else if (MainManager.Instance.difficulty == 1)
        {
            speed = 10;
        }
        else if (MainManager.Instance.difficulty == 2)
        {
            speed = 15;
        }
    }

    void Update()   
    {
        if (done) return;
        Vector3 directionToGoal = (goalPosition.position - transform.position).normalized;

        // Calculate the rotation needed for up direction to look at the goal
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, directionToGoal) * transform.rotation;

        // Gradually rotate the object towards the target rotation if not landing, otherwise face away from the goal
        if (!landing)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation * Quaternion.Euler(0, 180, 0), rotationSpeed * Time.deltaTime);
        }
        // transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        // transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(transform.up, goalPosition.position - transform.position), Random.Range(25f, 45f) * Time.deltaTime);

        // move towards goal
        if (game != null && !game.quizPanel.activeSelf) transform.position = Vector3.MoveTowards(transform.position, goalPosition.position, speed * Time.deltaTime);


        // if (!playerPrivileges)
        // {
        //     if (fuel <= 0)
        //     {
        //         floor.material = failure;
        //         SetReward(-1f);
        //         Debug.Log("A Failure");
        //         EndEpisode();
        //     }

        //     if (goalPosition.localPosition.y > transform.localPosition.y && transform.localPosition.y < lastY && inertiaTimer <= 0f)
        //     {
        //         floor.material = failure;
        //         Debug.Log("C Failure");
        //         SetReward(-1);
        //         EndEpisode();
        //     }
        //     else if (goalPosition.localPosition.y < transform.localPosition.y && transform.localPosition.y > lastY && inertiaTimer <= 0f)
        //     {
        //         floor.material = failure;
        //         SetReward(-1);
        //         EndEpisode();
        //     }
        // }
        // else
        // {
        //     if (fuel <= 0)
        //     {
        //         // floor.material = failure;
        //         SetReward(-1f);
        //         Debug.Log("PLayer A Failure");

        //         game.RequestRestart("You ran out of fuel!");
        //     }
        // }

        // if (transform.localPosition.y < lowestValidLocation.localPosition.y)
        // {
        //     floor.material = failure;
        //     Debug.Log("B Failure");
        //     SetReward(-1);
        //     EndEpisode();
        // }
        
        // if (inertiaTimer > 0)
        // {
        //     inertiaTimer -= Time.deltaTime;
        // }

        // lastY = transform.localPosition.y;
    }

    public void ToggleThrust()
    {
        thrusting = !thrusting;
        particles.SetActive(thrusting);
    }

    public override void OnEpisodeBegin()      
    {
        // Debug.Log($"New Episode Started {rocket.velocity}");
        transform.localPosition = new Vector3(0f, -4.15f, 0f);
        transform.rotation = Quaternion.identity;
        rocket.velocity = Vector3.zero;
        rocket.angularVelocity = Vector3.zero;
        goalPosition = initialGoal;
        fuel = 100f;
        inertiaTimer = 0.5f;
        isOnLastGoal = false;
        landing = false;
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
            if (!game.isQuizActive) fuel -= fuelEfficiencyCoefficient * Time.deltaTime;
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
    }

    public override void Heuristic(in ActionBuffers action)    
    {
        // ActionSegment<int> discreteActions = action.DiscreteActions;

        // if (Input.GetKey(KeyCode.UpArrow) || thrusting)     
        // {
        //     discreteActions[0] = 1;
        // }

        // if (Input.GetKey(KeyCode.W))
        // {
        //     discreteActions[1] = 1;
        // }

        // if (Input.GetKey(KeyCode.S))
        // {
        //     discreteActions[1] = 2;
        // }

        // if (Input.GetKey(KeyCode.A))
        // {
        //     discreteActions[2] = 1;
        // }

        // if (Input.GetKey(KeyCode.D))
        // {
        //     discreteActions[2] = 2;
        // }


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<checkpoint>(out checkpoint goal))
        {
            Debug.Log($"MY INFO: {rocket.velocity.y}");
            if (isOnLastGoal && goal.isLast)
            {
                SetReward(1f);
                floor.material = success;
                Debug.Log("Success");
                // EndEpisode();
                done = true;
                game.IndicateCompletion(false);
            }
            else if (!other.GetComponent<checkpoint>().isLast)
            {
                SetReward(1f);
                isOnLastGoal = true;
                floor.material = progress;
                // Transform newGoal = other.GetComponent<checkpoint>().next;
                // goalPosition = newGoal;
                goalPosition = lowestValidLocation;
                Debug.Log("Initial Success");
                inertiaTimer = 2f;

                // if (!game.isQuizActive && game.progress >= game.questionProgressRequirement)
                // {
                //     game.questionProgressRequirement = game.questionProgressRequirement + 0.25f;
                //     game.lastCheckpoint += 1;
                //     // all rockets must be stopped
                //     game.PauseMovement();
                //     // Debug.Log($"Trigger: {progress} {questionProgressRequirement}");
                //     // question should be asked
                //     Debug.Log("see");
                //     game.CreateQuizQuestion();
                // }
                if (game != null) game.opponentDirection = 1;
                landing = true;
            }
            // else if (!(rocket.velocity.y >= -4 && rocket.velocity.y <= 0))
            // {
            //     floor.material = failure;
            //     SetReward(-1f);
            //     Debug.Log($"Crash Failure {rocket.velocity.y}");
            //     EndEpisode();
            // }
        }
    }

    public void RestartAgent()
    {
        EndEpisode();
    }
}
