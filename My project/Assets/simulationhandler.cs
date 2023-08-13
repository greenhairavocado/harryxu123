using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class simulationhandler : MonoBehaviour
{
    public int totalAllowedAttempts = 1000;
    public float successRate;
    public float averageFuel;
    public float averageTime;

    int successes;
    int failures;

    public TextMeshProUGUI successText;
    public TextMeshProUGUI fuelText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI description;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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

        timeText.text = $"Average Time to Land: {FormatTime(averageTime)}";

        description.text = $"Total attempts: {successes + failures}\n";
    }

    public void SubmitReport(bool success, float fuel, float time)
    {
        if ((successes + failures) >= totalAllowedAttempts) return;
        if (success)
        {
            successes += 1;
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
