using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gamehandler : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI progressText;
    public Slider fuelSlider;
    public Slider progressSlider;
    public LandingAI rocket;
    float startTime;
    float currentFuel;

    [Header("Player Information")]
    public float progress;
    public int direction; // 0 is going up, 1 is going down
    public Transform initialGoal;
    public Transform platform;
    float initialDistance;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        initialDistance = Vector3.Distance(rocket.transform.position, initialGoal.position);
        Debug.Log($"The initial distance is {initialDistance}");
        // consider velocity if applicable
    }

    // Update is called once per frame
    void Update()
    {
        currentFuel = rocket.fuel;
        fuelSlider.value = currentFuel / 100;

        float t = Time.time - startTime;

        // 00:00:00
        // 120 vs 2:00
        string minutes = ((int)t / 60).ToString();
        string seconds = (t % 60).ToString("f2"); // --> 05

        timerText.text = minutes + ":" + seconds;

        if (direction == 0)
        {
            float currentDistance = Vector3.Distance(rocket.transform.position, initialGoal.position);
            // 0%-50% range
            progress = ((initialDistance - currentDistance) / initialDistance) / 2;
            progressText.text = $"{(int)(progress * 100)}%";

            if (progress >= 0.5)
            {
                direction = 1;
            }
        }
        else
        {
            float currentDistance = Vector3.Distance(rocket.transform.position, platform.position);
            // 51%-100%
            progress = (((initialDistance - currentDistance) / initialDistance) / 2f) + 0.5f;
            progressText.text = $"{(int)(progress * 100)}%";
        }


        progressSlider.value = progress;
    }
}
