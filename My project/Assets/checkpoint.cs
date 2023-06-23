using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    public bool isLast;
    public Transform next;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (isLast)
        {

        }
        else
        {
            LandingAI agent = other.GetComponent<LandingAI>();

            if (agent != null)
            {
                agent.goalPosition = next;
            }
        }
    }
}
