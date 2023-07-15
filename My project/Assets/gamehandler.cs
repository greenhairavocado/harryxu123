using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gamehandler : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public Slider fuelSlider;
    public LandingAI rocket;
    float startTime;
    float currentFuel;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
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
    }
}
