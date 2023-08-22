using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class simulationhandler : MonoBehaviour
{
    public bool recordData = false;
    public int totalAllowedAttempts = 1000;
    public float successRate;
    public float averageFuel;
    public float averageTime;

    int successes;
    int failures;
    float bestFuel;
    float worstFuel = 1000;
    float bestTime = 1000;
    float worstTime;

    public TextMeshProUGUI successText;
    public TextMeshProUGUI bestFuelText;
    public TextMeshProUGUI fuelText;
    public TextMeshProUGUI worstFuelText;
    public TextMeshProUGUI bestTimeText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI worstTimeText;
    public TextMeshProUGUI description;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (recordData)
        {
            if (successes + failures > 0)
            {
                successRate = ((successes * 1.0f) / (successes + failures)) * 100;
                
            } 
            else
            {
                successRate = 0;
            }
            
            successText.text = $"Success Rate: {successRate}%";

            fuelText.text = $"Average Fuel Remaining: {averageFuel}";
            bestFuelText.text = $"Best Fuel Remaining: {bestFuel}";
            worstFuelText.text = $"Worst Fuel Remaining: {worstFuel}";

            timeText.text = $"Average Time to Land: {FormatTime(averageTime)}";
            bestTimeText.text = $"Best Time to Land: {FormatTime(bestTime)}";
            worstTimeText.text = $"Worst Time to Land: {FormatTime(worstTime)}";

            description.text = $"Total attempts: {successes + failures}\n";
        }
        
    }

    public void SubmitReport(bool success, float fuel, float time)
    {
        if ((successes + failures) >= totalAllowedAttempts) return;
        if (success)
        {
            successes += 1;

            if (fuel > bestFuel || bestFuel == 0)
            {
                bestFuel = fuel;
            }

            if (fuel < worstFuel)
            {
                worstFuel = fuel;
            }

            if (time < bestTime)
            {
                bestTime = time;
            }

            if (time > worstTime)
            {
                worstTime = time;
            }
        }
        else
        {
            failures += 1;
        }

        int totalAttempts = successes + failures;
        averageFuel = ((averageFuel * (totalAttempts - 1)) + fuel) / totalAttempts;
        averageTime = ((averageTime * (totalAttempts - 1)) + time) / totalAttempts;
    }

    string FormatTime(float time)
    {
        int intTime = (int)time;
        int minutes = intTime / 60;
        int seconds = intTime % 60;
        float fraction = time;
        fraction = (fraction % 1) * 100;
        int milliseconds = (int)fraction;

        string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", minutes, seconds, milliseconds);

        return timeText;
    }
}
